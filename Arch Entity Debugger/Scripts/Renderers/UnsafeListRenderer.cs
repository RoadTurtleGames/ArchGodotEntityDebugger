namespace RoadTurtleGames.ArchEntityDebugger;

using Arch.LowLevel;
using Godot;

[EntityRenderer(typeof(UnsafeList<>))]
public class UnsafeListRenderer<T> : IEntityTreeRenderer
    where T : unmanaged
{
    public void Render(bool isNew, TreeItem rootItem, object component, string fieldName)
    {
        UnsafeList<T> unsafeList = (UnsafeList<T>)component;

        rootItem.SetText(0, $"{fieldName} ({unsafeList.Count}/{unsafeList.Capacity}):");

        Godot.Collections.Array<TreeItem> existingChildren = rootItem.GetChildren();

        //clean up old list items
        for (int i = unsafeList.Count; i < existingChildren.Count; i++)
        {
            existingChildren[i].Free();
        }

        //update / add list items
        for (int i = 0; i < unsafeList.Count; i++)
        {
            T item = unsafeList[i];
            EntityTreeRendering.Render(rootItem, item, i, item.ToString());
        }
    }
}