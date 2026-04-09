using Godot;
using System;

public partial class StoveTower : Tower
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

    protected override void ConstructMenu()
    {
        base.AttachLabel("Stove");

        base.AttachButton(upgradeSpeedButton, "Increase\nAttack Speed", 20, UpgradeAttackSpeed);

        base.AttachButton(upgradeAttackButton, "Multiple Burners\n(Increase Hit Rate)", 20, UpgradeAttackAction);



    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        base._Process(delta);
	}
}
