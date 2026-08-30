using Godot;
using Mario3D.scripts.Voxel.Database;

namespace Mario3D.scripts.Generation;

[GlobalClass]
public partial class LevelGenerator : Resource
{
    [Export, ExportGroup("Generation Parameter")]
    public FastNoiseLite Noise;

    [Export] public bool EnableNoise = true;

    public LevelGenerator()
    {
        Noise ??= new FastNoiseLite();
    }

    public VoxelDatabase GenerateLevel(VoxelDatabase voxelDatabase, Vector3I levelSize)
    {
        var newDatabase = voxelDatabase.DuplicateEmpty();

        for (var x = 0; x < levelSize.X; x++)
        {
            for (var y = 0; y < levelSize.Y; y++)
            {
                var position = new Vector3I(x, y, 0);

                var voxel = voxelDatabase[position];

                if (voxel == 0) continue;

                for (var z = -levelSize.Z; z <= levelSize.Z; z++)
                {
                    if (EnableNoise && z != 0 && Noise.GetNoise3D(x, y, z) > 0f) continue;

                    newDatabase[x, y, z] = voxel;
                }
            }
        }

        return newDatabase;
    }
}