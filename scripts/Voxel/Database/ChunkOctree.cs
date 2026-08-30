using Godot;

namespace Mario3D.scripts.Voxel.Database;

public class ChunkOctree : IChunkDatabase
{
    public bool SetVoxel(Vector3I index, int voxel)
    {
        throw new System.NotImplementedException();
    }

    public int GetVoxel(Vector3I index)
    {
        throw new System.NotImplementedException();
    }

    public void Normalize(int max)
    {
        throw new System.NotImplementedException();
    }
}