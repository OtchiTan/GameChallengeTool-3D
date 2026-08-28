using Godot;
using Mario3D.scripts.Voxel;

namespace Mario3D.scripts.Processing;

[GlobalClass]
public partial class ReachPheromoneGenerator : Resource
{
    public ReachPheromoneGenerator()
    {
    }

    public VoxelDatabase GenerateReachPheromoneMap(Vector3I chunkIndex, VoxelDatabase levelVoxels)
    {
        var pheromoneVoxels = levelVoxels.DuplicateEmpty();

        var origin = chunkIndex * levelVoxels.ChunkSize;
        for (var x = origin.X; x < origin.X + levelVoxels.ChunkSize.X; x++)
        {
            for (var y = origin.Y; y < origin.Y + levelVoxels.ChunkSize.Y; y++)
            {
                for (var z = origin.Z; z < origin.Z + levelVoxels.ChunkSize.Z; z++)
                {
                    var index = new Vector3I(x, y, z);
                    var voxel = levelVoxels[index];
                    var belowVoxel = levelVoxels[index + new Vector3I(0, -1, 0)];

                    if (voxel != 0 || belowVoxel == 0) continue;

                    var value = 6;

                    // Extend vertical
                    for (var oy = y; oy < levelVoxels.ChunkSize.Y; oy++)
                    {
                        if (levelVoxels[x, oy, z] > 0)
                            break;

                        pheromoneVoxels[x, oy, z] += value;


                        foreach (var direction in (Vector3I[])
                                 [Vector3I.Right, Vector3I.Left, Vector3I.Forward, Vector3I.Back])
                        {
                            ExtendPheromoneHorizontal(
                                new Vector3I(x, oy, z),
                                direction,
                                value,
                                levelVoxels,
                                pheromoneVoxels
                            );
                        }

                        value--;

                        if (value == 0)
                            break;
                    }
                }
            }
        }

        pheromoneVoxels.Normalize(100);

        return pheromoneVoxels;
    }

    private static void ExtendPheromoneHorizontal(
        Vector3I index,
        Vector3I direction,
        int value,
        VoxelDatabase levelVoxels,
        VoxelDatabase pheromoneVoxels)
    {
        var axis = direction.X != 0 ? 0 : 2;

        for (var offset = 0; offset < value; offset++)
        {
            var nextIndex = index;
            nextIndex[axis] += offset * direction[axis];

            if (levelVoxels[nextIndex] > 0)
                break;

            pheromoneVoxels[nextIndex] += value;

            // Extend Down
            for (var offsetY = nextIndex.Y - 1; offsetY > 0; offsetY--)
            {
                var nextIndexY = nextIndex;
                nextIndexY.Y = offsetY;

                if (levelVoxels[nextIndexY] > 0)
                    break;

                pheromoneVoxels[nextIndexY] += value;
            }

            value--;
        }
    }
}