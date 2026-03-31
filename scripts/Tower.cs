using Godot;
using System;

public partial class Tower : Node3D
{
	private MeshInstance3D visibilityRadius;
	private OmniLight3D light;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		visibilityRadius = GetNode<MeshInstance3D>("%visibility");
		light = GetNode<OmniLight3D>("%Light");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void showVisibility()
	{
		visibilityRadius.Show();
	}

	public void hideVisibility()
	{
		visibilityRadius.Hide();
	}

	public void highlight()
	{
		light.Visible = true;
	}

	public void unHighlight()
	{
		light.Visible = false;
	}


}
