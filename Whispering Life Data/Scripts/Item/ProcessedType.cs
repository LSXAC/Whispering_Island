using System;
using Godot;

[GlobalClass]
public partial class ProcessedType : ItemType
{
    public ProcessedType()
    {
        type = ItemResource.TYPE.PROCESSED;
    }
}
