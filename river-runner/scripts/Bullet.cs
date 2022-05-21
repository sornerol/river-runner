using Godot;

public class Bullet : KinematicBody2D
{
    [Export]
    public float defaultSpeed;

    public override void _Ready()
    {
        
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Vector2 movement = new Vector2(0, -defaultSpeed * delta);
        KinematicCollision2D collision = MoveAndCollide(movement);

        if (collision != null) {
            despawn();
        }
    }

    public void _OnScreenExited() {
        despawn();
    }

    public void despawn() 
    {
        QueueFree();
    }
}
