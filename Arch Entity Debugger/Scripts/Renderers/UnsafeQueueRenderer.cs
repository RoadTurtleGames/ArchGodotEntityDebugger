namespace RoadTurtleGames.ArchEntityDebugger;

using Arch.LowLevel;
using Godot;

[EntityRenderer(typeof(UnsafeQueue<>))]
public class UnsafeQueueRenderer<T> : IEntityTreeRenderer
    where T : unmanaged
{
    public void Render(bool isNew, TreeItem rootItem, object component, string fieldName)
    {
        UnsafeQueue<T> unsafeQueue = (UnsafeQueue<T>)component;

        rootItem.SetText(0, $"{fieldName} ({unsafeQueue.Count}/{unsafeQueue.Capacity}):");

        int i = 0;
        foreach (T item in unsafeQueue)
        {
            EntityTreeRendering.Render(rootItem, item, i, i.ToString());
            i++;
        }
    }
}