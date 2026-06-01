using Godot;
using System;
using System.Collections.Generic;

public partial class TestRoom
{
	public Vector2 RoomPosition;
	public bool[] AvailableDoor=new bool[4];

	public TestRoom(Vector2 pPos)
	{
		RoomPosition=pPos;
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
