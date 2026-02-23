#!/bin/bash
# Auto-deploy script for IT-outCRM
# Pulls latest from GitHub and rebuilds Docker containers
# Usage: ./deploy.sh
# Cron (every 5 min): */5 * * * * /opt/it-outcrm/scripts/deploy.sh >> /var/log/crm-deploy.log 2>&1

set -euo pipefail

DEPLOY_DIR="/opt/it-outcrm"
LOCK_FILE="/tmp/crm-deploy.lock"
LOG_PREFIX="[$(date '+%Y-%m-%d %H:%M:%S')]"

# Prevent concurrent deploys
if [ -f "$LOCK_FILE" ]; then
    LOCK_AGE=$(($(date +%s) - $(stat -c %Y "$LOCK_FILE" 2>/dev/null || echo 0)))
    if [ "$LOCK_AGE" -lt 600 ]; then
        exit 0
    fi
    echo "$LOG_PREFIX WARNING: Stale lock file found (${LOCK_AGE}s old), removing"
    rm -f "$LOCK_FILE"
fi

cd "$DEPLOY_DIR" || exit 1

# Check for new commits
git fetch origin master --quiet 2>/dev/null

LOCAL=$(git rev-parse HEAD)
REMOTE=$(git rev-parse origin/master)

if [ "$LOCAL" = "$REMOTE" ]; then
    exit 0
fi

echo "$LOG_PREFIX New commits detected: $LOCAL -> $REMOTE"
touch "$LOCK_FILE"

cleanup() {
    rm -f "$LOCK_FILE"
}
trap cleanup EXIT

echo "$LOG_PREFIX Pulling latest changes..."
git pull origin master --quiet

echo "$LOG_PREFIX Building and restarting containers..."
docker compose -f docker-compose.production.yml build --no-cache api blazor
docker compose -f docker-compose.production.yml up -d api blazor

echo "$LOG_PREFIX Waiting for health check..."
sleep 10

if curl -sf http://localhost:8080/health > /dev/null 2>&1; then
    echo "$LOG_PREFIX Deploy successful! API is healthy."
else
    echo "$LOG_PREFIX WARNING: API health check failed, but containers are running."
fi

echo "$LOG_PREFIX Deploy completed. Current commit: $(git rev-parse --short HEAD)"
