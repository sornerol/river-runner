using Godot;
public class ShootableBase : Area2D {

    [Signal]
    public delegate void shootableHit();

    [Export]
    public int scoreValue;

    public override void _Ready()
    {
        base._Ready();
        Connect(nameof(shootableHit), GetNode("/root/Main"), nameof(main._OnShootableHit));

    }
}