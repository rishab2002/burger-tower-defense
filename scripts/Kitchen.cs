using Godot;
using System;

public partial class Kitchen : Node3D
{
	private Node3D hiddenCounter = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void hideCounter(Vector2I vector)
	{
		if (hiddenCounter != null)
		{
			hiddenCounter.Visible = true;
		}
		hiddenCounter = this.GetNodeOrNull<Node3D>($"%counter_{vector.X},{vector.Y}");
		if (hiddenCounter != null)
		{
			hiddenCounter.Visible = false;
		}
	}

    public void cleanup()
    {
        if (hiddenCounter != null)
        {
            hiddenCounter.QueueFree();
			hiddenCounter = null;
        }
    }
}
