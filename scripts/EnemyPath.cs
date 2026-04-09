using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using static GlobalEnums;

public partial class EnemyPath : Path3D
{
    private PackedScene _enemy = GD.Load<PackedScene>("res://scenes/enemy.tscn");
    private Round currentRound = null;
	private float roundTime = 0;
	private int indexOfNextWave = 0;
	private List<EnemyType> remainingEnemyTypes = [EnemyType.BottomBun, EnemyType.Patty, EnemyType.Cheese, EnemyType.Lettuce, EnemyType.Tomato, EnemyType.Tomato, EnemyType.TopBun];


	[Signal]
	public delegate void GameOverEventHandler();

    [Signal]
    public delegate void RoundEndEventHandler();

    [Signal]
    public delegate void EnemyReachEndEventHandler(int enemyType);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (currentRound != null)
		{
			if (indexOfNextWave >= currentRound.waves.Count && this.GetChildCount() == 0)
			{
				this.EndRound();
			}
			else
			{
                roundTime += (float)delta;

                while (indexOfNextWave < currentRound.waves.Count && roundTime >= currentRound.waves[indexOfNextWave].startTime)
                {
					this.BeginWave(currentRound.waves[indexOfNextWave]);
                    indexOfNextWave++;
                }
            }
        }
	}

	public void StartRound(Round round)
	{
		this.currentRound = round;
		this.roundTime = 0;
		this.indexOfNextWave = 0;

	}

	public void EndRound()
	{
		this.currentRound = null;
		EmitSignal(SignalName.RoundEnd);
	}

	public void EnemyCompletePath(EnemyType type)
	{
        EmitSignal(SignalName.EnemyReachEnd, (int)type);
		remainingEnemyTypes.Remove(type);
		if (remainingEnemyTypes.Count == 0 )
		{
			EmitSignal(SignalName.GameOver);
		}
    }

	public bool RoundIsRunning()
	{
		return this.currentRound != null;
	}

	public void AddEnemy(float speed, int health, EnemyType type)
	{
		Enemy enemy = (Enemy)_enemy.Instantiate();
		enemy.Initialize(speed, health, type);
		this.AddChild(enemy);
	}

	public void BeginWave(Wave wave)
	{
		// if the wave's enemy types has already complete the path, choose one at random instead
        if (!remainingEnemyTypes.Contains(wave.enemyType) && remainingEnemyTypes.Count != 0)
        {
            wave.enemyType = remainingEnemyTypes[Random.Shared.Next(remainingEnemyTypes.Count)];
        }

        int remainingEnemies = wave.enemyCount;
        Timer timer = new Timer();
        AddChild(timer);
        timer.WaitTime = wave.timeInterval; 
        timer.OneShot = false; // Set to true to run only once
        timer.Timeout += OnTimeout; // Connect signal
        timer.Start();

		void OnTimeout()
		{
			remainingEnemies--;
			if (remainingEnemies == 0)
			{
				timer.Stop();
				timer.QueueFree();
			}

			AddEnemy(wave.enemySpeed, wave.enemyHealth, wave.enemyType);
        }
    }
}
