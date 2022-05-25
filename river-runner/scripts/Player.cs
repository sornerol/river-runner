using Godot;

public class Player : KinematicBody2D
{

    private const int LEFT = 0;
    private const int NEUTRAL = 1;
    private const int RIGHT = 2;

    [Export]
    public float turnSpeed;

    [Export]
    public PackedScene bullet;

    [Export]
    public float fuelIncreaseRate;

    [Export]
    public float fuelBurnRateBase;

    [Export]
    public float maxFuelCapacity;

    [Signal]
    public delegate void planeCrashed();

    [Signal]
    public delegate void fuelLevelChanged(float newFuelLevel);

    private AnimatedSprite playerSprite;

    private float fuelLevel;

    private bool playerIsMoving;

    private bool playerIsFueling;

    public override void _Ready()
    {
        playerSprite = GetNode<AnimatedSprite>("Player");
        GetNode<CollisionPolygon2D>("CollisionPolygon2D").SetDeferred("disabled", true);
        initializePlayerForNewTurn();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!playerIsMoving)
        {
            return;
        }
        Vector2 movement = new Vector2();
        if (Input.IsActionPressed("ui_left"))
        {
            playerSprite.Frame = LEFT;
            movement.x = -turnSpeed * delta;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            playerSprite.Frame = RIGHT;
            movement.x = turnSpeed * delta;
        }
        else
        {
            playerSprite.Frame = NEUTRAL;
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            Bullet newBullet = (Bullet)bullet.Instance();
            Vector2 bulletSpawnLocation = this.GlobalPosition;
            bulletSpawnLocation.y -= 24;
            newBullet.GlobalPosition = bulletSpawnLocation;
            GetTree().Root.AddChild(newBullet);
        }
        KinematicCollision2D collision = MoveAndCollide(movement);
        if (collision != null)
        {
            crashPlane();
        }
        adjustFuelLevel(delta);
    }

    public void adjustFuelLevel(float delta)
    {
        if (playerIsFueling)
        {
            fuelLevel += fuelIncreaseRate * delta;
        }
        else
        {
            fuelLevel -= fuelBurnRateBase * delta;
        }
        fuelLevel = Mathf.Clamp(fuelLevel, 0, maxFuelCapacity);
        int fuelLevelPercentage = (int)(fuelLevel / maxFuelCapacity * 100);
        EmitSignal(nameof(fuelLevelChanged), fuelLevelPercentage);
        if (fuelLevel <= 0)
        {
            GD.Print("Fuel level is zero");
            crashPlane();
        }
    }

    public void startTurn()
    {
        playerIsMoving = true;
        GetNode<CollisionPolygon2D>("CollisionPolygon2D").SetDeferred("disabled", false);
    }

    public void crashPlane()
    {
        EmitSignal(nameof(planeCrashed));
        playerSprite.Animation = "explosion";
        playerSprite.Play();
        playerIsMoving = false;
        playerIsFueling = false;
        GetNode<CollisionPolygon2D>("CollisionPolygon2D").SetDeferred("disabled", true);
    }

    public void startFueling()
    {
        playerIsFueling = true;
    }

    public void stopFueling()
    {
        playerIsFueling = false;
    }

    public void initializePlayerForNewTurn()
    {
        Vector2 viewportSize = GetViewport().Size;
        Vector2 pos = Position;
        pos.x = viewportSize.x / 2;
        pos.y = viewportSize.y * 0.8f;
        Position = pos;

        playerSprite.Animation = "default";
        playerSprite.Stop();
        playerSprite.Frame = 1;

        fuelLevel = maxFuelCapacity * 0.8f;
        int fuelLevelPercentage = (int)(fuelLevel / maxFuelCapacity * 100);
        EmitSignal(nameof(fuelLevelChanged), fuelLevelPercentage);
    }
}
