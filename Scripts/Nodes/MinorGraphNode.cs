using Godot;
using Godot.Collections;
using System;
using PacNoManFSharp;

public class MinorGraphNode : PacNoManFSharp.MinorNode
{
    [Export]
    public NodePath topNodePath = null;
    [Export]
    public NodePath bottomNodePath = null;
    [Export]
    public NodePath leftNodePath = null;
    [Export]
    public NodePath rightNodePath = null;

    public override void _Ready()
    {
        this.top = null;
        this.bottom = null;
        this.left = null;
        this.right = null;

        if (topNodePath != null)
            this.top = GetNodeOrNull<IGraphNode>(topNodePath);
        if (bottomNodePath != null)
            this.bottom = GetNodeOrNull<IGraphNode>(bottomNodePath);
        if (leftNodePath != null)
            this.left = GetNodeOrNull<IGraphNode>(leftNodePath);
        if (rightNodePath != null)
            this.right = GetNodeOrNull<IGraphNode>(rightNodePath);
    }
}
