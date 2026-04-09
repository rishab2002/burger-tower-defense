using Godot;
using System;
using System.Diagnostics;
using System.Transactions;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;

public partial class SilverwareTower : Tower
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
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
}
