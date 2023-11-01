namespace RoadTurtleGames.ArchEntityDebugger;

using Godot;

[EntityRenderer(typeof(System.Numerics.Quaternion))]
public class QuaternionRenderer : IEntityTreeRenderer
{
    public void Render(bool isNew, TreeItem rootItem, object component, string fieldName)
    {
        System.Numerics.Quaternion quaternion = (System.Numerics.Quaternion)component;

        rootItem.SetText(0, fieldName);
        rootItem.SetText(0, $"{fieldName}: {quaternion}");
    }
}
