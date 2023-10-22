#!/bin/bash

exdir=$(dirname `readlink -f "$0"`)

api_spec_url='http://localhost:5000/swagger/v3/swagger.json'

rm -fr "$exdir"/src/api
mkdir "$exdir"/src/api

cd $exdir

_JAVA_OPTS=""

# uncomment follow to disable trust remote openapi certificate
_JAVA_OPTS="-Dio.swagger.parser.util.RemoteUrl.trustAll=true -Dio.swagger.v3.parser.util.RemoteUrl.trustAll=true"

# NODE_TLS_REJECT_UNAUTHORIZED=0 \
#     npx @rtk-query/codegen-openapi openapi-config.json

JAVA_OPTS="$_JAVA_OPTS" \
npx @openapitools/openapi-generator-cli generate \
    -i "$api_spec_url" \
    -g typescript-axios \
    -o src/api
