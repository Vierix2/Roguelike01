using Godot;
using System;
using System.Collections.Generic;

public partial class TestRoom
{
	public Vector2 RoomPosition;
	bool[] AvailableDoor=new bool[4];

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
