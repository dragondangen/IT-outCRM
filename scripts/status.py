import paramiko
client = paramiko.SSHClient()
client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
client.connect("155.212.209.119", username="root", password="Xr%YjBiCeC3a", timeout=60, banner_timeout=60)

cmds = [
    "uptime",
    "docker ps --format 'table {{.Names}}\\t{{.Status}}'",
    "docker logs itoutcrm-blazor --tail 15 2>&1",
    "curl -sf -o /dev/null -w '%{http_code}' -k https://it-out-crm.ru/ || echo FAIL",
]
for cmd in cmds:
    print(f"\n>>> {cmd}")
    stdin, stdout, stderr = client.exec_command(cmd, timeout=30)
    stdout.channel.settimeout(30)
    try:
        print(stdout.read().decode().strip())
    except:
        print("TIMEOUT")
client.close()
