#!/bin/sh
# Regenera assets/config.json a partir de variables de entorno del contenedor
# (API_URL, SIGNALR_URL) antes de que nginx sirva los estáticos. Esto permite usar
# la misma imagen en distintos entornos sin reconstruir el build de Angular.
set -eu

API_URL="${API_URL:-http://localhost:5080/api}"
SIGNALR_URL="${SIGNALR_URL:-http://localhost:5080/hubs/board}"

CONFIG_FILE=/usr/share/nginx/html/assets/config.json

cat > "$CONFIG_FILE" <<EOF
{
  "apiUrl": "${API_URL}",
  "signalrUrl": "${SIGNALR_URL}"
}
EOF

echo "assets/config.json generado: apiUrl=${API_URL} signalrUrl=${SIGNALR_URL}"
