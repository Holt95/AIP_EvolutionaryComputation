using UnityEngine;

public interface IMutable<T>
{
    void Mutate();
    T Clone();
}

[System.Serializable]
public struct Genome : IMutable<Genome>{
    public Gene [] genes;

    public int generation;
    public float score;

    public Genome Clone()
    {
        Genome clone = new Genome();
        clone.genes = new Gene[genes.Length];
        for (int i = 0; i < genes.Length; i++)
            clone.genes[i] = genes[i].Clone();

        clone.score = score;
        clone.generation = generation;
        return clone;
    }

    public void Mutate ()
    {
        genes[Random.Range(0, genes.Length)].Mutate();
        score = 0;
    }

    public static Genome CreateRandom(int genomeLength, int geneLength)
    {
        Genome genome = new Genome();
        genome.genes = new Gene[   genomeLength     ];
        for (int i = 0; i < genome.genes.Length; i++)
            genome.genes[i] = Gene.CreateRandom(geneLength);
        genome.score = 0;
        return genome;
    }


    public static Genome Copulate (Genome g1, Genome g2)
    {
        Genome child = g1.Clone();

        // Crossing over on genes
        for (int g = 0; g < child.genes.Length; g++)
        {
            if (Random.Range(0f, 1f) > 0.5f)
                child.genes[g] = g2.genes[g].Clone();
        }
        child.generation++;

        return child;
    }
}






[System.Serializable]
public struct Gene : IMutable<Gene>
{
    // Sin parameters
    public float [] values;

    public Gene (int size)
    {
        values = new float[size];
    }

    public Gene Clone()
    {
        Gene clone = new Gene(values.Length);
        for (int i = 0; i < values.Length; i ++)
            clone.values[i] = values[i];
        return clone;
    }

    public void Mutate()
    {
        int i = Random.Range(0, values.Length-1);
        values[i] += Random.Range(-0.1f, +0.1f);
        values[i] = Mathf.Clamp01(values[i]);
    }
    
    public static Gene CreateRandom (int size)
    {
        Gene gene = new Gene(size);
        for (int i = 0; i < gene.values.Length; i++)
            gene.values[i] = Random.Range(0f,1f);
        return gene;
    }


    // Sinusoid
    public static float Evaluate4At(Gene gene, float time)
    {
        float min = gene.values[0];
        float max = gene.values[1];
        float period = gene.values[2];
        float offset = gene.values[3];

        min = Controller.linearInterpolation(0, 1, Evolution.S.minSin, Evolution.S.maxSin, min);
        max = Controller.linearInterpolation(0, 1, Evolution.S.minSin, Evolution.S.maxSin, max);
        period = Controller.linearInterpolation(0, 1, Evolution.S.minP, Evolution.S.maxP, period);
        offset = Controller.linearInterpolation(0, 1, Evolution.S.minP, Evolution.S.maxP, offset);

        return sinusoid(time + offset, min, max, period);
    }

    public static float sinusoid(float x, float min, float max, float period)
    {
        return (max - min) / 2 * (1 + Mathf.Sin(x * Mathf.PI * 2 / period)) + min;
    }





    public static float Evaluate3At(Gene gene, float time)
    {
        float amp = gene.values[0];
        float phase = gene.values[1];
        float freq = gene.values[2];

        amp = Controller.linearInterpolation(0, 1, Evolution.S.minSin, Evolution.S.maxSin, amp);
        phase = Controller.linearInterpolation(0, 1, Evolution.S.minP, Evolution.S.maxP, phase);
        freq = Controller.linearInterpolation(0, 1, 0, Mathf.PI, freq);

        return amp * Mathf.Cos(phase + freq * time);
    }



}

public delegate float GeneController(Gene gene, float time);