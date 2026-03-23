using Godot;
using System;
using System.Collections.Generic;

public partial class GameController : Node3D
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
    private Camera3D camera;
    private Plane floorPlane;
    private PackedScene _newTower = GD.Load<PackedScene>("res://scenes/tower.tscn");
    private Node3D newTower = null;
    private bool placementIsValid = false;
    private List<Node3D> towerList = new List<Node3D>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        pressedButton = Button.None;
        camera = GetViewport().GetCamera3D();
        floorPlane = new Plane(Vector3.Up, 0.5f); // normal, distance from origin
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    private void displayTowerOnFloor()
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePos);
        Vector3? intersection = floorPlane.IntersectsRay(rayOrigin, rayDirection);

        if (intersection != null)
        {
            Vector3 towerPos = intersection.Value;
            Vector2 towerPos2D = new Vector2(towerPos.X, towerPos.Z);
            FloorHighlight floor = this.GetNode<FloorHighlight>("%FloorHighlight");
            if (floor.isInside(towerPos2D))
            {
                placementIsValid = true;
                towerPos.X = Mathf.Floor(towerPos.X) + 0.5f;
                towerPos.Z = Mathf.Floor(towerPos.Z) + 0.5f;
                towerPos.Y = 0.5f;
                newTower.Position = towerPos;
            }
            else
            {
                placementIsValid = false;
                towerPos.Y = -3f;
                newTower.Position = towerPos;
            }
        }
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
            newTower = (Node3D)_newTower.Instantiate();
            this.AddChild(newTower);
            this.highlightFloor();
            pressedButton = Button.Silverware;
        }
        else
        {
            if (newTower != null)
            {
                newTower.QueueFree();
                newTower = null;
            }

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
            if (newTower != null)
            {
                newTower.QueueFree();
                newTower = null;
            }
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
            if (newTower != null)
            {
                newTower.QueueFree();
                newTower = null;
            }
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
            if (newTower != null)
            {
                newTower.QueueFree();
                newTower = null;
            }
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
            if (newTower != null)
            {
                newTower.QueueFree();
                newTower = null;
            }
            this.removeHighlights();
            pressedButton = Button.None;
        }
    }


    public void onTopMenuGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (newTower != null)
            {
                newTower.QueueFree();
                newTower = null;
            }
            this.removeHighlights();
            this.pressedButton = Button.None;
        }
    }


    public void OnViewPortGuiInput(InputEvent e)
    {
        if (e is InputEventMouseMotion)
        {
            switch (pressedButton)
            {
                case Button.Silverware:
                    displayTowerOnFloor();
                    break;
            }
        }
        else if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed && placementIsValid)
        {
            towerList.Add(newTower);
            newTower = null;
            this.removeHighlights();
            pressedButton = Button.None;
        }
    }
}
