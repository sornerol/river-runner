using Godot;

public class EnemyBase : KinematicBody2D
{
    private const int TERRAIN_MASK_BIT = 0;
    [Export]
    public bool isAquaticVehicle;

    [Export]
    public float horizontalSpeed;

    private bool directionFlipped;

    public override void _Ready()
    {
        SetCollisionMaskBit(TERRAIN_MASK_BIT, isAquaticVehicle);
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);
        Vector2 movement = new Vector2(getHorizontalMovement(delta), 0);
        KinematicCollision2D collision =  MoveAndCollide(movement);
        if (collision != null) {
            if(collision.Collider.IsClass("Node")) 
            {
                 Node collider = (Node) collision.Collider;
                 if (collider.IsInGroup("terrain")) {
                    Vector2 scale = Scale;
                    scale.x *= -1;
                    Scale = scale;
                    directionFlipped = !directionFlipped;
                 }
            }
        }
    }

    public void _OnScreenExited() {
        QueueFree();
    }

    private float getHorizontalMovement(float delta)
    {
        float movement = horizontalSpeed * delta;
        if (!directionFlipped) {
            movement *= -1;
        }
        return movement;
    }
}
