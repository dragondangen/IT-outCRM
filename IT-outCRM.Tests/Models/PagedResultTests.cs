using FluentAssertions;
using IT_outCRM.Application.DTOs.Common;

namespace IT_outCRM.Tests.Models;

public class PagedResultTests
{
    [Fact]
    public void TotalPages_CalculatesCorrectly()
    {
        var result = new PagedResult<string>
        {
            TotalCount = 25,
            PageSize = 10
        };

        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public void TotalPages_WhenExactlyDivisible_CalculatesCorrectly()
    {
        var result = new PagedResult<string>
        {
            TotalCount = 20,
            PageSize = 10
        };

        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public void TotalPages_WhenZeroItems_ReturnsZero()
    {
        var result = new PagedResult<string>
        {
            TotalCount = 0,
            PageSize = 10
        };

        result.TotalPages.Should().Be(0);
    }

    [Fact]
    public void HasPreviousPage_OnFirstPage_ReturnsFalse()
    {
        var result = new PagedResult<string> { PageNumber = 1, PageSize = 10, TotalCount = 25 };

        result.HasPreviousPage.Should().BeFalse();
    }

    [Fact]
    public void HasPreviousPage_OnSecondPage_ReturnsTrue()
    {
        var result = new PagedResult<string> { PageNumber = 2, PageSize = 10, TotalCount = 25 };

        result.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void HasNextPage_OnLastPage_ReturnsFalse()
    {
        var result = new PagedResult<string> { PageNumber = 3, PageSize = 10, TotalCount = 25 };

        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public void HasNextPage_OnFirstPage_ReturnsTrue()
    {
        var result = new PagedResult<string> { PageNumber = 1, PageSize = 10, TotalCount = 25 };

        result.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void Items_DefaultsToEmptyList()
    {
        var result = new PagedResult<string>();

        result.Items.Should().NotBeNull();
        result.Items.Should().BeEmpty();
    }
}
