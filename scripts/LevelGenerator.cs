using System.Collections.Generic;
using Godot;

namespace Mario3D.scripts;

[GlobalClass]
public partial class LevelGenerator : Node3D
{
    [Export] public LevelRenderer LevelRenderer { get; set; }
    
    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

   public void GenerateLevel(Dictionary<Vector3I, int> voxels)
    {
        
        
        LevelRenderer.DrawVoxels(voxels);
    }
}