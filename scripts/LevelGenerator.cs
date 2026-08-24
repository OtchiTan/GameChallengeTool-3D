using Godot;
using Mario3D.scripts.LevelInput;
using Mario3D.scripts.Processing;
using Mario3D.scripts.Voxel;

namespace Mario3D.scripts;

[GlobalClass]
public partial class LevelGenerator : Node3D
{
    [Export] public LevelParser LevelParser;
    [Export] public LevelRenderer LevelRenderer;
    [Export] public LevelPathfinder LevelPathfinder;
    [Export] public ReachPheromoneGenerator ReachPheromoneGenerator;
    [Export] public VoxelDatabase VoxelDatabase;

    [Export, ExportGroup("Generation Parameter")]
    public int LevelWidth { get; set; } = 2;

    [Export, ExportGroup("Generation Parameter")]
    public FastNoiseLite Noise;

    public override void _Ready()
    {
        LevelParser ??= new LevelParser();
        LevelPathfinder ??= new LevelPathfinder();
        ReachPheromoneGenerator ??= new ReachPheromoneGenerator();
        Noise ??= new FastNoiseLite();
        VoxelDatabase ??= new VoxelDatabase();


        var levelDescription = LevelParser.ParseLevelDescription(VoxelDatabase);

        GenerateLevel(levelDescription);
    }

    public override void _Process(double delta)
    {
    }

    private void GenerateLevel(LevelDescription description)
    {
        var levelSize = new Vector3I(description.LevelCols / 16, description.LevelRows / 16, LevelWidth);

        var newDatabase = new VoxelDatabase(VoxelDatabase.VoxelDatabaseType);

        for (var x = 0; x < levelSize.X; x++)
        {
            for (var y = 0; y < levelSize.Y; y++)
            {
                var position = new Vector3I(x, y, 0);

                var voxel = VoxelDatabase[position];

                if (voxel == 0) continue;

                for (var z = 0; z < levelSize.Z; z++)
                {
                    if (Noise.GetNoise3D(x, y, z) > 0.0F) continue;

                    newDatabase[x, y, z] = voxel;
                }
            }
        }

        VoxelDatabase = newDatabase;

        LevelRenderer?.DrawVoxels(new Vector3I(levelSize.X, 5, levelSize.Z), VoxelDatabase);

        var reach = ReachPheromoneGenerator.GenerateReachPheromoneMap(levelSize, VoxelDatabase);

        var path = LevelPathfinder.FindPath(
            description.Static.Spawn.GetOrigin(),
            description.Static.End.GetOrigin(),
            VoxelDatabase);
    }
}