using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class PathGenerator : Node
{
	List<TestRoom> Rooms=new List<TestRoom>();
	TestRoom currentRoom;
	int numberOfCreatedRoom;
	int roomToCreate;

	Vector2I nextPos;
	float timeStart;
	int attempt=0;
	RandomNumberGenerator rand=new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(GeneratePath(10));
	}

	TestRoom[] GeneratePath(int pRoomToCreate)
	{
		roomToCreate=pRoomToCreate;
		currentRoom=SaveRoom(Vector2I.Zero,TestRoom.ERoomType.Start);
		timeStart=Time.GetTicksMsec();
		while (numberOfCreatedRoom<(roomToCreate+1))
		{
			attempt++;
			if(attempt>roomToCreate*4)
			{
				Reset();
				return GeneratePath(roomToCreate);
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
				if(numberOfCreatedRoom==roomToCreate)currentRoom=SaveRoom(nextPos,TestRoom.ERoomType.End);
				else currentRoom=SaveRoom(nextPos,TestRoom.ERoomType.Regular);
				currentRoom.AvailableDoor[(lDoorSelected+2)%4]=false;
				numberOfCreatedRoom++;
			}
		}
		GD.Print("Duration ",Time.GetTicksMsec()-timeStart,"ms with ",attempt, "tries");
		for (int i = 0; i < Rooms.Count - 1; i++)
        {
			int door=(int)(((Vector2)(Rooms[i + 1].RoomPosition - Rooms[i].RoomPosition)).Angle()/(Mathf.Pi/2))-1;
			Rooms[i].FinalOpenedDoors[door]=true;
        }
		return Rooms.ToArray();
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
		GD.Print("Reset with Duration ",Time.GetTicksMsec()-timeStart,"ms with ",attempt, "tries");
		currentRoom=null;
		numberOfCreatedRoom=0;
		nextPos=Vector2I.Zero;
		timeStart=0;
		attempt=0;
		Rooms.Clear();
	}

	TestRoom SaveRoom(Vector2I pPos, TestRoom.ERoomType type)
	{
		TestRoom testRoom= new TestRoom(pPos);
		Rooms.Add(testRoom);
		return testRoom;
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
