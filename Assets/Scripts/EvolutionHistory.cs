// Alan Zucconi
// www.alanzucconi.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

[System.Serializable]
public class EvolutionHistory : ScriptableObject {

    public bool hasToBeInitialised = true;

    public int generation = 0;

    public List<Genome> bestGenomes = new List<Genome>();
    public float bestScore;

    // History of best genomes at every generation
    public List<Genome> generations = new List<Genome>();

    public void Clear ()
    {
        generation = 0;
        bestScore = 0;
        generations.Clear();
        bestGenomes.Clear();    
    }

    [MenuItem("Assets/Evolution History")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<EvolutionHistory>();
    }


    //Check if we have new highest/lowest scores
    public void CalculateMinMaxScore ()
    {
        //These are simple initial values, chosen to be very high and very low so that the first batch of results will always be correctly scored
        float min = 10000;
        float max = -10000;

        //Check if genome score was best or worst
        foreach (Genome genome in generations)
        {
            if (genome.score < min)
                min = genome.score;

            if (genome.score > max)
                max = genome.score;
        }

        minScore = min;
        maxScore = max;
    }
    public float minScore;
    public float maxScore;
}
