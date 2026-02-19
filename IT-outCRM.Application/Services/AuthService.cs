using AutoMapper;
using IT_outCRM.Application.DTOs.Auth;
using IT_outCRM.Application.Interfaces;
using IT_outCRM.Application.Interfaces.Services;
using IT_outCRM.Domain.Entity;
using Microsoft.Extensions.Configuration;

namespace IT_outCRM.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            IMapper mapper,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // 1. Basic Validation
            if (await _unitOfWork.Users.UsernameExistsAsync(registerDto.Username))
                throw new InvalidOperationException("Пользователь с таким именем уже существует.");

            if (await _unitOfWork.Users.EmailExistsAsync(registerDto.Email))
                throw new InvalidOperationException("Пользователь с таким email уже существует.");

            try 
            {
                await _unitOfWork.BeginTransactionAsync();

                // 2. Create User
                // Определяем роль на основе UserType, если она не указана явно
                var userRole = registerDto.Role;
                if (string.IsNullOrEmpty(userRole) || userRole == "User")
                {
                    // Если UserType указан, используем его для определения роли
                    if (registerDto.UserType == "Executor")
                    {
                        userRole = "Executor";
                    }
                    else if (registerDto.UserType == "Customer")
                    {
                        userRole = "User"; // Customer использует роль "User"
                    }
                }
                
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Role = userRole,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _unitOfWork.Users.AddAsync(user);

                // 3. If Company info provided, create related entities
                if (!string.IsNullOrEmpty(registerDto.CompanyName))
                {
                    // 3.1 Create ContactPerson linked to this User (conceptually)
                    var contactPerson = new ContactPerson
                    {
                        Id = Guid.NewGuid(),
                        FirstName = registerDto.Username,
                        LastName = registerDto.Username,
                        MiddleName = "-",
                        Email = registerDto.Email,
                        PhoneNumber = registerDto.Phone ?? "",
                        Role = "Director"
                    };
                    await _unitOfWork.ContactPersons.AddAsync(contactPerson);

                    // 3.2 Create Company
                    var company = new Company
                    {
                        Id = Guid.NewGuid(),
                        Name = registerDto.CompanyName,
                        Inn = registerDto.Inn,
                        LegalForm = registerDto.LegalForm,
                        ContactPersonID = contactPerson.Id
                    };
                    await _unitOfWork.Companies.AddAsync(company);

                    // 3.3 Create Account (Tenant)
                    var statuses = await _unitOfWork.AccountStatuses.GetAllAsync();
                    var defaultStatus = statuses.FirstOrDefault()?.Id ?? Guid.Empty;
                    
                    if (defaultStatus == Guid.Empty)
                    {
                         var newStatus = new AccountStatus { Id = Guid.NewGuid(), Name = "New" };
                         await _unitOfWork.AccountStatuses.AddAsync(newStatus);
                         defaultStatus = newStatus.Id;
                    }

                    var account = new Account
                    {
                        Id = Guid.NewGuid(),
                        CompanyName = registerDto.CompanyName,
                        FoundingDate = DateTime.UtcNow,
                        AccountStatusId = defaultStatus
                    };
                    await _unitOfWork.Accounts.AddAsync(account);

                    // 3.4 Create Customer or Executor based on UserType
                    // ВАЖНО: Создаем ТОЛЬКО одну сущность - либо Customer, либо Executor, но не обе
                    if (string.IsNullOrWhiteSpace(registerDto.UserType))
                    {
                        throw new ArgumentException("UserType должен быть указан: 'Customer' или 'Executor'");
                    }
                    
                    if (registerDto.UserType == "Customer")
                    {
                        var customer = new Customer
                        {
                            Id = Guid.NewGuid(),
                            AccountId = account.Id,
                            CompanyId = company.Id
                        };
                        await _unitOfWork.Customers.AddAsync(customer);
                    }
                    else if (registerDto.UserType == "Executor")
                    {
                        // Проверяем, что Customer не существует для этого Account
                        var existingCustomers = await _unitOfWork.Customers.GetCustomersByCompanyAsync(company.Id);
                        var customerWithSameAccount = existingCustomers.FirstOrDefault(c => c.AccountId == account.Id);
                        if (customerWithSameAccount != null)
                        {
                            throw new InvalidOperationException($"Для аккаунта {account.Id} уже существует Customer (ID: {customerWithSameAccount.Id}). Нельзя создать Executor для аккаунта с Customer. Сначала удалите Customer или используйте другой аккаунт.");
                        }
                        
                        // Также проверяем, что Executor еще не существует для этого Account
                        var existingExecutors = await _unitOfWork.Executors.GetAllAsync();
                        var executorWithSameAccount = existingExecutors.FirstOrDefault(e => e.AccountId == account.Id && e.CompanyId == company.Id);
                        if (executorWithSameAccount != null)
                        {
                            throw new InvalidOperationException($"Для аккаунта {account.Id} уже существует Executor (ID: {executorWithSameAccount.Id}).");
                        }
                        
                        var executor = new Executor
                        {
                            Id = Guid.NewGuid(),
                            AccountId = account.Id,
                            CompanyId = company.Id,
                            CompletedOrders = 0
                        };
                        await _unitOfWork.Executors.AddAsync(executor);
                    }
                    else
                    {
                        throw new ArgumentException($"Неверный UserType: '{registerDto.UserType}'. Должен быть 'Customer' или 'Executor'");
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // 4. Generate Token
                var token = _jwtService.GenerateToken(user);
                var expirationHours = Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24");

                return new AuthResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Поиск пользователя
            var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.Username)
                ?? throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");

            // Проверка пароля
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");

            // Проверка активности
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Учетная запись деактивирована");

            // Обновление времени последнего входа
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Генерация токена
            var token = _jwtService.GenerateToken(user);
            var expirationHours = Convert.ToDouble(_configuration["Jwt:ExpirationHours"] ?? "24");

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                ExpiresAt = DateTime.UtcNow.AddHours(expirationHours)
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            // Возвращаем всех, так как мы теперь удаляем физически
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            if (!string.IsNullOrWhiteSpace(updateDto.Username))
            {
                var existing = await _unitOfWork.Users.GetByUsernameAsync(updateDto.Username);
                if (existing != null && existing.Id != userId)
                    throw new InvalidOperationException("Пользователь с таким именем уже существует");
                user.Username = updateDto.Username;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Email))
            {
                var existing = await _unitOfWork.Users.GetByEmailAsync(updateDto.Email);
                if (existing != null && existing.Id != userId)
                    throw new InvalidOperationException("Пользователь с таким email уже существует");
                user.Email = updateDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Role))
            {
                var allowedRoles = new[] { "User", "Admin", "Manager", "Executor" };
                if (!allowedRoles.Contains(updateDto.Role))
                    throw new InvalidOperationException($"Недопустимая роль: {updateDto.Role}");
                user.Role = updateDto.Role;
            }

            if (updateDto.IsActive.HasValue)
                user.IsActive = updateDto.IsActive.Value;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task<UserDto> ToggleUserActiveAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            user.IsActive = !user.IsActive;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
