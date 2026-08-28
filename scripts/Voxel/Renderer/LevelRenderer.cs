using Godot;

namespace Mario3D.scripts.Voxel.Renderer;

[GlobalClass]
public partial class LevelRenderer : Node3D
{
    protected MultiMeshInstance3D MeshInstance;

    public override void _Ready()
    {
        MeshInstance = new MultiMeshInstance3D();
        MeshInstance.Multimesh = CreateMultiMesh();
        AddChild(MeshInstance);
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

    protected virtual void DrawVoxel(int instance, Vector3I index, int voxel)
    {
        if (voxel == 0) return;

        var transform = new Transform3D(Basis.Identity, new Vector3(index.X, index.Y, index.Z));
        MeshInstance.Multimesh.SetInstanceTransform(instance, transform);
    }

    private static StandardMaterial3D CreateMaterial()
    {
        var material = new StandardMaterial3D();
        material.VertexColorUseAsAlbedo = true;
        return material;
    }

    private static BoxMesh CreateMesh()
    {
        var mesh = new BoxMesh();
        mesh.Material = CreateMaterial();
        return mesh;
    }

    private static MultiMesh CreateMultiMesh()
    {
        var multiMesh = new MultiMesh();
        multiMesh.TransformFormat = MultiMesh.TransformFormatEnum.Transform3D;
        multiMesh.Mesh = CreateMesh();
        multiMesh.UseColors = true;
        return multiMesh;
    }
}