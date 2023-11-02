namespace RoadTurtleGames.ArchEntityDebugger;

using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class EntityTreeRendering
{
    static Dictionary<Type, Type> genericTypedRenderers = new();
    static Dictionary<Type, IEntityTreeRenderer> customRenderers = new();

    static EntityTreeRendering()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (typeof(IEntityTreeRenderer).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                Type renderedType = null;

                EntityRendererAttribute attr = (EntityRendererAttribute)Attribute.GetCustomAttribute(type, typeof(EntityRendererAttribute));
                if (attr != null)
                {
                    renderedType = attr.RenderedType;
                }

                if (renderedType != null)
                {
                    if (type.IsGenericTypeDefinition)
                    {
                        genericTypedRenderers[renderedType] = type;
                        GD.Print($"Loaded generic type {type.Name}");
                    }
                    else
                    {
                        IEntityTreeRenderer instance = Activator.CreateInstance(type) as IEntityTreeRenderer;
                        customRenderers[renderedType] = instance;
                        GD.Print($"Loaded type {type.Name}");
                    }
                }
            }
        }
    }

    public static void Render(TreeItem parentItem, object component, int childIndex, string fieldName = "", bool highlighted = false, int depth = 0)
    {
        if (depth > 3)
            return;

        bool isNew = parentItem.CreateOrGetChild(childIndex, out TreeItem componentItem);
        if (!string.IsNullOrEmpty(fieldName))
            componentItem.SetText(0, fieldName);

        if (highlighted)
        {
            componentItem.SetCustomBgColor(0, new Color(1, 1, 1, 0.1f));
        }

        if (component == null)
        {
            componentItem.SetText(0, $"{fieldName}: NULL");
            componentItem.SetCustomColor(0, Colors.Red);
            return;
        }

        Type componentType = component.GetType();

        if (componentType.IsPrimitive || componentType == typeof(string))
        {
            componentItem.SetText(0, $"{fieldName}: {component}");
            return;
        }

        if (componentType.IsEnum)
        {
            componentItem.SetText(0, $"{fieldName}: {component}");
            return;
        }

        if (customRenderers.TryGetValue(componentType, out IEntityTreeRenderer customRenderer))
        {
            customRenderer.Render(isNew, componentItem, component, fieldName);

            return;
        }

        if (componentType.IsGenericType)
        {
            Type genericTypeDefinition = componentType.GetGenericTypeDefinition();
            if (genericTypedRenderers.TryGetValue(genericTypeDefinition, out Type rendererType))
            {
                customRenderer = (IEntityTreeRenderer)Activator.CreateInstance(rendererType.MakeGenericType(componentType.GetGenericArguments()));
                customRenderer.Render(isNew, componentItem, component, fieldName);

                // Cache the instance in customRenderers for reuse
                customRenderers[componentType] = customRenderer;

                return;
            }
        }

        FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        int fieldIndex = -1;
        foreach (FieldInfo field in fields)
        {
            fieldIndex++;

            object fieldValue;
            try
            {
                fieldValue = field.GetValue(component);
                Render(componentItem, fieldValue, fieldIndex, field.Name, false, depth + 1);
            }
            catch (Exception e)
            {
                componentItem.CreateOrGetChild(fieldIndex, out TreeItem child);
                child.SetText(0, $"{field.Name} | {field.FieldType}: ERROR");
                child.SetTooltipText(0, e.Message);
                child.SetCustomColor(0, Colors.Red);
                return;
            }
        }
    }
}
