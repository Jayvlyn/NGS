using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyRebinder : MonoBehaviour
{
    [SerializeField] InputActionReference jumpAction = null;

    [SerializeField] GameObject startRebindObject = null;
    [SerializeField] GameObject waitingForInputObject = null;
    //[SerializeField] PlayerInput playerController = null;
    [SerializeField] TMP_Text jumpText = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private void Update()
    {

    }

    public void StartRebinding()
    {
        jumpText.text = ".....";
        //startRebindObject.SetActive(false);
        //waitingForInputObject.SetActive(true);

        //playerController.SwitchCurrentActionMap("Menu");

        jumpAction.action.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnMatchWaitForAnother(0.1f)
            .OnComplete(operation => RebindComplete()).Start();
    }

    private void RebindComplete()
    {
        int bindingIndex = jumpAction.action.GetBindingIndexForControl(jumpAction.action.controls[0]);

        jumpText.text = InputControlPath.ToHumanReadableString(jumpAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        //startRebindObject.SetActive(true);
        //waitingForInputObject.SetActive(false);

        //playerController.SwitchCurrentActionMap("Gameplay");
    }
}
