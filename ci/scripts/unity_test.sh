#!/usr/bin/env bash

# Set error flags
set -o nounset
set +o errexit
set -o pipefail

PROJECT=${1?Project path}
TEST_RESULTS_FOLDER=${2:-$PROJECT}
PLATFORMS="editmode\nplaymode"

passed=$((0))
failed=$((0))
skipped=$((0))
total=$((0))

while read platform
do
  echo
  echo ">>>>>> Executing Unity '$platform' tests"

  test_results_file=$TEST_RESULTS_FOLDER/test-results-$platform.xml
  echo "Results file: $test_results_file"
  echo

  ${UNITY_EXECUTABLE:-xvfb-run -as '-screen 0 640x480x24' /opt/Unity/Editor/Unity} \
      -projectPath $PROJECT \
      -runTests \
      -testPlatform $platform \
      -testResults $test_results_file \
      -buildTarget Linux \
      -batchmode \
      -logfile

  UNITY_EXIT_CODE=$?

  echo
  echo "<<<<<< Unity '$platform' execution complete"
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
  echo ">>>>>> Gathering '$platform' test results"
  echo

  regexTestsPassed='passed="([0-9]+)"'
  regexTestsFailed='failed="([0-9]+)"'
  regexTestsInconc='inconclusive="([0-9]+)"'
  regexTestsSkippe='skipped="([0-9]+)"'

  results="$(cat $test_results_file | grep "<test-run")"

  old_passed=$((passed))
  [[ $results =~ $regexTestsPassed ]] && [[ "${BASH_REMATCH[1]}" ]] && ((passed+=${BASH_REMATCH[1]}))
  echo "Passed: $passed (+$((passed-old_passed)))"
  old_failed=$((failed))
  [[ $results =~ $regexTestsInconc ]] && [[ "${BASH_REMATCH[1]}" ]] && ((failed+=${BASH_REMATCH[1]}))
  [[ $results =~ $regexTestsFailed ]] && [[ "${BASH_REMATCH[1]}" ]] && ((failed+=${BASH_REMATCH[1]}))
  echo "Failed: $failed (+$((failed-old_failed)))"
  old_skipped=$((skipped))
  [[ $results =~ $regexTestsSkippe ]] && [[ "${BASH_REMATCH[1]}" ]] && ((skipped+=${BASH_REMATCH[1]}))
  echo "Skipped: $skipped (+$((skipped-old_skipped)))"
  echo

done < <(echo -e "$PLATFORMS")

((total=passed+failed+skipped))

echo "Total: $total tests"

echo "export TEST_PASSED=$(($passed))" >> $BASH_ENV
echo "export TEST_FAILED=$(($failed))" >> $BASH_ENV
echo "export TEST_SKIPPED=$(($skipped))" >> $BASH_ENV
echo "export TEST_TOTAL=$(($total))" >> $BASH_ENV

exit $UNITY_EXIT_CODE