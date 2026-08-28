using System.Collections.Generic;
using Godot;
using Mario3D.scripts.Generation;
using Mario3D.scripts.LevelInput;
using Mario3D.scripts.Processing;
using Mario3D.scripts.Voxel;
using Mario3D.scripts.Voxel.Renderer;

namespace Mario3D.scripts;

[GlobalClass]
public partial class LevelManager : Node3D
{
    [Export] public LevelParser LevelParser;
    [Export] public LevelPathfinder LevelPathfinder;
    [Export] public ReachPheromoneGenerator ReachPheromoneGenerator;
    [Export] public VoxelDatabase VoxelDatabase;
    [Export] public LevelGenerator LevelGenerator;
    [Export] public bool DrawReachPheromone = true;
    [Export] public int ChunkSize = 16;

    [Export, ExportGroup("Generation Parameter")]
    public int VoxelSize = 16;

    [Export, ExportGroup("Generation Parameter")]
    public int LevelWidth { get; set; } = 2;

    private readonly Dictionary<Vector3I, Chunk> _chunks = new();
    private Vector3I _levelSize;
    private PheromoneRenderer _reachRenderer = new();
    private List<Vector3I> _path = [];
    private Node3D _debugDraw;

    public override void _Ready()
    {
        AddChild(_reachRenderer);

        _debugDraw = GetNode<Node3D>("DebugDraw");

        LevelParser ??= new LevelParser();
        LevelPathfinder ??= new LevelPathfinder();
        ReachPheromoneGenerator ??= new ReachPheromoneGenerator();
        VoxelDatabase ??= new VoxelDatabase();
        VoxelDatabase.ChunkSize = new Vector3I(ChunkSize, ChunkSize, ChunkSize);
        LevelGenerator ??= new LevelGenerator();

        CallDeferred("StartGeneration");
    }

    public override void _Process(double delta)
    {
        for (var i = 0; i < _path.Count; i++)
        {
            if (i == 0) continue;

            _debugDraw.CallDeferred("draw_line",
                _path[i - 1] + new Vector3(0.5F, 0.5F, -0.5F),
                _path[i] + new Vector3(0.5F, 0.5F, -0.5F)
            );
        }
    }

    private void SpawnChunk(Vector3I chunkIndex)
    {
        var chunk = new Chunk();
        chunk.ChunkIndex = chunkIndex;
        AddChild(chunk);
        _chunks.Add(chunkIndex, chunk);
    }

    private void StartGeneration()
    {
        GD.Print("Parse Level Description");

        var levelDescription = LevelParser.ParseLevelDescription(VoxelDatabase, VoxelSize);

        _levelSize = new Vector3I(
            levelDescription.LevelCols / VoxelSize,
            levelDescription.LevelRows / VoxelSize,
            LevelWidth
        );

        GD.Print("Generate level");

        VoxelDatabase = LevelGenerator.GenerateLevel(VoxelDatabase, _levelSize);

        for (var x = 0; x < Mathf.CeilToInt(_levelSize.X / (float)ChunkSize); x++)
        {
            for (var y = 0; y < Mathf.CeilToInt(_levelSize.Y / (float)ChunkSize); y++)
            {
                for (var z = 0; z < Mathf.CeilToInt(_levelSize.Z / (float)ChunkSize); z++)
                {
                    var chunkIndex = new Vector3I(x, y, z);
                    SpawnChunk(chunkIndex);
                }
            }
        }

        GD.Print("Render chunks");
        foreach (var chunk in _chunks)
        {
            chunk.Value.ReachPheromoneMap =
                ReachPheromoneGenerator.GenerateReachPheromoneMap(chunk.Key, VoxelDatabase);
            chunk.Value.RenderChunk(VoxelDatabase, DrawReachPheromone);
        }

        GD.Print("Find Path");

        _path = LevelPathfinder.FindPath(
            levelDescription.Static.Spawn.GetOrigin(VoxelSize) + Vector3I.Up,
            levelDescription.Static.End.GetOrigin(VoxelSize) - new Vector3I(0, 8, 0),
            _chunks[Vector3I.Zero].ReachPheromoneMap);
    }
}