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
    [Export] private PackedScene RoomStartScene { get; set; }
    [Export] private PackedScene RoomEndScene { get; set; }
    [Export] private PackedScene PlayerScene { get; set; }
    [Export] private PathGenerator pathGenerator;

    private Room[] listOfRoomGenerated;
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

        listOfRoomGenerated = new Room[numberOfRoomNeeded + 2];

        GD.Print("[Game] listOfRoomGenerated : " + listOfRoomGenerated.Length);

        listOfRoomGenerated = pathGenerator.GeneratePath(numberOfRoomNeeded);

        GD.Print("[Game] listOfRoomGenerated : " + listOfRoomGenerated.Length);

        var room = RoomStartScene.Instantiate<Room>();
        AddChild(room);

        // After the room, so bullets draw above the floor instead of under it.
        _projectiles = new Node2D { Name = "Projectiles" };
        AddChild(_projectiles);

        var player = SpawnPlayer();
        player.GlobalPosition = room.GlobalPosition;
        room.Enter(player);
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
        foreach(Room room in listOfRoomGenerated)
        {
			//GD.Print(i,":",Rooms[i].RoomPosition);
			// if (i == 0 || i == Rooms.Count - 1)
			// {
			// 	SpawnRoom(Rooms[i].RoomPosition, i==0?ERoomType.Start:ERoomType.End);
			// }else SpawnRoom(Rooms[i].RoomPosition,ERoomType.Regular,i);
        
            if (room.RoomType == ERoomType.Start)
            {
                roomListCreated.Add(SpawnRoom(room.RoomPosition, room.RoomType, iteration) as Room);
            }
            else if (room.RoomType == ERoomType.Regular)
            {
                roomListCreated.Add(SpawnRoom(room.RoomPosition, room.RoomType, iteration) as Room);
            }
            else if (room.RoomType == ERoomType.End)
            {
                roomListCreated.Add(SpawnRoom(room.RoomPosition, room.RoomType, iteration) as Room);
            }

            iteration++;
        }
    }

	Node2D SpawnRoom(Vector2I pPos, ERoomType type, int roomNumber=0)
	{
		Node2D room= RoomScene[(int)type].Instantiate<Node2D>();
		roomContainer.AddChild(room);
		room.Position=(Vector2)pPos*roomInterval;
        switch (type)
        {
            case ERoomType.Start:
				roomStartPosition=room.Position;
                break;
            case ERoomType.Regular:
				room.GetChild<Label>(1).Text=roomNumber.ToString();
                break;
            case ERoomType.End:
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
