using Godot;
using Godot.Collections;
using System;
using PacNoManFSharp;

public class MajorGraphNode : PacNoManFSharp.MajorNode
{
    [Export]
    public NodePath topNodePath = null;
    [Export]
    public NodePath bottomNodePath = null;
    [Export]
    public NodePath leftNodePath = null;
    [Export]
    public NodePath rightNodePath = null;

    [Export]
    public Array<NodePath> topNodePathArray = new Array<NodePath>();
    [Export]
    public Array<NodePath> bottomNodePathArray = new Array<NodePath>();
    [Export]
    public Array<NodePath> leftNodePathArray = new Array<NodePath>();
    [Export]
    public Array<NodePath> rightNodePathArray = new Array<NodePath>();

    public override void _Ready()
    {
        this.top = null;
        this.bottom = null;
        this.left = null;
        this.right = null;

        if (topNodePath != null)
            this.top = GetNodeOrNull<MajorGraphNode>(topNodePath);
        if (bottomNodePath != null)
            this.bottom = GetNodeOrNull<MajorGraphNode>(bottomNodePath);
        if (leftNodePath != null)
            this.left = GetNodeOrNull<MajorGraphNode>(leftNodePath);
        if (rightNodePath != null)
            this.right = GetNodeOrNull<MajorGraphNode>(rightNodePath);

        this.pathTop = new MinorNode[topNodePathArray.Count];
        this.pathBottom = new MinorNode[bottomNodePathArray.Count];
        this.pathLeft = new MinorNode[leftNodePathArray.Count];
        this.pathRight = new MinorNode[rightNodePathArray.Count];

        for (int i = 0; i < pathTop.Length; i++)
        {
            pathTop[i] = GetNode<MinorNode>(topNodePathArray[i]);
        }

        for (int i = 0; i < pathBottom.Length; i++)
        {
            pathBottom[i] = GetNode<MinorNode>(bottomNodePathArray[i]);
        }

        for (int i = 0; i < pathLeft.Length; i++)
        {
            pathLeft[i] = GetNode<MinorNode>(leftNodePathArray[i]);
        }

        for (int i = 0; i < pathRight.Length; i++)
        {
            pathRight[i] = GetNode<MinorNode>(rightNodePathArray[i]);
        }
    }
}
