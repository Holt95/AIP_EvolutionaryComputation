using UnityEngine;

public interface IMutable<T>
{
    void Mutate();
    T Clone();
}

[System.Serializable]
//Genome Struct
public struct Genome : IMutable<Genome>{
    public Gene [] genes;

    //Current generation of Genome
    public int generation;
    //Current score of Genome
    public float score;

    /// <summary>
    /// Function for cloning a given Genome
    /// </summary>
    /// <returns>Clone</returns>
    public Genome Clone()
    {
        //Inilialise new genome that is empty
        Genome clone = new Genome();
        //With genes that are empty
        clone.genes = new Gene[genes.Length];
        //For each gene, store the genes we wish to clone
        for (int i = 0; i < genes.Length; i++)
        {
            clone.genes[i] = genes[i].Clone();
        }
        //Set the clones score to be the same as the current score
        clone.score = score;
        //Set the clones generation to be the same as the current generation
        clone.generation = generation;
        //return the clone
        return clone;
    }

    /// <summary>
    ///Add together two genomes into one child with random genes from each parent
    /// </summary>
    /// <param name="g1">First parent</param>
    /// <param name="g2">Second parent</param>
    /// <returns>child</returns>
    public static Genome Mix(Genome g1, Genome g2)
    {
        //The child starts as a clone of first parent
        Genome child = g1.Clone();

        // For each gene the child has
        for (int g = 0; g < child.genes.Length; g++)
        {
            //With 50% chance, set gene to be either from first or second parent
            if (Random.Range(0f, 1f) > 0.5f)
                child.genes[g] = g2.genes[g].Clone();
        }

        //the generation of the child is increased
        child.generation++;

        //return the child
        return child;
    }

    /// <summary>
    /// Mutate genome by mutating genes
    /// </summary>
    public void Mutate ()
    {
        //Mutate a random gene in the Genome
        genes[Random.Range(0, genes.Length)].Mutate();
        //Set score to be 0
        score = 0;
    }

    /// <summary>
    /// Create a new random Genome
    /// </summary>
    /// <param name="genomeLength">length of Genome</param>
    /// <param name="geneLength">length of Gene</param>
    /// <returns>New genome</returns>
    public static Genome CreateRandom(int genomeLength, int geneLength)
    {
        //Inilialise new genome that is empty
        Genome genome = new Genome();
        //With genes that are empty
        genome.genes = new Gene[genomeLength];
        //For each gene in genome
        for (int i = 0; i < genome.genes.Length; i++)
        {
            //create a random gene at position i
            genome.genes[i] = Gene.CreateRandom(geneLength);
        }
        //Set genome score to be 0
        genome.score = 0;
        //return genome
        return genome;
    }
}

[System.Serializable]
//Gene struct
public struct Gene : IMutable<Gene>
{
    // Sin parameters
    public float [] values;

    public Gene (int size)
    {
        values = new float[size];
    }

    /// <summary>
    /// Clone a gene
    /// </summary>
    /// <returns></returns>
    public Gene Clone()
    {
        Gene clone = new Gene(values.Length);
        for (int i = 0; i < values.Length; i ++)
            clone.values[i] = values[i];
        return clone;
    }

    /// <summary>
    /// Mutate a gene by random variables
    /// </summary>
    public void Mutate()
    {
        int i = Random.Range(0, values.Length-1);
        values[i] += Random.Range(-0.2f, 0.2f);
        values[i] = Mathf.Clamp01(values[i]);
    }
    
    /// <summary>
    /// Creates a random new gene
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
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

        //Map the different values
        min = ControllerSpring.linearInterpolation(0, 1, Evolution.S.minSinusVal, Evolution.S.maxSinusVal, min);
        max = ControllerSpring.linearInterpolation(0, 1, Evolution.S.minSinusVal, Evolution.S.maxSinusVal, max);
        period = ControllerSpring.linearInterpolation(0, 1, Evolution.S.minPeriodSinus, Evolution.S.maxPeriodSinus, period);
        offset = ControllerSpring.linearInterpolation(0, 1, Evolution.S.minPeriodSinus, Evolution.S.maxPeriodSinus, offset);

        //return the sinusiod with the different values
        return sinusoid(time + offset, min, max, period);
    }

    public static float sinusoid(float x, float min, float max, float period)
    {
        return (max - min) / 2 * (1 + Mathf.Sin(x * Mathf.PI * 2 / period)) + min;
    }
}

public delegate float GeneController(Gene gene, float time);