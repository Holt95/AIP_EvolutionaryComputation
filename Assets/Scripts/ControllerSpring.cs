using UnityEngine;
using System.Collections;
using System;

public class ControllerSpring : MonoBehaviour
{
    public DistanceJoint2D spring;

    private float contracted;
    private float relaxed;

    public float max = 1.5f;
    public float min = 0.5f;
    
    [Range(-1, +1)]
    public float position = +1;

    // Use this for initialization
    void Start () {
        float distance = spring.distance;
        relaxed = distance * max;
        contracted = distance * min;
	}

    public void SetValue(float value)
    {
        position = value;
    }

    // Update is called once per frame
    void FixedUpdate () {
        spring.distance = linearInterpolation(-1, 1, contracted, relaxed, position);
    }

    /// <summary>
    /// Maps values from range of -1 to 1 over to the spring joint values of contracted and relaxed
    /// </summary>
    /// <param name="x0">-1</param>
    /// <param name="x1">1</param>
    /// <param name="y0">contracted</param>
    /// <param name="y1">relaxed</param>
    /// <param name="pos">position</param>
    /// <returns></returns>
    public static float linearInterpolation(float x0, float x1, float y0, float y1, float pos)
    {
        float d = x1 - x0;
        if (d == 0)
        {
            return (y0 + y1) / 2;
        }
        return y0 + (pos - x0) * (y1 - y0) / d;
    }
}
