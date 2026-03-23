using Godot;
using System;
using System.Collections.Generic;







public partial class FloorHighlight : Node3D
{

    private struct Rectangle
    {
        public Vector2 bottomLeft;
        public Vector2 topRight;

        public Rectangle(Vector2 bottomLeft, Vector2 topRight)
        {
            this.topRight = topRight;
            this.bottomLeft = bottomLeft;
        }

        public bool isInside(Vector2 point)
        {
            if (point.X >= bottomLeft.X && point.Y >= bottomLeft.Y &&
                point.X <= topRight.X && point.Y <= topRight.Y)
            {
                return true;
            }
            return false;
        }
    }

    private List<Rectangle> rectangles;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        setRectangles();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}


	public void setRectangles()
	{
        rectangles = new List<Rectangle>();
        
        
        foreach (Node node in GetChildren())
        {
            if (node is MeshInstance3D mesh && mesh.Mesh is PlaneMesh)
            {
                Aabb aabb = mesh.GetAabb();

                Vector3 min = aabb.Position;
                Vector3 max = aabb.Position + aabb.Size;

                

                Vector2 min2D = new Vector2(min.X, min.Z);
                Vector2 max2D = new Vector2(max.X, max.Z);
                min2D.X += mesh.Position.X;
                min2D.Y += mesh.Position.Z;
                max2D.X += mesh.Position.X;
                max2D.Y += mesh.Position.Z;

                rectangles.Add(new Rectangle(min2D, max2D));
            }
        }
        
    }

    public bool isInside(Vector2 pos)
    
    {
        foreach (Rectangle rectangle in rectangles)
        {
            if (rectangle.isInside(pos))
            {
                GD.Print("inside");
                return true;
            }
        }
        GD.Print("not");
        return false;
    }
}
