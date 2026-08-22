using System.Collections.Generic;
using Godot;

namespace Mario3D.scripts;

[GlobalClass]
public partial class LevelRenderer : Node3D
{
    [Export] public MultiMeshInstance3D MeshInstance { get; set; }

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public void DrawVoxels(Dictionary<Vector3I, int> voxels)
    {
        MeshInstance.Multimesh.SetInstanceCount(voxels.Count);
        var i = 0;
        foreach (var pair in voxels)
        {
            var transform = new Transform3D(Basis.Identity, new Vector3(pair.Key.X, -pair.Key.Y, pair.Key.Z));
            MeshInstance.Multimesh.SetInstanceTransform(i, transform);

            i++;
        }
    }
}