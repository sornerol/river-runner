using Godot;
public class ShootableBase : Area2D {

    [Signal]
    public delegate void shootableHit(int pointsScored);

    [Export]
    public int scoreValue;    
}