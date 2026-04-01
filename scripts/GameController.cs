using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class GameController : Node
{
    [Export] private BurgerIcon burgerIcon;

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
    private Plane counterPlane;
    private PackedScene _silverwareTower = GD.Load<PackedScene>("res://scenes/silverware_tower.tscn");
    private PackedScene _iceTower = GD.Load<PackedScene>("res://scenes/ice_tower.tscn");
    private PackedScene _stoveTower = GD.Load<PackedScene>("res://scenes/stove_tower.tscn");
    private PackedScene _sinkTower = GD.Load<PackedScene>("res://scenes/sink_tower.tscn");
    private PackedScene _chefTower = GD.Load<PackedScene>("res://scenes/chef_tower.tscn");
    private Tower newTower = null;
    private Tower highlightedTower = null;
    private Tower selectedTower = null;
    private bool placementIsValid = false;
    private List<Tower> floorTowerList = new List<Tower>();
    private List<Tower> counterTowerList = new List<Tower>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        pressedButton = Button.None;
        camera = GetViewport().GetCamera3D();
        floorPlane = new Plane(Vector3.Up, 0.5f); // normal, distance from origin
        counterPlane = new Plane(Vector3.Up, 1.5f); // normal, distance from origin
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}

    private Vector3? getMouseIntersection(Plane plane)
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePos);
        Vector3? intersection = plane.IntersectsRay(rayOrigin, rayDirection);
        return intersection;
    }

    private void displayTowerHighlight()
    {
        Vector3? intersection = getMouseIntersection(floorPlane);
        if (intersection != null)
        {
            Vector3 pos = intersection.Value;
            Vector2 pos2D = new Vector2(pos.X, pos.Z);

            foreach (Tower tower in floorTowerList)
            {
                Vector2 towerPos2D = new Vector2(tower.Position.X, tower.Position.Z);
                if (towerPos2D.Floor() == pos2D.Floor() && tower != selectedTower)
                {
                    tower.highlight();
                    if (highlightedTower != null && highlightedTower != tower)
                    {
                        highlightedTower.unHighlight();
                    }
                    highlightedTower = tower;
                }
                else if (tower == highlightedTower)
                {
                    tower.unHighlight();
                    highlightedTower = null;

                }
            }
        }

        intersection = getMouseIntersection(counterPlane);
        if (intersection != null)
        {
            Vector3 pos = intersection.Value;
            Vector2 pos2D = new Vector2(pos.X, pos.Z);

            foreach (Tower tower in counterTowerList)
            {
                Vector2 towerPos2D = new Vector2(tower.Position.X, tower.Position.Z);
                Vector2 difference = towerPos2D - pos2D;
                if (Mathf.Abs(difference.X) <= 0.8 && Mathf.Abs(difference.Y) <= 0.8 && tower != selectedTower)
                {
                    tower.highlight();
                    if (highlightedTower != null && highlightedTower != tower)
                    {
                        highlightedTower.unHighlight();
                    }
                    highlightedTower = tower;
                }
                else if (tower == highlightedTower)
                {
                    tower.unHighlight();
                    highlightedTower = null;
                }
            }
        }
    }

    public void selectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.hideVisibility();
        }

        selectedTower = highlightedTower;
        highlightedTower = null;
        if (selectedTower != null)
        {
            selectedTower.showVisibility();
            selectedTower.unHighlight();
        }

    }


    private void displayTowerOnFloor()
    {
        Vector3? intersection = getMouseIntersection(floorPlane);

        if (intersection != null)
        {
            Vector3 towerPos = intersection.Value;
            Vector2 towerPos2D = new Vector2(towerPos.X, towerPos.Z);
            HighlightArea floor = this.GetNode<HighlightArea>("%FloorHighlight");
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
                towerPos.Y = -10f;
                newTower.Position = towerPos;
            }
        }
    }

    private void displayStoveTower()
    {
        Vector3? intersection = getMouseIntersection(counterPlane);

        if (intersection != null)
        {
            Vector3 towerPos = intersection.Value;
            Vector2 towerPos2D = new Vector2(towerPos.X, towerPos.Z);
            HighlightArea countertop = this.GetNode<HighlightArea>("%CounterHighlight");

            if (countertop.isInside(towerPos2D))
            {
                if (countertop.isInside(towerPos2D + new Vector2(1f, 1f)) &&
                    countertop.isInside(towerPos2D + new Vector2(1f, 0f)) &&
                    countertop.isInside(towerPos2D + new Vector2(0f, 1f)))
                {
                    placementIsValid = true;
                    towerPos.X = Mathf.Floor(towerPos.X) + 1f;
                    towerPos.Z = Mathf.Floor(towerPos.Z) + 1f;
                    towerPos.Y = 0.5f;
                    newTower.Position = towerPos;
                }
                else if (countertop.isInside(towerPos2D + new Vector2(1f, -1f)) &&
                    countertop.isInside(towerPos2D + new Vector2(1f, 0f)) &&
                    countertop.isInside(towerPos2D + new Vector2(0f, -1f)))
                {
                    placementIsValid = true;
                    towerPos.X = Mathf.Floor(towerPos.X) + 1f;
                    towerPos.Z = Mathf.Floor(towerPos.Z);
                    towerPos.Y = 0.5f;
                    newTower.Position = towerPos;
                }
                else if (countertop.isInside(towerPos2D + new Vector2(-1f, 1f)) &&
                    countertop.isInside(towerPos2D + new Vector2(-1f, 0f)) &&
                    countertop.isInside(towerPos2D + new Vector2(0f, 1f)))
                {
                    placementIsValid = true;
                    towerPos.X = Mathf.Floor(towerPos.X);
                    towerPos.Z = Mathf.Floor(towerPos.Z) + 1f;
                    towerPos.Y = 0.5f;
                    newTower.Position = towerPos;
                }
                else if (countertop.isInside(towerPos2D + new Vector2(-1f, -1f)) &&
                    countertop.isInside(towerPos2D + new Vector2(-1f, 0f)) &&
                    countertop.isInside(towerPos2D + new Vector2(0f, -1f)))
                {
                    placementIsValid = true;
                    towerPos.X = Mathf.Floor(towerPos.X);
                    towerPos.Z = Mathf.Floor(towerPos.Z);
                    towerPos.Y = 0.5f;
                    newTower.Position = towerPos;
                }

            }
            else
            {
                placementIsValid = false;
                towerPos.Y = -10f;
                newTower.Position = towerPos;
            }
        }
    }

    void displaySinkTower()
    {
        Vector3? intersection = getMouseIntersection(counterPlane);

        if (intersection != null)
        {
            Vector3 towerPos = intersection.Value;
            SinkPlacementSystem placementSystem = this.GetNode<SinkPlacementSystem>("%SinkPlacementSystem");
            Vector3 sinkPos = placementSystem.getSinkPosition(towerPos);
            if (sinkPos.Y < 0f)
            {
                this.placementIsValid = false;
                
            }
            else
            {
                this.placementIsValid = true;
                
            }

            Vector2I sinkPos2D = new Vector2I((int)sinkPos.Z, (int)sinkPos.X);
            placementSystem.setSinkPlacementRules(newTower, sinkPos2D);
            newTower.Position = sinkPos;
            this.GetNode<Kitchen>("%Kitchen").hideCounter(sinkPos2D);
        }
    }

    public void highlightFloor()
    {
        this.GetNode<Node3D>("%FloorHighlight").Visible = true;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    public void highlightCounters()
    {
        this.GetNode<Node3D>("%CounterHighlight").Visible = true;
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    public void highlightSinkArea()
    {
        this.GetNode<Node3D>("%SinkHighlightA").Visible = true;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = true;
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
    }

    public void removeHighlights()
    {
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    public void onSilverwareButtonPressed()
    {
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }

        if (pressedButton != Button.Silverware)
        {
            newTower = (Tower)_silverwareTower.Instantiate();
            this.AddChild(newTower);
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
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }

        if (pressedButton != Button.Ice)
        {
            newTower = (Tower)_iceTower.Instantiate();
            this.AddChild(newTower);
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
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }

        if (pressedButton != Button.Stove)
        {
            newTower = (Tower)_stoveTower.Instantiate();
            this.AddChild(newTower);
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
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }
        if (pressedButton != Button.Sink)
        {
            newTower = (Tower)_sinkTower.Instantiate();
            this.AddChild(newTower);
            this.highlightSinkArea();
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
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }
        if (pressedButton != Button.Chef)
        {
            newTower = (Tower)_chefTower.Instantiate();
            this.AddChild(newTower);
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
                case Button.Chef:
                case Button.Silverware:
                case Button.Ice:
                    displayTowerOnFloor();
                    break;
                case Button.Stove:
                    displayStoveTower();
                    break;
                case Button.Sink:
                    displaySinkTower();
                    break;
                case Button.None:
                    displayTowerHighlight();
                    break;
            }
        }
        else if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (placementIsValid)
            {

                if (newTower != null)
                {
                    newTower.hideVisibility();
                }

                switch (pressedButton)
                {
                    case Button.Chef:
                    case Button.Silverware:
                    case Button.Ice:

                        floorTowerList.Add(newTower);

                        this.GetNode<HighlightArea>("%FloorHighlight").setOccupied(new Vector2(newTower.Position.X, newTower.Position.Z));



                        break;
                    case Button.Stove:
                    case Button.Sink:

                        counterTowerList.Add(newTower);

                        HighlightArea counter1 = this.GetNode<HighlightArea>("%CounterHighlight");
                        HighlightArea counter2 = this.GetNode<HighlightArea>("%SinkHighlightA");
                        HighlightArea counter3 = this.GetNode<HighlightArea>("%SinkHighlightB");
                        counter1.setOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z + 0.5f));
                        counter1.setOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z + 0.5f));
                        counter1.setOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z - 0.5f));
                        counter1.setOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z - 0.5f));
                        counter2.setOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z + 0.5f));
                        counter2.setOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z + 0.5f));
                        counter2.setOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z - 0.5f));
                        counter2.setOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z - 0.5f));
                        counter3.setOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z + 0.5f));
                        counter3.setOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z + 0.5f));
                        counter3.setOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z - 0.5f));
                        counter3.setOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z - 0.5f));
                        break;
                }

                newTower = null;
                placementIsValid = false;
                this.removeHighlights();
                pressedButton = Button.None;
                this.GetNode<Kitchen>("%Kitchen").cleanup();
            }
            else if (Button.None == pressedButton)
            {
                if (highlightedTower != null)
                {
                    this.GetNode<SideMenu>("%SideMenu").slideIn();
                }
                else
                {
                    this.GetNode<SideMenu>("%SideMenu").slideOut();
                }
                selectTower();
            }
        }
    }
}
