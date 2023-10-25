#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$0"))

cd "$exdir"

dotnet run --environment Production --urls "http://localhost:5000;https://localhost:5001"
