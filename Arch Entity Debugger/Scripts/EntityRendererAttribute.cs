namespace RoadTurtleGames.ArchEntityDebugger;

using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EntityRendererAttribute : Attribute
{
    public Type RenderedType { get; }

    public EntityRendererAttribute(Type renderedType)
    {
        RenderedType = renderedType;
    }
}