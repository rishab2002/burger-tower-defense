using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class TowerButton : Button
{
	int price;
	private FundsLabel fundsLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        price = this.GetChild<PriceTag>(0).getPrice();
        fundsLabel = this.GetNode<FundsLabel>("%FundsLabel");
		fundsLabel.FundsChanged += UpdateEnabled;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void UpdateEnabled(int funds)
	{
		if (price > funds)
		{
			this.Disabled = true;
		}
		else
		{
			this.Disabled = false;
		}
	}
}
