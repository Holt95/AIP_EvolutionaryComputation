using UnityEngine;
using System.Collections;
using System;

public class ControllerSpring : Controller
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

    public override void SetValue(float value)
    {
        position = value;
    }

    // Update is called once per frame
    void FixedUpdate () {
        spring.distance = linearInterpolation(-1, +1, contracted, relaxed, position);
    }
}
