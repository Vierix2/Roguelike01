using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class GraphGenerator : Node
{
	List<DataRoom> Rooms=new List<DataRoom>();
	DataRoom currentRoom;
	int numberOfCreatedRoom;
	[Export] int roomToCreate=10;
	[Export] float interval=100;
	float fullTime;

	[Export] PackedScene[] roomScene;

	enum ERoomType
	{
		Start,Regular,End
	}
	Vector2I nextPos;
	Vector2 start;
	Vector2 end;
	float timeStart;
	int attempt=0;

	RandomNumberGenerator rand=new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		//GD.Print("Start0");
		currentRoom=SaveStartRoom(Vector2I.Zero);
		timeStart=Time.GetTicksMsec();
		while (numberOfCreatedRoom<(roomToCreate+1))
		{
			attempt++;
			if(attempt>roomToCreate*4)
			{
				Reset();
				return;
			}

			if (CheckDeadEnd())
			{
				Rooms.RemoveAt(Rooms.Count-1);
				numberOfCreatedRoom--;
				currentRoom=Rooms[Rooms.Count-1];
				continue;
			}

			//GD.Print("NEW TRY, number of room created",numberOfCreatedRoom);
			int lDoorSelected=rand.RandiRange(0,3);
			//GD.Print("selected Door",lDoorSelected,currentRoom.RoomPosition+Vector2.FromAngle((lDoorSelected-1)*(Mathf.Pi/2)) );
			nextPos=(Vector2I)((currentRoom.RoomPosition+Vector2.FromAngle((lDoorSelected-1)*(Mathf.Pi/2))).Round());
			
			if (CheckPossibility(lDoorSelected))
			{
				currentRoom.AvailableDoor[lDoorSelected]=false;
				if(numberOfCreatedRoom==roomToCreate)currentRoom=SaveEndRoom(nextPos);
				else currentRoom=SaveRoom(nextPos);
				currentRoom.AvailableDoor[(lDoorSelected+2)%4]=false;
				numberOfCreatedRoom++;
			}
		}
		float finalTime=Time.GetTicksMsec()-timeStart;
		fullTime+=finalTime;
		GD.Print("Duration ",fullTime,"ms with ",attempt, "tries");
		for (int i = 0; i < Rooms.Count; i++)
		{
			//GD.Print(i,":",Rooms[i].RoomPosition);
			if (i == 0 || i == Rooms.Count - 1)
			{
				SpawnRoom(Rooms[i].RoomPosition, i==0?ERoomType.Start:ERoomType.End);
			}else SpawnRoom(Rooms[i].RoomPosition,ERoomType.Regular,i);
		}
		GetChild<Camera2D>(0).Position=end/2f;


	}
	bool CheckDeadEnd()
	{
		if (currentRoom.GetAvailableDoor() == 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	

	void Reset()
	{
		float finalTime=Time.GetTicksMsec()-timeStart;
		fullTime+=finalTime;
		GD.Print("Reset with Duration ",finalTime,"ms with ",attempt, "tries");
		currentRoom=null;
		numberOfCreatedRoom=0;
		nextPos=Vector2I.Zero;
		start=Vector2.Zero;
		end=Vector2.Zero;
		timeStart=0;
		attempt=0;
		Rooms.Clear();
		_Ready();
	}

	DataRoom SaveRoom(Vector2I pPos)
	{
		DataRoom testRoom= new DataRoom(pPos);
		Rooms.Add(testRoom);
		return testRoom;
	}
	DataRoom SaveEndRoom(Vector2I pPos)
	{
		return SaveRoom(pPos);
	}
	DataRoom SaveStartRoom(Vector2I pPos)
	{
		return SaveRoom(pPos);
	}


	Node2D SpawnRoom(Vector2I pPos, ERoomType type, int roomNumber=0)
	{
		Node2D room= roomScene[(int)type].Instantiate<Node2D>();
		AddChild(room);
		room.Position=(Vector2)pPos*interval;
		
        switch (type)
        {
            case ERoomType.Start:
				start=room.Position;
                break;
            case ERoomType.Regular:
				room.GetChild<Label>(1).Text=roomNumber.ToString();
                break;
            case ERoomType.End:
				end=room.Position;
                break;
            default:
				break;
        }
        return room;
    }

    bool CheckPossibility(int lDoorSelected)
	{
		if(!currentRoom.AvailableDoor[lDoorSelected])return false;
		bool result=true;
		foreach(DataRoom room in Rooms)
		{
			if (room.RoomPosition == nextPos)
			{
				result=false;
				currentRoom.AvailableDoor[lDoorSelected]=false;
				break;
			}
		}
		return result;
	}
}
