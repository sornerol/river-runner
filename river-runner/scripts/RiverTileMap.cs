using Godot;
using System;

public class RiverTileMap : TileMap
{
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        
    }

    public RiverGeneratorState generateTerrain(RiverGeneratorState currentState) {
        for(int y = 19; y >=0; y--) {
            for(int x = 0; x < 13; x++) {
                if (x < currentState.leftBankIndex || x > currentState.leftBankIndex + currentState.riverWidth + 1) {
                    SetCell(x, y, TileSet.FindTileByName("grass"));
                } else if (x == currentState.leftBankIndex) {
                    SetCell(x, y, TileSet.FindTileByName("straight_left"));                    
                } else if (x == currentState.leftBankIndex + currentState.riverWidth + 1) {
                    SetCell(x, y, TileSet.FindTileByName("straight_right"));                    
                } else {
                    SetCell(x, y, -1);                                        
                }
            }
        }
        return currentState;
    }
}
