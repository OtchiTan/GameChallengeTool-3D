using System.Collections.Generic;
using Godot;

namespace Mario3D.scripts.Voxel.Database;

public class ChunkDictionary : IChunkDatabase
{
    private readonly Dictionary<Vector3I, int> _voxels = new();

    private int _maxValue;

    public bool SetVoxel(Vector3I index, int voxel)
    {
        if (voxel > _maxValue)
            _maxValue = voxel;

        _voxels[index] = voxel;
        return true;
    }

    public int GetVoxel(Vector3I index)
    {
        return _voxels.GetValueOrDefault(index, 0);
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
}