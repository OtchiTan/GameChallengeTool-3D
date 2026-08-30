using Godot;
using Mario3D.scripts.Voxel.Database;
using Mario3D.scripts.Voxel.Renderer;

namespace Mario3D.scripts.Voxel;

public partial class Chunk : Node
{
    private LevelRenderer _levelRenderer = new();
    private PheromoneRenderer _pheromoneRenderer;
    public Vector3I ChunkIndex;

    public override void _Ready()
    {
        AddChild(_levelRenderer);
    }

    public override void _Process(double delta)
    {
    }

    public void RenderChunk(VoxelDatabase voxelDatabase, VoxelDatabase reachPheromoneMap,
        bool renderReachPheromoneMap, float voxelSize)
    {
        _levelRenderer.DrawVoxels(ChunkIndex, voxelDatabase, voxelSize);

        if (!renderReachPheromoneMap)
            return;

        _pheromoneRenderer = new PheromoneRenderer();
        AddChild(_pheromoneRenderer);
        _pheromoneRenderer.DrawVoxels(ChunkIndex, reachPheromoneMap, voxelSize);
    }
}