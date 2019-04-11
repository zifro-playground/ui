#!/bin/bash

PROJECT=${1?Project path}

# Gather Playground UI version
assemblyInfoPath="$(find "$PROJECT" -name ZifroPlaygroundUIAssemblyInfo.cs)"

if ! [ -f "$assemblyInfoPath" ]
then
    echo "Error: AssemblyInfo.cs file not found"
    exit 1
fi

assemblyVersionAttribute="$(grep AssemblyVersion "$assemblyInfoPath" | tr -d '\r')"

if ! [[ "$assemblyVersionAttribute" ]]
then
    echo "Error: AssemblyVersion attribute not found in '$assemblyInfoPath'"
    exit 1
fi

regex='AssemblyVersion\("([[:digit:].]+)"\)'

if ! [[ $assemblyVersionAttribute =~ $regex ]]
then
    echo "Error: version not extracted from attribute '$assemblyVersionAttribute'"
    exit 1
fi

version="${BASH_REMATCH[1]}"

if [[ "$version" ]]
then
    echo "$version"
    exit 0
else
    echo "Error: Unable to extract version from attribute."
    exit 1
fi
