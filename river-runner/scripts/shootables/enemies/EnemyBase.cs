using Godot;

public class EnemyBase : ShootableBase
{
    private const int TERRAIN_MASK_BIT = 0;

    [Export]
    public bool isAquaticVehicle;

    [Export]
    public float minHorizontalSpeed;

    [Export]
    public float maxHorizontalSpeed;

    public float horizontalSpeed;

    public bool directionFlipped;

    private bool isMoving;

    public override void _Ready()
    {
        base._Ready();
        SetCollisionMaskBit(TERRAIN_MASK_BIT, isAquaticVehicle);
        horizontalSpeed = (float)GD.RandRange(minHorizontalSpeed, maxHorizontalSpeed);
        isMoving = false;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!isMoving)
        {
            return;
        }
        base._PhysicsProcess(delta);
        Vector2 movement = getHorizontalMovement();
        Position += movement * delta;
    }

    public void _OnBodyEntered(PhysicsBody2D body)
    {
        int pointsToAdd = scoreValue;

        if (body.IsInGroup("terrain"))
        {
            flipDirection();
            return;
        }
        if (body.IsInGroup("bullet"))
        {
            ((Bullet)body).despawn();
        }

        if (body.IsInGroup("player"))
        {
            pointsToAdd = 0;
            ((Player)body).crashPlane();
        }
        isMoving = false;
        destroy(pointsToAdd);
    }

    public void flipDirection()
    {
        Vector2 scale = Scale;
        scale.x *= -1;
        Scale = scale;
        directionFlipped = !directionFlipped;
    }

    public void _OnScreenExited()
    {
        QueueFree();
    }

    public void _OnScreenEntered()
    {
        isMoving = true;
    }

    private Vector2 getHorizontalMovement()
    {
        Vector2 movement = Vector2.Zero;
        movement.x = directionFlipped ? 1 : -1;

        return movement.Normalized() * horizontalSpeed;
    }
}
