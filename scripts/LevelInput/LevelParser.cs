using Godot;
using System.Text.Json;
using Mario3D.scripts.Voxel;

namespace Mario3D.scripts.LevelInput;

[GlobalClass]
public partial class LevelParser : Resource
{
    private LevelDescription _description;

    public LevelParser()
    {
        _description = new LevelDescription();
    }

    public LevelDescription ParseLevelDescription(VoxelDatabase voxels)
    {
        const string path = "res://data/level.json";
        _description = LoadJson<LevelDescription>(path);

        var levelHeight = _description.LevelRows / 16;

        foreach (var square in _description.Static.PositionCollisions)
        {
            for (var inX = 0; inX < square.Width / 16; inX++)
            {
                for (var inY = 0; inY < square.Height / 16; inY++)
                {
                    var position = new Vector3I(square.X / 16 + inX, levelHeight - square.Y / 16 + inY, 0);
                    voxels.SetVoxel(position, 1);
                }
            }
        }

        return _description;
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