namespace RoadTurtleGames.ArchEntityDebugger;

using Godot;

public partial class EntityDebuggerHotkeyToggler : Node
{
    [Export]
    private string toggleAction = "toggle_arch_entity_debugger";

    [Export]
    private EntityDebugger entityDebugger;

    public override void _Ready()
    {
        if (!InputMap.HasAction(toggleAction))
        {
            InputMap.AddAction(toggleAction);
            InputEventKey defaultKeybind = new();
            defaultKeybind.Keycode = Key.Quoteleft;
            defaultKeybind.ShiftPressed = true;
            InputMap.ActionAddEvent(toggleAction, defaultKeybind);
        }
    }

    public override void _Input(InputEvent _event)
    {
        if (_event.IsActionPressed(toggleAction))
        {
            if (entityDebugger == null)
            {
                GD.PrintErr("entityDebugger is not set in EntityDebuggerHotkeyToggler!");
            }

            entityDebugger.SetActive(!entityDebugger.IsActive);
        }
    }
}
