namespace RoadTurtleGames.ArchEntityDebugger;

using Arch.Core;
using Arch.Core.Extensions;
using Godot;

[EntityRenderer(typeof(Entity))]
public class EntityRenderer : IEntityTreeRenderer
{
    static readonly Color refColor = new("#89B0F1");

    public void Render(bool isNew, TreeItem rootItem, object component, string fieldName)
    {
        Entity entity = (Entity)component;

        if (isNew)
        {
            rootItem.AddButton(0, ResourceLoader.Load<Texture2D>("res://addons/Arch Entity Debugger/Assets/Icons/shortcut.png"));
            rootItem.AddButton(1, ResourceLoader.Load<Texture2D>("res://addons/Arch Entity Debugger/Assets/Icons/expand.png"));
        }

        if (entity == Entity.Null)
        {
            rootItem.SetText(0, $"{fieldName}: REF{{NULL}}");
            rootItem.SetCustomColor(0, Colors.IndianRed);
        }
        else if (!entity.IsAlive())
        {
            rootItem.SetText(0, $"{fieldName}: REF{{INVALID}}");
            rootItem.SetCustomColor(0, Colors.MediumVioletRed);
        }
        else
        {
            rootItem.SetText(0, $"{fieldName}: REF{{{entity.Id}}}");

            rootItem.SetMeta("ENTITY_REF", entity.Id);

            bool expand = rootItem.HasMeta("EXPAND_ENTITY_REF") && (bool)rootItem.GetMeta("EXPAND_ENTITY_REF");
            if (expand)
            {
                rootItem.SetCustomBgColor(0, new Color(refColor, 0.25f));
                rootItem.ClearCustomColor(0);

                object[] components = entity.GetAllComponents();
                for (int i = 0; i < components.Length; i++)
                {
                    EntityTreeRendering.Render(rootItem, components[i], i, components[i].GetType().Name, true);
                }
            }
            else
            {
                rootItem.ClearCustomBgColor(0);
                rootItem.SetCustomColor(0, refColor);

                foreach (TreeItem child in rootItem.GetChildren())
                {
                    child.Free();
                }
            }
        }
    }
}