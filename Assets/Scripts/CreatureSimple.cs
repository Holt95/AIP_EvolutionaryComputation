using UnityEngine;
using System.Collections;

public class CreatureSimple : Evolvable
{
    public GameObject head;

    public ControllerSpring[] limbs;
    public GeneController express = Gene.Evaluate4At;

    public float headUpTime = 0;
    
    public void FixedUpdate()
    {
        //base.FixedUpdate(); THIS WAS TP CALL THE CODE BELOW EARLIER
        for (int i = 0; i < limbs.Length; i++)
            limbs[i].SetValue(express(genome.genes[i], Time.time - Evolution.startTime));
        // Keeps the score updated
        genome.score = GetScore();

        // Body and head UP!
        if (IsUp(head, 20))
            headUpTime += Time.fixedDeltaTime;
    }

    public override float GetScore ()
    {
        //return head.transform.position.x;
        float position = head.transform.position.x;
        return
            position
            * (IsDown(head) ? 0.5f : 1f)
            + (IsUp(head) ? 2f : 0f)
            + headUpTime / Evolution.S.simulationTime
            ;
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
