#!/usr/bin/env bash
# Avvia l'app e controlla che parta e che il login dei tre profili funzioni.
# Va lanciato dopo `npm install` e dopo la compilazione (`dotnet build`) nella stessa configurazione.
#
# Uso in locale, dalla radice del repository:
#   bash .github/scripts/prova-avvio.sh
# Con la compilazione Release:
#   CONFIGURAZIONE=Release bash .github/scripts/prova-avvio.sh

set -euo pipefail

CONFIGURAZIONE="${CONFIGURAZIONE:-Debug}"
URL="http://localhost:5178"
PASSWORD="Prova123"

LOG="$(mktemp)"
BARATTOLO="$(mktemp)"

cd "$(dirname "$0")/../../src/PortPlanner.Web"

ASPNETCORE_ENVIRONMENT=Development dotnet run --no-build --configuration "$CONFIGURAZIONE" --urls "$URL" >"$LOG" 2>&1 &
APP=$!

# Alla fine, in ogni caso, si ferma l'app.
ferma() {
  kill "$APP" 2>/dev/null || true
  wait "$APP" 2>/dev/null || true
}
trap ferma EXIT

fallisci() {
  echo "ERRORE: $1" >&2
  echo "--- log dell'app ---" >&2
  cat "$LOG" >&2
  exit 1
}

# Aspetta che l'app risponda (al massimo due minuti).
codice=000
for _ in $(seq 1 60); do
  codice="$(curl -s -o /dev/null -w '%{http_code}' "$URL/Login/Login" || true)"
  [ "$codice" = "200" ] && break
  sleep 2
done
[ "$codice" = "200" ] || fallisci "la pagina di login non risponde (ultimo codice: $codice)"
echo "L'app risponde."

# Entra con un utente e restituisce il contenuto della Dashboard.
accedi() {
  local email="$1" password="$2"
  : >"$BARATTOLO"
  local pagina token
  pagina="$(curl -s -c "$BARATTOLO" "$URL/Login/Login")"
  token="$(echo "$pagina" | grep -o 'name="__RequestVerificationToken" type="hidden" value="[^"]*"' | sed 's/.*value="//;s/"$//' | head -1)"
  [ -n "$token" ] || fallisci "token antiforgery non trovato nella pagina di login"

  curl -s -b "$BARATTOLO" -c "$BARATTOLO" -o /dev/null -w '%{redirect_url}' \
    --data-urlencode "__RequestVerificationToken=$token" \
    --data-urlencode "Email=$email" \
    --data-urlencode "Password=$password" \
    --data-urlencode "RememberMe=false" \
    "$URL/Login/Login"
}

# Un utente per profilo: il profilo deve comparire nella Dashboard.
controlla_profilo() {
  local email="$1" profilo="$2" destinazione
  destinazione="$(accedi "$email" "$PASSWORD")"
  case "$destinazione" in
    */Dashboard/Index) ;;
    *) fallisci "$email: dopo il login la destinazione è '$destinazione' invece della Dashboard" ;;
  esac
  local dashboard
  dashboard="$(curl -s -b "$BARATTOLO" "$URL/Dashboard/Index")"
  echo "$dashboard" | grep -q "<dd class=\"col-sm-8\">$profilo</dd>" \
    || fallisci "$email: la Dashboard non mostra il profilo $profilo"
  echo "Login ok: $email ($profilo)"
}

controlla_profilo "mario.rossi@portplanner.it" "Supervisore"
controlla_profilo "anna.conti@portplanner.it" "Capogruppo"
controlla_profilo "sara.fontana@portplanner.it" "Dipendente"

# Una password sbagliata non deve far entrare.
destinazione="$(accedi "mario.rossi@portplanner.it" "Sbagliata")"
case "$destinazione" in
  */Dashboard/Index) fallisci "con la password sbagliata si è entrati" ;;
esac
echo "Login rifiutato con la password sbagliata."

# Senza login la Dashboard rimanda al login.
destinazione="$(curl -s -o /dev/null -w '%{redirect_url}' "$URL/Dashboard/Index")"
case "$destinazione" in
  */Login/Login*) ;;
  *) fallisci "la Dashboard senza login non rimanda al login ('$destinazione')" ;;
esac
echo "Dashboard protetta."

grep -q -i -E "fail|exception|crit" "$LOG" && fallisci "il log dell'app contiene errori" || true
echo "Tutto a posto."
