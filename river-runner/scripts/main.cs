using Godot;

public class main : Node2D
{

    [Export]
    public int initialLives;

    [Export]
    public int freeLifeScoreInterval;

    private int score;

    private int livesRemaining;

    private int lastFreeLifeEarnedScore;

    private bool awaitingPlayerStart;

    private River river;

    private Timer getReadyTimer;

    private Player player;

    HUD hud;

    public override void _Ready()
    {
        GD.Randomize();
        hud = GetNode<HUD>("HUD");
        river = GetNode<River>("River");
        getReadyTimer = GetNode<Timer>("GetReadyTimer");
        player = GetNode<Player>("Player");
        initializeNewGame();
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
        if (!awaitingPlayerStart)
        {
            return;
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            awaitingPlayerStart = false;
            startGetReadyInterval();
        }
    }

    public void initializeNewGame()
    {
        resetScore();
        resetLives();
        startGetReadyInterval();
    }

    public void resetScore()
    {
        score = 0;
        hud.updateScore(score);
    }

    public void resetLives()
    {
        livesRemaining = initialLives;
        lastFreeLifeEarnedScore = 0;
        hud.updateLives(livesRemaining);
    }

    public void startGetReadyInterval()
    {
        hud.showMessage("Ready!");
        getReadyTimer.Start();
    }

    public void _OnGetReadyTimeout()
    {
        hud.clearMessage();
        river.startMoving();
        player.startTurn();
    }

    public void _OnShootableHit(int pointsToAdd)
    {
        score += pointsToAdd;
        if (score - lastFreeLifeEarnedScore > freeLifeScoreInterval)
        {
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
