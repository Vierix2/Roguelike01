using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class GraphGenerator : Node
{
	DataRoom[] Rooms;
	DataRoom currentRoom;
	int numberOfCreatedRoom;
	[Export] int roomToCreate=10;
	[Export] float interval=100;
	float fullTime;

	[Export] PackedScene[] roomScene;


	Vector2I nextPos;
	Vector2 start;
	Vector2 end;
	float timeStart;
	int attempt=0;
	PathGeneratorWithSR pathGenerator=new PathGeneratorWithSR();

	RandomNumberGenerator rand=new RandomNumberGenerator();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		
		Rooms=pathGenerator.GeneratePath(roomToCreate,Mathf.CeilToInt(roomToCreate*0.4f),0.25f,0.75f);
		for (int i = 0; i < Rooms.Length; i++)
		{
			//GD.Print(i,":",Rooms[i].RoomPosition);
			SpawnRoom(Rooms[i].RoomPosition, Rooms[i].RoomType,i);
			
		}
		for (int i = 0; i < Rooms.Length; i++)
		{
			for(int j=0;j<Rooms[i].FinalOpenedDoors.Length;j++)
			{
				if (Rooms[i].FinalOpenedDoors[j])
				{
					SpawnLine(Rooms[i].RoomPosition,(Vector2I)((Rooms[i].RoomPosition+Vector2.FromAngle((j-1)*(Mathf.Pi/2))).Round()));
				}
			}
		}
		GetChild<Camera2D>(0).Position=end/2f;


	}

	private void SpawnLine(Vector2I pPos1,Vector2I pPos2)
	{
		Line2D line=new Line2D();
		line.Width=2;
		line.AddPoint((Vector2)pPos1*interval);
		line.AddPoint((Vector2)pPos2*interval);
		line.ZIndex=-1;
		line.DefaultColor=Colors.Black;
		AddChild(line);
	}
	

	Node2D SpawnRoom(Vector2I pPos, DataRoom.ERoomType type, int roomNumber=0)
	{
		Node2D room= roomScene[(int)type].Instantiate<Node2D>();
		AddChild(room);
		room.Position=(Vector2)pPos*interval;
		
        switch (type)
        {
            case DataRoom.ERoomType.Start:
				start=room.Position;
                break;
            case DataRoom.ERoomType.Regular:
				room.GetChild<Label>(1).Text=roomNumber.ToString();
                break;
            case DataRoom.ERoomType.End:
				end=room.Position;
                break;
            default:
				break;
        }
        return room;
    }

   
}
