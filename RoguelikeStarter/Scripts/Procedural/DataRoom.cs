using Godot;
using System;
using System.Collections.Generic;

public partial class DataRoom
{
	public Vector2I RoomPosition;
	public ERoomType RoomType;
	public bool[] AvailableDoor=new bool[4];
	public bool[] FinalOpenedDoors=new bool[4];
	public int LockedDoor=-1;
	public enum ERoomType
	{
		Start,Regular,End,Locked,Key
	}

	public DataRoom(Vector2I pPos,ERoomType pType=ERoomType.Regular)
	{
		RoomPosition=pPos;
		RoomType=pType;
		for (int i = 0; i < AvailableDoor.Length; i++)
		{
			AvailableDoor[i]=true;
		}
	}
	public int GetAvailableDoor()
	{
		int result=0;
		foreach(bool door in AvailableDoor)
		{
			if(door)result++;
		}
		return result;
	}
}
