using Godot;
public class ShootableBase : Area2D
{

    [Signal]
    public delegate void shootableHit();

    [Export]
    public int scoreValue;

    private AudioStreamPlayer explosionSfx;

    public override void _Ready()
    {
        base._Ready();
        explosionSfx = GetNode<AudioStreamPlayer>("ExplosionSfx");
        Connect(nameof(shootableHit), GetNode("/root/Main"), nameof(main._OnShootableHit));

    }

    public async void destroy(int scoreValue)
    {
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
        AnimatedSprite explosionAnimation = GetNode<AnimatedSprite>("AnimatedSprite");
        explosionAnimation.Animation = "explosion";
        explosionAnimation.Play();
        explosionSfx.Play();
        EmitSignal(nameof(shootableHit), scoreValue);
        await ToSignal(explosionAnimation, "animation_finished");
        QueueFree();
    }
}