using System.Collections.Generic;
using Godot;

namespace Mario3D.scripts;

[GlobalClass]
public partial class LevelGenerator : Node3D
{
    [Export] public LevelRenderer LevelRenderer { get; set; }
    [Export] public LevelPathfinder LevelPathfinder { get; set; }

    [Export, ExportGroup("Generation Parameter")]
    public int LevelWidth { get; set; } = 2;

    [Export, ExportGroup("Generation Parameter")]
    public FastNoiseLite Noise { get; set; }

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public void GenerateLevel(LevelDescription description, Dictionary<Vector3I, int> voxels)
    {
        for (var x = 0; x < description.LevelCols / 16; x++)
        {
            for (var y = 0; y < description.LevelRows / 16; y++)
            {
                var position = new Vector3I(x, y, 0);

                if (!voxels.TryGetValue(position, out var voxel) || voxel == 0) continue;

                for (var z = -LevelWidth; z < LevelWidth; z++)
                {
                    if (Noise.GetNoise3D(x, y, z) > 0.0F) continue;

                    voxels.TryAdd(new Vector3I(x, y, z), voxel);
                }
            }
        }

        LevelRenderer.DrawVoxels(voxels);

        var path = LevelPathfinder.FindPath(
            description.Static.Spawn.GetOrigin(),
            description.Static.End.GetOrigin(),
            voxels);
    }
}