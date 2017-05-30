using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Evolution : MonoBehaviour
{

    public static Evolution S;

    public int cycles; //Number of cycles we run 
    public int keepTop; //Keeps the top genomes
    public int mutationsPerGenome; //Mutations for each of the best genomes
    
    public int minMutations = 1; //Minimum amount of mutations a genome goes through
    public int maxMutations = 5; //Maximum amount of...

    public float simulationTime = 10; // time in seconds simulation is run

    public int sexed = 1; // How many sexual reproductions to have in each generation?

    public int genomeLength = 2;
    public int geneLength = 4;

    public GameObject prefab;
    public GameObject container;

    public EvolutionHistory history;

    private List<List<Environment>> environments = new List<List<Environment>>();

    public float minSinusVal = -1;
    public float maxSinusVal = +1;

    public float minPeriodSinus = 0.1f;
    public float maxPeriodSinus = 5f;

    

    // Use this for initialization
    void Start()
    {
        S = this;

        //If true, we clean the history of genomes and start over
        if (history.hasToBeInitialised)
        {
            history.Clear();

            //We add starting genomes created randomly
            for (int i = 0; i < keepTop; i++)
                history.bestGenomes.Add(Genome.CreateRandom(genomeLength, geneLength));
            history.hasToBeInitialised = false; //To avoid making a new swipe
        }

        environments.Clear();

        history.CalculateMinMaxScore();
        StartCoroutine(Simulation());
    }

    void Update()
    {
        if (isRunning)
        {
            FindEnvironmentWithHighestScore();
        }
    }


    public void CreateCreatures(List<Genome> genomes)
    {
        int i = 0; //Number of creeps created
        foreach (Genome genome in genomes)
        {
            Genome g;

            // Mutated genome
            for (int c = 0; c < mutationsPerGenome; c++)
            {
                g = genome.Clone();
                g.generation++;
                int M = Random.Range(minMutations, maxMutations + 1);
                for (int m = 0; m < M; m++)
                    g.Mutate();
                CreateEnvironments(g, i);
                i++; //Number of creeps created increased
            }
            
            // Non mutated genome
            g = genome.Clone();
            g.score = 0;
            CreateEnvironments(g, i);
            i++;
        }
    }

    
    public List<Environment> CreateEnvironments(Genome genome, int i)
    {
        // Variants
        List<Environment> tests = new List<Environment>();
        environments.Add(tests);
        CreateEnvironment(genome.Clone(), i);
        return tests;
    }

    //Return value is an environment, parameters are a genome and an integer/amount of creeps
    public Environment CreateEnvironment(Genome genome, int i)
    {
        // Instantiate the environment
        Vector3 position = new Vector3(0, 3 * i, 0); //Space all creeps on the y-axis in relation to amount
        Environment environment = (Instantiate(prefab, position, Quaternion.identity) as GameObject).GetComponent<Environment>(); //Instantiate a creep prefab as environment 
        environments[i].Add(environment); //Environment added to the list of environments

        environment.evolvable.genome = genome;
        environment.name = "Creep: " + i;
        environment.transform.parent = container.transform;

        return environment;
    }

    /// <summary>
    /// Check which creature is best
    /// </summary>
    public void EvaluateBestCreature()
    {
        //List of genomes
        List<Genome> scores = new List<Genome>();
        //For each environment
        foreach (List<Environment> tests in environments)
        {
            // Average score
            float score = 0;
            foreach (Environment environment in tests)
                score += environment.evolvable.GetScore();
            score /= tests.Count;

            // Put in the scores
            Genome genome = tests[0].evolvable.genome.Clone();
            genome.score = score; // Updates with the average score
            scores.Add(genome);
        }

        //Sort scores
        scores.Sort(
            delegate (Genome a, Genome b)
            {
                return b.score.CompareTo(a.score);  // Larger first
            }
         );


        // Sorts the already existing genomes and extract the best one
        history.bestGenomes.Sort(
            delegate (Genome a, Genome b)
            {
                return b.score.CompareTo(a.score); // Larger first
            }
         );
        Genome oldBest = history.bestGenomes[0].Clone();


        history.bestGenomes.Clear();
        for (int i = 0; i < keepTop; i++)
            history.bestGenomes.Add(scores[i].Clone());

        history.bestScore = history.bestGenomes[0].score;


        history.generations.Add(history.bestGenomes[0].Clone());

        // Add backs the best genome
        history.bestGenomes.Add(oldBest.Clone());

        // Best genome (overall best)
        history.bestGenomes.Add(GetBestGenomeIn(history.generations));

        // Best scores for gizmos draw
        history.CalculateMinMaxScore();
    }
    private Genome GetBestGenomeIn(List<Genome> list)
    {
        Genome best = list[0].Clone();
        float bestScore = best.score;

        foreach (Genome genome in list)
        {
            if (genome.score > bestScore)
            {
                best = genome.Clone();
                bestScore = best.score;
            }
        }
        return best.Clone();
    }

    /// <summary>
    /// Method for removing creatues
    /// </summary>
    public void DestroyCreatures()
    {
        //For each test
        foreach (List<Environment> tests in environments)
            //For each environment
            foreach (Environment environment in tests)
                //Destroy the game object
                Destroy(environment.gameObject);
        //Clear environment list
        environments.Clear();
    }

    /// <summary>
    /// Main loop
    /// </summary>
    /// <returns></returns>
    public IEnumerator Simulation()
    {
        //For each cycle
        for (int i = 0; i < cycles; i++)
        {
            //Create creatures with history as paramters
            CreateCreatures(history.bestGenomes);
            //Set simulation as running
            SetSimulation(true);
            //Wait for specified time
            yield return new WaitForSeconds(simulationTime);

            //Set simulation as not running
            SetSimulation(false);

            //Check which creature was best
            EvaluateBestCreature();

            //Mix best creatures
            MixBestCreatures();

            //Remove all creatures
            DestroyCreatures();
            //Output score to log
            Debug.Log("Best score: " + history.bestScore);
            //Increase generation
            history.generation++;
            //wait 1 second
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }
    
    /// <summary>
    /// Mix together two creatues
    /// </summary>
    public void MixBestCreatures ()
    {
        List<Genome> genomes = new List<Genome>();
        //For how many creatues we want to mix
        for (int i = 0; i < sexed; i ++)
        {
            //Get two random of the best Genomes
            Genome g1 = history.bestGenomes[Random.Range(0, history.bestGenomes.Count - 1)];
            Genome g2 = history.bestGenomes[Random.Range(0, history.bestGenomes.Count - 1)];

            //Add a genome which is a mix of both
            genomes.Add(Genome.Mix(g1, g2));
        }

        history.bestGenomes.AddRange(genomes);
    }

    public void SetSimulation(bool enabled)
    {
        isRunning = enabled;
        startTime = Time.time;
        foreach (List<Environment> tests in environments)
            foreach (Environment environment in tests)
                environment.evolvable.enabled = enabled;
    }

    public Environment FindEnvironmentWithHighestScore()
    {
        float bestScore = 0;
        Environment bestEnvironment = null;
        foreach (List<Environment> tests in environments)
            foreach (Environment environment in tests)
            {
                float score = environment.evolvable.GetScore();
                environment.evolvable.GetComponent<CreatureSimple>().body.GetComponent<SpriteRenderer>().color = Color.grey;
                if (score > bestScore)
                {
                    bestScore = score;
                    bestEnvironment = environment;
                    bestEnvironment.evolvable.GetComponent<CreatureSimple>().body.GetComponent<SpriteRenderer>().color = Color.red;
                   
                } 
            }

        return bestEnvironment;
    }
    private bool isRunning;
    public static float startTime = 0;
}
