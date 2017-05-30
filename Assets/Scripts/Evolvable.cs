using UnityEngine;
using System.Collections;

public class Evolvable : MonoBehaviour {

    public Genome genome;

    public virtual float GetScore()
    {
        return 0;
    }
}
