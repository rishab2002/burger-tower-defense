using Godot;
using System;
using static GlobalEnums;

public partial class ColorMenu : PanelContainer
{
	[Signal]
	public delegate void ColorPressedEventHandler(int color);


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

    public void OnRedGuiInput(InputEvent e)
	{
        if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            EmitSignal(SignalName.ColorPressed, (int)EnemyColor.Red);
        }
    }
    public void OnGreenGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            EmitSignal(SignalName.ColorPressed, (int)EnemyColor.Green);
        }
    }
    public void OnYellowGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            EmitSignal(SignalName.ColorPressed, (int)EnemyColor.Yellow);
        }
    }
    public void OnBrownGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            EmitSignal(SignalName.ColorPressed, (int)EnemyColor.Brown);
        }
    }
    public void OnTanGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            EmitSignal(SignalName.ColorPressed, (int)EnemyColor.Tan);
        }
    }
}
