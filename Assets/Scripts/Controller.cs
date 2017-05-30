/*
using UnityEngine;

public abstract class Controller : MonoBehaviour {

    public abstract void SetValue(float value);

    public static float linearInterpolation(float x0, float x1, float y0, float y1, float x)
    {
        float d = x1 - x0;
        if (d == 0)
            return (y0 + y1) / 2;
        return y0 + (x - x0) * (y1 - y0) / d;
    }
}
*/