using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

// Main controller responsible for handling tower placement, selection, highlighting, and UI interactions
public partial class GameController : Node
{
    // Reference to a UI icon (likely used externally or later)
    [Export] private BurgerIcon burgerIcon;

    // Enum representing which build button is currently active
    private enum Button
    {
        None,
        Silverware,
        Ice,
        Stove,
        Sink,
        Chef
    }

    private Button pressedButton;              // Tracks which button is currently selected
    private Camera3D camera;                  // Camera used for raycasting from mouse
    private Plane floorPlane;                 // Plane representing floor placement area
    private Plane counterPlane;               // Plane representing countertop placement area

    // Preloaded scenes for each tower type
    private PackedScene _silverwareTower = GD.Load<PackedScene>("res://scenes/silverware_tower.tscn");
    private PackedScene _iceTower = GD.Load<PackedScene>("res://scenes/ice_tower.tscn");
    private PackedScene _stoveTower = GD.Load<PackedScene>("res://scenes/stove_tower.tscn");
    private PackedScene _sinkTower = GD.Load<PackedScene>("res://scenes/sink_tower.tscn");
    private PackedScene _chefTower = GD.Load<PackedScene>("res://scenes/chef_tower.tscn");

    private Tower newTower = null;            // The tower currently being previewed for placement
    private Tower highlightedTower = null;    // Tower currently under mouse hover
    private Tower selectedTower = null;       // Tower currently selected by the player
    private bool placementIsValid = false;    // Whether current preview position is valid

    // Lists of placed towers for each surface type
    private List<Tower> floorTowerList = new List<Tower>();
    private List<Tower> counterTowerList = new List<Tower>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Default to no active button
        pressedButton = Button.None;

        // Get the active camera from the viewport for raycasting
        camera = GetViewport().GetCamera3D();

