using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public static class PlayerInputFunctions
{
    public static void ChangeControlScheme(InputActions inputActions, InputControlScheme controlScheme)
	{
		string bindingGroup = inputActions.controlSchemes.First(x => x.name == controlScheme.name).bindingGroup;

		inputActions.bindingMask = InputBinding.MaskByGroup(bindingGroup);
	}

	public static void RemoveBindingMask(InputActions inputActions)
	{
		string keyboardMouseBindingGroup = inputActions.controlSchemes.First(x => x.name == inputActions.Mouse_KeyboardScheme.name).bindingGroup;
		string gamepadBindingGroup = inputActions.controlSchemes.First(x => x.name == inputActions.GamepadScheme.name).bindingGroup;

		inputActions.bindingMask = InputBinding.MaskByGroups(new string[2] { keyboardMouseBindingGroup, gamepadBindingGroup });
	}

	public static InputBinding KeyboardMouseBinding(InputActions inputActions)
	{
		string bindingGroup = inputActions.controlSchemes.First(x => x.name == inputActions.Mouse_KeyboardScheme.name).bindingGroup;

		return InputBinding.MaskByGroup(bindingGroup);
	}

	public static InputBinding GamepadBinding(InputActions inputActions)
	{
		string bindingGroup = inputActions.controlSchemes.First(x => x.name == inputActions.GamepadScheme.name).bindingGroup;

		return InputBinding.MaskByGroup(bindingGroup);
	}
}
