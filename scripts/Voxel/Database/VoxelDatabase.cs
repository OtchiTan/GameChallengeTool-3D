using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Mario3D.scripts.Voxel.Database;

public interface IChunkDatabase
{
    public bool SetVoxel(int x, int y, int z, int voxel);
    public int GetVoxel(int x, int y, int z);
    public void Normalize(int max);
    public int CountVoxels();
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

    public VoxelDatabase()
    {
        _chunkDatabases[Vector3I.Zero] = InitDatabase();
    }

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

    public int CountVoxels()
    {
        return _chunkDatabases.Sum(chunkDatabase => chunkDatabase.Value.CountVoxels());
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
        /*var chunkIndex = index / ChunkSize;

        if (_chunkDatabases.TryGetValue(chunkIndex, out var chunkDatabase))
        {
            index -= ChunkSize * chunkIndex;
            return chunkDatabase.SetVoxel(index.X, index.Y, index.Z, voxel);
        }

        _chunkDatabases[chunkIndex] = InitDatabase();*/
        return _chunkDatabases[Vector3I.Zero].SetVoxel(index.X, index.Y, index.Z, voxel);
    }

    private int GetVoxel(Vector3I index)
    {
        //var chunkIndex = index / ChunkSize;
        //index -= ChunkSize * chunkIndex;
        return _chunkDatabases.TryGetValue(Vector3I.Zero, out var chunkDatabase)
            ? chunkDatabase.GetVoxel(index.X, index.Y, index.Z)
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