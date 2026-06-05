using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class PathGeneratorWithSR : GodotObject
{
	List<DataRoom> Rooms=new List<DataRoom>();
	DataRoom currentRoom;
	int numberOfCreatedRoom;
	int roomToCreate;
	Vector2I nextPos;

	int attempt=0;
	RandomNumberGenerator rand=new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	
	public DataRoom[] GeneratePath(int pRoomToCreate,int pRoomToKey,float minLockedDoorPosRatio, float maxLockedDoorPosRatio)
	{
		roomToCreate=pRoomToCreate;
		//Create Starter room
		currentRoom=SaveRoom(Vector2I.Zero,DataRoom.ERoomType.Start,Rooms);

		while (numberOfCreatedRoom<(roomToCreate+1))
		{
			attempt++;
			if(attempt>roomToCreate*4)
			{
				Reset();
				return GeneratePath(roomToCreate,pRoomToKey,minLockedDoorPosRatio,maxLockedDoorPosRatio);
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

				//Create Boss room
				if(numberOfCreatedRoom==roomToCreate)currentRoom=SaveRoom(nextPos,DataRoom.ERoomType.End,Rooms);

				else currentRoom=SaveRoom(nextPos,DataRoom.ERoomType.Regular,Rooms);
				currentRoom.AvailableDoor[(lDoorSelected+2)%4]=false;
				numberOfCreatedRoom++;
			}
		}

		int lockedRoom=Mathf.RoundToInt(Rooms.Count*rand.RandfRange(minLockedDoorPosRatio,maxLockedDoorPosRatio));
		Rooms[lockedRoom].RoomType=DataRoom.ERoomType.Locked;
		for (int i = 0; i < Rooms.Count; i++)//To change for multiple path
        {

			if (i != 0)
			{
				SetPreviousDoor(i);
			}
			if(i != Rooms.Count-1)
			{
				SetNextDoor(i);
			}
			
			GD.Print("[PathGenerator] i : " + i + " / Room count : " + Rooms.Count);
			
        }
		int startRoom=rand.RandiRange(2,lockedRoom-2);
		CreateSecondaryPath(pRoomToKey,Rooms[startRoom]);
		
		




		return Rooms.ToArray();
	}
	private void CreateSecondaryPath(int pRoomToKey,DataRoom pStartRoom)
	{	
		List<DataRoom> rooms=Rooms;
		currentRoom=pStartRoom;
		
		nextPos=Vector2I.Zero;

		attempt=0;
		while (pRoomToKey<(roomToCreate+1))
		{
			attempt++;
			if(attempt>roomToCreate*4)
			{
				Rooms=rooms;
				CreateSecondaryPath(pRoomToKey,pStartRoom);
			}

			if (CheckDeadEnd()&&currentRoom!=pStartRoom)
			{
				Rooms.RemoveAt(Rooms.Count-1);
				pRoomToKey--;
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

				//Create Boss room
				if(pRoomToKey==roomToCreate)currentRoom=SaveRoom(nextPos,DataRoom.ERoomType.Key,Rooms);

				else currentRoom=SaveRoom(nextPos,DataRoom.ERoomType.Regular,Rooms);
				currentRoom.AvailableDoor[(lDoorSelected+2)%4]=false;
				pRoomToKey++;
			}
		}


		
	}
	private void SetNextDoor(int i)
	{
		int door=(int)((((Vector2)(Rooms[i + 1].RoomPosition - Rooms[i].RoomPosition)).Angle()/(Mathf.Pi/2))+1)%4;
		GD.Print("[PathGenerator] door : " + door);
		Rooms[i].FinalOpenedDoors[door]=true;
		if (Rooms[i].RoomType == DataRoom.ERoomType.Locked)
		{
			Rooms[i].LockedDoor=door;
		}
	}
	private void SetDoor(DataRoom room1, DataRoom room2)
	{
		int door=(int)((((Vector2)(room2.RoomPosition - room1.RoomPosition)).Angle()/(Mathf.Pi/2))+1)%4;
		GD.Print("[PathGenerator] door : " + door);
		room1.FinalOpenedDoors[door]=true;
		if (room1.RoomType == DataRoom.ERoomType.Locked)
		{
			room1.LockedDoor=door;
		}
		door=(int)((((Vector2)(room1.RoomPosition - room2.RoomPosition)).Angle()/(Mathf.Pi/2))+1)%4;
		room2.FinalOpenedDoors[door]=true;
	}
	private void SetPreviousDoor(int i)
	{
		int door=(int)((((Vector2)(Rooms[i].RoomPosition - Rooms[i-1].RoomPosition)).Angle()/(Mathf.Pi/2))+3)%4;
		GD.Print("[PathGenerator] door : " + door);
		Rooms[i].FinalOpenedDoors[door]=true;
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
		currentRoom=null;
		numberOfCreatedRoom=0;
		nextPos=Vector2I.Zero;
		attempt=0;
		Rooms.Clear();
	}

	DataRoom SaveRoom(Vector2I pPos, DataRoom.ERoomType type,List<DataRoom> rooms)
	{
		DataRoom testRoom= new DataRoom(pPos, type);
		rooms.Add(testRoom);
		return testRoom;
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
