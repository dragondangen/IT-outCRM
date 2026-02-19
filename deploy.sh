#!/bin/bash
set -e

echo "========================================="
echo "  IT-outCRM — Deployment to Production"
echo "========================================="

# Check that .env file exists
if [ ! -f .env ]; then
    echo "ERROR: .env file not found!"
    echo "Copy .env.example to .env and fill in real values:"
    echo "  cp .env.example .env"
    echo "  nano .env"
    exit 1
fi

# Load environment variables
source .env

# Validate required variables
if [ "$POSTGRES_PASSWORD" = "CHANGE_ME_STRONG_PASSWORD_HERE" ] || [ -z "$POSTGRES_PASSWORD" ]; then
    echo "ERROR: Set a real POSTGRES_PASSWORD in .env"
    exit 1
fi

if [ "$JWT_KEY" = "CHANGE_ME_MIN_32_CHARS_RANDOM_SECRET_KEY_HERE" ] || [ -z "$JWT_KEY" ]; then
    echo "ERROR: Set a real JWT_KEY in .env (min 32 characters)"
    exit 1
fi

echo ""
echo "[1/4] Pulling base images..."
docker compose -f docker-compose.production.yml pull postgres nginx certbot

echo ""
echo "[2/4] Building application images..."
docker compose -f docker-compose.production.yml build --no-cache api blazor

echo ""
echo "[3/4] Starting services..."
docker compose -f docker-compose.production.yml up -d

echo ""
echo "[4/4] Waiting for services to be healthy..."
sleep 10

echo ""
echo "Service status:"
docker compose -f docker-compose.production.yml ps

echo ""
echo "========================================="
echo "  Deployment complete!"
echo "  Site: http://it-out-crm.ru"
echo "========================================="
echo ""
echo "To set up SSL (HTTPS), run:"
echo "  ./ssl-init.sh"
