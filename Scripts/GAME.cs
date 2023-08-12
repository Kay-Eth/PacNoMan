using Godot;
using System;
using System.Collections.Generic;

public class GAME : Node
{
    Ghost aquaGhost;
    Ghost orangeGhost;
    Ghost pinkGhost;
    Ghost redGhost;

    [Signal]
    public delegate void CanAquaStart();
    [Signal]
    public delegate void CanOrangeStart();
    [Signal]
    public delegate void CanPinkStart();
    [Signal]
    public delegate void CanRedStart();

    public bool AreGhostsScared { get; private set; }

    Timer _changeModeTimer;
    Timer _energizerTimer;

    public override void _Ready()
    {
        OS.WindowMaximized = true;

        aquaGhost = GetNode<Ghost>("Ghosts/AquaGhost");
        orangeGhost = GetNode<Ghost>("Ghosts/OrangeGhost");
        pinkGhost = GetNode<Ghost>("Ghosts/PinkGhost");
        redGhost = GetNode<Ghost>("Ghosts/RedGhost");

        PacNoManFSharp.Ghost.TopLeftCornerNode = GetNode<MajorGraphNode>("GraphNodes/Major4");
        PacNoManFSharp.Ghost.TopRightCornerNode = GetNode<MajorGraphNode>("GraphNodes/Major9");
        PacNoManFSharp.Ghost.BottomLeftCornerNode = GetNode<MajorGraphNode>("GraphNodes/Major31");
        PacNoManFSharp.Ghost.BottomRightCornerNode = GetNode<MajorGraphNode>("GraphNodes/Major32");

        PacNoManFSharp.Ghost.Major15Node = GetNode<MajorGraphNode>("GraphNodes/Major15");
        PacNoManFSharp.Ghost.Major18Node = GetNode<MajorGraphNode>("GraphNodes/Major18");

        _changeModeTimer = GetNode<Timer>("ChangeModeTimer");
        _energizerTimer = GetNode<Timer>("EnergizerTimer");

        InitGame();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey iek)
        {
            if (iek.Pressed && iek.Scancode == (int)KeyList.Space)
                ReturnToNormal();
        }
    }

    async void InitGame()
    {
        StartRed();
        StartPink();
        StartAqua();
        StartOrange();

        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        EmitSignal(nameof(CanRedStart));
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        EmitSignal(nameof(CanPinkStart));
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        EmitSignal(nameof(CanAquaStart));
        await ToSignal(GetTree().CreateTimer(1f), "timeout");
        EmitSignal(nameof(CanOrangeStart));
    }

    public void SetGhostToScaredMode()
    {
        _changeModeTimer.Paused = true;
        _energizerTimer.Start();

        AreGhostsScared = true;

        aquaGhost.GoToScaredMode();
        orangeGhost.GoToScaredMode();
        pinkGhost.GoToScaredMode();
        redGhost.GoToScaredMode();
    }

    public void ReturnToNormal()
    {
        GD.Print("RETURN TO NORMAL");
        _changeModeTimer.Paused = false;

        AreGhostsScared = false;

        aquaGhost.IsDead = false;
        orangeGhost.IsDead = false;
        pinkGhost.IsDead = false;
        redGhost.IsDead = false;

        if (aquaGhost.CurrentState == PacNoManFSharp.GhostState.Scared || aquaGhost.NextState == PacNoManFSharp.GhostState.Scared)
            aquaGhost.GoToScatterMode();
        if (orangeGhost.CurrentState == PacNoManFSharp.GhostState.Scared || orangeGhost.NextState == PacNoManFSharp.GhostState.Scared)
            orangeGhost.GoToScatterMode();
        if (pinkGhost.CurrentState == PacNoManFSharp.GhostState.Scared || pinkGhost.NextState == PacNoManFSharp.GhostState.Scared)
            pinkGhost.GoToScatterMode();
        if (redGhost.CurrentState == PacNoManFSharp.GhostState.Scared || redGhost.NextState == PacNoManFSharp.GhostState.Scared)
            redGhost.GoToScatterMode();
    }

    async void StartOrange()
    {
        await ToSignal(this, nameof(CanOrangeStart));
        orangeGhost.Init(GetNode<MajorGraphNode>("GraphNodes/StartMajor3"));
    }

    async void StartPink()
    {
        await ToSignal(this, nameof(CanPinkStart));
        pinkGhost.Init(GetNode<MajorGraphNode>("GraphNodes/StartMajor"));
    }

    async void StartAqua()
    {
        await ToSignal(this, nameof(CanAquaStart));
        aquaGhost.Init(GetNode<MajorGraphNode>("GraphNodes/StartMajor2"));
    }

    async void StartRed()
    {
        await ToSignal(this, nameof(CanRedStart));
        redGhost.Init(GetNode<MajorGraphNode>("GraphNodes/ExitMajor"));
    }

    PacNoManFSharp.GhostState _currentState = PacNoManFSharp.GhostState.Scatter;
    public void OnChangeModeTimerTimeout()
    {
        if (_currentState == PacNoManFSharp.GhostState.Scatter)
            _currentState = PacNoManFSharp.GhostState.Follow;
        else
            _currentState = PacNoManFSharp.GhostState.Scatter;
        if (aquaGhost.CurrentState != PacNoManFSharp.GhostState.Dead && aquaGhost.CurrentState != PacNoManFSharp.GhostState.Idle) aquaGhost.SetCurrentState(_currentState);
        if (orangeGhost.CurrentState != PacNoManFSharp.GhostState.Dead && orangeGhost.CurrentState != PacNoManFSharp.GhostState.Idle) orangeGhost.SetCurrentState(_currentState);
        if (pinkGhost.CurrentState != PacNoManFSharp.GhostState.Dead && pinkGhost.CurrentState != PacNoManFSharp.GhostState.Idle) pinkGhost.SetCurrentState(_currentState);
        if (redGhost.CurrentState != PacNoManFSharp.GhostState.Dead && redGhost.CurrentState != PacNoManFSharp.GhostState.Idle) redGhost.SetCurrentState(_currentState);

        //if (aquaGhost.CurrentState == PacNoManFSharp.GhostState.Scared || aquaGhost.NextState == PacNoManFSharp.GhostState.Scared)
    }

    public void OnEnergizerTimerTimeout()
    {
        ReturnToNormal();
        _changeModeTimer.Start();
    }
}
