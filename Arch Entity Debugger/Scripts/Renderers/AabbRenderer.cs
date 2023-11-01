namespace RoadTurtleGames.ArchEntityDebugger;

using Godot;

[EntityRenderer(typeof(Aabb))]
public class AabbRenderer : IEntityTreeRenderer
{
    public void Render(bool isNew, TreeItem rootItem, object component, string fieldName)
    {
        Aabb aabb = (Aabb)component;

        rootItem.SetText(0, fieldName);
        rootItem.SetText(0, $"{fieldName}: {aabb}");
    }
}
