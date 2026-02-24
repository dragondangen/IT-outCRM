import paramiko
import time

client = paramiko.SSHClient()
client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
client.connect("155.212.209.119", username="root", password="Xr%YjBiCeC3a", timeout=60, banner_timeout=60)

def run(cmd, timeout=120):
    print(f"\n>>> {cmd}")
    stdin, stdout, stderr = client.exec_command(cmd, timeout=timeout)
    stdout.channel.settimeout(timeout)
    out = stdout.read().decode().strip()
    err = stderr.read().decode().strip()
    if out:
        print(out)
    if err:
        print(f"STDERR: {err}")
    return out

# Pull latest
run("cd /opt/it-outcrm && git fetch origin master && git reset --hard origin/master 2>&1")
run("cd /opt/it-outcrm && git log --oneline -1")

# Restart with new docker-compose (volumes for keys)
run("cd /opt/it-outcrm && docker compose -f docker-compose.production.yml up -d 2>&1")

time.sleep(15)

# Check status
run("docker ps --format 'table {{.Names}}\\t{{.Status}}'")

# Check blazor logs
run("docker logs itoutcrm-blazor --tail 10 2>&1")

# Test site
run("curl -sf -o /dev/null -w '%{http_code}' -k https://it-out-crm.ru/ || echo FAIL")
run("curl -sf -o /dev/null -w '%{http_code}' -k https://it-out-crm.ru/reports || echo FAIL")

client.close()
print("\n=== Done ===")
