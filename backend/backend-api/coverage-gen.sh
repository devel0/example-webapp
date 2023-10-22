#!/usr/bin/env bash

exdir="$(dirname `readlink -f "$0"`)"

cd "$exdir"

dotnet test ../backend-tests/integration-tests \
	/p:CollectCoverage=true \
	/p:CoverletOutputFormat=\"opencover,lcov\" \
	/p:CoverletOutput=../../backend-api/lcov
