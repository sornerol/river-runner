using Godot;
using System.Collections;

public class River : Node {
	
	[Export]
	public NodePath riverTileMap1Path;
	[Export]
	public NodePath riverTileMap2Path;

	[Export]
	public float minSpeed;

	[Export]
	public float defaultSpeed;

	[Export]
	public float maxSpeed;

	[Export]
	public float speedSensitivity;

	[Export]
	public PackedScene[] enemyScenes;

	private float currentSpeed;

	private RiverTileMap riverTileMap1;
	private RiverTileMap riverTileMap2;

	private int tileMapWidth;
	private int tileMapHeight;

	private bool isMoving;

	private Queue riverTileMaps = new Queue();

	private RiverGeneratorState riverState = new RiverGeneratorState();

	public override void _Ready() {
        GD.Randomize();
		setRiverTileMaps();
		setTileMapDimensions();
		initializeRiverState();
		initializeRiver();
		isMoving = true;
		currentSpeed = defaultSpeed / 2;
	}

	public override void _PhysicsProcess(float delta) {
		if(isMoving) {
			adjustSpeed(delta);
			foreach(RiverTileMap riverTileMap in riverTileMaps) {
				Vector2 pos = riverTileMap.Position;
				pos.y += currentSpeed * delta;
				riverTileMap.Position = pos;
			}
			RiverTileMap lowerTileMap = (RiverTileMap) riverTileMaps.Peek();
			if (GetViewport().Size.y - lowerTileMap.Position.y < 0) {
				riverTileMaps.Dequeue();
				Vector2 pos = lowerTileMap.Position;
				pos.y -= GetViewport().Size.y * 2;
				lowerTileMap.Position = pos;
				riverState = lowerTileMap.generateTerrain(riverState);
				riverTileMaps.Enqueue(lowerTileMap);
			}
		}		
	}

	private void setRiverTileMaps() {
		riverTileMap1 = (RiverTileMap) GetNode(riverTileMap1Path);
		riverTileMap2 = (RiverTileMap) GetNode(riverTileMap2Path);		
	}

	private void initializeRiverState() {
		riverState.leftBankIndex = 2;
		riverState.rightBankIndex = tileMapWidth - 3;
		riverState.leftBankDirection = BankDirection.STRAIGHT;
		riverState.rightBankDirection = BankDirection.STRAIGHT;
		riverState.linesGenerated = 0;
	}

	private void initializeRiver() {
		riverTileMaps.Clear();

		riverTileMap1.mapHeight = tileMapHeight;
		riverTileMap1.mapWidth = tileMapWidth;
		riverTileMap2.mapHeight = tileMapHeight;
		riverTileMap2.mapWidth = tileMapWidth;

		riverState = riverTileMap1.generateTerrain(riverState);
		riverTileMaps.Enqueue(riverTileMap1);
		riverState = riverTileMap2.generateTerrain(riverState);
		Vector2 position = riverTileMap2.Position;
		position.y = -GetViewport().Size.y;
		riverTileMap2.Position = position;
		riverTileMaps.Enqueue(riverTileMap2);
	}

	private void setTileMapDimensions () {
		Vector2 viewportSize = GetViewport().Size;
		float tileSizePixels = riverTileMap1.CellSize.x * riverTileMap1.Scale.x;  //I'm assuming tiles are square

		tileMapWidth = (int) viewportSize.x / (int) tileSizePixels;
		tileMapHeight = (int) viewportSize.y / (int) tileSizePixels;		
	}

	private void adjustSpeed(float delta) {
		if (Input.IsActionPressed("ui_up")) {
			speedUp(delta);
		} else if (Input.IsActionPressed("ui_down")) {
			slowDown(delta);
		} else {
			returnToDefaultSpeed(delta);
		}
	}

	private void speedUp(float delta) {
		currentSpeed += speedSensitivity * delta;
		if (currentSpeed > maxSpeed) {
			currentSpeed = maxSpeed;
		}
	}

	private void slowDown(float delta) {
		currentSpeed -= speedSensitivity * delta;
		if (currentSpeed < minSpeed) {
			currentSpeed = minSpeed;
		}
	}

	private void returnToDefaultSpeed(float delta) {
		if (currentSpeed > defaultSpeed) {
			currentSpeed -= speedSensitivity * delta;
			if (currentSpeed < defaultSpeed) {
				currentSpeed = defaultSpeed;
			}
		} else if (currentSpeed < defaultSpeed) {
			currentSpeed += speedSensitivity * delta;
			if (currentSpeed > defaultSpeed) {
				currentSpeed = defaultSpeed;
			}
		}
	}

	public void onPlaneCrashed() {
		isMoving = false;
	}
}
