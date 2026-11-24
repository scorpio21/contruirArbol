# Guildlands · Optimizador del árbol de pasivas (todas las clases)

Genera el árbol de pasivas óptimo (según pesos editables) para cualquier raza y clase, nivel y mazmorras.
Datos: árbol **v5** (`guildlands_tree_v5.json`, 1.222 nodos) y tablas de las 20 clases (`guildlands_classes.json`),
ambas extraídas del código del planificador. Si el juego cambia el árbol hay que regenerar esos archivos.

## Estado

| Parte | Estado |
|---|---|
| Motor genérico por clase + solver en Python (`gl_solver/glengine.py`) | Ejecutado; reproduce las capturas de un paladín (daño, vida, maná, armadura, crítico, curación) |
| Port a C# (`Core`, `Cli`, `App`) | **Escrito y revisado a mano, NO compilado por mí** (el entorno no tiene .NET). Hay que compilar y pegar los errores |
| Dibujo del árbol (`App/TreeCanvas.cs`) | Sin probar. Falta el icono de la cabeza de cada raza (nodo de inicio) |

## Compilar y probar

```
dotnet build -c Release
dotnet run --project Cli          # evaluación (segundos)
dotnet run --project Cli opt      # además optimiza el paladín (1-3 min)
dotnet run --project App          # interfaz WinForms
```

`Cli` debe imprimir (valores del prototipo Python):

```
tu build   score 12.713 | físico 123 mágico 49 vida 944 maná 284 armadura 83.8 crit 21.6% x154 velAtq 122 curación 14.3% evasión 7.6
mago       score 8.726  | mágico 139 vida 393 maná 884 armadura 48.6 crit 7.5% velAtq 101
```

Si no coincide, hay un error de port en `StatModel`.

## Cómo funciona

1. **Stats por clase** (`StatModel.Compute`): atributos (base de clase + raza + reparto + equipo + árbol) → estadísticas con las
   tablas del planificador (foco ×1,3 o ×1,15 con *Equilibrio de las Cinco*, vida de clase, bonos por nivel, perk de clase, topes).
2. **Objetivo**: Σ peso·log(stat). Los **pesos son suposiciones tuyas**, no datos del juego; cada clase trae unos por defecto.
3. **Solver**: CP-SAT. Conectividad por flujo desde el nodo de la raza; máx. 2 piedras angulares; familias excluyentes;
   grupos «elige uno»; glifos en nexos. El log-score se linealiza con gradiente numérico y se itera.
4. **Enlace**: `rc` = IDs en base 36 sin el nodo de inicio; `at` = FUE.AGI.INT.CAR.VIT en base 36.

## Límites conocidos (importan)

- **×1,14**: en tu juego el daño sale multiplicado por 1,14 con la etiqueta «Árbol de pasivas». No sé de dónde viene; lo trato como
  constante (no cambia qué árbol es mejor si no depende del árbol).
- No se valoran: efectos condicionales (contra bichos con poca/mucha vida, jefes), armas concretas (espada/hacha/arco...), auras,
  bloqueo/curación al bloquear, y piedras con reglas no lineales. Dales valor a mano con `KeyValue` si quieres.
- La velocidad de ataque del paladín sale 122 y en el juego 124 (diferencia sin explicar).
- Los datos son v5 del bundle anterior; el bundle nuevo retocó 15 nodos (p. ej. Fuente Serena y los +3 INT). Aún no regenerados.
- El solver no siempre demuestra optimalidad en el tiempo dado.
- La distribución de atributos (`at`) se toma como dato; no se optimiza.
