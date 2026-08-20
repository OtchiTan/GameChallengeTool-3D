using Godot;
using System.Collections.Generic;


namespace Mario3D.scripts;

public static class Constants
{
    public const int OctreeMaxDepth = 6;
}

public class VoxelBox(Vector3I min, Vector3I max)
{
    public Vector3I Min = min;
    public Vector3I Max = max;

    public bool Contains(Vector3I point)
    {
        return point.X >= Min.X && point.X <= Max.X
                                && point.Y >= Min.Y && point.Y <= Max.Y
                                && point.Z >= Min.Z && point.Z <= Max.Z;
    }

    public Vector3I Center()
    {
        return (Min + Max) / 2;
    }

    public Vector3I Size()
    {
        return Max - Min;
    }
}

public class NodeId(int value)
{
    public NodeId CreateChildren(byte childId)
    {
        var currentDepth = GetDepth();
        var nextDepth = (currentDepth) + 1;
        var cleanedParent = value & ~0x7;
        var childShifted = (childId & 0x7) << (currentDepth * 3 + 3);

        return new NodeId(cleanedParent | childShifted | nextDepth);
    }

    public byte GetDepth()
    {
        return (byte)(value & 0x7);
    }

    public byte GetChild(byte depth)
    {
        return (byte)(value >> (depth * 3 + 3) & 0x7);
    }

    public bool GetParent(ref NodeId parentId)
    {
        var depth = GetDepth();
        if (depth == 0)
            return false;

        var newDepth = depth - 1;

        var cleanedValue = value & ~0x7;
        var targetChildShift = newDepth * 3 + 3;
        var childMask = ~(0x7 << targetChildShift);

        parentId = new NodeId(cleanedValue & childMask | newDepth);

        return true;
    }

    public VoxelBox GetBoundingBox()
    {
        var maxOctreeSize = 2 << Constants.OctreeMaxDepth;
        var box = new VoxelBox(Vector3I.Zero, new Vector3I(maxOctreeSize, maxOctreeSize, maxOctreeSize));

        for (byte i = 0; i < GetDepth(); i++)
        {
            var childId = GetChild(i);
            box.Max = box.Center();
            var size = box.Size();
            for (var j = 0; j < 3; j++)
            {
                if ((childId & 1 << j) <= 0) continue;

                box.Min[j] += size[j];
                box.Max[j] += size[j];
            }
        }

        return box;
    }
}

[GlobalClass]
public partial class Octree : Node
{
    public readonly NodeId NodeId = new(0);
    public int VoxelType;

    public List<Octree> Children = new();

    public Octree()
    {
    }

    public bool IsLeaf()
    {
        return Children.Count == 0;
    }

    private void Subdivide()
    {
        Children.Clear();

        for (byte i = 0; i < 8; i++)
        {
            Children.Add(new Octree(NodeId.CreateChildren(i), VoxelType));
        }
    }

    public void Insert(Vector3I position, int voxel)
    {
        Insert(position, voxel, this);
    }

    public int GetVoxel(Vector3I position)
    {
        if (IsLeaf())
        {
            return VoxelType;
        }


        for (byte i = 0; i < 8; i++)
        {
            if (!Children[i].NodeId.GetBoundingBox().Contains(position)) continue;

            return Children[i].GetVoxel(position);
        }

        return 0;
    }

    private Octree(NodeId nodeId, int voxelType)
    {
        NodeId = nodeId;
        VoxelType = voxelType;
    }

    private void Insert(Vector3I position, int voxel, Octree root)
    {
        if (IsLeaf())
        {
            if (NodeId.GetDepth() == Constants.OctreeMaxDepth)
            {
                VoxelType = voxel;

                var parentId = new NodeId(0);
                if (NodeId.GetParent(ref parentId))
                {
                    root.TryMerge(parentId);
                }
            }
            else if (VoxelType != voxel)
            {
                Subdivide();
                Insert(position, voxel, root);
            }
        }
        else
        {
            for (byte i = 0; i < 8; i++)
            {
                if (!Children[i].NodeId.GetBoundingBox().Contains(position)) continue;

                Children[i].Insert(position, voxel, root);
            }
        }
    }

    private void TryMerge(NodeId nodeId)
    {
        var node = this;
        for (byte i = 0; i < nodeId.GetDepth(); i++)
        {
            node = node.Children[nodeId.GetChild(i)];
        }

        if (node == null)
            return;

        var childVoxelType = node.Children[0].VoxelType;
        for (var i = 1; i < 8; i++)
        {
            if (!node.Children[i].IsLeaf() || node.Children[i].VoxelType != childVoxelType)
            {
                return;
            }
        }

        node.Merge(this);
    }

    private void Merge(Octree root)
    {
        VoxelType = Children[0].VoxelType;

        Children.Clear();

        var parentId = new NodeId(0);
        if (NodeId.GetParent(ref parentId))
        {
            root.TryMerge(parentId);
        }
    }
}