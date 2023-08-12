using Godot;
using Godot.Collections;
using PacNoManFSharp;
using System;
using System.Linq;

public class Ghost : PacNoManFSharp.Ghost
{
    [Signal]
    public delegate void TargetNodeReached();

    Sprite _sprite;

    [Export]
    public Array<NodePath> scatterPathNodePaths = new Array<NodePath>();
    IGraphNode[] scatterPath;

    public override IGraphNode[] ScatterPath { get { return scatterPath; } }

    [Export]
    public NodePath startMajorNodePath;
    MajorNode _startMajorNode;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Sprite");
        this.StartAnimationFrame = 0;

        scatterPath = new IGraphNode[scatterPathNodePaths.Count];
        _startMajorNode = GetNode<MajorNode>(startMajorNodePath);
        LastVisitedNode = _startMajorNode;

        for (int i = 0; i < scatterPathNodePaths.Count; i++)
        {
            scatterPath[i] = GetNode<IGraphNode>(scatterPathNodePaths[i]);
        }
    }

    public override void SendReachedSignal()
    {
        EmitSignal(nameof(TargetNodeReached));
    }

    int _scaredIndex = 8;
    public override void ChangeAnimationFrame()
    {
        if (this.CurrentState == GhostState.Scared || this.NextState == GhostState.Scared)
        {
            _sprite.Frame = _scaredIndex++;
            if (_scaredIndex == 12) _scaredIndex = 8;
        }
        else if (this.CurrentState == GhostState.Dead)
        {
            if (this.StartAnimationFrame == (int)Directions.Top) _sprite.Frame = 12;
            else if (this.StartAnimationFrame == (int)Directions.Bottom) _sprite.Frame = 13;
            else if (this.StartAnimationFrame == (int)Directions.Left) _sprite.Frame = 14;
            else _sprite.Frame = 15;
        }
        else
        {
            if (_sprite.Frame == this.StartAnimationFrame)
                _sprite.Frame = this.StartAnimationFrame + 1;
            else
                _sprite.Frame = this.StartAnimationFrame;
        }
    }

    public override MajorNode GetStartMajorNode()
    {
        return _startMajorNode;
    }
}
