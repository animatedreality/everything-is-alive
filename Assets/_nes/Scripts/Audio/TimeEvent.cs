using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeEvent
{

    public double time;
    public Action<double, int> callback;
    public int beatIndex;

    public TimeEvent(double time, Action<double, int> callback, int beatIndex)
    {
        this.time = time;
        this.callback = callback;
        this.beatIndex = beatIndex;
    }
}
