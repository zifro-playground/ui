#!/bin/bash

PROJECT=${1?Project path}

# Gather Playground UI version
assemblyInfoPath="$(find "$PROJECT" -name ZifroPlaygroundUIAssemblyInfo.cs)"
echo "Looking at file \`$assemblyInfoPath\`"
assemblyVersionAttribute="$(grep AssemblyVersion "$assemblyInfoPath" | tr -d '\r')"
echo "Found attribute \`$assemblyVersionAttribute\`"

regex='AssemblyVersion\("([[:digit:].]+)"\)'
echo "Using RegEx: \`$regex\`"
[[ $assemblyVersionAttribute =~ $regex ]]
PLAYGROUND_UI_VERSION="${BASH_REMATCH[1]}"

if [[ "$PLAYGROUND_UI_VERSION" ]]
then
    echo "Successfully extracted version \`$PLAYGROUND_UI_VERSION\`"
    echo "Saving to envrionment variable \`\$PLAYGROUND_UI_VERSION\`"
    echo "export PLAYGROUND_UI_VERSION='${BASH_REMATCH[1]}'" >> $BASH_ENV
    exit 0
else
    echo "Unable to extract version from attribute."
    exit 1
fi
