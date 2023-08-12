using Godot;
using System;

public class Food : PacNoManFSharp.Food
{
    public override void EndGame()
    {
        GetTree().ChangeScene("res://Victory.scn");
    }
}
