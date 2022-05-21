using Godot;
using System;

public class HUD : CanvasLayer
{
    
    private Label scoreLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        scoreLabel = GetNode<Label>("ColorRect/Score");
    }

    public void updateScore(int newScore) {
        scoreLabel.Text = newScore.ToString();
    }
}
