namespace RoadTurtleGames.ArchEntityDebugger;

using Godot;
using System.Numerics;


[EntityRenderer(typeof(Matrix4x4))]
public class Matrix4x4Renderer : IEntityTreeRenderer
{
    public void Render(bool isNew, TreeItem rootItem, object component, string fieldName)
    {
        if (isNew)
            rootItem.Collapsed = true;

        Matrix4x4 matrix4X4 = (Matrix4x4)component;
        rootItem.SetTooltipText(0, matrix4X4.ToString());

        // Render each row of the matrix as a child item
        rootItem.CreateOrGetChild(0, out TreeItem row1);
        rootItem.CreateOrGetChild(1, out TreeItem row2);
        rootItem.CreateOrGetChild(2, out TreeItem row3);
        rootItem.CreateOrGetChild(3, out TreeItem row4);
        row1.SetText(0, $"[{matrix4X4.M11:F2}, {matrix4X4.M12:F2}, {matrix4X4.M13:F2}, {matrix4X4.M14:F2}]");
        row2.SetText(0, $"[{matrix4X4.M21:F2}, {matrix4X4.M22:F2}, {matrix4X4.M23:F2}, {matrix4X4.M24:F2}]");
        row3.SetText(0, $"[{matrix4X4.M31:F2}, {matrix4X4.M32:F2}, {matrix4X4.M33:F2}, {matrix4X4.M34:F2}]");
        row4.SetText(0, $"[{matrix4X4.M41:F2}, {matrix4X4.M42:F2}, {matrix4X4.M43:F2}, {matrix4X4.M44:F2}]");
    }
}