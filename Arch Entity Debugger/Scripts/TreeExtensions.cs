namespace RoadTurtleGames.ArchEntityDebugger;

using Godot;
using System;

public static class TreeExtensions
{
    /// <summary>
    /// Tries to get a child, and if it doesn't exist, creates a new one
    /// </summary>
    /// <returns>true if child is new, false if it already existed</returns>
    public static bool CreateOrGetChild(this TreeItem parentItem, int childIndex, out TreeItem child)
    {
        if (parentItem.GetChildCount() > childIndex)
        {
            child = parentItem.GetChild(childIndex);
            return false;
        }

        child = parentItem.CreateChild();
        return true;
    }

    public static TreeItem CreateChildAlphabetically(this TreeItem parentItem, string text, int textColumn = 0)
    {
        int insertIndex = GetInsertIndex(parentItem, text, textColumn);
        TreeItem child = parentItem.CreateChild(insertIndex);
        child.SetText(textColumn, text);

        return child;
    }

    private static int GetInsertIndex(TreeItem parent, string newText, int column)
    {
        TreeItem child = parent.GetFirstChild();
        int index = 0;

        while (child != null)
        {
            if (newText.CompareTo(child.GetText(column)) < 0)
                return index;

            index++;
            child = child.GetNext();
        }

        return index;
    }

    public static TreeItem CreateChildWithMetadataOrdering(this TreeItem parentItem, Func<Variant, Variant, int> comparisonFunc, int comparisonColumn, Variant metadata)
    {
        int insertIndex = GetInsertIndex(parentItem, comparisonFunc, comparisonColumn, metadata);
        TreeItem child = parentItem.CreateChild(insertIndex);
        child.SetMetadata(comparisonColumn, metadata);
        return child;
    }

    private static int GetInsertIndex(TreeItem parent, Func<Variant, Variant, int> comparisonFunc, int column, Variant newMetadata)
    {
        TreeItem child = parent.GetFirstChild();
        int index = 0;

        while (child != null)
        {
            Variant childMetadata = child.GetMetadata(column);

            if (comparisonFunc(newMetadata, childMetadata) > 0)
                return index;

            index++;
            child = child.GetNext();
        }

        return index;
    }
}
