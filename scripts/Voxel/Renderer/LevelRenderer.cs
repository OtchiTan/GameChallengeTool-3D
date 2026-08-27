using Godot;

namespace Mario3D.scripts.Voxel.Renderer;

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

    public void DrawVoxels(Vector3I levelSize, VoxelDatabase voxels)
    {
        MeshInstance.Multimesh.SetInstanceCount(levelSize.X * levelSize.Y * levelSize.Z);
        var i = 0;
        for (var x = 0; x < levelSize.X; x++)
        {
            for (var y = 0; y < levelSize.Y; y++)
            {
                for (var z = 0; z < levelSize.Z; z++)
                {
                    var index = new Vector3I(x, y, z);
                    var voxel = voxels[index];

                    DrawVoxel(i, index, voxel);

                    i++;
                }
            }
        }
    }

    public virtual void DrawVoxel(int instance, Vector3I index, int voxel)
    {
        if (voxel == 0) return;

        var transform = new Transform3D(Basis.Identity, new Vector3(index.X, index.Y, index.Z));
        MeshInstance.Multimesh.SetInstanceTransform(instance, transform);
    }
}