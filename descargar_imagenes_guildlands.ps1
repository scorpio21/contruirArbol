# Descarga las imágenes del planificador de Guildlands conservando la estructura de carpetas.
# Uso: clic derecho -> "Ejecutar con PowerShell", o desde una consola:  .\descargar_imagenes_guildlands.ps1
# Las imágenes quedan en .\guildlands_assets\...

$urls = @'
https://assets.guildlands.com/assets/ui/frames/paper.png
https://assets.guildlands.com/assets/ui/icons/atlas-icons.png?v=20261120b
https://assets.guildlands.com/assets/ui/logo-castle.png
https://assets.guildlands.com/assets/ui/passive-tree/background.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-heart.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-anvil.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-wall.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-oath.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-shadow.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-choir.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-tower.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-banner.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-sanctuary.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-ambush.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-deck.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-forest.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-arena.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-crusade.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-grove.png
https://assets.guildlands.com/assets/ui/passive-tree/zone-preserve.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-cross.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-spear.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-shield.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-guard.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-crescent.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-star.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-trident.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-cloister.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-prism.png
https://assets.guildlands.com/assets/ui/passive-tree/outskirt-clock.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-throne.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-colossus.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-bastion.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-gale.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-observatory.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-wound.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-host.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-tempest.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-oracle.png
https://assets.guildlands.com/assets/ui/passive-tree/confin-venom.png
https://assets.guildlands.com/assets/ui/icons/atlas-passives.png
https://assets.guildlands.com/assets/ui/frames/parchment-small.png
https://assets.guildlands.com/assets/head/head1.png
https://assets.guildlands.com/assets/head/head21.png
https://assets.guildlands.com/assets/head/head41.png
https://assets.guildlands.com/assets/head/head61.png
https://assets.guildlands.com/assets/head/head81.png
https://assets.guildlands.com/assets/head/head101.png
https://assets.guildlands.com/assets/body/armor000_01.png
https://assets.guildlands.com/assets/body/armor000_02.png
'@ -split "`r?`n" | Where-Object { $_.Trim() -ne '' }

$root = Join-Path (Get-Location) 'guildlands_assets'
$ok = 0; $fail = @()
foreach ($u in $urls) {
    $path = ([uri]$u).AbsolutePath -replace '^/assets/', ''      # p.ej. ui/passive-tree/zone-heart.png
    $dest = Join-Path $root $path
    New-Item -ItemType Directory -Force -Path (Split-Path $dest) | Out-Null
    try {
        Invoke-WebRequest -Uri $u -OutFile $dest -UseBasicParsing
        $ok++
        Write-Host "OK   $path"
    } catch {
        $fail += $u
        Write-Host "FALLO $path  ($($_.Exception.Message))" -ForegroundColor Red
    }
}
Write-Host ""
Write-Host "Descargadas: $ok de $($urls.Count)  ->  $root"
if ($fail.Count) { Write-Host "Fallidas:"; $fail | ForEach-Object { Write-Host "  $_" } }
