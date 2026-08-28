using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Mario3D.scripts.Voxel;

public interface IVoxelDatabase
{
    public bool SetVoxel(int x, int y, int z, int voxel);
    public int GetVoxel(int x, int y, int z);
    public void Normalize(int max);
    public int CountVoxels();
}

public enum VoxelDatabaseType
{
    VoxelDictionary,
}

[GlobalClass]
public partial class VoxelDatabase : Resource
{
    private readonly Dictionary<Vector3I, IVoxelDatabase> _voxelDatabases = new();

    [Export] public VoxelDatabaseType VoxelDatabaseType = VoxelDatabaseType.VoxelDictionary;
    public Vector3I ChunkSize;

    public VoxelDatabase()
    {
    }

    /**
     * Create empty database with same parameters
     * Don't duplicate voxels
     */
    public VoxelDatabase DuplicateEmpty()
    {
        var newDatabase = new VoxelDatabase();
        newDatabase.VoxelDatabaseType = VoxelDatabaseType;
        newDatabase.ChunkSize = ChunkSize;
        return newDatabase;
    }

    public void Normalize(int max)
    {
        foreach (var voxelDatabase in _voxelDatabases)
        {
            voxelDatabase.Value.Normalize(max);
        }
    }

    public int CountVoxels()
    {
        return _voxelDatabases.Sum(voxelDatabase => voxelDatabase.Value.CountVoxels());
    }

    private IVoxelDatabase InitDatabase()
    {
        return VoxelDatabaseType switch
        {
            VoxelDatabaseType.VoxelDictionary => new VoxelDictionary(),
            _ => new VoxelDictionary()
        };
    }

    private bool SetVoxel(Vector3I index, int voxel)
    {
        var chunkIndex = index / ChunkSize;

        if (_voxelDatabases.TryGetValue(chunkIndex, out var voxelDatabase))
        {
            index -= ChunkSize * chunkIndex;
            return voxelDatabase.SetVoxel(index.X, index.Y, index.Z, voxel);
        }

        _voxelDatabases[chunkIndex] = InitDatabase();
        return _voxelDatabases[chunkIndex].SetVoxel(index.X, index.Y, index.Z, voxel);
    }

    private int GetVoxel(Vector3I index)
    {
        var chunkIndex = index / ChunkSize;
        index -= ChunkSize * chunkIndex;
        return _voxelDatabases.TryGetValue(chunkIndex, out var voxelDatabase)
            ? voxelDatabase.GetVoxel(index.X, index.Y, index.Z)
            : 0;
    }

    public int this[Vector3I index]
    {
        get => GetVoxel(index);
        set => SetVoxel(index, value);
    }

    public int this[int x, int y, int z]
    {
        get => GetVoxel(new Vector3I(x, y, z));
        set => SetVoxel(new Vector3I(x, y, z), value);
    }
}