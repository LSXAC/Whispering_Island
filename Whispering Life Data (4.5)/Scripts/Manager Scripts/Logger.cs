using System;
using System.Runtime.CompilerServices;
using Godot;

public partial class Logger
{
    public static bool NodeIsNotNull<Node>(
        Node reference,
        [CallerLineNumber] int lineNumber = 0,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = ""
    )
    {
        if (reference == null)
        {
            GD.PushWarning(
                $"Reference is null: {typeof(Node).Name} (Line {lineNumber}) in {System.IO.Path.GetFileName(filePath)}::{memberName} (Node name: {(reference as Godot.Node)?.Name ?? "unknown"})"
            );
            return false;
        }
        return true;
    }

    public static bool NodeIsNull<Node>(
        Node reference,
        [CallerLineNumber] int lineNumber = 0,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = ""
    )
    {
        if (reference == null)
        {
            GD.PushWarning(
                $"Reference is null: {typeof(Node).Name} (Line {lineNumber}) in {System.IO.Path.GetFileName(filePath)}::{memberName}"
            );
            return true;
        }
        return false;
    }

    public static void PrintWrongSaveType(
        [CallerLineNumber] int lineNumber = 0,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = ""
    )
    {
        GD.PrintErr(
            $"Wrong save type! (Line {lineNumber}) in {System.IO.Path.GetFileName(filePath)}::{memberName}"
        );
    }

    public static void PrintEmptyList(
        [CallerLineNumber] int lineNumber = 0,
        [CallerFilePath] string filePath = "",
        [CallerMemberName] string memberName = ""
    )
    {
        GD.PrintErr(
            $"List is Empty! (Line {lineNumber}) in {System.IO.Path.GetFileName(filePath)}::{memberName}"
        );
    }

    public static bool HasNodeOrPrintError(Node parent, string nodePath)
    {
        if (!parent.HasNode(nodePath))
        {
            GD.PrintErr($"Node '{nodePath}' not found in '{parent.Name}'.");
            return false;
        }
        return true;
    }

    public static bool ListHasZeroItems<[MustBeVariant] T>(Godot.Collections.Array<T> list)
    {
        if (list.Count == 0)
        {
            GD.PrintErr($"List is empty: {typeof(T).Name}");
            return true;
        }
        return false;
    }
}
