using Godot;
using System;
using PacNoManFSharp;

public class Player : PacNoManFSharp.Player
{
    [Signal]
    public delegate void LastVisitedMajorChanged();
    [Signal]
    public delegate void LastVisitedMinorChanged();
    [Signal]
    public delegate void EndGame();

    Sprite _sprite;

    GAME _game;

    public override void _Ready()
    {
        LastVisitedMajorNode = GetNode<MajorGraphNode>("../GraphNodes/Major27");
        _sprite = GetNode<Sprite>("Sprite");
        StartAnimationFrame = 0;
        PlayerRayCast = GetNode<RayCast2D>("RayCast2D");
        _game = GetParent<GAME>();
    }

    public override void ChangeAnimationFrame()
    {
        if (_sprite.Frame == StartAnimationFrame)
            _sprite.Frame = StartAnimationFrame + 1;
        else
            _sprite.Frame = StartAnimationFrame;
    }

    public override void SendLastVisitedMajorChangedSignal()
    {
        
    }

    public override void SendLastVisitedMinorChangedSignal()
    {
        
    }

    public override void SendEndGameSignal()
    {
        
    }

    public void OnPlayerBodyEntered(PhysicsBody2D body)
    {
        if (body is Ghost gh)
        {
            if (_game.AreGhostsScared)
            {
                gh.Die();
            }
            else
            {
                GetTree().ChangeScene("res://Failure.scn");
            }
        }
        else if (body.GetParent() is MajorNode mn && mn.Name != "ExitMajor")
        {
            LastVisitedMajorNode = mn;
            LastVisitedMinorNode = null;
        }
        else if (body.GetParent() is MinorNode mmn)
        {
            LastVisitedMinorNode = mmn;
        }
        else if (body is Food food)
        {
            food.EatFood();
        }
        else if (body is Energizer energizer)
        {
            energizer.QueueFree();

            _game.SetGhostToScaredMode();
        }
    }
}
