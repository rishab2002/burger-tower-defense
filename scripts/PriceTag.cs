using Godot;
using System;

public partial class PriceTag : Label
{
	[Export] private int price;


    public override void _Ready()
    {
        setPrice(price);
    }


    public int getPrice()
	{
		return price;
    }

	public void setPrice(int price)
	{
		this.price = price;
		this.Text = $"${price}";
	}
}
