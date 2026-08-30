using System.Collections.Generic;
using Godot;
using Mario3D.scripts.Voxel.Database;

namespace Mario3D.scripts.Voxel.Renderer;

public struct MeshInstanceParameter(Transform3D transform)
{
    public Transform3D Transform = transform;
    public Color Color;
}

[GlobalClass]
public partial class LevelRenderer : Node3D
{
    private MultiMeshInstance3D _meshInstance;
    protected float VoxelSize;
    protected readonly List<MeshInstanceParameter> MeshParameters = [];

    public override void _Ready()
    {
        _meshInstance = new MultiMeshInstance3D();
        _meshInstance.Multimesh = CreateMultiMesh();
        AddChild(_meshInstance);
    }

    public override void _Process(double delta)
    {
    }

    public void DrawVoxels(Vector3I chunkIndex, VoxelDatabase voxels, float voxelSize)
    {
        VoxelSize = voxelSize;

        MeshParameters.Clear();

        var origin = chunkIndex * voxels.ChunkSize;
        for (var x = origin.X; x < origin.X + voxels.ChunkSize.X; x++)
        {
            for (var y = origin.Y; y < origin.Y + voxels.ChunkSize.Y; y++)
            {
                for (var z = origin.Z; z < origin.Z + voxels.ChunkSize.Z; z++)
                {
                    var index = new Vector3I(x, y, z);

                    DrawVoxel(index, voxels);
                }
            }
        }

        _meshInstance.Multimesh.SetInstanceCount(MeshParameters.Count);

        for (var i = 0; i < MeshParameters.Count; i++)
        {
            _meshInstance.Multimesh.SetInstanceTransform(i, MeshParameters[i].Transform);
            _meshInstance.Multimesh.SetInstanceColor(i, MeshParameters[i].Color);
        }
    }

    protected virtual void DrawVoxel(Vector3I index, VoxelDatabase voxels)
    {
        var voxel = voxels[index];

        if (voxel == 0)
            return;

        foreach (var direction in (Vector3I[])
                 [Vector3I.Right, Vector3I.Left, Vector3I.Forward, Vector3I.Back, Vector3I.Up, Vector3I.Down])
        {
            var nextVoxel = voxels[index + direction];
            if (nextVoxel == 0)
            {
                var transform = new Transform3D(
                    Basis.FromScale(Vector3.One * VoxelSize),
                    new Vector3(index.X, index.Y, index.Z) * VoxelSize
                );

                MeshParameters.Add(new MeshInstanceParameter(transform));
                return;
            }
        }
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