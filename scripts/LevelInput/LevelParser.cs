using System.Text.Json;
using Godot;
using Mario3D.scripts.Voxel.Database;

namespace Mario3D.scripts.LevelInput;

[GlobalClass]
public partial class LevelParser : Resource
{
    private LevelDescription _description = new();

    public LevelDescription ParseLevelDescription(VoxelDatabase voxels, int voxelSize)
    {
        const string path = "res://data/level.json";
        _description = LoadJson<LevelDescription>(path);

        var levelHeight = _description.LevelRows / voxelSize;

        foreach (var square in _description.Static.PositionCollisions)
        {
            ParseSquare(square, voxels, levelHeight, voxelSize);
        }

        foreach (var square in _description.Static.Pipes)
        {
            ParseSquare(square, voxels, levelHeight, voxelSize);
        }

        _description.Static.Spawn.Y = _description.LevelRows - _description.Static.Spawn.Y;
        _description.Static.End.Y = _description.LevelRows - _description.Static.End.Y;

        return _description;
    }

    private static void ParseSquare(Square square, VoxelDatabase voxels, int levelHeight, int voxelSize)
    {
        for (var inX = 0; inX < square.Width / voxelSize; inX++)
        {
            for (var inY = 0; inY < square.Height / voxelSize; inY++)
            {
                var position = new Vector3I(square.X / voxelSize + inX, levelHeight - square.Y / voxelSize - inY, 0);
                voxels[position] = 1;
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