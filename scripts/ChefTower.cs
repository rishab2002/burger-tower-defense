using Godot;
using System;

public partial class ChefTower : Tower
{
	[Export]
	private AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		base.towerColor = GlobalEnums.EnemyColor.Tan;
		base.UpdateAppearance();
		base.footprint = 1;
		base.isOnCounter = false;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
    }

    protected override void ConstructMenu()
    {
        base.AttachLabel("Chef");

        base.AttachButton(upgradeRangeButton, "Increase Range", 30, UpgradeRange);

    }
}
