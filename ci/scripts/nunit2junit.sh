#!/usr/bin/env bash

INPUT_FOLDER=${1?Folder of input NUnit XML files}
OUTPUT_FOLDER=${2?Folder of output JUnit XML files}
: ${PWSH_SCRIPT:=/usr/local/bin/nunit2junit.ps1}

echo "Converting NUnit XML files from folder '$INPUT_FOLDER'"
echo "Outputting JUnit XML files into folder '$OUTPUT_FOLDER'"

(cd $INPUT_FOLDER && find . -name '*.xml') | cut -c 3- |
while read path
do
    folder="$(dirname "$path")"
    file="$(basename "$path")"
    echo "Working on '$folder' :: '$file'"

    abs_folder=$OUTPUT_FOLDER/$folder
    abs_file=$abs_folder/$file

    mkdir -p $abs_folder
    pwsh $PWSH_SCRIPT $path $abs_folder/$file
done

echo "Convertion complete"
