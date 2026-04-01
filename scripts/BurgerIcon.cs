using Godot;
using System.Collections.Generic;

public partial class BurgerIcon : Node
{
    [Export] private SubViewport subViewport;
    [Export] private Node3D modelContainer;
    [Export] private Camera3D camera;

    public Texture2D GetTexture()
    {
        return subViewport.GetTexture();
    }

    // MAIN FUNCTION
    public void ShowModel(string ModelName)
    {
        this.GetNode<Node3D>($"%{ModelName}").Visible = true;
    }

    public void HideAll()
    {
        Node3D collection = this.GetNode<Node3D>("%ModelCollection");
        foreach (Node3D model in collection.GetChildren())
        {
            model.Visible = false;
        }
    }
   
}
