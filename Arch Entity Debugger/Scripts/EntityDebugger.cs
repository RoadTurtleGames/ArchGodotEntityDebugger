namespace RoadTurtleGames.ArchEntityDebugger;

using Arch.Core;
using Arch.Core.Extensions;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class EntityDebugger : Control
{
    private TabBar worldOptions;
    private Tree entityListTree;
    private Tree entityDetailsTree;

    [Export]
    private bool activateOnReady = true;
    [Export]
    private float refreshRate = 0.05f;
    [Export]
    private PackedScene debugWindowPrefab;
    private Window debugWindow;

    private EntityReference selectedEntity = EntityReference.Null;

    private double timer;

    public World ActiveWorld => worldOptions.CurrentTab != -1 && World.WorldSize > worldOptions.CurrentTab ? World.Worlds[worldOptions.CurrentTab] : null;
    public bool IsActive => debugWindow.Visible;

    /// <summary>
    /// Sets the window's visibility, and enables/disables processing of entities
    /// </summary>
    public void SetActive(bool active)
    {
        debugWindow.Visible = active;
    }

    /// <summary>
    /// Sets the active world to display entities from
    /// </summary>
    public void SetWorld(int index)
    {
        if (index > 0 && index < World.WorldSize)
        {
            worldOptions.CurrentTab = index;
            selectedEntity = EntityReference.Null;
            entityDetailsTree.Clear();
            ClearEntityListTree();
        }
    }

    /// <summary>
    /// Selects an entity in the details tree
    /// </summary>
    /// <param name="entity"></param>
    public void SelectEntity(EntityReference entity)
    {
        if (selectedEntity != entity)
        {
            selectedEntity = entity;
            entityDetailsTree.Clear();
            RefreshEntityDetailsTree();
        }
    }

    public override void _Ready()
    {
        debugWindow = GD.Load<PackedScene>(debugWindowPrefab.ResourcePath).Instantiate() as Window;
        debugWindow.CloseRequested += () => SetActive(false);

        GetViewport().GuiEmbedSubwindows = false;
        SetActive(activateOnReady);
        debugWindow.Title = "Arch Entity Debugger";
        AddChild(debugWindow);
        debugWindow.Position = GetWindow().Position;

        worldOptions = debugWindow.GetNode<TabBar>("MarginContainer/WorldOptions");
        entityListTree = debugWindow.GetNode<Tree>("Container/EntitiesListTree");
        entityDetailsTree = debugWindow.GetNode<Tree>("Container/EntityDetailsTree");

        entityListTree.Connect("item_selected", Callable.From(OnEntitySelected));
        entityDetailsTree.Connect("button_clicked", Callable.From<TreeItem, int, int, int>(OnEntityComponentButtonClicked));

        worldOptions.Connect("tab_changed", Callable.From<int>(SetWorld));

        entityListTree.Columns = 2;
        entityListTree.SetColumnCustomMinimumWidth(1, 10);
        entityListTree.SetColumnExpandRatio(0, 100);
        entityListTree.SetColumnExpandRatio(1, 2);


        entityDetailsTree.Columns = 4;
        entityDetailsTree.SetColumnExpandRatio(0, 100);
        entityDetailsTree.SetColumnExpandRatio(1, 1);
        entityDetailsTree.SetColumnExpandRatio(2, 1);
        entityDetailsTree.SetColumnExpandRatio(3, 1);
    }

    public override void _Process(double delta)
    {
        if (!IsActive)
            return;

        timer += delta;
        if (timer > refreshRate)
        {
            timer = 0;
            RefreshWorlds();
            if (ActiveWorld == null)
                return;
            RefreshEntityListTree();
            RefreshEntityDetailsTree();
            CheckForOverlap();
        }
    }

    private void CheckForOverlap()
    {
        Rect2 debugRect = new(debugWindow.Position, debugWindow.Size);
        Rect2 mainRect = new(GetWindow().Position, GetWindow().Size);

        if (debugRect.Intersects(mainRect))
        {
            debugWindow.GetViewport().TransparentBg = true;
        }
        else
        {
            debugWindow.GetViewport().TransparentBg = false;
        }
    }

    private void RefreshWorlds()
    {
        if (worldOptions.TabCount != World.WorldSize)
        {
            worldOptions.ClearTabs();

            for (int i = 0; i < World.WorldSize; i++)
            {
                worldOptions.AddTab($"{i}");
            }
        }

        if (worldOptions.CurrentTab == -1 && !(World.WorldSize == 0))
        {
            worldOptions.CurrentTab = 0;
        }
    }

    private readonly Dictionary<string, TreeItem> archetypeItems = new();
    private readonly Dictionary<string, TreeItem> entityItems = new();
    private readonly Dictionary<string, TreeItem> categoryItems = new();
    private readonly StringBuilder stringBuilder = new();
    private readonly HashSet<string> currentEntities = new();
    private readonly List<string> toRemove = new();

    private void ClearEntityListTree()
    {
        entityListTree.Clear();
        archetypeItems.Clear();
        entityItems.Clear();
        categoryItems.Clear();
    }

    private void RefreshEntityListTree()
    {
        if (entityListTree.GetRoot() == null)
        {
            TreeItem root = entityListTree.CreateItem();
            root.SetText(0, "Entities List");
        }
        TreeItem rootItem = entityListTree.GetRoot();

        foreach (ref Archetype archetype in ActiveWorld)
        {
            if (!ArchetypeManager.TryGetArchetypeDisplayName(archetype.Types, out string archetypeKey))
            {
                stringBuilder.Clear();

                foreach (Arch.Core.Utils.ComponentType type in archetype.Types)
                {
                    stringBuilder.Append(type.Type.Name).Append(", ");
                }
                if (stringBuilder.Length > 0)
                    stringBuilder.Length -= 2;  // Remove trailing ", "

                archetypeKey = stringBuilder.ToString();
            }

            if (archetype.Entities == 0)
            {
                if (archetypeItems.ContainsKey(archetypeKey))
                {
                    archetypeItems[archetypeKey].SetText(1, "0");
                }

                continue;
            }

            string[] categories = archetypeKey.Split('/');
            if (categories.Length == 1)
            {
                categories = categories.Prepend("Misc").ToArray();
            }
            TreeItem parentItem = rootItem;

            string builtPath = "";
            foreach (string category in categories)
            {
                builtPath = string.IsNullOrEmpty(builtPath) ? category : $"{builtPath}/{category}";
                if (!categoryItems.ContainsKey(builtPath))
                {
                    TreeItem newCategoryItem = parentItem.CreateChildAlphabetically(category);
                    newCategoryItem.SetSelectable(0, false);
                    newCategoryItem.SetSelectable(1, false);
                    newCategoryItem.SetCustomColor(1, Colors.LightYellow);
                    categoryItems[builtPath] = newCategoryItem;
                }
                parentItem = categoryItems[builtPath];
            }

            if (!archetypeItems.ContainsKey(archetypeKey))
            {
                archetypeItems[archetypeKey] = parentItem;
                parentItem.SetTooltipText(0, archetypeKey);
                parentItem.SetCustomBgColor(0, new Color(1, 1, 1, 0.1f));
                parentItem.SetCustomBgColor(1, new Color(1, 1, 1, 0.1f));
                parentItem.Collapsed = true;
            }

            TreeItem archetypeItem = parentItem;
            archetypeItem.SetText(1, archetype.Entities.ToString());

            foreach (ref Chunk chunk in archetype)
            {
                foreach (int index in chunk)
                {
                    Entity entity = chunk.Entities[index];
                    string entityIdKey = $"{entity.Id} | {entity.Version()}";
                    currentEntities.Add(entityIdKey);

                    if (!entityItems.ContainsKey(entityIdKey))
                    {
                        TreeItem newEntityItem = archetypeItem.CreateChild(0);
                        newEntityItem.SetMetadata(0, entity.Id);
                        newEntityItem.SetText(0, entityIdKey);
                        entityItems[entityIdKey] = newEntityItem;
                    }

                    TreeItem entityItem = entityItems[entityIdKey];
                    if (entity == selectedEntity.Entity)
                    {
                        entityListTree.Disconnect("item_selected", Callable.From(OnEntitySelected));
                        entityItem.Select(0);
                        entityListTree.Connect("item_selected", Callable.From(OnEntitySelected));
                    }
                }
            }
        }

        toRemove.AddRange(entityItems.Keys.Except(currentEntities));
        foreach (string key in toRemove)
        {
            TreeItem parentArchetypeItem = entityItems[key].GetParent();
            entityItems[key].Free();
            entityItems.Remove(key);
        }

        currentEntities.Clear();
        toRemove.Clear();
    }

    private void OnEntitySelected()
    {
        TreeItem selected = entityListTree.GetSelected();
        if (selected == null) return;

        if (entityDetailsTree.GetRoot() == null)
        {
            entityDetailsTree.CreateItem();
        }
        Variant selectedMetadata = selected.GetMetadata(0);
        Entity entity = FindEntityById((int)selectedMetadata);
        SelectEntity(entity.Reference());
    }

    private void OnEntityComponentButtonClicked(TreeItem item, int buttonId, int column, int mouseButtonIndex)
    {
        if (item.HasMeta("ENTITY_REF"))
        {
            int entityId = (int)item.GetMeta("ENTITY_REF");

            if (buttonId == 0)
            {
                Entity entity = FindEntityById(entityId);
                SelectEntity(entity.Reference());
            }
            if (buttonId == 1)
            {
                bool isExpanded = item.HasMeta("EXPAND_ENTITY_REF") && (bool)item.GetMeta("EXPAND_ENTITY_REF");

                item.SetMeta("EXPAND_ENTITY_REF", !isExpanded);
            }
        }
    }

    private Entity FindEntityById(int id)
    {
        foreach (ref Archetype archetype in ActiveWorld)
        {
            foreach (ref Chunk chunk in archetype)
            {
                foreach (int index in chunk)
                {
                    Entity entity = chunk.Entities[index];
                    if (entity.Id == id)
                        return entity;
                }
            }
        }
        return Entity.Null;
    }

    private void RefreshEntityDetailsTree()
    {
        if (selectedEntity == EntityReference.Null)
        {
            entityDetailsTree.Clear();
            return;
        }

        Entity entity = selectedEntity.Entity;

        TreeItem entityRoot = entityDetailsTree.GetRoot();

        if (entityRoot == null)
        {
            entityRoot = entityDetailsTree.CreateItem();
            if (ArchetypeManager.TryGetArchetypeDisplayName(entity.GetComponentTypes(), out string archetypeName))
                archetypeName = archetypeName.Split("/").LastOrDefault();
            else
                archetypeName = "Entity";

            entityRoot.SetText(0, $"{archetypeName} | ID: {entity.Id} | Version: {entity.Version()}");
        }

        object[] components = entity.GetAllComponents();
        for (int i = 0; i < components.Length; i++)
        {
            EntityTreeRendering.Render(entityRoot, components[i], i, components[i].GetType().Name, true);
        }
    }
}