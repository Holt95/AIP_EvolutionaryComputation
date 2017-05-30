using UnityEngine;
using System.Collections;

public class CreatureSimple : Evolvable
{
    public GameObject body;

    public ControllerSpring[] limbs;
    public GeneController express = Gene.Evaluate4At;

    public float bodyUpTime = 0;
    
    public void FixedUpdate()
    {
        //for each limb, express the genes values over set amount of time
        for (int i = 0; i < limbs.Length; i++)
        {
            limbs[i].SetValue(express(genome.genes[i], Time.time - Evolution.startTime));
        }

        // Keeps the score updated
        genome.score = GetScore();

        //If body is up with 20 degree
        if (IsUp(body))
        {
            //The amount of time it is able to hold up its body
            bodyUpTime += Time.fixedDeltaTime; 
        }
    }

    /// <summary>
    /// Get score of the creep
    /// </summary>
    /// <returns></returns>
    public override float GetScore ()
    {
        float walkingScore = body.transform.position.x; //body position on the x-axis used to score (starting value 0 so higher is better)
        return  walkingScore 
                * (IsDown(body) ? 0.1f : 1f) //WalkingScore valid if were not down, otherwise is only half
                + (IsUp(body) ? 1f : 0f) //Bonus given if body is up by the end
                + bodyUpTime / Evolution.S.simulationTime; //Amount body is up divided by the amount the similation is run (sets the score between 0 and 1)
    }

    /// <summary>
    /// Check if body is pointing up
    /// </summary>
    /// <param name="body">body to check</param>
    /// <param name="angle">Angle to check at</param>
    /// <returns></returns>
    public bool IsUp(GameObject body)
    {
        //Body angle has to be within the amount of accepted degrees to score (resisting sliding across floor)
        return body.transform.eulerAngles.z < 45 ||
                body.transform.eulerAngles.z > 315;
    }

    /// <summary>
    /// Check if body is pointing down
    /// </summary>
    /// <param name="body">body to check</param>
    /// <param name="angle">Angle to check at</param>
    /// <returns></returns>
    public bool IsDown(GameObject body)
    {
        //If were flipped we penalise the walking score (not zero, it might have walked really far but messed up in the end, that is still good genes)
        return body.transform.eulerAngles.z > 90 &&
               body.transform.eulerAngles.z < 270;
    }
}
