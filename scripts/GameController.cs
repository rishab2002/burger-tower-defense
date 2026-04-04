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

    private Vector3? GetMouseIntersection(Plane plane)
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePos);
        Vector3? intersection = plane.IntersectsRay(rayOrigin, rayDirection);
        return intersection;
    }

    private void DisplayTowerHighlight()
    {
        Vector3? intersection = GetMouseIntersection(floorPlane);
        if (intersection != null)
        {
            Vector3 pos = intersection.Value;
            Vector2 pos2D = new Vector2(pos.X, pos.Z);

            foreach (Tower tower in floorTowerList)
            {
                Vector2 towerPos2D = new Vector2(tower.Position.X, tower.Position.Z);
                if (towerPos2D.Floor() == pos2D.Floor() && tower != selectedTower)
                {
                    tower.Highlight();
                    if (highlightedTower != null && highlightedTower != tower)
                    {
                        highlightedTower.UnHighlight();
                    }
                    highlightedTower = tower;
                }
                else if (tower == highlightedTower)
                {
                    tower.UnHighlight();
                    highlightedTower = null;

                }
            }
        }

        intersection = GetMouseIntersection(counterPlane);
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
                    tower.Highlight();
                    if (highlightedTower != null && highlightedTower != tower)
                    {
                        highlightedTower.UnHighlight();
                    }
                    highlightedTower = tower;
                }
                else if (tower == highlightedTower)
                {
                    tower.UnHighlight();
                    highlightedTower = null;
                }
            }
        }
    }

    public void SelectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.HideRange();
        }

        selectedTower = highlightedTower;
        highlightedTower = null;
        if (selectedTower != null)
        {
            selectedTower.ShowRange();
            selectedTower.UnHighlight();
        }

    }


    private void DisplayTowerOnFloor()
    {
        Vector3? intersection = GetMouseIntersection(floorPlane);

        if (intersection != null)
        {
            Vector3 towerPos = intersection.Value;
            Vector2 towerPos2D = new Vector2(towerPos.X, towerPos.Z);
            HighlightArea floor = this.GetNode<HighlightArea>("%FloorHighlight");
            if (floor.IsInside(towerPos2D))
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

    private void DisplayStoveTower()
    {
        Vector3? intersection = GetMouseIntersection(counterPlane);

        if (intersection != null)
        {
            Vector3 towerPos = intersection.Value;
            Vector2 towerPos2D = new Vector2(towerPos.X, towerPos.Z);
            HighlightArea countertop = this.GetNode<HighlightArea>("%CounterHighlight");

            if (countertop.IsInside(towerPos2D))
            {
                if (countertop.IsInside(towerPos2D + new Vector2(1f, 1f)) &&
                    countertop.IsInside(towerPos2D + new Vector2(1f, 0f)) &&
                    countertop.IsInside(towerPos2D + new Vector2(0f, 1f)))
                {
                    placementIsValid = true;
                    towerPos.X = Mathf.Floor(towerPos.X) + 1f;
                    towerPos.Z = Mathf.Floor(towerPos.Z) + 1f;
                    towerPos.Y = 0.5f;
                    newTower.Position = towerPos;
                }
                else if (countertop.IsInside(towerPos2D + new Vector2(1f, -1f)) &&
                    countertop.IsInside(towerPos2D + new Vector2(1f, 0f)) &&
                    countertop.IsInside(towerPos2D + new Vector2(0f, -1f)))
                {
                    placementIsValid = true;
                    towerPos.X = Mathf.Floor(towerPos.X) + 1f;
                    towerPos.Z = Mathf.Floor(towerPos.Z);
                    towerPos.Y = 0.5f;
                    newTower.Position = towerPos;
                }
                else if (countertop.IsInside(towerPos2D + new Vector2(-1f, 1f)) &&
                    countertop.IsInside(towerPos2D + new Vector2(-1f, 0f)) &&
                    countertop.IsInside(towerPos2D + new Vector2(0f, 1f)))
                {
                    placementIsValid = true;
                    towerPos.X = Mathf.Floor(towerPos.X);
                    towerPos.Z = Mathf.Floor(towerPos.Z) + 1f;
                    towerPos.Y = 0.5f;
                    newTower.Position = towerPos;
                }
                else if (countertop.IsInside(towerPos2D + new Vector2(-1f, -1f)) &&
                    countertop.IsInside(towerPos2D + new Vector2(-1f, 0f)) &&
                    countertop.IsInside(towerPos2D + new Vector2(0f, -1f)))
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

    void DisplaySinkTower()
    {
        Vector3? intersection = GetMouseIntersection(counterPlane);

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

    public void HighlightFloor()
    {
        this.GetNode<Node3D>("%FloorHighlight").Visible = true;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    public void HighlightCounters()
    {
        this.GetNode<Node3D>("%CounterHighlight").Visible = true;
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    public void HighlightSinkArea()
    {
        this.GetNode<Node3D>("%SinkHighlightA").Visible = true;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = true;
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
    }

    public void RemoveHighlights()
    {
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    public void OnSilverwareButtonPressed()
    {
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }

        if (pressedButton != Button.Silverware)
        {
            newTower = (Tower)_silverwareTower.Instantiate();
            newTower.SetFundUpdateSingal(this.GetNode<FundsLabel>("%FundsLabel"));
            this.AddChild(newTower);
            this.HighlightFloor();
            pressedButton = Button.Silverware;
        }
        else
        {
           

            this.RemoveHighlights();
            pressedButton = Button.None;
        }

    }

    public void OnIceButtonPressed()
    {
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }

        if (pressedButton != Button.Ice)
        {
            newTower = (Tower)_iceTower.Instantiate();
            newTower.SetFundUpdateSingal(this.GetNode<FundsLabel>("%FundsLabel"));
            this.AddChild(newTower);
            this.HighlightFloor();
            pressedButton = Button.Ice;
        }
        else
        {
            
            this.RemoveHighlights();
            pressedButton = Button.None;
        }
    }

    public void OnStoveButtonPressed()
    {
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }

        if (pressedButton != Button.Stove)
        {
            newTower = (Tower)_stoveTower.Instantiate();
            newTower.SetFundUpdateSingal(this.GetNode<FundsLabel>("%FundsLabel"));
            this.AddChild(newTower);
            this.HighlightCounters();
            pressedButton = Button.Stove;
        }
        else
        {

            this.RemoveHighlights();
            pressedButton = Button.None;
        }
    }

    public void OnSinkButtonPressed()
    {
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }
        if (pressedButton != Button.Sink)
        {
            newTower = (Tower)_sinkTower.Instantiate();
            newTower.SetFundUpdateSingal(this.GetNode<FundsLabel>("%FundsLabel"));
            this.AddChild(newTower);
            this.HighlightSinkArea();
            pressedButton = Button.Sink;
        }
        else
        {
            
            this.RemoveHighlights();
            pressedButton = Button.None;
        }
    }

    public void OnChefButtonPressed()
    {
        if (newTower != null)
        {
            newTower.QueueFree();
            newTower = null;
        }
        if (pressedButton != Button.Chef)
        {
            newTower = (Tower)_chefTower.Instantiate();
            newTower.SetFundUpdateSingal(this.GetNode<FundsLabel>("%FundsLabel"));
            this.AddChild(newTower);
            this.HighlightFloor();
            pressedButton = Button.Chef;
        }
        else
        {
            
            this.RemoveHighlights();
            pressedButton = Button.None;
        }
    }


    public void OnTopMenuGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (newTower != null)
            {
                newTower.QueueFree();
                newTower = null;
            }
            this.RemoveHighlights();
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
                    DisplayTowerOnFloor();
                    break;
                case Button.Stove:
                    DisplayStoveTower();
                    break;
                case Button.Sink:
                    DisplaySinkTower();
                    break;
                case Button.None:
                    DisplayTowerHighlight();
                    break;
            }
        }
        else if (e is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            if (placementIsValid)
            {

                if (newTower != null)
                {
                    newTower.HideRange();
                }

                switch (pressedButton)
                {
                    case Button.Chef:
                    case Button.Silverware:
                    case Button.Ice:

                        floorTowerList.Add(newTower);

                        this.GetNode<HighlightArea>("%FloorHighlight").SetOccupied(new Vector2(newTower.Position.X, newTower.Position.Z));



                        break;
                    case Button.Stove:
                    case Button.Sink:

                        counterTowerList.Add(newTower);

                        HighlightArea counter1 = this.GetNode<HighlightArea>("%CounterHighlight");
                        HighlightArea counter2 = this.GetNode<HighlightArea>("%SinkHighlightA");
                        HighlightArea counter3 = this.GetNode<HighlightArea>("%SinkHighlightB");
                        counter1.SetOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z + 0.5f));
                        counter1.SetOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z + 0.5f));
                        counter1.SetOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z - 0.5f));
                        counter1.SetOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z - 0.5f));
                        counter2.SetOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z + 0.5f));
                        counter2.SetOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z + 0.5f));
                        counter2.SetOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z - 0.5f));
                        counter2.SetOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z - 0.5f));
                        counter3.SetOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z + 0.5f));
                        counter3.SetOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z + 0.5f));
                        counter3.SetOccupied(new Vector2(newTower.Position.X + 0.5f, newTower.Position.Z - 0.5f));
                        counter3.SetOccupied(new Vector2(newTower.Position.X - 0.5f, newTower.Position.Z - 0.5f));
                        break;
                }


                int cost = 0;
                switch (pressedButton)
                {
                    case Button.Chef:
                        cost = this.GetNode<PriceTag>("%ChefTowerPrice").getPrice();
                        break;
                    case Button.Silverware:
                        cost = this.GetNode<PriceTag>("%SilverwareTowerPrice").getPrice();
                        break;
                    case Button.Ice:
                        cost = this.GetNode<PriceTag>("%IceTowerPrice").getPrice();
                        break;
                    case Button.Stove:
                        cost = this.GetNode<PriceTag>("%StoveTowerPrice").getPrice();
                        break;
                    case Button.Sink:
                        cost = this.GetNode<PriceTag>("%SinkTowerPrice").getPrice();
                        break;
                }

                this.GetNode<FundsLabel>("%FundsLabel").reduceFunds(cost);
                newTower = null;
                placementIsValid = false;
                this.RemoveHighlights();
                pressedButton = Button.None;
                this.GetNode<Kitchen>("%Kitchen").cleanup();
            }
            else if (Button.None == pressedButton)
            {
                
                if (highlightedTower != null)
                {
                    Control upgradeMenu = this.GetNode<Control>("%UpgradeMenu");
                    foreach (Node child in upgradeMenu.GetChildren())
                    {
                        upgradeMenu.RemoveChild(child);
                    }
                    highlightedTower.displayMenu(upgradeMenu);
                    this.GetNode<SideMenu>("%SideMenu").slideIn();
                }
                else
                {
                    this.GetNode<SideMenu>("%SideMenu").slideOut();

                }
                SelectTower();
            }
        }
    }
}
