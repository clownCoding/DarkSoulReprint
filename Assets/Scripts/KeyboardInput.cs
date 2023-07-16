using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IUserInput
{
    [Header("===== Key settings =====")]
    public KeyCode keyUp = KeyCode.W;
    public KeyCode keyDown = KeyCode.S;
    public KeyCode keyRight = KeyCode.D;
    public KeyCode keyLeft = KeyCode.A;

    public KeyCode KeyA = KeyCode.LeftControl;
    public KeyCode KeyB = KeyCode.Space;
    public KeyCode KeyC = KeyCode.J;
    public KeyCode KeyD = KeyCode.K;

    public MyButton buttonA;
    public MyButton buttonB;
    public MyButton buttonC;
    public MyButton buttonD;

    public KeyCode keyJUp = KeyCode.UpArrow;
    public KeyCode keyJDown = KeyCode.DownArrow;
    public KeyCode keyJLeft = KeyCode.LeftArrow;
    public KeyCode keyJRight = KeyCode.RightArrow;

    [Header("===== Mouse settings =====")]
    public bool mouseEnable = false;
    public float mouseSensitivityX = 1.0f;
    public float mouseSensitivityY = 1.0f;
    public KeyCode mouse0 = KeyCode.Mouse0;
    public KeyCode mouse1 = KeyCode.Mouse1;

    private void Awake()
    {
        buttonA = new MyButton();
        buttonB = new MyButton();
        buttonC = new MyButton();
        buttonD = new MyButton();
    }

    void Update()
    {
        buttonA.Tick(Input.GetKey(KeyA));
        buttonB.Tick(Input.GetKey(KeyB));
        buttonC.Tick(Input.GetKey(KeyC));
        buttonD.Tick(Input.GetKey(KeyD));

        if (mouseEnable)
        {
            Jup = Input.GetAxis("Mouse Y") * 2.0f * mouseSensitivityY;
            Jright = Input.GetAxis("Mouse X") * 1.5f * mouseSensitivityX;
        }
        else
        {
            Jup = (Input.GetKey(keyJUp) ? 1.0f : 0f) - (Input.GetKey(keyJDown) ? 1.0f : 0f);
            Jright = (Input.GetKey(keyJRight) ? 1.0f : 0f) - (Input.GetKey(keyJLeft) ? 1.0f : 0f);
        }
        
        targetDup = (Input.GetKey(keyUp) ? 1.0f : 0f) - (Input.GetKey(keyDown) ? 1.0f : 0f);
        targetDright = (Input.GetKey(keyRight) ? 1.0f : 0f) - (Input.GetKey(keyLeft) ? 1.0f : 0f);

        if(inputEnabled == false)
        {
            targetDup = 0f;
            targetDright = 0f;
        }

        Dup = Mathf.SmoothDamp(Dup, targetDup, ref velocityDup, 0.1f);
        Dright = Mathf.SmoothDamp(Dright, targetDright, ref velocityDright, 0.1f);

        Vector2 tempDAxis = SquareToCircle(new Vector2(Dright, Dup));
        float Dright2 = tempDAxis.x;
        float Dup2 = tempDAxis.y;

        Dmag = Mathf.Sqrt(Dup2 * Dup2 + Dright2 * Dright2);
        Direction = transform.forward * Dup2 + transform.right * Dright2;

        run = (buttonB.IsPressing && !buttonB.IsDelaying) || buttonB.IsExtending;
        jump = buttonB.OnPressed && buttonB.IsExtending;
        roll = buttonB.OnReleased && buttonB.IsDelaying;

        defense = mouseEnable == true ? Input.GetKey(mouse1) : buttonD.IsPressing;
        attack = mouseEnable == true ? Input.GetKeyDown(mouse0) : buttonC.OnPressed;

        lockon = buttonA.OnPressed;
    }
}
