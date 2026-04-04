using Godot;
using System;

public partial class IceTower : Tower
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

    protected override void ConstructMenu()
    {
        base.AttachLabel("Icen\nMachine");

        base.AttachButton(upgradeSpeedButton, "Increase\nAttack Speed", 15, UpgradeAttackSpeed);

        base.AttachButton(upgradeRangeButton, "Increase\nAttack Range", 15, UpgradeRange);

        base.AttachButton(upgradeAttackButton, "Deep Freeze\n(Enemies Stop)", 15, UpgradeAttackAction);

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        base._Process(delta);
	}
}
