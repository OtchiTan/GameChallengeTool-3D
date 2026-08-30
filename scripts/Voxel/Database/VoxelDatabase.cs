using System.Collections.Generic;
using Godot;

namespace Mario3D.scripts.Voxel.Database;

public interface IChunkDatabase
{
    public bool SetVoxel(Vector3I index, int voxel);
    public int GetVoxel(Vector3I index);
    public void Normalize(int max);
}

public enum ChunkDatabaseType
{
    VoxelDictionary,
    VoxelOctree,
}

[GlobalClass]
public partial class VoxelDatabase : Resource
{
    private readonly Dictionary<Vector3I, IChunkDatabase> _chunkDatabases = new();

    [Export] public ChunkDatabaseType ChunkDatabaseType = ChunkDatabaseType.VoxelDictionary;
    public Vector3I ChunkSize;

    /**
     * Create empty database with same parameters
     * Don't duplicate voxels
     */
    public VoxelDatabase DuplicateEmpty()
    {
        var newDatabase = new VoxelDatabase();
        newDatabase.ChunkDatabaseType = ChunkDatabaseType;
        newDatabase.ChunkSize = ChunkSize;
        return newDatabase;
    }

    public void Normalize(int max)
    {
        foreach (var chunkDatabase in _chunkDatabases)
        {
            chunkDatabase.Value.Normalize(max);
        }
    }

    private IChunkDatabase InitDatabase()
    {
        return ChunkDatabaseType switch
        {
            ChunkDatabaseType.VoxelDictionary => new ChunkDictionary(),
            ChunkDatabaseType.VoxelOctree => new ChunkOctree(),
            _ => new ChunkDictionary()
        };
    }

    private bool SetVoxel(Vector3I index, int voxel)
    {
        var chunkIndex = index / ChunkSize;

        var internalIndex = index - ChunkSize * chunkIndex;

        if (_chunkDatabases.TryGetValue(chunkIndex, out var chunkDatabase))
        {
            return chunkDatabase.SetVoxel(internalIndex, voxel);
        }

        _chunkDatabases[chunkIndex] = InitDatabase();
        return _chunkDatabases[chunkIndex].SetVoxel(internalIndex, voxel);
    }

    private int GetVoxel(Vector3I index)
    {
        var chunkIndex = index / ChunkSize;
        var internalIndex = index - ChunkSize * chunkIndex;
        return _chunkDatabases.TryGetValue(chunkIndex, out var chunkDatabase)
            ? chunkDatabase.GetVoxel(internalIndex)
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