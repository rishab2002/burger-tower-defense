using Godot;
using System.Collections.Generic;
using static GlobalEnums;

public partial class BurgerIcon : Node
{
    [Export] private SubViewport subViewport;
    [Export] private Node3D modelContainer;
    [Export] private Camera3D camera;

    [Export] private Node3D bottomBun;
    [Export] private Node3D cheese;
    [Export] private Node3D lettuce;
    [Export] private Node3D patty;
    [Export] private Node3D topBun;
    [Export] private Node3D tomato1;
    [Export] private Node3D tomato2;

    [Export] private Material black;
    [Export] private Material colored;

    int invisibleParts = 7;

    public override void _Ready()
    {
        this.GetNode<EnemyPath>("%EnemyPath").EnemyReachEnd += ShowModel;
    }

    public Texture2D GetTexture()
    {
        return subViewport.GetTexture();
    }

    public void ShowModel(int type)
    {
        EnemyType enemyType = (EnemyType)type;
        switch (enemyType)
        {
            case EnemyType.TopBun:
                topBun.GetChild<MeshInstance3D>(0).MaterialOverride = colored;
                invisibleParts--;
                break;
            case EnemyType.Tomato:
                if (tomato1.GetChild<MeshInstance3D>(0).MaterialOverride == colored)
                {
                    tomato2.GetChild<MeshInstance3D>(0).MaterialOverride = colored;
                    invisibleParts--;
                }
                else 
                {
                    tomato1.GetChild<MeshInstance3D>(0).MaterialOverride = colored;
                    invisibleParts--;
                }
                break;
            case EnemyType.Lettuce:
                lettuce.GetChild<MeshInstance3D>(0).MaterialOverride = colored;
                invisibleParts--;
                break;
            case EnemyType.Cheese:
                cheese.GetChild<MeshInstance3D>(0).MaterialOverride = colored;
                invisibleParts--;
                break;
            case EnemyType.Patty:
                patty.GetChild<MeshInstance3D>(0).MaterialOverride = colored;
                invisibleParts--;
                break;
            case EnemyType.BottomBun:
                bottomBun.GetChild<MeshInstance3D>(0).MaterialOverride = colored;
                invisibleParts--;
                break;
        }
        
    }

    public void HideAll()
    {
        Node3D collection = this.GetNode<Node3D>("%ModelCollection");
        foreach (Node3D model in collection.GetChildren())
        {
            model.GetChild<MeshInstance3D>(0).MaterialOverride = black;
        }
        invisibleParts = 7;
    }
   
}
