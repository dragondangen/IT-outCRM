import paramiko
import time

client = paramiko.SSHClient()
client.set_missing_host_key_policy(paramiko.AutoAddPolicy())
client.connect("155.212.209.119", username="root", password="Xr%YjBiCeC3a", timeout=120, banner_timeout=120)

def run(cmd, timeout=120):
    print(f"\n>>> {cmd}")
    try:
        stdin, stdout, stderr = client.exec_command(cmd, timeout=timeout)
        stdout.channel.settimeout(timeout)
        out = stdout.read().decode().strip()
        err = stderr.read().decode().strip()
        if out:
            print(out)
        if err:
            print(f"ERR: {err}")
        return out
    except Exception as e:
        print(f"TIMEOUT: {e}")
        return ""

run("uptime")
run("docker ps -a --format 'table {{.Names}}\\t{{.Status}}'")

# Check nginx logs
run("docker logs itoutcrm-nginx --tail 30 2>&1")

# Check what config nginx is using
run("docker exec itoutcrm-nginx cat /etc/nginx/conf.d/default.conf 2>&1 | head -30")

# Check if SSL certs exist
run("docker exec itoutcrm-nginx ls -la /etc/letsencrypt/live/ 2>&1")
run("docker exec itoutcrm-nginx ls -la /etc/letsencrypt/live/it-out-crm.ru/ 2>&1")

# Check nginx test
run("docker exec itoutcrm-nginx nginx -t 2>&1")

# Check deploy log
run("tail -30 /var/log/crm-deploy.log 2>/dev/null")

# Check what's currently on disk
run("ls -la /opt/it-outcrm/nginx/")
run("head -5 /opt/it-outcrm/nginx/nginx.conf")

# Test internal connectivity
run("curl -sf -o /dev/null -w '%{http_code}' http://localhost:80/ 2>&1 || echo 'HTTP_FAIL'")

client.close()
