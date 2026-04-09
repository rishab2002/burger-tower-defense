using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class Round : Resource
{
    [Export] public Godot.Collections.Array<Wave> waves;

    public void Sort()
    {
        waves = new Godot.Collections.Array<Wave>(
            waves.OrderBy(w => w.startTime)
        );
    }
}
