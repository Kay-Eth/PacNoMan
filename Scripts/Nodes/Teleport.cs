using Godot;
using System;

public class Teleport : Area2D
{
    [Export]
    public Vector2 teleportTo;

    public void OnTeleportBodyEntered(PhysicsBody2D body)
    {
        if (body is Ghost gh)
        {
            GD.Print("TELEPORT");
            gh.SetProcess(false);
            gh.Position = teleportTo;
            gh.TeleportHack();
            gh.SetProcess(true);
        }
    }

    public void OnTeleportAreaEntered(Area2D area)
    {
        if (area is Player player)
        {
            player.SetProcess(false);
            player.Position = teleportTo;
            player.ResetDistance();
            player.SetProcess(true);
        }
    }
}
