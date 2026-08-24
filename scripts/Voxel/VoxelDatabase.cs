using System;
using Godot;

namespace Mario3D.scripts.Voxel;

public interface IVoxelDatabase
{
    public bool SetVoxel(Vector3I index, int voxel);
    public int GetVoxel(Vector3I index);
}

public enum VoxelDatabaseType
{
    VoxelDictionary,
}

[GlobalClass]
public partial class VoxelDatabase : Resource
{
    private IVoxelDatabase _voxelDatabase;

    [Export] public VoxelDatabaseType VoxelDatabaseType = VoxelDatabaseType.VoxelDictionary;

    public VoxelDatabase()
    {
        InitDatabase(VoxelDatabaseType);
    }

    public VoxelDatabase(VoxelDatabaseType voxelDatabaseType)
    {
        InitDatabase(voxelDatabaseType);
    }

    public void InitDatabase(VoxelDatabaseType voxelDatabaseType)
    {
        VoxelDatabaseType = voxelDatabaseType;

        _voxelDatabase = VoxelDatabaseType switch
        {
            VoxelDatabaseType.VoxelDictionary => new VoxelDictionary(),
            _ => _voxelDatabase
        };
    }

    public bool SetVoxel(Vector3I index, int voxel)
    {
        return _voxelDatabase.SetVoxel(index, voxel);
    }

    public bool SetVoxel(int x, int y, int z, int voxel)
    {
        return _voxelDatabase.SetVoxel(new Vector3I(x, y, z), voxel);
    }

    public int GetVoxel(Vector3I index)
    {
        return _voxelDatabase.GetVoxel(index);
    }

    public int GetVoxel(int x, int y, int z)
    {
        return _voxelDatabase.GetVoxel(new Vector3I(x, y, z));
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