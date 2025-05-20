using UnityEngine;
using UnityEngine.InputSystem;

public static class InputKeyName
{
    public static string GetKeyName(InputAction action, string compositePartName = "", bool isNegative = false)
    {
        if (action == null) return "";

        foreach (var binding in action.bindings)
        {
            if (!string.IsNullOrEmpty(compositePartName))
            {
                if (binding.isPartOfComposite && binding.name == compositePartName)
                    return ToReadable(binding.effectivePath);
            }
            else if (action.type == InputActionType.Value && action.expectedControlType == "Axis")
            {
                if ((isNegative && binding.name == "negative") || (!isNegative && binding.name == "positive"))
                    return ToReadable(binding.effectivePath);
            }
            else if (!binding.isComposite && !binding.isPartOfComposite)
            {
                return ToReadable(binding.effectivePath);
            }
        }

        return "Unbound";
    }

    private static string ToReadable(string path)
    {
        return InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }
}
