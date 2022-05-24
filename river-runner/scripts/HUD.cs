using Godot;
using System;

public class HUD : CanvasLayer
{

    private Label scoreLabel;

    private Label livesLabel;

    private Label messageLabel;

    private ProgressBar fuelGauge;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        scoreLabel = GetNode<Label>("ColorRect/Score");
        fuelGauge = GetNode<ProgressBar>("ColorRect/Fuel");
        livesLabel = GetNode<Label>("ColorRect/Lives");
        messageLabel = GetNode<Label>("Message");
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
        messageLabel.Text = message;
        messageLabel.Visible = true;
    }

    public void clearMessage()
    {
        messageLabel.Visible = false;
    }

    public void updateFuelLevel(int newFuelLevel)
    {
        fuelGauge.Value = newFuelLevel;
        float r = getGaugeRColorValue(newFuelLevel);
        float g = getGaugeGColorValue(newFuelLevel);
        StyleBox styleBox = (StyleBox)fuelGauge.Get("custom_styles/fg");
        Color gaugeColor = new Color(r, g, 0);
        styleBox.Set("bg_color", gaugeColor);
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
