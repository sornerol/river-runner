using Godot;
using System;
using System.Threading.Tasks;

public class HUD : CanvasLayer
{
    private Color WHITE = new Color(1, 1, 1);

    private Color RED = new Color(1, 0, 0);

    [Export]
    public int fuelWarningThreshold;

    private Label scoreLabel;

    private Label livesLabel;

    private Label messageLabel;

    private Label instructionsLabel;

    private ProgressBar fuelGauge;

    private AnimationPlayer animationPlayer;

    private Timer fuelWarningTimer;

    private AudioStreamPlayer fuelWarningSfx;

    public override void _Ready()
    {
        scoreLabel = GetNode<Label>("ColorRect/Score");
        fuelGauge = GetNode<ProgressBar>("ColorRect/Fuel");
        livesLabel = GetNode<Label>("ColorRect/Lives");
        messageLabel = GetNode<Label>("Message");
        instructionsLabel = GetNode<Label>("Instructions");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        fuelWarningTimer = GetNode<Timer>("FuelWarningTimer");
        fuelWarningSfx = GetNode<AudioStreamPlayer>("FuelWarningSfx");
    }

    public void updateScore(int newScore)
    {
        scoreLabel.Text = newScore.ToString();
    }

    public void updateLives(int newLivesCount)
    {
        livesLabel.Text = "x" + newLivesCount.ToString();
    }

    public void showMessage(String message)
    {
        updateMessage(message);
        messageLabel.Visible = true;
        animationPlayer.Play("MessageIn");
    }

    public void updateMessage(String message)
    {
        messageLabel.Text = message;
    }

    public async Task clearMessage()
    {
        animationPlayer.Play("MessageOut");
        await ToSignal(animationPlayer, "animation_finished");
        messageLabel.Visible = false;
    }

    public void showInstructions()
    {
        instructionsLabel.Visible = true;
    }

    public void hideInstructions()
    {
        instructionsLabel.Visible = false;
    }

    public void updateFuelLevel(int newFuelLevel)
    {
        StyleBox fgStyleBox = (StyleBox)fuelGauge.Get("custom_styles/fg");
        
        int oldFuelLevel = (int) fuelGauge.Value;
        fuelGauge.Value = newFuelLevel;
        if (oldFuelLevel > fuelWarningThreshold && newFuelLevel <= fuelWarningThreshold) {
            startFuelWarning();
        }

        if (oldFuelLevel <= fuelWarningThreshold && newFuelLevel > fuelWarningThreshold) {
            stopFuelWarning();
        }

        float r = getGaugeRColorValue(newFuelLevel);
        float g = getGaugeGColorValue(newFuelLevel);
        Color gaugeColor = new Color(r, g, 0);
        fgStyleBox.Set("bg_color", gaugeColor);
    }

    public void _OnFuelWarningTimerTimeout()
    {
        fuelWarningSfx.Play();
    }

    public void startFuelWarning()
    {
        StyleBox fgStyleBox = (StyleBox)fuelGauge.Get("custom_styles/fg");
        StyleBox bgStyleBox = (StyleBox)fuelGauge.Get("custom_styles/bg");
        fuelWarningSfx.Play();
        fuelWarningTimer.Start();
        fgStyleBox.Set("border_color", RED);
        bgStyleBox.Set("border_color", RED);
    }

    public void stopFuelWarning()
    {
        StyleBox fgStyleBox = (StyleBox)fuelGauge.Get("custom_styles/fg");
        StyleBox bgStyleBox = (StyleBox)fuelGauge.Get("custom_styles/bg");
        fuelWarningTimer.Stop();
        fgStyleBox.Set("border_color", WHITE);
        bgStyleBox.Set("border_color", WHITE);
    }

    private float getGaugeRColorValue(int fuelLevel)
    {
        float rValue = (100 - fuelLevel) / 50f;
        return Mathf.Clamp(rValue, 0f, 1f);
    }

    private float getGaugeGColorValue(int fuelLevel)
    {

        float gValue = fuelLevel / 50f;
        return Mathf.Clamp(gValue, 0f, 1f);
    }
}
