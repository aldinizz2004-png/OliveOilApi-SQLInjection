# Runs the same inputs against the vulnerable and the secure endpoint (local only).
$base = "http://localhost:5149/api/OliveOilProducts"
$inputs = @(
  "Nabali",
  "' OR 1=1 --",
  "xyz' OR Region = 'Jenin' --",
  "' UNION SELECT object_id, name, 'sys.tables', 0, 0 FROM sys.tables --"
)
foreach ($t in $inputs) {
  $q = [uri]::EscapeDataString($t)
  foreach ($ep in @("search-vulnerable", "search-secure")) {
    try {
      $r = Invoke-RestMethod "$base/$ep`?term=$q"
      $names = ($r | ForEach-Object { "$($_.id):$($_.name)" }) -join ", "
      Write-Output ("[{0}] term=<{1}> -> {2} rows: {3}" -f $ep, $t, @($r).Count, $names)
    } catch {
      Write-Output ("[{0}] term=<{1}> -> ERROR {2}" -f $ep, $t, $_.Exception.Message)
    }
  }
}
