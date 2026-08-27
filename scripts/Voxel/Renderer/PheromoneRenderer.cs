using Godot;

namespace Mario3D.scripts.Voxel.Renderer;

[GlobalClass]
public partial class PheromoneRenderer : LevelRenderer
{
    public override void DrawVoxel(int instance, Vector3I index, int voxel)
    {
        if (voxel == 0) return;

        var intensity = voxel / 100f;
        var scale = Mathf.Lerp(0.1f, 1.0f, intensity);
        var transform = new Transform3D(Basis.FromScale(Vector3.One * scale), index);
        MeshInstance.Multimesh.SetInstanceTransform(instance, transform);

        var color = Colors.Green.Lerp(Colors.Red, intensity);
        MeshInstance.Multimesh.SetInstanceColor(instance, color);
    }
}