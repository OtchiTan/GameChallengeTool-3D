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
    [Export] public LevelRenderer LevelRenderer;
    [Export] public PheromoneRenderer ReachRenderer;
    [Export] public LevelPathfinder LevelPathfinder;
    [Export] public ReachPheromoneGenerator ReachPheromoneGenerator;
    [Export] public VoxelDatabase VoxelDatabase;
    [Export] public int VoxelSize = 16;

    [Export, ExportGroup("Generation Parameter")]
    public int LevelWidth { get; set; } = 2;

    [Export, ExportGroup("Generation Parameter")]
    public FastNoiseLite Noise;

    private Vector3I _levelSize;

    public override void _Ready()
    {
        LevelParser ??= new LevelParser();
        LevelPathfinder ??= new LevelPathfinder();
        ReachPheromoneGenerator ??= new ReachPheromoneGenerator();
        Noise ??= new FastNoiseLite();
        VoxelDatabase ??= new VoxelDatabase();

        GD.Print("Parse Level Description");

        var levelDescription = LevelParser.ParseLevelDescription(VoxelDatabase, VoxelSize);

        GD.Print("Generate level");

        GenerateLevel(levelDescription);

        LevelRenderer?.DrawVoxels(_levelSize, VoxelDatabase);

        GD.Print("Generate reach pheromone map");

        var reach = ReachPheromoneGenerator.GenerateReachPheromoneMap(_levelSize, VoxelDatabase);

        ReachRenderer?.DrawVoxels(_levelSize, reach);

        GD.Print("Find Path");

        LevelPathfinder.FindPath(
            levelDescription.Static.Spawn.GetOrigin(VoxelSize) + Vector3I.Up,
            levelDescription.Static.End.GetOrigin(VoxelSize) - new Vector3I(0, 8, 0),
            reach);
    }

    public override void _Process(double delta)
    {
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