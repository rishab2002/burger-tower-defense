using Godot;
using System.Collections.Generic;

public partial class Viewport : Control
{
    [Export] private BurgerIcon burgerIcon;
    [Export] private TextureRect textureRect;

    public override void _Ready()
    {

        // This is the connection
        textureRect.Texture = burgerIcon.GetTexture();
    }
}