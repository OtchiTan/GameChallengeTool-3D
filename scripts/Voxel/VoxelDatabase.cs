using Godot;

namespace Mario3D.scripts.Voxel;

public interface IVoxelDatabase
{
    public bool SetVoxel(int x, int y, int z, int voxel);
    public int GetVoxel(int x, int y, int z);
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

    private void InitDatabase(VoxelDatabaseType voxelDatabaseType)
    {
        VoxelDatabaseType = voxelDatabaseType;

        _voxelDatabase = VoxelDatabaseType switch
        {
            VoxelDatabaseType.VoxelDictionary => new VoxelDictionary(),
            _ => _voxelDatabase
        };
    }

    private bool SetVoxel(Vector3I index, int voxel)
    {
        return _voxelDatabase.SetVoxel(index.X, index.Y, index.Z, voxel);
    }

    private bool SetVoxel(int x, int y, int z, int voxel)
    {
        return _voxelDatabase.SetVoxel(x, y, z, voxel);
    }

    private int GetVoxel(Vector3I index)
    {
        return _voxelDatabase.GetVoxel(index.X, index.Y, index.Z);
    }

    private int GetVoxel(int x, int y, int z)
    {
        return _voxelDatabase.GetVoxel(x, y, z);
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