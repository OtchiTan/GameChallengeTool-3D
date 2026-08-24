using Godot;
using Mario3D.scripts.Voxel;

namespace Mario3D.scripts.Processing;

[GlobalClass]
public partial class ReachPheromoneGenerator : Resource
{
    public ReachPheromoneGenerator()
    {
    }

    public VoxelDatabase GenerateReachPheromoneMap(Vector3I levelSize, VoxelDatabase levelVoxels)
    {
        var pheromoneVoxels = new VoxelDatabase(levelVoxels.VoxelDatabaseType);

        for (var x = 0; x < levelSize.X; x++)
        {
            for (var y = 0; y < levelSize.Y; y++)
            {
                for (var z = 0; z < levelSize.Z; z++)
                {
                    var index = new Vector3I(x, y, z);
                    var voxel = levelVoxels[index];
                    var belowVoxel = levelVoxels[index + new Vector3I(0, -1, 0)];

                    if (voxel != 0 || belowVoxel == 0) continue;

                    var value = 100;

                    for (var oy = y - 1; oy < 0; oy--)
                    {
                        if (levelVoxels[x, oy, z] > 0)
                            break;

                        pheromoneVoxels[x, oy, z] += Mathf.Max(0, value);

                        var valueX = value;
                        for (var ox = x; ox < x - value; ox--)
                        {
                            if (ox < 0 || levelVoxels[ox, oy, z] > 0)
                                break;

                            var vX = Mathf.Max(0, valueX);
                            pheromoneVoxels[ox, oy, z] += vX;
                        }
                    }
                }
            }
        }


        return pheromoneVoxels;
    }
}