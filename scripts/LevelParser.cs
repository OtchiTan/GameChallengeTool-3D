using System.Collections.Generic;
using Godot;
using System.Text.Json;

namespace Mario3D.scripts;

[GlobalClass]
public partial class LevelParser : Node
{
    private LevelDescription _description;

    private Octree _octree = new();
    private Dictionary<Vector3I, int> _voxels = new();

    [Export] public LevelGenerator LevelGenerator { get; set; }

    public override void _Ready()
    {
        const string path = "res://data/level.json";
        _description = LoadJson<LevelDescription>(path);

        ParseLevelDescription();

        LevelGenerator.GenerateLevel(_description, _voxels);
    }

    private void ParseLevelDescription()
    {
        foreach (var square in _description.Static.PositionCollisions)
        {
            for (var inX = 0; inX < square.Width / 16; inX++)
            {
                for (var inY = 0; inY < square.Height / 16; inY++)
                {
                    var position = new Vector3I(square.X / 16 + inX, square.Y / 16 + inY, 0);
                    _octree.Insert(position, 1);
                    _voxels.TryAdd(position, 1);
                }
            }
        }
    }


    private static T LoadJson<T>(string path)
    {
        if (!FileAccess.FileExists(path))
        {
            GD.PrintErr($"File not found : {path}");
            return default;
        }

        using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        var jsonString = file.GetAsText();

        try
        {
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };

            return JsonSerializer.Deserialize<T>(jsonString, options);
        }
        catch (JsonException e)
        {
            GD.PrintErr($"Error while parsing JSON for {path} : {e.Message}");
            return default;
        }
    }
}