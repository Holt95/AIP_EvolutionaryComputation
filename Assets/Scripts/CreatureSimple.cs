using UnityEngine;
using System.Collections;

public class CreatureSimple : Creature
{
    public GameObject head;

    public float headUpTime = 0;
    
    public override void FixedUpdate()
    {
        base.FixedUpdate();

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
}
