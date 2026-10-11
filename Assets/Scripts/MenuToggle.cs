
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class MenuToggle : MonoBehaviour
{
    private GameObject menu;
    private bool wasPressed;

    private void Awake()
    {
        if (transform.childCount > 0)
            menu = transform.GetChild(0).gameObject;
        else
            Debug.LogWarning("MenuToggle: parent has no child!");
    }

    private void Start()
    {
        if (menu != null)
            menu.SetActive(false);
    }

    private void Update()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is not XRController controller)
                continue;

            var hand = controller.usages;

            if (!hand.Contains(UnityEngine.InputSystem.CommonUsages.LeftHand))
                continue;

            var button = device.TryGetChildControl<ButtonControl>("secondaryButton");

            if (button == null)
                continue;

            bool isPressed = button.isPressed;

            if (isPressed && !wasPressed)
            {
                if (menu != null)
                    menu.SetActive(!menu.activeSelf);
            }

            wasPressed = isPressed;
        }
    }
}
