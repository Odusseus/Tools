#!/bin/bash

URL="https://anjacross.nl"
STATUS=$(curl --head --silent --write-out "%{http_code}" --output /dev/null "$URL")
STATUSFILE="/home/pascal/site.status"

PREVSTATUS=$(cat "$STATUSFILE")

# Website is DOWN
if [[ "$STATUS" -ne 200 ]]; then
    if [[ "$PREVSTATUS" != "DOWN" ]]; then
        {
            echo "ALERT: Website is offline!"
            echo "URL: $URL"
            echo "Status: $STATUS"
            echo "Tijd: $(date)"
            echo ""
            echo "Laatste logregels:"
            tail -n 20 /home/pascal/sitecheck.log
        } | mail -s "ALERT: Website offline!" pascalboittin@gmail.com info@anjacross.nl

        echo "DOWN" > "$STATUSFILE"
    fi
fi

# Website is UP
if [[ "$STATUS" -eq 200 ]]; then
    if [[ "$PREVSTATUS" != "UP" ]]; then
        {
            echo "RESOLVED: Website is weer online!"
            echo "URL: $URL"
            echo "Status: $STATUS"
            echo "Tijd: $(date)"
            echo ""
            echo "Laatste logregels:"
            tail -n 20 /home/pascal/sitecheck.log
        } | mail -s "RESOLVED: Website online!" pascalboittin@gmail.com info@anjacross.nl

        echo "UP" > "$STATUSFILE"
    fi
fi
