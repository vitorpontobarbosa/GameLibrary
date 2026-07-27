#!/bin/sh
set -e

mkdir -p /results

# Clear stale outputs from prior runs. Deleting only needs write access to
# the directory (not the file itself), so this is safe even if leftover
# files were created by a differently-uid'd k6 image and can't be
# overwritten in place (permission denied), which also breaks the
# end-of-test summary since it depends on the --out stream succeeding.
rm -f /results/*-summary.json /results/*-raw.json.gz

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