        // Define horizontal planes for ray intersection (floor and counter heights)
        floorPlane = new Plane(Vector3.Up, 0.5f); // Floor at Y = 0.5
        counterPlane = new Plane(Vector3.Up, 1.5f); // Counter at Y = 1.5
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Currently unused (input-driven logic is handled elsewhere)
    }

    // Returns the 3D position where the mouse ray intersects a given plane
    private Vector3? GetMouseIntersection(Plane plane)
    {
        // Get mouse position in screen space
        Vector2 mousePos = GetViewport().GetMousePosition();

        // Convert screen position into a ray in world space
        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePos);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePos);

        // Compute intersection with the given plane
        Vector3? intersection = plane.IntersectsRay(rayOrigin, rayDirection);
        return intersection;
    }

    // Handles highlighting towers when hovering over them (no build mode active)
    private void DisplayTowerHighlight()
    {
        // Check floor towers first
        Vector3? intersection = GetMouseIntersection(floorPlane);
        if (intersection != null)
        {
            Vector3 pos = intersection.Value;

            // Convert to 2D grid coordinates (XZ plane)
            Vector2 pos2D = new Vector2(pos.X, pos.Z);

            foreach (Tower tower in floorTowerList)
            {
                Vector2 towerPos2D = new Vector2(tower.Position.X, tower.Position.Z);

                // Compare floored positions to detect same grid tile
                if (towerPos2D.Floor() == pos2D.Floor() && tower != selectedTower)
                {
                    tower.Highlight();

                    // Ensure only one tower is highlighted at a time
                    if (highlightedTower != null && highlightedTower != tower)
                    {
                        highlightedTower.UnHighlight();
                    }

                    highlightedTower = tower;
                }
                else if (tower == highlightedTower)
                {
                    // Remove highlight if no longer under cursor
                    tower.UnHighlight();
                    highlightedTower = null;

                }
            }
        }

        // Check counter towers with a tolerance instead of strict grid matching
        intersection = GetMouseIntersection(counterPlane);
        if (intersection != null)
        {
            Vector3 pos = intersection.Value;
            Vector2 pos2D = new Vector2(pos.X, pos.Z);

            foreach (Tower tower in counterTowerList)
            {
                Vector2 towerPos2D = new Vector2(tower.Position.X, tower.Position.Z);
                Vector2 difference = towerPos2D - pos2D;

                // Highlight if cursor is within ~0.8 units
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

    // Selects the currently highlighted tower
    public void SelectTower()
    {
        // Hide range of previously selected tower
        if (selectedTower != null)
        {
            selectedTower.HideRange();
        }

        // Assign new selection
        selectedTower = highlightedTower;
        highlightedTower = null;

        if (selectedTower != null)
        {
            // Show range indicator for selected tower
            selectedTower.ShowRange();
            selectedTower.UnHighlight();
        }

    }

    // Handles preview placement of floor-based towers
    private void DisplayTowerOnFloor()
    {
        Vector3? intersection = GetMouseIntersection(floorPlane);

        if (intersection != null)
        {
            Vector3 towerPos = intersection.Value;
            Vector2 towerPos2D = new Vector2(towerPos.X, towerPos.Z);

            // Get floor highlight area for placement validation
            HighlightArea floor = this.GetNode<HighlightArea>("%FloorHighlight");

            if (floor.IsInside(towerPos2D))
            {
                placementIsValid = true;

                // Snap to grid center
                towerPos.X = Mathf.Floor(towerPos.X) + 0.5f;
                towerPos.Z = Mathf.Floor(towerPos.Z) + 0.5f;
                towerPos.Y = 0.5f;

                newTower.Position = towerPos;
            }
            else
            {
                // Move tower below ground if invalid
                placementIsValid = false;
                towerPos.Y = -10f;
                newTower.Position = towerPos;
            }
        }
    }

    // Handles preview placement for stove (2x2 footprint on counters)
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
                // Check all 4 possible orientations for 2x2 placement

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

    // Handles sink placement using a specialized placement system
    void DisplaySinkTower()
    {
        Vector3? intersection = GetMouseIntersection(counterPlane);

        if (intersection != null)
        {
            Vector3 towerPos = intersection.Value;

            SinkPlacementSystem placementSystem = this.GetNode<SinkPlacementSystem>("%SinkPlacementSystem");

            // Get computed valid sink position
            Vector3 sinkPos = placementSystem.getSinkPosition(towerPos);

            // If Y < 0, placement is invalid
            if (sinkPos.Y < 0f)
            {
                this.placementIsValid = false;
            }
            else
            {
                this.placementIsValid = true;
            }

            Vector2I sinkPos2D = new Vector2I((int)sinkPos.Z, (int)sinkPos.X);

            // Apply placement rules
            placementSystem.setSinkPlacementRules(newTower, sinkPos2D);

            newTower.Position = sinkPos;

            // Hide overlapping counter visuals
            this.GetNode<Kitchen>("%Kitchen").hideCounter(sinkPos2D);
        }
    }

    // Enable floor highlight visuals
    public void HighlightFloor()
    {
        this.GetNode<Node3D>("%FloorHighlight").Visible = true;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    // Enable counter highlight visuals
    public void HighlightCounters()
    {
        this.GetNode<Node3D>("%CounterHighlight").Visible = true;
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    // Enable sink-specific highlight visuals
    public void HighlightSinkArea()
    {
        this.GetNode<Node3D>("%SinkHighlightA").Visible = true;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = true;
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
    }

    // Disable all highlight visuals
    public void RemoveHighlights()
    {
        this.GetNode<Node3D>("%FloorHighlight").Visible = false;
        this.GetNode<Node3D>("%CounterHighlight").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightA").Visible = false;
        this.GetNode<Node3D>("%SinkHighlightB").Visible = false;
    }

    // Button handlers follow a consistent pattern:
    // 1. Remove existing preview tower
    // 2. Toggle selected state
    // 3. Instantiate new preview if activated

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

    // Cancels placement when interacting with top UI
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

    // Main input handler for world interactions
    public void OnViewPortGuiInput(InputEvent e)
    {
        if (e is InputEventMouseMotion)
        {
            // Update preview or highlighting depending on mode
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
            // Handle placement if valid
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

                        // Mark tile as occupied
                        this.GetNode<HighlightArea>("%FloorHighlight").SetOccupied(new Vector2(newTower.Position.X, newTower.Position.Z));
                        break;

                    case Button.Stove:
                    case Button.Sink:
                        counterTowerList.Add(newTower);

                        // Mark all 4 tiles of the 2x2 structure as occupied
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

                // Determine cost based on tower type
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

                // Deduct funds
                this.GetNode<FundsLabel>("%FundsLabel").reduceFunds(cost);

                // Reset placement state
                newTower = null;
                placementIsValid = false;
                this.RemoveHighlights();
                pressedButton = Button.None;

                // Cleanup any temporary visuals
                this.GetNode<Kitchen>("%Kitchen").cleanup();
            }
            else if (Button.None == pressedButton)
            {
                // Handle tower selection and upgrade menu
                if (highlightedTower != null)
                {
                    Control upgradeMenu = this.GetNode<Control>("%UpgradeMenu");

                    // Clear existing menu items
                    foreach (Node child in upgradeMenu.GetChildren())
                    {
                        upgradeMenu.RemoveChild(child);
                    }

                    // Populate menu from selected tower
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