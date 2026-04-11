using Godot;
using System;
using System.Diagnostics;
using System.Transactions;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

public partial class SilverwareTower : Tower
{
    [Export]
    private Node3D gunModel;

    [Export]
    private Node3D standModel;

    [Export]
    private Godot.Collections.Array<Material> colorMaterialsGun;

    [Export]
    private Godot.Collections.Array<Material> colorMaterialsStand;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		base._Ready();
        base.footprint = 2;
        base.isOnCounter = false;
        towerColor = GlobalEnums.EnemyColor.Red;
        this.UpdateAppearance();
	}

    protected override void ConstructMenu()
    {
        base.AttachLabel("Silverware\nDispenser");

        base.AttachButton(upgradeSpeedButton, "Increase\nAttack Speed", 10, UpgradeAttackSpeed);

        base.AttachButton(upgradeRangeButton, "Increase\nAttack Range", 10, UpgradeRange);

        base.AttachButton(upgradeAttackButton, "Shoot Knives\n(Double Damage)", 10, UpgradeAttackAction);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        base._Process(delta);
	}

    public override void UpdateAppearance()
    {
        gunModel.GetChild<MeshInstance3D>(0).MaterialOverride = colorMaterialsGun[(int)this.towerColor];
        standModel.GetChild<MeshInstance3D>(0).MaterialOverride = colorMaterialsStand[(int)this.towerColor];
    }
}
