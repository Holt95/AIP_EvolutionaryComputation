using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Evolution : MonoBehaviour
{

    public static Evolution S;

    public int cycles = 10; //Number of cycles we run 
    public int keepTop = 5; // keeps the 5 tops genomes
    public int mutationsPerGenome = 25; // 25 mutations for each of the best genomes
    public int testsPerMutation = 5;    // It runs the same genome multiple times to get a reliable score

    public int minMutations = 1;
    public int maxMutations = 5;

    public float yGap = 1; // length spacing on y-axis
    public float zGap = 1f; // length spacing on z-axis
    public float simulationTime = 10; // time in secons simulation is run

    public int sexed = 1; // How many sexual reproductions to have in each generation?

    public int genomeLength = 2;
    public int geneLength = 4;

    public GameObject prefab;


    public EvolutionHistory history;

    public GameObject container;

    public bool start = true;
    
   
    private List<List<Environment>> environments = new List<List<Environment>>();

    public new Camera camera; // the camera in the scene
    public bool moveCamera = true; // if we should move the camera (to the best score)

    public float minSin = -1;
    public float maxSin = +1;

    public float minP = 0.1f;
    public float maxP = 5f;

    

    // Use this for initialization
    void Start()
    {
        S = this;

        if (!start)
            return;

        if (history.hasToBeInitialised)
        {

            history.Clear();

            for (int i = 0; i < keepTop; i++)
                history.bestGenomes.Add(Genome.CreateRandom(genomeLength, geneLength));
            history.hasToBeInitialised = false;

        }

        environments.Clear();

        history.CalculateMinMaxScore();
        StartCoroutine(Simulation());
    }

    // Update is called once per frame
    void Update()
    {
        if (!start)
            return;

        // Checks the one that has the highest score
        if (isRunning && moveCamera)
        {
            Environment best = FindEnvironmentWithHighestScore();
            if (best != null)
                camera.transform.position = best.evolvable.transform.position - new Vector3(-6, 0, +10); //Camera move to best performing mutation
        }
    }

    public void CreateCreatures(List<Genome> genomes)
    {
        int i = 0;
        foreach (Genome genome in genomes)
        {
            Genome g;
            //Environment environment;

            // Mutated genome
            for (int c = 0; c < mutationsPerGenome; c++)
            {
                g = genome.Clone();
                g.generation++;
                int M = Random.Range(minMutations, maxMutations + 1);
                for (int m = 0; m < M; m++)
                    g.Mutate();
                CreateEnvironments(g, i);
                i++;
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
        for (int t = 0; t < testsPerMutation; t++)
        {
            CreateEnvironment(genome.Clone(), i, t);
        }
        return tests;
    }


    public Environment CreateEnvironment(Genome genome, int i, int t)
    {
        // Instantiate the environment
        Vector3 position = Vector3.zero + yGap * Vector3.down * (i * testsPerMutation + t) - zGap * Vector3.back * (i * testsPerMutation + t);
        Environment environment = (Instantiate(prefab, position, Quaternion.identity) as GameObject).GetComponent<Environment>();
        //environments.Add(environment);
        environments[i].Add(environment);

        environment.evolvable.genome = genome;
        environment.name = "Environment (Variant-Test: " + (i + 1) + "-" + (t + 1) + ")";
        environment.transform.parent = container.transform;

        return environment;
    }

    public void EvaluateBestCreature()
    {
        List<Genome> scores = new List<Genome>();
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

    public void DestroyCreatures()
    {
        foreach (List<Environment> tests in environments)
            foreach (Environment environment in tests)
                Destroy(environment.gameObject);

        environments.Clear();
    }

    public IEnumerator Simulation()
    {
        for (int i = 0; i < cycles; i++)
        {
            CreateCreatures(history.bestGenomes);
            SetSimulation(true);

            yield return new WaitForSeconds(simulationTime);

            SetSimulation(false);
            EvaluateBestCreature();

            CopulateBestCreatures();

            DestroyCreatures();
            Debug.Log("Best score: " + history.bestScore);

            history.generation++;
            yield return new WaitForSeconds(1);
        }
        yield return null;
    }




    public void CopulateBestCreatures ()
    {
        List<Genome> genomes = new List<Genome>();
        for (int i = 0; i < sexed; i ++)
        {
            Genome g1 = history.bestGenomes[Random.Range(0, history.bestGenomes.Count - 1)];
            Genome g2 = history.bestGenomes[Random.Range(0, history.bestGenomes.Count - 1)];

            Genome g3 = g1.Clone();

            genomes.Add(Genome.Copulate(g1, g2));
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
    private bool isRunning;
    public static float startTime = 0;

    public Environment FindEnvironmentWithHighestScore()
    {
        float bestScore = 0;
        Environment bestEnvironment = null;
        foreach (List<Environment> tests in environments)
            foreach (Environment environment in tests)
            {
                float score = environment.evolvable.GetScore();
                if (score > bestScore)
                {
                    bestScore = score;
                    bestEnvironment = environment;
                }
            }

        return bestEnvironment;
    }

    public void OnDrawGizmosSelected()
    {
        Vector3 size = new Vector2(20,10);
        Vector3 border = new Vector2(2, 2);

        Vector3 prev = border;

        Vector3 prev_best = border;
        float bestSoFar = 0;


        

        for (int i = 0; i < history.generations.Count; i ++)
        {
            float x = Controller.linearInterpolation
                   (
                       0, history.generations.Count,
                       border.x, border.x + size.x,
                       i
                   );


            bestSoFar = Mathf.Max(bestSoFar, history.generations[i].score);
            Vector3 best = new Vector3
               (
                  x,
                    Controller.linearInterpolation
                   (
                       history.minScore, history.maxScore,
                       border.y, border.y + size.y,
                       bestSoFar
                   )
               );

            Gizmos.color = Color.red;
            Gizmos.DrawLine(prev_best, best);


            Vector3 next = new Vector3
                (
                   x,
                    Controller.linearInterpolation
                    (
                        history.minScore, history.maxScore,
                        border.y, border.y + size.y,
                        history.generations[i].score
                    )
                );

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(prev, next);


            



            prev = next;
            prev_best = best;

            
        }





        // Draw all scores
        foreach (List<Environment> list in environments)
        {
            foreach (Environment environment in list)
            {
                float x_start = Controller.linearInterpolation
                    (
                        history.minScore, history.maxScore,
                        0, size.x,
                        environment.evolvable.genome.score
                    );

                Gizmos.color = Color.Lerp(Color.red, Color.green,

                        Controller.linearInterpolation
                        (
                            history.minScore, history.maxScore,
                            0, 1,
                            environment.evolvable.genome.score
                        )
                        );
                Gizmos.DrawLine
                    (
                        environment.transform.position + new Vector3(x_start, -yGap/2),
                        environment.transform.position + new Vector3(x_start, +yGap / 2)
                    );
            }
        }
    }
}