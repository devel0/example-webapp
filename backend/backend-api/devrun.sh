#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$0"))

cd "$exdir"

dotnet run --urls "http://localhost:5000"
