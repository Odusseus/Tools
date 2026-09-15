#!/bin/bash

set -u
set -o pipefail

###############################################################################
# Configuratie
###############################################################################

URL="https://anjacross.nl/"
EXPECTED_TEXT="Anja Cross"

# Alleen voor internetproblemen
ADMIN_EMAIL="mymail@example.com"

# Voor websiteproblemen
SITE_EMAILS=(
    "mymail@example.com"
    "anderepersoon@example.com"
)

REFERENCES=(
    "https://www.google.com/"
    "https://www.cloudflare.com/"
    "https://www.microsoft.com/"
)

CONNECT_TIMEOUT=5
REFERENCE_MAX_TIME=10
SITE_MAX_TIME=20

###############################################################################
# Bestanden naast het script
###############################################################################

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

SITE_STATEFILE="$SCRIPT_DIR/anjacross_down.state"
INTERNET_STATEFILE="$SCRIPT_DIR/internet_down.state"
LOGFILE="$SCRIPT_DIR/site-monitor.log"

###############################################################################
# Voorbereiding
###############################################################################

TIMESTAMP=$(date '+%Y-%m-%d %H:%M:%S')

touch "$LOGFILE" || {
    echo "FOUT: logbestand $LOGFILE kan niet worden aangemaakt." >&2
    exit 1
}

log() {
    printf '%s - %s\n' "$TIMESTAMP" "$1" >> "$LOGFILE"
}

if ! command -v curl >/dev/null 2>&1; then
    log "FOUT: curl niet gevonden"
    exit 1
fi

if ! command -v mail >/dev/null 2>&1; then
    log "FOUT: mail niet gevonden"
    exit 1
fi

###############################################################################
# Stap 1 - Internetverbinding controleren
###############################################################################

REFERENCE_OK=0
WORKING_REFERENCE=""

for REFERENCE_URL in "${REFERENCES[@]}"
do
    if curl \
        --silent \
        --show-error \
        --location \
        --connect-timeout "$CONNECT_TIMEOUT" \
        --max-time "$REFERENCE_MAX_TIME" \
        --output /dev/null \
        "$REFERENCE_URL" 2>>"$LOGFILE"
    then
        REFERENCE_OK=1
        WORKING_REFERENCE="$REFERENCE_URL"
        break
    fi
done

###############################################################################
# Internetstoring
###############################################################################

if [ "$REFERENCE_OK" -eq 0 ]; then

    log "Geen enkele referentiesite bereikbaar"

    if [ ! -f "$INTERNET_STATEFILE" ]; then

        if cat <<EOF | mail \
            -s "WAARSCHUWING: Internetverbinding Raspberry Pi uitgevallen" \
            "$ADMIN_EMAIL"
Internetverbinding niet beschikbaar.

Datum: $TIMESTAMP

Geen van de referentiesites was bereikbaar:

- ${REFERENCES[0]}
- ${REFERENCES[1]}
- ${REFERENCES[2]}

Controle van anjacross.nl is overgeslagen.
EOF
        then
            echo "$TIMESTAMP" > "$INTERNET_STATEFILE"
            log "Internetwaarschuwing verstuurd"
        else
            log "FOUT: internetwaarschuwing kon niet worden verstuurd"
        fi

    fi

    exit 1
fi

###############################################################################
# Internet hersteld
###############################################################################

if [ -f "$INTERNET_STATEFILE" ]; then

    if cat <<EOF | mail \
        -s "HERSTELD: Internetverbinding Raspberry Pi" \
        "$ADMIN_EMAIL"
Internetverbinding is weer beschikbaar.

Datum: $TIMESTAMP

Bereikbare referentiesite:
$WORKING_REFERENCE
EOF
    then
        rm -f "$INTERNET_STATEFILE"
        log "Internet herstelmelding verstuurd"
    fi

fi

###############################################################################
# Stap 2 - anjacross.nl controleren
###############################################################################

TMPFILE=$(mktemp "/tmp/anjacross-monitor.XXXXXX")

cleanup() {
    rm -f "$TMPFILE"
}

trap cleanup EXIT HUP INT TERM

HTTP_CODE=$(curl \
    --silent \
    --show-error \
    --location \
    --connect-timeout "$CONNECT_TIMEOUT" \
    --max-time "$SITE_MAX_TIME" \
    --output "$TMPFILE" \
    --write-out "%{http_code}" \
    "$URL" 2>>"$LOGFILE")

CURL_EXIT=$?

if [ "$CURL_EXIT" -ne 0 ]; then

    STATUS="CONNECTION_ERROR"
    DETAIL="curl foutcode $CURL_EXIT"

elif [[ ! "$HTTP_CODE" =~ ^[23][0-9][0-9]$ ]]; then

    STATUS="HTTP_ERROR"
    DETAIL="HTTP status $HTTP_CODE"

elif grep -Fqi -- "$EXPECTED_TEXT" "$TMPFILE"; then

    STATUS="OK"
    DETAIL="Tekst gevonden"

else

    STATUS="CONTENT_ERROR"
    DETAIL="Tekst niet gevonden"

fi

log "Status anjacross.nl: $STATUS ($DETAIL)"

###############################################################################
# Stap 3 - Website storing
###############################################################################

if [ "$STATUS" != "OK" ]; then

    if [ ! -f "$SITE_STATEFILE" ]; then

        if cat <<EOF | mail \
            -s "ALARM: anjacross.nl niet bereikbaar" \
            "${SITE_EMAILS[@]}"
Monitoring alarm

Website   : $URL
Datum     : $TIMESTAMP
Status    : $STATUS
HTTP-code : ${HTTP_CODE:-onbekend}
Details   : $DETAIL

Mogelijke oorzaken:
- Website offline
- Cloudflare Tunnel offline
- ASP.NET Core applicatie gestopt
- HTTP fout
- Verwachte tekst '$EXPECTED_TEXT' niet gevonden
EOF
        then
            echo "$TIMESTAMP" > "$SITE_STATEFILE"
            log "Website alarm verstuurd"
        else
            log "FOUT: website alarm kon niet worden verstuurd"
        fi

    fi

    exit 1
fi

###############################################################################
# Website hersteld
###############################################################################

if [ -f "$SITE_STATEFILE" ]; then

    if cat <<EOF | mail \
        -s "HERSTELD: anjacross.nl weer beschikbaar" \
        "${SITE_EMAILS[@]}"
Website weer beschikbaar

Website   : $URL
Datum     : $TIMESTAMP
Status    : OK
HTTP-code : $HTTP_CODE

Controle:
- Internetverbinding OK
- HTTP status OK
- Inhoud gecontroleerd
- Tekst '$EXPECTED_TEXT' gevonden
EOF
    then
        rm -f "$SITE_STATEFILE"
        log "Website herstelmelding verstuurd"
    fi

fi

exit 0