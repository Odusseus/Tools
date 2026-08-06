#!/bin/bash

URL="https://anjacross.nl/"
EMAIL="jouwnaam@example.com"

STATEFILE="/var/lib/site-monitor/anjacross_down"
LOGFILE="/var/log/site-monitor.log"

TIMESTAMP=$(date '+%Y-%m-%d %H:%M:%S')

HTTP_CODE=$(curl \
    --silent \
    --max-time 20 \
    --output /dev/null \
    --write-out "%{http_code}" \
    "$URL")

echo "$TIMESTAMP - HTTP status: $HTTP_CODE" >> "$LOGFILE"

if [ "$HTTP_CODE" != "200" ]; then

    if [ ! -f "$STATEFILE" ]; then

        echo "$TIMESTAMP" > "$STATEFILE"

        echo "$TIMESTAMP - ALARM verstuurd (status $HTTP_CODE)" \
            >> "$LOGFILE"

        echo "
Website: $URL
Datum: $TIMESTAMP
HTTP status: $HTTP_CODE
" | mail -s "ALARM: anjacross.nl niet bereikbaar" "$EMAIL"

    else

        echo "$TIMESTAMP - Site nog steeds niet bereikbaar (status $HTTP_CODE)" \
            >> "$LOGFILE"

    fi

else

    if [ -f "$STATEFILE" ]; then

        rm "$STATEFILE"

        echo "$TIMESTAMP - HERSTELD melding verstuurd" \
            >> "$LOGFILE"

        echo "
Website: $URL
Datum: $TIMESTAMP
Status: hersteld
" | mail -s "HERSTELD: anjacross.nl weer beschikbaar" "$EMAIL"

    fi

fi