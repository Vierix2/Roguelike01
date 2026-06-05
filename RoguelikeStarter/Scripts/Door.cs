using Godot;

/// <summary>
/// A gap in the wall.
/// Raises PlayerCrossed, with its direction, when the player walks through.
/// Reacting to it (changing room) is up to you.
/// </summary>
public partial class Door : Area2D
{
    public enum Side { North, South, East, West }

    [Export] public Side Direction { get; private set; } = Side.North;

    [Signal] public delegate void PlayerCrossedEventHandler(Node2D player, int direction);

    public bool IsOpen = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public void OpenDoor()
    {
        IsOpen = true;
    }

    public void Init(Side doorDirection)
    {
        Direction = doorDirection;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            GD.Print("joueur deteker");
            EmitSignal(SignalName.PlayerCrossed, body, (int)Direction);
        }
    }
}
