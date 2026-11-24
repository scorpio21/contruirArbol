# Descarga los gráficos de la ficha de personaje (rutas leídas de las variables CSS de la captura de F12).
# Uso:  .\descargar_ui_personaje.ps1   (desde la carpeta del proyecto)
$base = "https://assets.guildlands.com/assets/"
$dest = Join-Path $PSScriptRoot "guildlands_assets"
$files = @(
 "ui/frames/wood-planks.png","ui/frames/wood-leaves.png","ui/frames/paper.png","ui/frames/parchment-small.png",
 "ui/frames/panel-parchment.png","ui/frames/nameplate-label.png","ui/frames/cloth-blue.png","ui/frames/paper-veined.png",
 "ui/frames/box-paper.png","ui/bars/experience-empty.png","ui/bars/experience-full.png",
 "ui/buttons/parchment-normal.png","ui/buttons/parchment-hover.png","ui/buttons/parchment-pressed.png",
 "ui/buttons/wood-normal.png","ui/buttons/wood-hover.png","ui/buttons/wood-pressed.png",
 "ui/grimorio/parchment-tile.png","ui/grimorio/parchment-card-tile.png","ui/grimorio/panel-wood.png",
 "ui/grimorio/slot-locked.png","ui/grimorio/locked-stripes.png","ui/grimorio/plaque-title.png",
 "ui/grimorio/section-header.png","ui/grimorio/separator-line.png","ui/grimorio/separator-gem.png",
 "ui/grimorio/cell-black.png","ui/grimorio/row.png","ui/grimorio/row-hover.png","ui/grimorio/row-selected.png",
 "ui/grimorio/button-normal.png","ui/grimorio/button-hover.png","ui/grimorio/button-pressed.png",
 "ui/grimorio/tab-active.png","ui/grimorio/tab-inactive.png",
 "ui/frames/cloth-rivets.png","ui/frames/scroll.png","ui/frames/stone-blood.png",
 "items/atlas-armor.png","items/atlas-farm.png","items/atlas-helm.png","items/atlas-jewel.png","items/atlas-misc.png",
 "items/atlas-shield.png","items/atlas-weapon.png","items/cape-icons/cape_royal_blue.png","items/mount-icons/camel.png",
 "ui/prayers/atlas-prayers.png","ui/spells/atlas-spells-2.png","ui/effects/atlas-effects.png","npc/jungle/tiger.png"
)
foreach ($f in $files) {
  $out = Join-Path $dest $f
  New-Item -ItemType Directory -Force (Split-Path $out) | Out-Null
  try { Invoke-WebRequest ($base + $f) -OutFile $out -ErrorAction Stop; Write-Host "OK   $f" }
  catch { Write-Host "FALLO $f  (anótalo y dímelo)" -ForegroundColor Yellow }
}
