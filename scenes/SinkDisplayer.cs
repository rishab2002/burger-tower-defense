using Godot;
using System;

public partial class SinkDisplayer : Node3D
{

	private Node3D nonmetal;
	private Node3D metal;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        nonmetal = this.GetNode<Node3D>("%nonmetal");
        metal = this.GetNode<Node3D>("%metal");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void display(int rotation, bool isMetal)
	{
		this.RotationDegrees = new Vector3(0, rotation, 0);

		if (isMetal)
		{
			nonmetal.Visible = false;
			metal.Visible = true;
		}
		else
		{
			nonmetal.Visible = true;
			metal.Visible = false;
		}
    }
}
