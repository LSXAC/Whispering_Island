using System;
using Godot;

[GlobalClass]
public abstract partial class ItemAttributeBase : Resource
{
    public abstract string GetNameOfAttribute();
}
