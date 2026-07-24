#!/bin/sh
set -e

mkdir -p /results

if [ -f /scripts/seed.js ]; then
    echo "==> Seeding data (seed.js)"
    k6 run /scripts/seed.js
fi

run_one() {
    file="$1"
    name=$(basename "$file" .js)
    echo "==> Running $file"
    k6 run \
        --summary-export="/results/${name}-summary.json" \
        --out "json=/results/${name}-raw.json.gz" \
        "$file"
}

status=0

if [ -n "$TEST_FILE" ]; then
    run_one "/scripts/tests/$TEST_FILE" || status=1
else
    for file in /scripts/tests/*.js; do
        [ -e "$file" ] || continue
        run_one "$file" || status=1
    done
fi

exit $status
