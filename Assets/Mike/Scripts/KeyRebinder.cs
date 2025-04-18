using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyRebinder : MonoBehaviour
{
    [SerializeField] InputActionReference keyAction = null;
    [SerializeField] PlayerInput playerController = null;
    [SerializeField] TMP_Text keyText = null;
    public string compositPartName = "";

    public bool isNegitive = false;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;


    public void StartRebinding()
    {
        keyText.text = ".....";

        playerController.SwitchCurrentActionMap("RebindKeys");

        if (keyAction.action.type == InputActionType.Value && keyAction.action.expectedControlType == "Axis")
        {
            if (isNegitive)
            {
                rebindingOperation = keyAction.action.PerformInteractiveRebinding(1).OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete()).Start();
            }
            else
            {
                rebindingOperation = keyAction.action.PerformInteractiveRebinding(2).OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete()).Start();
            }
        }
        else
        {
            rebindingOperation = keyAction.action.PerformInteractiveRebinding(GetBindingIndexByName(compositPartName)).OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete()).Start();
        }
    }

    private void RebindComplete()
    {
        int bindingIndex = (keyAction.action.type == InputActionType.Value)
                ? keyAction.action.GetBindingIndexForControl(keyAction.action.controls[(isNegitive) ? 0 : 1])
                : keyAction.action.GetBindingIndexForControl(keyAction.action.controls[0]);

        if(compositPartName != "") bindingIndex = GetBindingIndexByName(compositPartName);

        keyText.text = InputControlPath.ToHumanReadableString(keyAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        playerController.SwitchCurrentActionMap("Platformer");
    }

    private int GetBindingIndexByName(string name)
    {        
        for (int i = 0; i < keyAction.action.bindings.Count; i++)
        {
            if (keyAction.action.bindings[i].isPartOfComposite && keyAction.action.bindings[i].name == name)
            {
                return i;
            }
        }

        return -1;
    }
}
