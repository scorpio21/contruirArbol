using System.Text.Json;

namespace GuildlandsTree.Core;

public sealed class ClassDef
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public Dictionary<string, double> Attributes { get; set; } = new();
    public List<string> Focus { get; set; } = new();
}

public sealed class Perk { public string Stat { get; set; } = ""; public double Pct { get; set; } }

/// <summary>Tablas del motor de estadísticas del planificador (extraídas del bundle), una entrada por clase/raza.</summary>
public sealed class ClassData
{
    public Dictionary<string, ClassDef> Classes { get; set; } = new();
    public Dictionary<string, Perk> Perk { get; set; } = new();
    public Dictionary<string, Dictionary<string, double>> Ob { get; set; } = new();   // CAR -> stat
    public Dictionary<string, Dictionary<string, double>> Nb { get; set; } = new();   // VIT -> stat
    public Dictionary<string, double> Kb { get; set; } = new();                        // regeneración de vida por VIT
    public Dictionary<string, Dictionary<string, double>> Fb { get; set; } = new();    // INT -> stat
    public Dictionary<string, Dictionary<string, double>> Vb { get; set; } = new();    // AGI -> stat
    public Dictionary<string, double> Mb { get; set; } = new();                        // penalización vida/armadura
    public Dictionary<string, string> PassiveAdd { get; set; } = new();
    public Dictionary<string, string> PassiveMul { get; set; } = new();
    public Dictionary<string, Dictionary<string, double>> AttrCoef { get; set; } = new();
    public Dictionary<string, double> Base { get; set; } = new();
    public Dictionary<string, double> PerLevel { get; set; } = new();
    public Dictionary<string, double> Caps { get; set; } = new();
    public Dictionary<string, Dictionary<string, double>> RaceMod { get; set; } = new();
    public Dictionary<string, double> Constants { get; set; } = new();
    public List<string> Stats { get; set; } = new();

    public static ClassData Current { get; private set; } = new();

    public static ClassData Load(string path)
    {
        var d = JsonSerializer.Deserialize<ClassData>(File.ReadAllText(path),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("guildlands_classes.json vacío");
        Current = d;
        return d;
    }
}
