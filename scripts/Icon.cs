using Godot;
using System;

public partial class Icon : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
       
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized)
        {
            PivotOffset = new Vector2(0, Size.Y / 2f);
        }
    }

    public void Grow()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.2f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.Out);
    }

    public void Shrink()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.2f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.Out);
    }

    public void OnMouseEntered()
    {
        Grow();
    }

    public void OnMouseExited()
    {
        Shrink();
    }
}
