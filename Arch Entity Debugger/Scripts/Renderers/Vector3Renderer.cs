namespace RoadTurtleGames.ArchEntityDebugger;

using Godot;

[EntityRenderer(typeof(System.Numerics.Vector3))]
public class Vector3Renderer : IEntityTreeRenderer
{
    public void Render(bool isNew, TreeItem rootItem, object component, string fieldName)
    {
        System.Numerics.Vector3 vector3 = (System.Numerics.Vector3)component;

        rootItem.SetText(0, $"{fieldName}: {vector3}");
    }
}
