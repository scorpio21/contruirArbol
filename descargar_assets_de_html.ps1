<#
 Descarga TODOS los gráficos que usa una página guardada de Guildlands (HTML + CSS), conservando las rutas.
 Sobrescribe lo que ya tengas (por si el juego cambió el diseño pero no el nombre del archivo).

 Uso (desde la carpeta del proyecto, donde está guildlands_assets):
   powershell -ExecutionPolicy Bypass -File .\descargar_assets_de_html.ps1 -Html "C:\ruta\Guildlands.html"
 Opciones: -SoloNuevos  (no vuelve a bajar lo que ya existe)
#>
param(
  [Parameter(Mandatory=$true)][string]$Html,
  [switch]$SoloNuevos
)
$dest = Join-Path $PSScriptRoot "guildlands_assets"
$txt  = [IO.File]::ReadAllText($Html, [Text.Encoding]::UTF8)
$rx   = [regex]'https://assets\.guildlands\.com/assets/([A-Za-z0-9_\-./\[\]]+\.(?:png|webp|jpg|jpeg|gif|svg|ttf|woff2?))'
$rutas = $rx.Matches($txt) | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique
Write-Host "Encontrados $($rutas.Count) archivos distintos"
$ok = 0; $mal = 0; $saltados = 0
foreach ($r in $rutas) {
  $out = Join-Path $dest $r
  if ($SoloNuevos -and (Test-Path $out)) { $saltados++; continue }
  New-Item -ItemType Directory -Force (Split-Path $out) | Out-Null
  try { Invoke-WebRequest ("https://assets.guildlands.com/assets/" + $r) -OutFile $out -ErrorAction Stop; $ok++; Write-Host "OK    $r" }
  catch { $mal++; Write-Host "FALLO $r" -ForegroundColor Yellow }
}
Write-Host "`nListo: $ok bajados, $mal fallidos, $saltados saltados."
