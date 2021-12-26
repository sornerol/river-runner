using Godot;
using System.Collections;

public class River : Node {
	
	[Export]
	public NodePath riverTileMap1Path;
	[Export]
	public NodePath riverTileMap2Path;
	[Export]
	public int startingRiverWidth;

	private RiverTileMap riverTileMap1;
	private RiverTileMap riverTileMap2;

	private int tileMapWidth;
	private int tileMapHeight;

	private Queue riverTileMaps = new Queue();

	private RiverGeneratorState riverState = new RiverGeneratorState();

	public override void _Ready() {
		setRiverTileMaps();
		setTileMapDimensions();
		initializeRiverState();
		initializeRiver();
	}

	public override void _Process(float delta)
	{
		
	}

	private void setRiverTileMaps() {
		riverTileMap1 = (RiverTileMap) GetNode(riverTileMap1Path);
		riverTileMap2 = (RiverTileMap) GetNode(riverTileMap2Path);		
	}

	private void initializeRiverState() {
		riverState.riverWidth = startingRiverWidth;
		riverState.leftBankDirection = BankDirection.STRAIGHT;
		riverState.rightBankDirection = BankDirection.STRAIGHT;
		riverState.leftBankIndex = ((tileMapWidth - startingRiverWidth) / 2) - 1;
	}

	private void initializeRiver() {
		riverTileMaps.Clear();
		riverState = riverTileMap1.generateTerrain(riverState);
		riverTileMaps.Enqueue(riverTileMap1);
		riverState = riverTileMap2.generateTerrain(riverState);
		Vector2 position = riverTileMap2.Position;
		position.y = GetViewport().Size.y;
		riverTileMap2.Position = position;
		riverTileMaps.Enqueue(riverTileMap2);
	}

	private void setTileMapDimensions () {
		Vector2 viewportSize = GetViewport().Size;
		float tileSizePixels = riverTileMap1.CellSize.x * riverTileMap1.Scale.x;  //I'm assuming tiles are square

		tileMapWidth = (int) viewportSize.x / (int) tileSizePixels;
		tileMapHeight = (int) viewportSize.y / (int) tileSizePixels;
		GD.Print("Map width = " + tileMapWidth);
		GD.Print("Map height = " + tileMapHeight);
		
	}
}
