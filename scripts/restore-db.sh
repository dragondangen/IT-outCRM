#!/bin/bash
# PostgreSQL restore script for IT-outCRM
# Usage: ./restore-db.sh /opt/it-outcrm/backups/crm_backup_YYYYMMDD_HHMMSS.sql.gz

set -euo pipefail

BACKUP_FILE="$1"
CONTAINER_NAME="itoutcrm-postgres"

if [ -z "$BACKUP_FILE" ]; then
    echo "Usage: $0 <backup_file.sql.gz>"
    echo ""
    echo "Available backups:"
    ls -lh /opt/it-outcrm/backups/crm_backup_*.sql.gz 2>/dev/null || echo "  No backups found"
    exit 1
fi

if [ ! -f "$BACKUP_FILE" ]; then
    echo "Error: File not found: $BACKUP_FILE"
    exit 1
fi

echo "WARNING: This will overwrite the current database!"
read -p "Are you sure? (yes/no): " CONFIRM
if [ "$CONFIRM" != "yes" ]; then
    echo "Aborted."
    exit 0
fi

echo "[$(date)] Stopping API and Blazor containers..."
docker stop itoutcrm-api itoutcrm-blazor 2>/dev/null || true

echo "[$(date)] Restoring from: $BACKUP_FILE"
gunzip -c "$BACKUP_FILE" | docker exec -i "$CONTAINER_NAME" psql -U "$POSTGRES_USER"

echo "[$(date)] Starting containers..."
docker start itoutcrm-api itoutcrm-blazor

echo "[$(date)] Restore completed successfully."
