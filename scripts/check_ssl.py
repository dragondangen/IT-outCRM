import paramiko
client = paramiko.SSHClient()
client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
client.connect("155.212.209.119", username="root", password="Xr%YjBiCeC3a", timeout=60, banner_timeout=60)

cmds = [
    "docker exec itoutcrm-nginx ls -la /etc/letsencrypt/live/it-out-crm.ru/ 2>&1",
    "docker exec itoutcrm-nginx cat /etc/nginx/conf.d/default.conf | head -20",
    "docker exec itoutcrm-nginx nginx -t 2>&1",
    "curl -vk https://it-out-crm.ru/ 2>&1 | head -20",
]

for cmd in cmds:
    print(f"\n>>> {cmd}")
    stdin, stdout, stderr = client.exec_command(cmd, timeout=30)
    stdout.channel.settimeout(30)
    print(stdout.read().decode().strip())

client.close()
