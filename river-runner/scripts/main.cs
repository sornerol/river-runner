using Godot;

public class main : Node2D {

    int score;
    public override void _Ready() {

    }

    public void initializeNewGame()
    {
        score = 0;
    }

    public void _OnShootableHit(int pointsToAdd)
    {
        score += pointsToAdd;
    }
}
