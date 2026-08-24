using System.Collections.Generic;
using Godot;

namespace Mario3D.scripts.Voxel;

public class VoxelDictionary : IVoxelDatabase
{
    private readonly Dictionary<Vector3I, int> _voxels = new();

    public bool SetVoxel(int x, int y, int z, int voxel)
    {
        _voxels[new Vector3I(x, y, z)] = voxel;
        return true;
    }

    public int GetVoxel(int x, int y, int z)
    {
        return _voxels.GetValueOrDefault(new Vector3I(x, y, z), 0);
    }
}