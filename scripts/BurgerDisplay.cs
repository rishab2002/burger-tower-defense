using Godot;
using System;

public partial class BurgerDisplay : PanelContainer
{
    [Export] private SubViewport subViewport;
    [Export] private TextureRect textureRect;

    public override void _Ready()
    {
        textureRect.Texture = subViewport.GetTexture();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}

}
