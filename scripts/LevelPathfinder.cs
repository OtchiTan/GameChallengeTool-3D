using Godot;
using System;
using System.Collections.Generic;
using Mario3D.scripts.Voxel;

namespace Mario3D.scripts;

[GlobalClass]
public partial class LevelPathfinder : Node
{
    [Export] public Node3D DebugDraw { get; set; }

    private List<Vector3I> _path = [];

    private static readonly Vector3I[] Directions =
    [
        new(1, 0, 0), new(-1, 0, 0),
        new(0, 1, 0), new(0, -1, 0),
        new(0, 0, 1), new(0, 0, -1)
    ];

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        for (var i = 0; i < _path.Count; i++)
        {
            if (i == 0) continue;

            DebugDraw.CallDeferred("draw_line",
                _path[i - 1] + new Vector3(0.5F, 0.5F, -0.5F),
                _path[i] + new Vector3(0.5F, 0.5F, -0.5F)
            );
        }
    }

    public List<Vector3I> FindPath(Vector3I start, Vector3I end, VoxelDatabase voxels)
    {
        _path.Clear();


        if (voxels.GetVoxel(start) != 0 || voxels.GetVoxel(end) != 0)
            return _path;

        var openSet = new PriorityQueue<Vector3I, float>();
        var cameFrom = new Dictionary<Vector3I, Vector3I>();
        var gScore = new Dictionary<Vector3I, float>();

        gScore[start] = 0;
        openSet.Enqueue(start, Heuristic(start, end));

        while (openSet.Count > 0)
        {
            Vector3I current = openSet.Dequeue();

            if (current == end)
            {
                _path = ReconstructPath(cameFrom, current);
                return _path;
            }

            foreach (Vector3I dir in Directions)
            {
                Vector3I neighbor = current + dir;


                if (voxels.GetVoxel(neighbor) != 0)
                    continue;

                float tentativeGScore = gScore[current] + 1;

                if (tentativeGScore < gScore.GetValueOrDefault(neighbor, float.MaxValue))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    float fScore = tentativeGScore + Heuristic(neighbor, end);

                    openSet.Enqueue(neighbor, fScore);
                }
            }
        }

        return _path;
    }

    private static float Heuristic(Vector3I a, Vector3I b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y) + Math.Abs(a.Z - b.Z);
    }

    private static List<Vector3I> ReconstructPath(Dictionary<Vector3I, Vector3I> cameFrom, Vector3I current)
    {
        var totalPath = new List<Vector3I> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }

        totalPath.Reverse();
        return totalPath;
    }
}