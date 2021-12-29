using Godot;
using System;

public class Player : Sprite {

    private const int LEFT = 0;
    private const int NEUTRAL = 1;
    private const int RIGHT = 2;

    [Export]
    public float turnSpeed;

    public override void _Ready() {
        initializePosition();
    }

    public override void _Process(float delta) {
        checkInputAndMovePlayer(delta);
    }

    private void initializePosition() {
        Vector2 viewportSize = GetViewport().Size;
        Vector2 pos = Position;
        pos.x = viewportSize.x / 2;
        pos.y = viewportSize.y * 0.8f;

        Position = pos;
    }

    private void checkInputAndMovePlayer(float delta) {
        Vector2 pos = Position;
        if (Input.IsActionPressed("ui_left")) {
            Frame = LEFT;
            pos.x -= turnSpeed * delta;
            Position = pos;
        } else if (Input.IsActionPressed("ui_right")) {
            Frame = RIGHT;
            pos.x += turnSpeed * delta;
            Position = pos;
        } else {
            Frame = NEUTRAL;
        }
    }
}
