using Godot;
using Mario3D.scripts.Voxel.Database;

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

    public void DrawVoxels(Vector3I chunkIndex, VoxelDatabase voxels)
    {
        MeshInstance.Multimesh.SetInstanceCount(voxels.ChunkSize.X * voxels.ChunkSize.Y * voxels.ChunkSize.Z);
        var i = 0;

        var origin = chunkIndex * voxels.ChunkSize;
        for (var x = origin.X; x < origin.X + voxels.ChunkSize.X; x++)
        {
            for (var y = origin.Y; y < origin.Y + voxels.ChunkSize.Y; y++)
            {
                for (var z = origin.Z; z < origin.Z + voxels.ChunkSize.Z; z++)
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

        var transform = new Transform3D(
            Basis.FromScale(Vector3.One),
            new Vector3(index.X, index.Y, index.Z)
        );

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