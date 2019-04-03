#!/usr/bin/env bash

# Set error flags
set -o nounset
set +o errexit
set -o pipefail

PROJECT=${1?Project path}
TEST_RESULTS=${2:-$PROJECT/test-results.xml}

echo ">>>>>> Executing Unity"
echo

${UNITY_EXECUTABLE:-xvfb-run -as '-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
    -projectPath $PROJECT \
    -runEditorTests \
    -editorTestsResultFile $TEST_RESULTS \
    -batchmode \
    -buildTarget Linux \
    -logfile

UNITY_EXIT_CODE=$?

echo
echo "<<<<<< Unity execution complete"
echo

if [ $UNITY_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
elif [ $UNITY_EXIT_CODE -eq 2 ]; then
  echo "Run succeeded, some tests failed";
elif [ $UNITY_EXIT_CODE -eq 3 ]; then
  echo "Run failure (other failure)";
else
  echo "Unexpected exit code $UNITY_EXIT_CODE";
  exit $UNITY_EXIT_CODE
fi

echo
echo ">>>>>> Gathering test results"
echo

passed=$((0))
failed=$((0))
skipped=$((0))
total=$((0))

regexTestsPassed='passed="([0-9]+)"'
regexTestsFailed='failed="([0-9]+)"'
regexTestsInconc='inconclusive="([0-9]+)"'
regexTestsSkippe='skipped="([0-9]+)"'

results="$(cat $TEST_RESULTS | grep "<test-run")"

[[ $results =~ $regexTestsPassed ]] && [[ "${BASH_REMATCH[1]}" ]] && ((passed=${BASH_REMATCH[1]}))
echo "Passed: $passed"
[[ $results =~ $regexTestsFailed ]] && [[ "${BASH_REMATCH[1]}" ]] && ((failed=${BASH_REMATCH[1]}))
[[ $results =~ $regexTestsInconc ]] && [[ "${BASH_REMATCH[1]}" ]] && ((failed+=${BASH_REMATCH[1]}))
echo "Failed: $failed"
[[ $results =~ $regexTestsSkippe ]] && [[ "${BASH_REMATCH[1]}" ]] && ((skipped=${BASH_REMATCH[1]}))
echo "Skipped: $skipped"

((total=passed+failed+skipped))

echo "Total: $total"

echo "export TEST_PASSED=$(($passed))" >> $BASH_ENV
echo "export TEST_FAILED=$(($failed))" >> $BASH_ENV
echo "export TEST_SKIPPED=$(($skipped))" >> $BASH_ENV
echo "export TEST_TOTAL=$(($total))" >> $BASH_ENV

exit $UNITY_EXIT_CODE