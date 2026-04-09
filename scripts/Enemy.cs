using Godot;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.Marshalling;
using static GlobalEnums;

public partial class Enemy : PathFollow3D
{
	private EnemyColor color;
	private EnemyType type;
	private float startSpeed = 5.0f;
    private int startHealth = 5;
	private PackedScene _enemyModel;

    private float speed;
	private int health;
	public bool isBurned {  get; private set; }
	public bool isFrozen { get; private set; }




    public void Initialize(float startSpeed, int startHealth, EnemyType enemyType)
	{
		this.startSpeed = startSpeed;
		this.startHealth = startHealth;
		this.type = enemyType;
		switch(enemyType)
		{
			case EnemyType.TopBun:
                _enemyModel = GD.Load<PackedScene>("res://scenes/top_bun.tscn");
				color = EnemyColor.Tan;
                break;
			case EnemyType.Tomato:
				_enemyModel = GD.Load<PackedScene>("res://scenes/tomato.tscn");
                color = EnemyColor.Red;
                break;
			case EnemyType.Lettuce:
                _enemyModel = GD.Load<PackedScene>("res://scenes/lettuce.tscn");
                color = EnemyColor.Green;
                break;
			case EnemyType.Cheese:
                _enemyModel = GD.Load<PackedScene>("res://scenes/cheese.tscn");
                color = EnemyColor.Yellow;
                break;
			case EnemyType.Patty:
                _enemyModel = GD.Load<PackedScene>("res://scenes/patty.tscn");
                color = EnemyColor.Brown;
                break;
			case EnemyType.BottomBun:
                _enemyModel = GD.Load<PackedScene>("res://scenes/bottom_bum.tscn");
                color = EnemyColor.Tan;
                break;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        speed = startSpeed;
		health = startHealth;
		this.AddChild(_enemyModel.Instantiate());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Progress += speed * (float)delta;
        if (Mathf.IsEqualApprox(ProgressRatio, 1.0f))
        {
			this.GetParent<EnemyPath>().EnemyCompletePath(this.type);
            QueueFree();
        }
    }

	public Vector2 GetPosition2D()
	{
		Vector3 position3d = this.Position;
		return new Vector2(position3d.X, position3d.Z);
	}

	public void Defeat()
	{
		this.QueueFree();
	}

	public void Burn()
	{
		if (isFrozen)
		{
			this.speed = startSpeed; //undo freeze
		}
		else if (health <= 1)
		{
			this.Defeat();
		}
		else
		{
            this.speed *= 2;
            this.health = 1;
		}
	}

	public void Freeze()
	{
		if (isBurned)
		{
			this.speed = startSpeed;
			this.health = startHealth;
		}
		else
		{
			this.speed /= 2;
		}
	}

	public void TakeDamage(int amount)
	{
		this.health -= amount;
		if (amount <= 0)
		{
			this.Defeat();
		}
	}


}
