$failed = @()
Get-ChildItem tests -Filter *.csproj -Recurse | Where-Object { $_.Name -notmatch 'UI' } | ForEach-Object {
    $name = $_.BaseName
    $output = dotnet test $_.FullName --verbosity minimal 2>&1 | Out-String
    if ($output -match 'Failed:\s+(\d+)') {
        $failCount = [regex]::Match($output, 'Failed:\s+(\d+)').Groups[1].Value
        if ([int]$failCount -gt 0) {
            $failed += "$name : Failed=$failCount"
        }
    }
    elseif ($output -match 'error CS') {
        $failed += "$name : BUILD FAILED"
    }
}
$failed | Out-File -FilePath test-failures.txt
if ($failed.Count -eq 0) { 'All passed' | Out-File -FilePath test-failures.txt }
