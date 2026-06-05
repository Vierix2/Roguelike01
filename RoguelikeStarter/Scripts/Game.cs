using System.Collections.Generic;
using System.Linq;
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
    [Export] float roomInterval = 100;
    private PathGenerator pathGenerator = new PathGenerator();
    [ExportGroup("PackedScene")]
    [Export] private PackedScene[] RoomScene { get; set; }
    [Export] private PackedScene PlayerScene { get; set; }

    private DataRoom[] listOfDataRoomGenerated;
    private List<Door> currentListDoorRoom = new List<Door>();
    public bool[] AvailableDoor = new bool[4];
    private Room currentPlayerRoom;
    private List<Room> roomList = new List<Room>();
    private Room[] roomArray;
    private Node2D _projectiles;
    Vector2 roomStartPosition;
	Vector2 roomEndPosition;
    Door door;

    Player player;

    public override void _Ready()
    {
        if (RoomScene == null || PlayerScene == null)
        {
            GD.PushWarning("Game: RoomScene and PlayerScene must be assigned in the editor.");
            return;
        }

        listOfDataRoomGenerated = new DataRoom[numberOfRoomNeeded + 2];
        listOfDataRoomGenerated = pathGenerator.GeneratePath(numberOfRoomNeeded);

        CreateAllRoom();

        // After the room, so bullets draw above the floor instead of under it.
        _projectiles = new Node2D { Name = "Projectiles" };
        AddChild(_projectiles);

        player = SpawnPlayer();
        player.GlobalPosition = roomList[0].GlobalPosition;
        roomList[0].Enter(player);
    }

    public void test(Node2D body, int direction)
    {

        if (direction == 0)
        {

            GD.Print("[Game] Door North good \nStat : " + currentPlayerRoom.RoomPosition);
            GD.Print("\nNext room Stat : " + currentPlayerRoom.RoomPosition + Vector2I.Up);

            GD.Print("\nNext room Stat : " + roomArray[1].RoomPosition);


            // North
            foreach(Room room in roomArray)
            {
                if (room == currentPlayerRoom) 
                {
                    continue;
                }
                if (currentPlayerRoom.RoomPosition + Vector2I.Up == room.RoomPosition)
                {
                    GD.Print("[Game] Room trouvé : " + room + " Position : " + room.RoomPosition + " // Ancienne room : " + currentPlayerRoom + " Position : " + currentPlayerRoom.RoomPosition);
                    ChangeCurrentRoom(room);
                    room.Enter(player);
                    break;
                } 
            }
        }
        if (direction == 1)
        {

            GD.Print("[Game] Door East \nStat : " + currentPlayerRoom.RoomPosition);
            GD.Print("\nNext room Stat : " + currentPlayerRoom.RoomPosition + Vector2I.Right);

            GD.Print("\nNext room Stat : " + roomArray[1].RoomPosition);


            // East
            foreach(Room room in roomArray)
            {
                
                if (room == currentPlayerRoom)
                {
                    continue;
                } 
                if (currentPlayerRoom.RoomPosition + Vector2I.Right == room.RoomPosition)
                {
                    GD.Print("[Game] Room trouvé : " + room + " Position : " + room.RoomPosition + " // Ancienne room : " + currentPlayerRoom + " Position : " + currentPlayerRoom.RoomPosition);
                    ChangeCurrentRoom(room);
                    room.Enter(player);
                    break;
                } 
            }
        }
        if (direction == 2)
        {

            GD.Print("[Game] Door South \nStat : " + currentPlayerRoom.RoomPosition);
            GD.Print("\nNext room logical Stat : " + currentPlayerRoom.RoomPosition + Vector2I.Down);

            GD.Print("\nNext room Stat : " + roomArray[1].RoomPosition);

            // South
            foreach(Room room in roomArray)
            {
                if (room == currentPlayerRoom) 
                {
                    continue;
                }
                if (currentPlayerRoom.RoomPosition + Vector2I.Down == room.RoomPosition)
                {
                    GD.Print("[Game] Room trouvé : " + room + " Position : " + room.RoomPosition + " // Ancienne room : " + currentPlayerRoom + " Position : " + currentPlayerRoom.RoomPosition);
                    ChangeCurrentRoom(room);
                    room.Enter(player);
                    break;
                } 
            }
        }
        if (direction == 3) 
        {

            GD.Print("[Game] Door West \nStat : " + currentPlayerRoom.RoomPosition);
            GD.Print("\nNext room Stat : " + currentPlayerRoom.RoomPosition + Vector2I.Left);

            GD.Print("\nNext room Stat : " + roomArray[1].RoomPosition);


            // West
            foreach(Room room in roomArray)
            {
                if (room == currentPlayerRoom) 
                {
                    continue;
                }
                if (currentPlayerRoom.RoomPosition + Vector2I.Left == room.RoomPosition)
                {
                    GD.Print("[Game] Room trouvé : " + room + " Position : " + room.RoomPosition + " // Ancienne room : " + currentPlayerRoom + " Position : " + currentPlayerRoom.RoomPosition);
                    ChangeCurrentRoom(room);
                    room.Enter(player);
                    break;
                } 
            }
        }
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

    public void ChangeCurrentRoom(Room pNewRoom)
    {
        if (currentPlayerRoom != null) foreach(Door door in currentListDoorRoom) door.PlayerCrossed -= test;

        currentPlayerRoom = pNewRoom;
        currentListDoorRoom = currentPlayerRoom.listOfDoor;

        foreach(Door door in currentListDoorRoom)
        {
            door.PlayerCrossed += test;
        }

        GD.Print("[Game] Change main room");
    }

    public void CreateAllRoom()
    {
        int iteration = 0;
        foreach(DataRoom room in listOfDataRoomGenerated)
        {
            if (room.RoomType == DataRoom.ERoomType.Start)
            {
                Room NextRoom = SpawnRoom(room.RoomPosition, room.RoomType, iteration);
                ChangeCurrentRoom(NextRoom);
                roomList.Add(NextRoom);
            }
            else if (room.RoomType == DataRoom.ERoomType.Regular)
            {
                roomList.Add(SpawnRoom(room.RoomPosition, room.RoomType, iteration));
            }
            else if (room.RoomType == DataRoom.ERoomType.End)
            {
                roomList.Add(SpawnRoom(room.RoomPosition, room.RoomType, iteration));
            }
            iteration++;
        }

        roomArray = roomList.ToArray();

        for (int i = 0 ; i < roomArray.Length; i++)
        {
            GD.Print(roomArray[i].RoomPosition);
        }

        for (int i = 0 ; i < listOfDataRoomGenerated.Length; i++)
        {
            GD.Print(listOfDataRoomGenerated[i].RoomPosition);
        }
    }

    Room SpawnRoom(Vector2I pPos, DataRoom.ERoomType type, int roomNumber = 0)
    {
        // GD.Print("" + (int)type);
		Room room = RoomScene[(int)type].Instantiate<Room>();
		roomContainer.AddChild(room);
        room.SetDoor(listOfDataRoomGenerated[roomNumber].FinalOpenedDoors);

        room.RoomPosition = pPos;

		room.Position=(Vector2)pPos*roomInterval;
        switch (type)
        {
            case DataRoom.ERoomType.Start:
                roomStartPosition = room.Position;
                break;
            case DataRoom.ERoomType.Regular:
                room.GetChild<Label>(1).Text = roomNumber.ToString();
                break;
            case DataRoom.ERoomType.End:
                roomEndPosition = room.Position;
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
