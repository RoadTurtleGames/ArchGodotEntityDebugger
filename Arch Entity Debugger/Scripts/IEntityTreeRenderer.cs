namespace RoadTurtleGames.ArchEntityDebugger;

using Godot;

public interface IEntityTreeRenderer
{
    void Render(bool isNew, TreeItem rootItem, object component, string fieldName);
}