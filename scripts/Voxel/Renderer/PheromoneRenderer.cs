using Godot;
using Mario3D.scripts.Voxel.Database;

namespace Mario3D.scripts.Voxel.Renderer;

[GlobalClass]
public partial class PheromoneRenderer : LevelRenderer
{
    protected override void DrawVoxel(Vector3I index, VoxelDatabase voxels)
    {
        var voxel = voxels[index];
        
        if (voxel == 0)
            return;

        var intensity = voxel / 100f;
        var scale = Mathf.Lerp(0.1f, 1.0f, intensity);
        var transform = new Transform3D(
            Basis.FromScale(Vector3.One * scale * VoxelSize),
            new Vector3(index.X, index.Y, index.Z) * VoxelSize
        );

        var parameter = new MeshInstanceParameter(transform)
        {
            Color = Colors.Green.Lerp(Colors.Red, intensity)
        };

        MeshParameters.Add(parameter);
    }
}