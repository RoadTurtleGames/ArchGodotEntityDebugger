namespace RoadTurtleGames.ArchEntityDebugger;

using Arch.Core.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Manages the mapping between archetypes and their display names.
/// </summary>
public static class ArchetypeManager
{
    private static Dictionary<ComponentType[], string> _archetypeDictionary = new(new ComponentTypeArrayComparer());

    static ArchetypeManager()
    {
        RegisterArchetypes();
    }

    private static void RegisterArchetypes()
    {
        Type[] typesInAssembly = Assembly.GetExecutingAssembly().GetTypes();

        foreach (Type type in typesInAssembly)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo field in fields)
            {
                ArchetypeAttribute attribute = field.GetCustomAttribute<ArchetypeAttribute>();
                if (attribute != null)
                {
                    ComponentType[] archetype = field.GetValue(null) as ComponentType[];
                    if (archetype != null)
                    {
                        _archetypeDictionary[archetype] = attribute.DisplayName;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tries to get the display name for a given archetype.
    /// </summary>
    public static bool TryGetArchetypeDisplayName(ComponentType[] types, out string displayName)
    {
        return _archetypeDictionary.TryGetValue(types, out displayName);
    }
}