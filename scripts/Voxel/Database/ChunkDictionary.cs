using System.Collections.Generic;
using Godot;

namespace Mario3D.scripts.Voxel.Database;

public class ChunkDictionary : IChunkDatabase
{
    private Dictionary<Vector3I, int> _voxels = new();

    private int _maxValue;

    public bool SetVoxel(int x, int y, int z, int voxel)
    {
        if (voxel > _maxValue)
            _maxValue = voxel;

        _voxels[new Vector3I(x, y, z)] = voxel;
        return true;
    }

    public int GetVoxel(int x, int y, int z)
    {
        return _voxels.GetValueOrDefault(new Vector3I(x, y, z), 0);
    }

    public void Normalize(int max)
    {
        foreach (var pair in _voxels)
        {
            var normalizedValue = (float)pair.Value / _maxValue * max;
            _voxels[pair.Key] = Mathf.RoundToInt(normalizedValue);
        }

        _maxValue = max;
    }

    public int CountVoxels()
    {
        return _voxels.Count;
    }
}