using Godot;
using Mario3D.scripts.Voxel.Renderer;

namespace Mario3D.scripts.Voxel;

public partial class Chunk : Node
{
    private LevelRenderer _levelRenderer = new();
    private PheromoneRenderer _pheromoneRenderer;
    public Vector3I ChunkIndex;
    public VoxelDatabase ReachPheromoneMap = new();

    public override void _Ready()
    {
        AddChild(_levelRenderer);
    }

    public override void _Process(double delta)
    {
    }

    public void RenderChunk(VoxelDatabase voxelDatabase, bool renderReachPheromoneMap)
    {
        _levelRenderer.DrawVoxels(ChunkIndex, voxelDatabase);

        if (!renderReachPheromoneMap)
            return;

        _pheromoneRenderer = new PheromoneRenderer();
        AddChild(_pheromoneRenderer);
        _pheromoneRenderer.DrawVoxels(ChunkIndex, ReachPheromoneMap);
    }
}