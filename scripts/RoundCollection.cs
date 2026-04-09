using Godot;
using System;

[GlobalClass]
public partial class RoundCollection : Resource
{
    [Export] public Godot.Collections.Array<Round> Rounds;
}
