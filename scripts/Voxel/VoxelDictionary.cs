using System.Collections.Generic;
using Godot;

namespace Mario3D.scripts.Voxel;

public class VoxelDictionary : IVoxelDatabase
{
    private readonly Dictionary<Vector3I, int> _voxels = new();

    public bool SetVoxel(Vector3I index, int voxel)
    {
        _voxels[index] = voxel;
        return true;
    }

    public int GetVoxel(Vector3I index)
    {
        return _voxels.GetValueOrDefault(index, 0);
    }
}