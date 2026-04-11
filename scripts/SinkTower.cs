using Godot;
using System;

public partial class SinkTower : Tower
{
    [Export]
    private Node3D metalicModel;

    [Export]
    private Node3D nonMetalicModel;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();
        base.towerColor = GlobalEnums.EnemyColor.Brown;
        this.UpdateAppearance();
        base.footprint = 2;
        base.isOnCounter = true;
	}

    protected override void ConstructMenu()
    {
        base.AttachLabel("Garbage\nDisposal");

        base.AttachButton(upgradeSpeedButton, "Increase\nAttack Speed", 25, UpgradeAttackSpeed);

        base.AttachButton(upgradeAttackButton, "Suction\n(Slow Enemies)", 25, UpgradeAttackAction);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        base._Process(delta);
	}

    public override void UpdateAppearance()
    {
        metalicModel.GetChild<MeshInstance3D>(0).MaterialOverride = colorMaterials[(int)this.towerColor];
        nonMetalicModel.GetChild<MeshInstance3D>(0).MaterialOverride = colorMaterials[(int)this.towerColor];
    }
}
