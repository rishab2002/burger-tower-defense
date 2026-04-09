using Godot;
using System;
using static GlobalEnums;

[GlobalClass]
public partial class Wave : Resource
{
    [Export] public EnemyType enemyType;
    [Export] public float enemySpeed;
    [Export] public int enemyHealth;
    [Export] public int enemyCount;
    [Export] public float timeInterval;
    [Export] public float startTime;


}
