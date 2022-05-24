using Godot;

public class main : Node2D {

    [Export]
    public int initialLives;

    [Export]
    public int freeLifeScoreInterval;
    
    private int score;

    private int livesRemaining;

    private int lastFreeLifeEarnedScore;

    private River river;

    HUD hud;

    public override void _Ready() {
        hud = GetNode<HUD>("HUD");
        river = GetNode<River>("River");
        initializeNewGame();
    }

    public void initializeNewGame()
    {
        resetScore();
        resetLives();
    }

    public void resetScore() {
        score = 0;
        hud.updateScore(score);
    }

    public void resetLives() {
        livesRemaining = initialLives;
        lastFreeLifeEarnedScore = 0;
        hud.updateLives(livesRemaining);
    }

    public void _OnShootableHit(int pointsToAdd)
    {
        score += pointsToAdd;
        if (score - lastFreeLifeEarnedScore > freeLifeScoreInterval) {
            livesRemaining++;
            lastFreeLifeEarnedScore += freeLifeScoreInterval;
            hud.updateLives(livesRemaining);
        }
        hud.updateScore(score);
    }

    public void _OnPlaneCrashed()
    {
        livesRemaining--;
        hud.updateLives(livesRemaining);
        river.stopMoving();
        //TODO: If livesRemaining < 0, game over
        
    }
}
