using Godot;

public class main : Node2D {

    int score;

    HUD hud;

    public override void _Ready() {
        hud = GetNode<HUD>("HUD");
        initializeNewGame();
    }

    public void initializeNewGame()
    {
        resetScore();
    }

    public void resetScore() {
        score = 0;
        hud.updateScore(score);
    }

    public void _OnShootableHit(int pointsToAdd)
    {
        score += pointsToAdd;
        hud.updateScore(score);
    }
}
