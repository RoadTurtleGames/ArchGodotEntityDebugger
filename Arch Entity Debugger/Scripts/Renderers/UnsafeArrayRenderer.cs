namespace RoadTurtleGames.ArchEntityDebugger;

using Arch.LowLevel;
using Godot;

[EntityRenderer(typeof(UnsafeArray<>))]
public class UnsafeArrayRenderer<T> : IEntityTreeRenderer
    where T : unmanaged
{
    public void Render(bool isNew, TreeItem rootItem, object component, string fieldName)
    {
        UnsafeArray<T> unsafeArray = (UnsafeArray<T>)component;

        rootItem.SetText(0, $"{fieldName} | Length {unsafeArray.Length}:");

        for (int i = 0; i < unsafeArray.Length; i++)
        {
            T item = unsafeArray[i];
            EntityTreeRendering.Render(rootItem, item, i, i.ToString());
        }
    }
}