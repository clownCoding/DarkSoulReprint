using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;
    public bool IsExtending = false;
    public bool IsDelaying = false;

    public float extendingDuration = 0.15f;
    public float delayingDuration = 0.15f;

    private bool curState = false;
    private bool lastState = false;

    private MyTimer extTimer = new();
    private MyTimer delayTimer = new();
   

    public void Tick(bool input)
    {
        extTimer.Tick();
        delayTimer.Tick();
        IsPressing = input;

        curState = input;
        OnPressed = false;
        OnReleased = false;

        if(curState != lastState)
        {
            if(curState == true)
            {
                OnPressed = true;
                StartTimer(delayTimer, delayingDuration);
            }else
            {
                OnReleased = true;
                StartTimer(extTimer, extendingDuration);
            }
            lastState = curState;
        }
        
        IsExtending = extTimer.state == MyTimer.STATE.RUN ? true : false;
        IsDelaying = delayTimer.state == MyTimer.STATE.RUN ? true : false;
    }

    private void StartTimer(MyTimer timer, float duration)
    {
        timer.duration = duration;
        timer.Go();  
    }
}   
    