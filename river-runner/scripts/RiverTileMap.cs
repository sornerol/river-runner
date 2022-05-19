using Godot;
using System.Collections;

public class RiverTileMap : TileMap {

	[Export]
	public PackedScene[] enemyScenes;

    public int mapWidth;
    public int mapHeight;

    private const int MINIMUM_RIVER_WIDTH = 2;
    private const int LINES_WITHOUT_CHANGE = 10;
    private const int EMPTY_TILE = -1;
   
    public override void _Ready() {
        
    }

    public override void _Process(float delta) {
        
    }

    public RiverGeneratorState generateTerrain(RiverGeneratorState currentState) {
        
        for(int y = mapHeight - 1; y >=0; y--) {
            for(int x = 0; x < mapWidth; x++) {
                if (x < currentState.leftBankIndex || x > currentState.rightBankIndex) {
                    SetCell(x, y, TileSet.FindTileByName("grass"));
                } else if (x == currentState.leftBankIndex) {                    
                    SetCell(x, y, TileSet.FindTileByName(currentState.leftBankDirection + "_left"));
                } else if (x == currentState.rightBankIndex) {
                    SetCell(x, y, TileSet.FindTileByName(currentState.rightBankDirection + "_right"));                    
                } else {
                    SetCell(x, y, EMPTY_TILE);                                        
                }
            }
            attemptEnemySpawn(currentState, y);
            currentState = updateStateForNextRow(currentState);
        }
        return currentState;
    }

    private RiverGeneratorState updateStateForNextRow(RiverGeneratorState state) {
        string previousLeftBankDir = state.leftBankDirection;
        string previousRightBankDir = state.rightBankDirection;
        if (state.linesGenerated > LINES_WITHOUT_CHANGE) {
            state.leftBankDirection = attemptChangeDirection(state, "left");
            state.rightBankDirection = attemptChangeDirection(state, "right");
        }

        if (state.leftBankDirection == BankDirection.RIGHT && previousLeftBankDir != BankDirection.LEFT) {
            state.leftBankIndex++;
        }   
        if (state.leftBankDirection == BankDirection.LEFT  && previousLeftBankDir == BankDirection.LEFT) {
            state.leftBankIndex--;
        }
        if (state.leftBankDirection == BankDirection.STRAIGHT && previousLeftBankDir == BankDirection.LEFT) {
            state.leftBankIndex--;
        }        

        if (state.rightBankDirection == BankDirection.RIGHT && previousRightBankDir == BankDirection.RIGHT) {
            state.rightBankIndex++;
        }      
        if (state.rightBankDirection == BankDirection.LEFT && previousRightBankDir != BankDirection.RIGHT) {
            state.rightBankIndex--;
        }
        if(state.rightBankDirection == BankDirection.STRAIGHT && previousRightBankDir == BankDirection.RIGHT) {
            state.rightBankIndex++;
        }

        state = validateAndCorrectDirection(state, "left");
        state = validateAndCorrectDirection(state, "right");

        state.linesGenerated++;
        return state;
    }

    private string attemptChangeDirection(RiverGeneratorState currentState, string bank) {
        ArrayList possibleDirections = new ArrayList();

        possibleDirections.Add(BankDirection.STRAIGHT);
        possibleDirections.Add(BankDirection.LEFT);
        possibleDirections.Add(BankDirection.RIGHT);

        if (bank == "left") {
            if (currentState.leftBankIndex <= 0) {
                possibleDirections.Remove(BankDirection.LEFT);
            }
            if (currentState.rightBankIndex - (currentState.leftBankIndex +1) <= MINIMUM_RIVER_WIDTH +1 && currentState.rightBankDirection != BankDirection.RIGHT) {
                possibleDirections.Remove(BankDirection.RIGHT);
            }
        } else {
            if (currentState.rightBankIndex >= mapWidth) {
                possibleDirections.Remove(BankDirection.RIGHT);
            }
            if (currentState.rightBankIndex - (currentState.leftBankIndex +1) <= MINIMUM_RIVER_WIDTH +1 && currentState.leftBankDirection != BankDirection.LEFT) {
                possibleDirections.Remove(BankDirection.LEFT);
            }
        }
        uint newDirectionIndex = GD.Randi() % (uint) possibleDirections.Count;
        return (string) possibleDirections[(int) newDirectionIndex];
    }

    private void attemptEnemySpawn(RiverGeneratorState currentState, int currentRow) {
        if (currentState.linesGenerated < LINES_WITHOUT_CHANGE) {
            return;
        }
        if (GD.Randi() % 100 > 20) 
        {
            return;
        }
        uint enemyToSpawn = GD.Randi() % (uint) enemyScenes.Length;
        EnemyBase newEnemy = (EnemyBase) enemyScenes[(int) enemyToSpawn].Instance();
        AddChild(newEnemy);
        Vector2 spawnPosition = new Vector2();
        spawnPosition.y = 8 * currentRow;
        if (newEnemy.isAquaticVehicle) {
            int riverWidth = currentState.rightBankIndex - currentState.leftBankIndex;
            int spawnTile = (riverWidth / 2) + currentState.leftBankIndex;
            spawnPosition.x = (int) spawnTile * 8;
        } else {
            spawnPosition.x = mapWidth * 8;
        }
        newEnemy.Position = spawnPosition;
    }
    
    private RiverGeneratorState validateAndCorrectDirection(RiverGeneratorState currentState, string bank) {
        if (bank == "left") {
            if (currentState.leftBankDirection == BankDirection.LEFT && currentState.leftBankIndex <= 0) {
                currentState.leftBankDirection = BankDirection.STRAIGHT;
            }
            if (currentState.leftBankDirection == BankDirection.RIGHT 
                    && currentState.rightBankIndex - (currentState.leftBankIndex +1) <= MINIMUM_RIVER_WIDTH +1 
                    && currentState.rightBankDirection != BankDirection.RIGHT) {
                currentState.leftBankDirection = BankDirection.STRAIGHT;
                currentState.leftBankIndex--;
            }
        } else {
            if (currentState.rightBankDirection == BankDirection.RIGHT && currentState.rightBankIndex >= mapWidth - 1) {
                currentState.rightBankDirection = BankDirection.STRAIGHT;
            }
            if (currentState.rightBankDirection == BankDirection.LEFT 
                    && currentState.rightBankIndex - (currentState.leftBankIndex +1) <= MINIMUM_RIVER_WIDTH +1 
                    && currentState.leftBankDirection != BankDirection.LEFT) {
                currentState.rightBankDirection = BankDirection.STRAIGHT;
                currentState.rightBankIndex++;
            }
        }
        if (currentState.leftBankIndex < 0) {
            GD.Print("Left bank index less than zero (this should not happen) Resetting to zero");
            currentState.leftBankIndex = 0;
            currentState.leftBankDirection = BankDirection.STRAIGHT;
        }
        if (currentState.rightBankIndex >= mapWidth) {
            GD.Print("Right bank index greater than or equal to map width (this should not happen) Resetting to mapWidth - 1");
            currentState.rightBankIndex = mapWidth - 1;
            currentState.rightBankDirection = BankDirection.STRAIGHT;
        }
        return currentState;
    }
}
