#!/usr/bin/env bash
# Runs the suite with Microsoft Code Coverage, turns the Cobertura file into a summary with ReportGenerator,
# and fails when line coverage of src/Checkout is below 90% or branch coverage below 100%. Extra arguments go to dotnet test,
# for example: ./coverage.sh --filter "TestCategory=Theater"
set -euo pipefail
cd "$(dirname "$0")"
rm -rf TestResults coverage-report

dotnet test --results-directory TestResults \
  --coverage --coverage-output-format cobertura --coverage-output coverage.cobertura.xml "$@"

dotnet tool restore > /dev/null
dotnet reportgenerator \
  -reports:TestResults/coverage.cobertura.xml \
  -targetdir:coverage-report \
  "-reporttypes:TextSummary;Html" \
  -verbosity:Warning \
  minimumCoverageThresholds:lineCoverage=90 \
  minimumCoverageThresholds:branchCoverage=100 \
  || { cat coverage-report/Summary.txt; echo "FAILED: coverage is below the threshold (lines 90%, branches 100%)."; exit 1; }

cat coverage-report/Summary.txt
echo "PASSED: coverage meets the threshold. Open coverage-report/index.html to see each line."
