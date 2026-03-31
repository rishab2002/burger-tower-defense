using Godot;
using System;

public partial class SideMenu : Godot.PanelContainer
{
	// Called when the node enters the scene tree for the first time.

	private Vector2 shownPosition;
	private Vector2 hiddenPosition;
	public bool isHidden { get; private set; }
	public override void _Ready()
	{
		shownPosition = this.Position;
		hiddenPosition = this.Position - new Vector2(200, 0);
		this.Position = hiddenPosition;
		isHidden = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void slideIn()
	{
        Tween tween = CreateTween();
        tween.TweenProperty(this, "position", shownPosition, 0.4f);
		isHidden = false;
    }

	public void slideOut()
	{
        Tween tween = CreateTween();
        tween.TweenProperty(this, "position", hiddenPosition, 0.4f);
		isHidden = true;
    }
}
