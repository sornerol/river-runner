using Godot;

public class Player : KinematicBody2D {

    private const int LEFT = 0;
    private const int NEUTRAL = 1;
    private const int RIGHT = 2;

    [Export]
    public float turnSpeed;

    [Signal]
    public delegate void planeCrashed();

    private Sprite playerSprite;

    private bool playerIsMoving;

    public override void _Ready() {
        playerSprite = GetNode<Sprite>("Player");
        initializePosition();
        playerIsMoving = true;
    }

    public override void _Process(float delta) {
    }

    public override void _PhysicsProcess(float delta) {
        if (!playerIsMoving) {
            return;
        }
        Vector2 movement = new Vector2();
        if (Input.IsActionPressed("ui_left")) {
            playerSprite.Frame = LEFT;
            movement.x = -turnSpeed * delta;
        } else if (Input.IsActionPressed("ui_right")) {
            playerSprite.Frame = RIGHT;
            movement.x = turnSpeed * delta;
        } else {
            playerSprite.Frame = NEUTRAL;
        }
        KinematicCollision2D collision = MoveAndCollide(movement);
        if (collision != null) {
            EmitSignal(nameof(planeCrashed));
            playerIsMoving = false;
        }
    }
    private void initializePosition() {
        Vector2 viewportSize = GetViewport().Size;
        Vector2 pos = Position;
        pos.x = viewportSize.x / 2;
        pos.y = viewportSize.y * 0.8f;

        Position = pos;
    }
}
