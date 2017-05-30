using UnityEngine;
using System.Collections;
public class Creature : Evolvable {

    public Controller[] limbs;

    public GeneController express = Gene.Evaluate4At;

	// Update is called once per frame
	public virtual void FixedUpdate () {
        for (int i = 0; i < limbs.Length; i++)
            limbs[i].SetValue(  express(genome.genes[i], Time.time - Evolution.startTime));
        // Keeps the score updated
        genome.score = GetScore();
    }

    public override float GetScore()
    {
        return transform.position.x;
    }

    public bool IsUp(GameObject obj, float angle = 30)
    {
        return obj.transform.eulerAngles.z < 0 + angle ||
                obj.transform.eulerAngles.z > 360 - angle;
    }

    public bool IsDown(GameObject obj, float angle = 45)
    {
        return obj.transform.eulerAngles.z > 180 - angle &&
                obj.transform.eulerAngles.z < 180 + angle;
    }
}
