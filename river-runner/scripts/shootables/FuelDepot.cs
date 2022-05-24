using Godot;
using System;

public class FuelDepot : ShootableBase
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }

    public void _OnScreenEntered() { }

    public void _OnScreenExited()
    {
        QueueFree();
    }
    public void _OnBodyEntered(PhysicsBody2D body)
    {
        if (body.IsInGroup("player"))
        {
            ((Player)body).startFueling();
            return;
        }

        if (body.IsInGroup("bullet"))
        {
            ((Bullet)body).despawn();
            destroy(scoreValue);
        }
    }

    public void _OnBodyExited(PhysicsBody2D body)
    {
        if (body.IsInGroup("player"))
        {
            ((Player)body).stopFueling();
        }
    }
}
