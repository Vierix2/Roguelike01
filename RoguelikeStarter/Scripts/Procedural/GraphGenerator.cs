using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class GraphGenerator : Node
{
	List<TestRoom> Rooms=new List<TestRoom>();
	TestRoom currentRoom;
	int numberOfCreatedRoom;
	[Export] int roomToCreate=10;
	Vector2I nextPos;
	RandomNumberGenerator rand=new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Start0");
		currentRoom=SaveStartRoom(Vector2I.Zero);
		while (numberOfCreatedRoom<(roomToCreate+1))
		{
			if (CheckDeadEnd())
			{
				Rooms.RemoveAt(Rooms.Count-1);
				currentRoom=Rooms[Rooms.Count-1];
				continue;
			}
			
			GD.Print("NEW TRY, number of room created",numberOfCreatedRoom);
			int lDoorSelected=rand.RandiRange(0,3);
			GD.Print("selected Door",lDoorSelected,currentRoom.RoomPosition+Vector2.FromAngle((lDoorSelected-1)*(Mathf.Pi/2)) );
			nextPos=(Vector2I)((currentRoom.RoomPosition+Vector2.FromAngle((lDoorSelected-1)*(Mathf.Pi/2))).Round());
			
			if (CheckPossibility(lDoorSelected))
			{
				currentRoom.AvailableDoor[lDoorSelected]=false;
				if(numberOfCreatedRoom==roomToCreate)currentRoom=SaveEndRoom(nextPos);
				else currentRoom=SaveRoom(nextPos);
				currentRoom.AvailableDoor[lDoorSelected]=false;
				numberOfCreatedRoom++;
			}
		}
		for (int i = 0; i < Rooms.Count; i++)
		{
			GD.Print(i,":",Rooms[i].RoomPosition);
		}


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
		
	}

	TestRoom SaveRoom(Vector2I pPos)
	{
		TestRoom testRoom= new TestRoom(pPos);
		Rooms.Add(testRoom);
		return testRoom;
	}
	TestRoom SaveEndRoom(Vector2I pPos)
	{
		return SaveRoom(pPos);
	}
	TestRoom SaveStartRoom(Vector2I pPos)
	{
		return SaveRoom(pPos);
	}
	bool CheckPossibility(int lDoorSelected)
	{
		if(!currentRoom.AvailableDoor[lDoorSelected])return false;
		bool result=true;
		foreach(TestRoom room in Rooms)
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
