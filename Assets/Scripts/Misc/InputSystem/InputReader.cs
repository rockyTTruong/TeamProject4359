using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum GamePadButton
{
    DpadUp, DpadDown, DpadLeft, DpadRight,
    NorthButton, SouthButton, WestButton, EastButton,
    LeftShoulder, LeftTrigger, RightShoulder, RightTrigger,
    LeftStickPress, RightStickPress,
    Select, Start,
    Count
}

public class InputReader : SingletonMonobehaviour<InputReader>, PlayerInput.IFreeLookActions
{
    public delegate void ButtonPress();
    public delegate void ButtonLongPress();

    public float longPressDuration = 0.2f;

    [HideInInspector] public Vector2 leftStickValue;
    [HideInInspector] public Vector2 rightStickValue;
    [HideInInspector] public bool[] buttonHold = new bool[(int)GamePadButton.Count];
    [HideInInspector] public ButtonPress[] buttonPress = new ButtonPress[(int)GamePadButton.Count];
    [HideInInspector] public ButtonLongPress[] buttonLongPress = new ButtonLongPress[(int)GamePadButton.Count];

    private PlayerInput playerInput;
    private float timer = 0f;
    private int buttonIndex;

    private ButtonLongPress longPressAction;

    protected override void Awake()
    {
        base.Awake();
        playerInput = new PlayerInput();
        playerInput.FreeLook.SetCallbacks(this);
    }

    private void Start()
    {
        EnableInput();
    }

    private void Update()
    {
        if (buttonHold[buttonIndex])
        {
            timer += Time.unscaledDeltaTime;
            if (timer >= longPressDuration)
            {
                longPressAction?.Invoke();
                longPressAction = null;
            }
        }
    }

    private void HandleInputAction(InputAction.CallbackContext context, GamePadButton button)
    {
        buttonIndex = (int)button;
        if (context.started)
        {
            buttonPress[buttonIndex]?.Invoke();
            timer = 0f;
            buttonHold[buttonIndex] = true;
            longPressAction = buttonLongPress[buttonIndex];
        }
        else if (context.canceled)
        {
            buttonHold[buttonIndex] = false;
        }
    }

    public void EnableInput()
    {
        playerInput.FreeLook.Enable();
    }

    public void DisableInput()
    {
        playerInput.FreeLook.Disable();
    }

    public void OnLeftStick(InputAction.CallbackContext context)
    {
        leftStickValue = context.ReadValue<Vector2>();
    }

    public void OnRightStick(InputAction.CallbackContext context)
    {
        rightStickValue = context.ReadValue<Vector2>();
    }

    public void OnNorthButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.NorthButton);
    }

    public void OnSouthButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.SouthButton);
    }

    public void OnEastButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.EastButton);
    }

    public void OnWestButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.WestButton);
    }

    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.LeftTrigger);
    }

    public void OnRightTrigger(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.RightTrigger);
    }

    public void OnLeftShoulder(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.LeftShoulder);
    }

    public void OnRightShoulder(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.RightShoulder);
    }

    public void OnStartButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.Start);
    }

    public void OnSelectButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.Select);
    }

    public void OnLeftStickPress(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.LeftStickPress);
    }

    public void OnRightStickPress(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.RightStickPress);
    }

    public void OnDpadUpButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.DpadUp);
    }

    public void OnDpadDownButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.DpadDown);
    }

    public void OnDpadLeftButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.DpadLeft);
    }

    public void OnDpadRightButton(InputAction.CallbackContext context)
    {
        HandleInputAction(context, GamePadButton.DpadRight);
    }
}
