using Godot;
using System;

public partial class Kitchen : Node3D
{
	private enum Button
	{	
		None,
		Silverware,
		Ice,
		Stove,
		Sink,
		Chef
	}

	private Button pressedButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pressedButton = Button.None;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void highlightFloor()
	{
		this.GetNode<Node3D>("%FloorHighlight").Visible = true;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
    }

	public void highlightCounters()
	{
		this.GetNode<Node3D>("%CounterHighlight").Visible = true;
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
    }


	public void removeHighlights()
	{
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
    }

	public void onSilverwareButtonPressed()
	{
		if (pressedButton != Button.Silverware)
		{
            this.highlightFloor();
			pressedButton = Button.Silverware;
        }
		else
		{
			this.removeHighlights();
			pressedButton = Button.None;
		}
		
	}

	public void onIceButtonPressed()
	{
        if (pressedButton != Button.Ice)
        {
            this.highlightFloor();
            pressedButton = Button.Ice;
        }
        else
        {
            this.removeHighlights();
            pressedButton = Button.None;
        }
    }

	public void onStoveButtonPressed()
	{
        if (pressedButton != Button.Stove)
        {
            this.highlightCounters();
            pressedButton = Button.Stove;
        }
        else
        {
            this.removeHighlights();
            pressedButton = Button.None;
        }
    }

	public void onSinkButtonPressed()
	{
        if (pressedButton != Button.Sink)
        {
            this.highlightCounters();
            pressedButton = Button.Sink;
        }
        else
        {
            this.removeHighlights();
            pressedButton = Button.None;
        }
    }

	public void onChefButtonPressed()
	{
        if (pressedButton != Button.Chef)
        {
            this.highlightFloor();
            pressedButton = Button.Chef;
        }
        else
        {
            this.removeHighlights();
            pressedButton = Button.None;
        }
    }


    public void onTopMenuGuiInput(InputEvent e)
	{
        if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			this.removeHighlights();
			this.pressedButton = Button.None;
		}
    }
}
