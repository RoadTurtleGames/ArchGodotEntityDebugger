namespace RoadTurtleGames.ArchEntityDebugger;

using System;
/// <summary>
/// Attribute to define the display name for an archetype.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ArchetypeAttribute : Attribute
{
    public string DisplayName { get; }

    public ArchetypeAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}