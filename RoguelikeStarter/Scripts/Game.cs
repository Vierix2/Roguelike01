using System.Collections.Generic;
using Godot;

/// <summary>
/// Sets up the game: spawns the room and the player, then connects them (player health to the HUD, the player's weapon to bullet spawning).
///
/// Dungeon generation and room transitions are up to you.
/// </summary>
public partial class Game : Node2D
{
    [Export] private int numberOfRoomNeeded;
    [Export] private Node2D roomContainer;
	[Export] float roomInterval=100;

    [ExportGroup("PackedScene")]
    [Export] private PackedScene[] RoomScene { get; set; }
    [Export] private PackedScene PlayerScene { get; set; }
    [Export] private PathGenerator pathGenerator;

    private DataRoom[] listOfRoomGenerated;
    private Room[] listOfRoomCreated;
    private List<Room> roomListCreated = new List<Room>();
    private Node2D _projectiles;
    Vector2 roomStartPosition;
	Vector2 roomEndPosition;

    public override void _Ready()
    {
        if (RoomScene == null || PlayerScene == null)
        {
            GD.PushWarning("Game: RoomScene and PlayerScene must be assigned in the editor.");
            return;
        }

        listOfRoomGenerated = new DataRoom[numberOfRoomNeeded + 2];
        listOfRoomGenerated = pathGenerator.GeneratePath(numberOfRoomNeeded);

        CreateAllRoom();

        // After the room, so bullets draw above the floor instead of under it.
        _projectiles = new Node2D { Name = "Projectiles" };
        AddChild(_projectiles);

        var player = SpawnPlayer();
        player.GlobalPosition = roomListCreated[0].GlobalPosition;
        roomListCreated[0].Enter(player);
    }

    private Player SpawnPlayer()
    {
        var player = PlayerScene.Instantiate<Player>();

        // Connect the signals before the player enters the tree.
        var hud = GetNodeOrNull<HUD>("HUD");
        if (hud != null)
            player.Health.HealthChanged += hud.OnPlayerHealthChanged;

        player.Weapon.ShotFired += OnShotFired;

        AddChild(player);
        player.AddToGroup("player"); // used by doors to detect the player
        return player;
    }

    public void CreateAllRoom()
    {
        int iteration = 0;
        foreach(DataRoom room in listOfRoomGenerated)
        {
            if (room.RoomType == DataRoom.ERoomType.Start)
            {
                roomListCreated.Add(SpawnRoom(room.RoomPosition, room.RoomType, iteration));
            }
            else if (room.RoomType == DataRoom.ERoomType.Regular)
            {
                roomListCreated.Add(SpawnRoom(room.RoomPosition, room.RoomType, iteration));
            }
            else if (room.RoomType == DataRoom.ERoomType.End)
            {
                roomListCreated.Add(SpawnRoom(room.RoomPosition, room.RoomType, iteration));
            }
            iteration++;
        }
    }

	Room SpawnRoom(Vector2I pPos, DataRoom.ERoomType type, int roomNumber=0)
	{
        // GD.Print("" + (int)type);
		Room room= RoomScene[(int)type].Instantiate<Room>();
		roomContainer.AddChild(room);
        room.SetDoor(listOfRoomGenerated[roomNumber].FinalOpenedDoors);
		room.Position=(Vector2)pPos*roomInterval;
        switch (type)
        {
            case DataRoom.ERoomType.Start:
				roomStartPosition=room.Position;
                break;
            case DataRoom.ERoomType.Regular:
				room.GetChild<Label>(1).Text=roomNumber.ToString();
                break;
            case DataRoom.ERoomType.End:
				roomEndPosition=room.Position;
                break;
            default:
				break;
        }
        return room;
    }

    private void OnShotFired(Bullet bullet, Vector2 globalPosition)
    {
        _projectiles.AddChild(bullet);
        bullet.GlobalPosition = globalPosition;
    }
}
