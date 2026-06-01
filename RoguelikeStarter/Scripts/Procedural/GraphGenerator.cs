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
	RandomNumberGenerator rand=new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentRoom=SaveStartRoom(Vector2.Zero);
		while (numberOfCreatedRoom<roomToCreate)
		{
			int lDoorSelected=rand.RandiRange(0,4);
			Vector2 nextPos=currentRoom.RoomPosition+Vector2.FromAngle((lDoorSelected-1)*(Mathf.Pi/2));//add memory check
			if (CheckPossibility(nextPos))
			{
				currentRoom=SaveRoom(nextPos);
				numberOfCreatedRoom++;
			}
		}
	}

	

	void Reset()
	{
		
	}

	TestRoom SaveRoom(Vector2 pPos)
	{
		return null;
	}
	TestRoom SaveEndRoom(Vector2 pPos)
	{
		return null;
	}
	TestRoom SaveStartRoom(Vector2 pPos)
	{
		return null;
	}
	bool CheckPossibility(Vector2 pPos)
	{
		
		bool result=false;
		foreach(TestRoom room in Rooms)
		{
			if (room.RoomPosition == pPos)
			{
				result=true;
				break;
			}
		}
		return result;
	}
}
