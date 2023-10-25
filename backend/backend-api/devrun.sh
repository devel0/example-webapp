#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$0"))

cd "$exdir"

dotnet run --environment Development --urls "http://localhost:5000"
