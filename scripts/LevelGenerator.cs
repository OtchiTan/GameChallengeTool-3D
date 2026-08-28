using System.Collections.Generic;
using Godot;
using Mario3D.scripts.LevelInput;
using Mario3D.scripts.Processing;
using Mario3D.scripts.Voxel;
using Mario3D.scripts.Voxel.Renderer;

namespace Mario3D.scripts;

[GlobalClass]
public partial class LevelGenerator : Node3D
{
    [Export] public LevelParser LevelParser;
    [Export] public LevelPathfinder LevelPathfinder;
    [Export] public ReachPheromoneGenerator ReachPheromoneGenerator;
    [Export] public VoxelDatabase VoxelDatabase;
    [Export] public bool DrawReachPheromone = true;

    [Export, ExportGroup("Generation Parameter")]
    public int VoxelSize = 16;

    [Export, ExportGroup("Generation Parameter")]
    public int LevelWidth { get; set; } = 2;

    [Export, ExportGroup("Generation Parameter")]
    public FastNoiseLite Noise;

    private Vector3I _levelSize;
    private LevelRenderer _levelRenderer;
    private PheromoneRenderer _reachRenderer;
    private List<Vector3I> _path = [];
    private Node3D _debugDraw;

    public override void _Ready()
    {
        _levelRenderer = new LevelRenderer();
        AddChild(_levelRenderer);
        _reachRenderer = new PheromoneRenderer();
        AddChild(_reachRenderer);

        _debugDraw = GetNode<Node3D>("DebugDraw");

        LevelParser ??= new LevelParser();
        LevelPathfinder ??= new LevelPathfinder();
        ReachPheromoneGenerator ??= new ReachPheromoneGenerator();
        Noise ??= new FastNoiseLite();
        VoxelDatabase ??= new VoxelDatabase();

        CallDeferred("StartGeneration");
    }

    private void StartGeneration()
    {
        GD.Print("Parse Level Description");

        var levelDescription = LevelParser.ParseLevelDescription(VoxelDatabase, VoxelSize);

        GD.Print("Generate level");

        GenerateLevel(levelDescription);

        _levelRenderer.DrawVoxels(_levelSize, VoxelDatabase);

        GD.Print("Generate reach pheromone map");

        var reach = ReachPheromoneGenerator.GenerateReachPheromoneMap(_levelSize, VoxelDatabase);

        if (DrawReachPheromone)
            _reachRenderer.DrawVoxels(_levelSize, reach);

        GD.Print("Find Path");

        _path = LevelPathfinder.FindPath(
            levelDescription.Static.Spawn.GetOrigin(VoxelSize) + Vector3I.Up,
            levelDescription.Static.End.GetOrigin(VoxelSize) - new Vector3I(0, 8, 0),
            reach);
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

    private void GenerateLevel(LevelDescription description)
    {
        _levelSize = new Vector3I(description.LevelCols / VoxelSize, description.LevelRows / VoxelSize, LevelWidth);

        var newDatabase = new VoxelDatabase(VoxelDatabase.VoxelDatabaseType);

        for (var x = 0; x < _levelSize.X; x++)
        {
            for (var y = 0; y < _levelSize.Y; y++)
            {
                var position = new Vector3I(x, y, 0);

                var voxel = VoxelDatabase[position];

                if (voxel == 0) continue;

                for (var z = 0; z < _levelSize.Z; z++)
                {
                    if (Noise.GetNoise3D(x, y, z) > 0.0F) continue;

                    newDatabase[x, y, z] = voxel;
                }
            }
        }

        VoxelDatabase = newDatabase;
    }
}