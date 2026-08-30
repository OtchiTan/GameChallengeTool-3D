using System;
using System.Collections.Generic;
using Godot;
using Mario3D.scripts.Voxel.Database;

namespace Mario3D.scripts.Processing;

[GlobalClass]
public partial class LevelPathfinder : Resource
{
    private static readonly Vector3I[] Directions =
    [
        new(1, 0, 0), new(-1, 0, 0),
        new(0, 1, 0), new(0, -1, 0),
        new(0, 0, 1), new(0, 0, -1)
    ];

    public static List<Vector3I> FindPath(Vector3I start, Vector3I end, VoxelDatabase voxels)
    {
        var openSet = new PriorityQueue<Vector3I, float>();
        var cameFrom = new Dictionary<Vector3I, Vector3I>();
        var gScore = new Dictionary<Vector3I, float>
        {
            [start] = 0
        };

        GD.Print(start);
        GD.Print(end);

        openSet.Enqueue(start, Heuristic(start, end));

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (var dir in Directions)
            {
                var neighbor = current + dir;

                var voxel = voxels[current];

                if (voxel == 0)
                    continue;

                var tentativeGScore = gScore[current] + 1 + (100 - voxel);

                if (!(tentativeGScore < gScore.GetValueOrDefault(neighbor, float.MaxValue)))
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                var fScore = tentativeGScore + Heuristic(neighbor, end);

                openSet.Enqueue(neighbor, fScore);
            }
        }

        return [];
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