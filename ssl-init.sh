#!/bin/bash
set -e

DOMAIN="it-out-crm.ru"
EMAIL="admin@it-out-crm.ru"

echo "========================================="
echo "  SSL Certificate Setup (Let's Encrypt)"
echo "========================================="

echo ""
echo "[1/3] Requesting SSL certificate for $DOMAIN..."
docker compose -f docker-compose.production.yml run --rm certbot \
    certonly --webroot \
    --webroot-path=/var/www/certbot \
    --email "$EMAIL" \
    --agree-tos \
    --no-eff-email \
    -d "$DOMAIN" \
    -d "www.$DOMAIN"

echo ""
echo "[2/3] Switching nginx to SSL configuration..."
cp nginx/nginx-ssl.conf nginx/nginx.conf

echo ""
echo "[3/3] Reloading nginx..."
docker compose -f docker-compose.production.yml exec nginx nginx -s reload

echo ""
echo "========================================="
echo "  SSL setup complete!"
echo "  Site: https://$DOMAIN"
echo "========================================="
