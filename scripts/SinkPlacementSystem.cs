using Godot;
using Godot.Collections;

public partial class SinkPlacementSystem : Node3D
{
    partial class PlacementRule : GodotObject
    {
        public int rotationAngle {  get; set; }
        public bool isMetalic {  get; set; }

        public PlacementRule(int rotationAngle, bool isMetalic)
        {
            this.rotationAngle = rotationAngle;
            this.isMetalic = isMetalic;
        }
    }

    private Dictionary<Vector2, PlacementRule> possiblePlaces = new Dictionary<Vector2, PlacementRule>();   



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        possiblePlaces.Add(new Vector2(1, 5), new PlacementRule(0, false));
        possiblePlaces.Add(new Vector2(1, 7), new PlacementRule(0, false));
        possiblePlaces.Add(new Vector2(1, 9), new PlacementRule(0, false));
        possiblePlaces.Add(new Vector2(1, 11), new PlacementRule(0, false));
        possiblePlaces.Add(new Vector2(1, 13), new PlacementRule(0, false));
        possiblePlaces.Add(new Vector2(1, 15), new PlacementRule(0, false));
        possiblePlaces.Add(new Vector2(1, 17), new PlacementRule(0, false));
        possiblePlaces.Add(new Vector2(3, 19), new PlacementRule(270, false));
        possiblePlaces.Add(new Vector2(5, 19), new PlacementRule(270, false));
        possiblePlaces.Add(new Vector2(11, 19), new PlacementRule(270, false));
        possiblePlaces.Add(new Vector2(13, 19), new PlacementRule(270, false));
        possiblePlaces.Add(new Vector2(15, 17), new PlacementRule(180, false));
        possiblePlaces.Add(new Vector2(15, 15), new PlacementRule(180, false));
        possiblePlaces.Add(new Vector2(15, 13), new PlacementRule(180, false));
        possiblePlaces.Add(new Vector2(17, 11), new PlacementRule(270, false));
        possiblePlaces.Add(new Vector2(19, 9), new PlacementRule(180, false));
        possiblePlaces.Add(new Vector2(21, 7), new PlacementRule(270, false));
        possiblePlaces.Add(new Vector2(23, 5), new PlacementRule(180, false));
        possiblePlaces.Add(new Vector2(10, 7), new PlacementRule(180, true));
        possiblePlaces.Add(new Vector2(9, 17), new PlacementRule(0, true));
        possiblePlaces.Add(new Vector2(9, 15), new PlacementRule(0, true));
        possiblePlaces.Add(new Vector2(9, 13), new PlacementRule(0, true));
        possiblePlaces.Add(new Vector2(9, 11), new PlacementRule(0, true));
        possiblePlaces.Add(new Vector2(11, 17), new PlacementRule(90, true));
        possiblePlaces.Add(new Vector2(6, 15), new PlacementRule(180, true));
        possiblePlaces.Add(new Vector2(6, 13), new PlacementRule(180, true));
        possiblePlaces.Add(new Vector2(6, 11), new PlacementRule(180, true));
        possiblePlaces.Add(new Vector2(6, 9), new PlacementRule(180, true));
        possiblePlaces.Add(new Vector2(6, 7), new PlacementRule(180, true));
        possiblePlaces.Add(new Vector2(6, 5), new PlacementRule(90, true));
        possiblePlaces.Add(new Vector2(8, 5), new PlacementRule(90, true));
        possiblePlaces.Add(new Vector2(10, 5), new PlacementRule(90, true));
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Vector3 getSinkPosition(Vector3 towerPos)
	{

        Vector2 towerPos2D = new Vector2(towerPos.X, towerPos.Z);
        HighlightArea countertopA = this.GetNode<HighlightArea>("%SinkHighlightA");
        HighlightArea countertopB = this.GetNode<HighlightArea>("%SinkHighlightB");
        if (countertopA.isInside(towerPos2D))
        {
            towerPos2D = (towerPos2D / 2).Floor() * 2;
            if (!(countertopA.isOccupied(towerPos2D) ||
                (countertopA.isOccupied(towerPos2D + new Vector2(0, 1))) ||
                (countertopA.isOccupied(towerPos2D + new Vector2(1, 1))) ||
                (countertopA.isOccupied(towerPos2D + new Vector2(1, 0)))))
            {
                towerPos.X = towerPos2D.X + 1;
                towerPos.Z = towerPos2D.Y + 1;
                towerPos.Y = 0.5f;
            }
            else
            {
                towerPos.Y = -10f;
                towerPos.X = 0;
                towerPos.Z = 0;

            }
        }
        else if (countertopB.isInside(towerPos2D))
        {
            towerPos2D = (((towerPos2D + new Vector2(0, 1)) / 2).Floor() * 2) - new Vector2(0, 1);
            if (!(countertopB.isOccupied(towerPos2D) ||
                 (countertopB.isOccupied(towerPos2D + new Vector2(0, 1))) ||
                 (countertopB.isOccupied(towerPos2D + new Vector2(1, 1))) ||
                 (countertopB.isOccupied(towerPos2D + new Vector2(1, 0)))))
            {
                towerPos.X = towerPos2D.X + 1;
                towerPos.Z = towerPos2D.Y + 1;
                towerPos.Y = 0.5f;
            }
            else
            {
                towerPos.Y = -10f;
                towerPos.X = 0;
                towerPos.Z = 0;

            }
        }
        else
        {
            towerPos.Y = -10f;
            towerPos.X = 0;
            towerPos.Z = 0;

        }
        return towerPos;


    }

    public void setSinkPlacementRules(Tower tower, Vector2I v2)
    {
        SinkDisplayer displayer = tower.GetNodeOrNull<SinkDisplayer>("%SinkDisplayer");
        if (displayer != null)
        {
            if (possiblePlaces.ContainsKey(v2))
            {
                PlacementRule rule = possiblePlaces[v2];
                displayer.display(rule.rotationAngle, rule.isMetalic);
            }
            
        }
    }
}
