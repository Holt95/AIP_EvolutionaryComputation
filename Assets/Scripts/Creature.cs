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

    /// <summary>
    /// Draw a line on creature to show how well it's scoring
    /// </summary>
    public void OnDrawGizmosSelected()
    {
        Vector3 size = new Vector2(2, 1);
        Vector3 border = new Vector2(2,0);

        float finalTime = Evolution.S.maxP;
        float timeInterval = 0.05f;


        float y_min = Evolution.S.minSin;
        float y_max = Evolution.S.maxSin;

        for (int i = 0; i < genome.genes.Length; i++)
        {
            Vector3 p_prev = Vector3.zero;
            for (float t = 0; t < finalTime; t += timeInterval)
            {
                float x = Controller.linearInterpolation
                        (
                            0, finalTime,
                            0, size.x,
                            t
                        );

                Vector3 p = transform.position + border +
                    new Vector3
                    (x,
                        Controller.linearInterpolation
                        (
                            y_min, y_max,
                            0, size.y,
                            express(genome.genes[i], t)
                        )
                    );

                if (t != 0)
                {
                    Gizmos.color = Color.Lerp(Color.red, Color.green, i/(float)genome.genes.Length);
                    Gizmos.DrawLine(p_prev, p);
                }

                p_prev = p;
            }
        }


        float x_now = Controller.linearInterpolation
            (
                0, finalTime,
                0, size.x,
                (Time.time - Evolution.startTime) % finalTime
            );
        Vector3 now = transform.position + border + new Vector3(x_now, 0);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(now, now + Vector3.up * size.y);
    }
}
