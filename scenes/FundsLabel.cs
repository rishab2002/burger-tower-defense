using Godot;
using System;
using System.Diagnostics;

public partial class FundsLabel : Label
{
	int funds;


    [Signal]
    public delegate void FundsChangedEventHandler(int newFunds);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		setFunds(100);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}



	public int getFunds()
	{
		return funds;
	}

	public void setFunds(int funds)
	{
		this.funds = funds;
        this.Text = $"${funds}";
        EmitSignal(SignalName.FundsChanged, funds);
    }

	public void reduceFunds(int cost)
	{
		setFunds(funds - cost);
	}
}
