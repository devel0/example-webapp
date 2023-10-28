#!/usr/bin/env bash

exdir=$(dirname $(readlink -f "$0"))

function genJwt() {
    dotnet script eval 'using System.Security.Cryptography; using System.Text; var rsaKey = RSA.Create(); var privateKey = rsaKey.ExportRSAPrivateKey(); System.Console.WriteLine(Convert.ToBase64String(privateKey));'
}

genJwt