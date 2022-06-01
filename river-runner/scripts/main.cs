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

    private bool attractMode;

    private River river;

    private Timer getReadyTimer;

    private Timer pauseAfterCrashTimer;

    private Player player;

    HUD hud;

    public override void _Ready()
    {
        GD.Randomize();
        hud = GetNode<HUD>("HUD");
        river = GetNode<River>("River");
        getReadyTimer = GetNode<Timer>("GetReadyTimer");
        pauseAfterCrashTimer = GetNode<Timer>("PauseAfterCrashTimer");
        player = GetNode<Player>("Player");
        startAttractMode();
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
            if (attractMode)
            {
                initializeNewGame();
            }
            else
            {
                initializeNewTurn();
            }
            
        }
    }

    public async void startAttractMode() {
        await hud.clearMessage();
        hud.showMessage("RIVER RUNNER\nby Sornerol\nPress <space>...");
        hud.showInstructions();
        attractMode = true;
        awaitingPlayerStart = true;
        player.Visible = false;
        river.startMoving();
    }
    
    public void initializeNewGame()
    {
        attractMode = false;
        player.Visible = true;
        river.stopMoving();
        river.setupForNewGame();
        resetScore();
        resetLives();
        hud.hideInstructions();
        initializeNewTurn();
    }

    public void initializeNewTurn() 
    {
        awaitingPlayerStart = false;
        GetTree().CallGroup("enemies", "queue_free");
        river.setupForNewTurn();
        player.initializePlayerForNewTurn();
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

    public async void startGetReadyInterval()
    {
        await hud.clearMessage();      
        hud.showMessage("Ready!");
        getReadyTimer.Start();
    }

    public async void _OnGetReadyTimeout()
    {
        await hud.clearMessage();
        river.startMoving();
        player.startTurn();
    }

    public void _OnPauseTimeout()
    {
        if (livesRemaining < 1)
        {
            startAttractMode();
        }
        else
        {
            awaitingPlayerStart = true;
        }
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
        hud.stopFuelWarning();
        if (livesRemaining < 1)
        {
            hud.showMessage("Game over");
        }
        else
        {
            hud.showMessage("Crash!\nPress <space>...");
        }
        river.stopMoving();
        pauseAfterCrashTimer.Start();
    }
}
