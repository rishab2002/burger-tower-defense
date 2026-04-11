using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using static Godot.DisplayServer;
using static GlobalEnums;

public partial class Tower : Node3D
{
	private MeshInstance3D visibilityRadius;
	private OmniLight3D light;
    protected VBoxContainer upgradeMenu = new VBoxContainer();
	protected Button upgradeSpeedButton = new Button();
    protected Button upgradeRangeButton = new Button();
    protected Button upgradeAttackButton = new Button();
    public int footprint { get; protected set; }

    public bool isOnCounter { get; protected set; }


    [Export]
    protected Node3D towerModel;

    [Export]
    protected Godot.Collections.Array<Material> colorMaterials;

    protected int upgradeSpeedPrice = 0;
    protected int upgradeRangePrice = 0;
    protected int upgradeAttackPrice = 0;

    public EnemyColor towerColor { get; set; }

    protected PackedScene _priceTag = GD.Load<PackedScene>("res://scenes/price_tag.tscn");

    private FundsLabel fundsLabel;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		visibilityRadius = GetNode<MeshInstance3D>("%visibility");
		light = GetNode<OmniLight3D>("%Light");
        upgradeMenu.AddThemeConstantOverride("separation", 15);
        ConstructMenu();

        PriceTag tag = upgradeSpeedButton.GetChildOrNull<PriceTag>(0);
        if (tag != null)
        {
            upgradeSpeedPrice = tag.getPrice();
        }

        tag = upgradeRangeButton.GetChildOrNull<PriceTag>(0);
        if (tag != null)
        {
            upgradeRangePrice = tag.getPrice();
        }

        tag = upgradeAttackButton.GetChildOrNull<PriceTag>(0);
        if (tag != null)
        {
            upgradeAttackPrice = tag.getPrice();
        }





        this.SetProcess(true);

        GD.Print("READY");
        ProcessMode = Node.ProcessModeEnum.Always;
        SetProcess(true);
    }

    public void SetFundUpdateSingal(FundsLabel fundsLabel)
    {
        fundsLabel.FundsChanged += UpdateEnabled;
        this.fundsLabel = fundsLabel;
    }

    public void UpdateEnabled(int funds)
    {

        if (upgradeSpeedPrice != 0)
        {
            if (upgradeSpeedPrice > funds)
            {
                upgradeSpeedButton.Disabled = true;
            }
            else
            {
                upgradeSpeedButton.Disabled = false;
            }
        }

        if (upgradeRangePrice != 0)
        {
            if (upgradeRangePrice > funds)
            {
                upgradeRangeButton.Disabled = true;
            }
            else
            {
                upgradeRangeButton.Disabled = false;
            }
        }

        if (upgradeAttackPrice != 0)
        {
            if (upgradeAttackPrice > funds)
            {
                upgradeAttackButton.Disabled = true;
            }
            else
            {
                upgradeAttackButton.Disabled = false;
            }
        }

    }

  

    protected virtual void ConstructMenu()
	{
		return;
	}

	protected void AttachButton(Button button, string text, int price, Action action)
	{
		button.Theme = GD.Load<Theme>("res://Themes/button_theme.tres");
        button.Text = text;
        button.CustomMinimumSize = new Vector2(0, 100);
        button.Pressed += action;
        PriceTag priceTag = (PriceTag)_priceTag.Instantiate();
        priceTag.setPrice(price);
        button.AddChild(priceTag);
        upgradeMenu.AddChild(button);
    }

	protected void AttachLabel(string text)
	{
        Label label = new Label();
		label.Theme = GD.Load<Theme>("res://Themes/label_text_small.tres");
        label.Text = text;
        label.HorizontalAlignment = HorizontalAlignment.Center;
        upgradeMenu.AddChild(label);
    }


	public void ShowRange()
	{
		visibilityRadius.Show();
	}

	public void HideRange()
	{
		visibilityRadius.Hide();
	}

	public void Highlight()
	{
		light.Visible = true;
	}

	public void UnHighlight()
	{
		light.Visible = false;
	}

	public void displayMenu(Control control)
	{
		control.AddChild(upgradeMenu);

	}

	public void UpgradeAttackSpeed()
	{
        fundsLabel.reduceFunds(upgradeSpeedPrice);
        upgradeSpeedPrice = 0;
        MarkPurchased(upgradeSpeedButton);
        //TO DO: implement attack speed upgrade
    }

	public void UpgradeRange()
	{
        fundsLabel.reduceFunds(upgradeRangePrice);
        upgradeRangePrice = 0;
        visibilityRadius.Scale = new Vector3(1.25f, 1.25f, 1.25f);
        MarkPurchased(upgradeRangeButton);
    }

	public virtual void UpgradeAttackAction()
	{
        fundsLabel.reduceFunds(upgradeAttackPrice);
        upgradeAttackPrice = 0;
        //TO DO: implement for all children
        MarkPurchased(upgradeAttackButton);
    }

	private void MarkPurchased(Button button)
	{
        button.Text = "PURCHASED";
        button.Disabled = true;
        PriceTag p = button.GetChildOrNull<PriceTag>(0);
        if (p != null)
        {
           p.QueueFree();
        }
        
    }

    public virtual void UpdateAppearance()
    {
        towerModel.GetChild<MeshInstance3D>(0).MaterialOverride = colorMaterials[(int)this.towerColor];
    }

}
