using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyRebinder : MonoBehaviour
{
    [SerializeField] InputActionReference keyAction = null;
    [SerializeField] PlayerInput playerController = null;
    [SerializeField] TMP_Text keyText = null;

    public bool isWalkLeft = false;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;


    public void StartRebinding()
    {
        keyText.text = ".....";

        playerController.SwitchCurrentActionMap("TestUI");

        if (keyAction.action.type == InputActionType.Value)
        {
            if(isWalkLeft)
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
            //rebindingOperation = keyAction.action.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnMatchWaitForAnother(0.1f)
            //    .OnComplete(operation => RebindComplete()).Start();
            rebindingOperation = keyAction.action.PerformInteractiveRebinding().OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete()).Start();
        }
    }

    private void RebindComplete()
    {
        int bindingIndex = (keyAction.action.type == InputActionType.Value) 
            ? keyAction.action.GetBindingIndexForControl(keyAction.action.controls[(isWalkLeft) ? 0 : 1])
            : keyAction.action.GetBindingIndexForControl(keyAction.action.controls[0]);

        keyText.text = InputControlPath.ToHumanReadableString(keyAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        playerController.SwitchCurrentActionMap("Test");
    }
}
