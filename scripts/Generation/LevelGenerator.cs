using Godot;
using Mario3D.scripts.LevelInput;
using Mario3D.scripts.Voxel;

namespace Mario3D.scripts.Generation;

[GlobalClass]
public partial class LevelGenerator : Resource
{
    [Export, ExportGroup("Generation Parameter")]
    public FastNoiseLite Noise;

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

                for (var z = 0; z < levelSize.Z; z++)
                {
                    if (Noise.GetNoise3D(x, y, z) > 0.0F) continue;

                    newDatabase[x, y, z] = voxel;
                }
            }
        }

        return newDatabase;
    }
}