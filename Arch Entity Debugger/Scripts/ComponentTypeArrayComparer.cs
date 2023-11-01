namespace RoadTurtleGames.ArchEntityDebugger;

using Arch.Core.Utils;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Custom comparer for ComponentType arrays.
/// Necessary for using arrays as keys in dictionaries.
/// </summary>
public class ComponentTypeArrayComparer : IEqualityComparer<ComponentType[]>
{
    public bool Equals(ComponentType[] x, ComponentType[] y)
    {
        return Enumerable.SequenceEqual(x, y);
    }

    public int GetHashCode(ComponentType[] obj)
    {
        int hash = 19;
        foreach (ComponentType comp in obj)
        {
            hash = hash * 31 + comp.GetHashCode();
        }
        return hash;
    }
}
