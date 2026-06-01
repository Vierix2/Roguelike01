using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class GraphGenerator : Node
{
	List<TestRoom> Rooms=new List<TestRoom>();
	TestRoom currentRoom;
	int numberOfCreatedRoom;
	[Export] int roomToCreate;
	Vector2 nextPos;
	RandomNumberGenerator rand=new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentRoom=SaveStartRoom(Vector2.Zero);
		while (numberOfCreatedRoom<(roomToCreate+1))
		{
			int lDoorSelected=rand.RandiRange(0,4);
			nextPos=currentRoom.RoomPosition+Vector2.FromAngle((lDoorSelected-1)*(Mathf.Pi/2));//add memory check
			if (CheckPossibility(lDoorSelected))
			{
				if(numberOfCreatedRoom==roomToCreate)currentRoom=SaveEndRoom(nextPos);
				else currentRoom=SaveRoom(nextPos);
				numberOfCreatedRoom++;
			}
		}
		for (int i = 0; i < Rooms.Count; i++)
		{
			GD.Print(Rooms[i].RoomPosition);
		}


	}

	

	void Reset()
	{
		
	}

	TestRoom SaveRoom(Vector2 pPos)
	{
		return new TestRoom(pPos);
	}
	TestRoom SaveEndRoom(Vector2 pPos)
	{
		return SaveRoom(pPos);
	}
	TestRoom SaveStartRoom(Vector2 pPos)
	{
		return SaveRoom(pPos);
	}
	bool CheckPossibility(int lDoorSelected)
	{
		if(!currentRoom.AvailableDoor[lDoorSelected])return false;
		bool result=false;
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
