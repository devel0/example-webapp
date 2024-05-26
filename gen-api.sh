#!/usr/bin/env bash

exdir=$(dirname `readlink -f "$0"`)

API_SPEC_URL="https://webapp-test.searchathing.com/swagger/v1/swagger.json"

APIFLD=$exdir/clientapp/api

rm -fr $APIFLD

mkdir $APIFLD

cd $exdir

npx @openapitools/openapi-generator-cli generate \
    -i "$API_SPEC_URL" \
    --skip-validate-spec \
    -g typescript-axios \
    -o $APIFLD
