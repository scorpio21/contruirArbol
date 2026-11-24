namespace GuildlandsTree.App;

/// <summary>Manual de uso. El diseño (RichTextBox, botón Cerrar) está en ManualForm.Designer.cs.</summary>
public partial class ManualForm : Form
{
    public ManualForm()
    {
        InitializeComponent();
    }

    void ManualForm_Load(object? sender, EventArgs e)
    {
        if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        // Líneas con "# " = título de sección (negrita); "- " = viñeta.
        foreach (var raw in Texto.Split('\n'))
        {
            var line = raw.TrimEnd('\r');
            bool h = line.StartsWith("# ");
            rtbManual.SelectionFont = new Font("Segoe UI", h ? 12F : 10F, h ? FontStyle.Bold : FontStyle.Regular);
            rtbManual.SelectionColor = h ? Color.FromArgb(120, 70, 20) : Color.Black;
            rtbManual.AppendText((h ? line[2..] : line) + "\n");
        }
        rtbManual.Select(0, 0);
    }

    const string Texto = @"# 1. Qué hace el programa
Calcula el árbol de pasivas que más puntúa para la clase y el personaje que indiques, y lo dibuja con los gráficos del juego. Funciona con las 20 clases. El resultado es un enlace del planificador oficial (botón «Abrir en el navegador»).

El score es un modelo propio con TUS pesos: no es un dato del juego. Sirve para comparar builds entre sí con los mismos pesos y el mismo equipo.

# 2. Bloque «Personaje»
- Raza y Clase: elige de las listas. Al cambiar de clase se restablecen los pesos por defecto de esa clase.
- Nivel: el de tu personaje. El presupuesto de puntos del árbol es (nivel − 1) + mazmorras completadas.
- Mazmorras completadas: las que tengas hechas (cada una da 1 punto de árbol).
- Puntos repartidos FUE/AGI/INT/CAR/VIT: los puntos que TÚ has gastado en la ficha, sin contar base, equipo ni árbol. Debe sumar 2 × (nivel − 1); abajo aparece «Reparto: X / Y» en verde si cuadra.
  En el planificador están en el campo «Puestos» del panel Atributos.

# 3. Bloque «Equipo (suma de todas tus piezas)»
Suma lo que aportan TODAS tus piezas equipadas (mira el tooltip de cada una):
- FUE/AGI/INT/CAR/VIT de equipo: suma de atributos de las piezas.
- Daño físico y Daño mágico (arma, piezas): valores planos que suman las piezas.
- Maná y Vida: valores planos de las piezas.
- % Armadura, % Defensa, % Chance de crítico: suma de los porcentajes de las piezas.
Si lo dejas todo en 0 el programa calcula como si fueras sin equipo, y las cifras saldrán más bajas que en el juego.

# 4. Bloque «Pesos del objetivo»
Cada peso dice cuánto te importa un valor. El programa maximiza la suma de peso × log(valor).
- Daño físico / mágico: pon el principal de tu clase en 1 y el otro en 0.
- Vida y Armadura: aguante. Subir su peso da árboles más defensivos.
- Maná, Regeneración, Velocidad de lanzamiento, Enfriamiento, Ahorro de coste, Valor de +1 ranura: útiles sobre todo para lanzadores.
- Curación hecha y Evasión: ponlos solo si te importan.
- Elementos (fire=1;ice=0.5): reparto del daño elemental, para clases con glifos.
«Pesos por defecto de la clase» restablece valores razonables para la clase elegida. Son suposiciones mías, ajústalos a tu estilo de juego.

# 5. Bloque «Opciones»
- Forzar Mente de Engranajes (gnomo): solo para gnomos; obliga a pasar por ese nodo.
- Permitir piedras angulares: si lo desmarcas, el árbol no usará piedras angulares (máximo 2 y algunas se excluyen entre sí).
- Glifos que puedes fabricar (vapor, agua, brasa, luz): marca los que tengas. Se colocan en los nexos del árbol.
- Tiempo máximo (s): cuanto más tiempo, más posibilidades de encontrar un árbol mejor. 30 s suele bastar; para afinar prueba 120 s o más.

# 6. Generar un árbol
1. Rellena Personaje, Equipo y Pesos.
2. Pulsa «Generar árbol». Cancelar detiene la búsqueda y descarta el resultado.
3. Resultado: pestaña Árbol (dibujo), Resumen (estadísticas, pesos y equipo usados) y Nodos elegidos (lista).
4. Bajo «Enlace generado»: «Copiar enlace» y «Abrir en el navegador» (abre ese árbol en el planificador oficial).

# 7. Evaluar un enlace existente
Pega un enlace del planificador arriba y pulsa «Evaluar enlace». Se cargan raza, clase, nivel, mazmorras y reparto (el equipo NO viaja en el enlace: rellénalo tú antes) y se muestran sus estadísticas. Sirve para comparar tu build actual con el generado.

# 8. El árbol dibujado
- Arrastrar: mover. Rueda: zoom. Doble clic: ajustar a la vista.
- Pasa el ratón sobre un nodo para ver su nombre, tipo y efecto.
- Dorado = nodos elegidos; el resto aparece atenuado.

# 9. Menús
- Archivo → Salir: cierra el programa.
- Ayuda → Manual (F1): esta ventana. Ayuda → Acerca de…: versión de los datos.

# 10. Límites que debes conocer
- Los números de daño incluyen un factor ×1,14 observado en el juego, cuyo origen no conozco.
- La velocidad de ataque puede diferir unos puntos del juego.
- No se valoran los efectos condicionales (contra bichos con poca vida, armas concretas, auras…).
- El solver no demuestra que el árbol sea el óptimo absoluto: puede dar resultados ligeramente distintos entre ejecuciones.
- Solo están validadas contra el juego las clases Mago y Paladín.
- Si el juego cambia el árbol, hay que actualizar los datos (guildlands_tree_v5.json).
";
}
