using Godot;
using System;
using static GlobalEnums;

public partial class EnemyPath : Path3D
{
    private PackedScene _enemy = GD.Load<PackedScene>("res://scenes/enemy.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		Timer timer = new Timer();
        AddChild(timer);
        timer.WaitTime = 2.0f; // Interval in seconds
        timer.OneShot = false; // Set to true to run only once
        timer.Timeout += AddTomato; // Connect signal
        timer.Start();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddTomato()
	{
		AddEnemy(1f, 5, EnemyType.Tomato);
	}

	public void AddEnemy(float speed, int health, EnemyType type)
	{
		Enemy enemy = (Enemy)_enemy.Instantiate();
		enemy.CreateInstance(speed, health, type);
		this.AddChild(enemy);
	}
}
