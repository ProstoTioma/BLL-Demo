using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour
{
    // Initiate main variables
    public static GeneticAlgorithm Instance;

    [Header("Settings")]
    public GameObject objectPrefab;
    public int populationSize = 10;
    [Range(0f, 1f)] public float mutationProbability = 0.03f;

    private List<GenerativeObject> population = new List<GenerativeObject>();
    private List<GenerativeObject> clickedObjects = new List<GenerativeObject>();
    private float minDistance = 3f;  // Minimum distance between objects

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Generate first population at the start of the script
    void Start()
    {
        InitializePopulation();
    }

    void InitializePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector2 position = FindValidPosition(population);

            GameObject obj = Instantiate(objectPrefab, position, Quaternion.identity);

            Color objectColor = new Color(
                Random.Range(0.1f, 1.1f),
                Random.Range(0.1f, 1.1f),
                Random.Range(0.1f, 1.1f)
            );

            GenerativeObject generativeObject = obj.GetComponent<GenerativeObject>();
            generativeObject.SetColor(objectColor);

            population.Add(generativeObject);
        }
    }

    // Find the valid position so the circle don't ovelap
    Vector2 FindValidPosition(List<GenerativeObject> population)
    {
        Vector2 position;
        bool positionIsValid;

        do
        {
            position = new Vector2(Random.Range(-9f, 9f), Random.Range(-4f, 4f));
            positionIsValid = true;

            foreach (var obj in population)
            {
                if (Vector2.Distance(obj.transform.position, position) < minDistance)
                {
                    positionIsValid = false;
                    break;
                }
            }
        } while (!positionIsValid);

        return position;
    }

    public void RegisterClick(GenerativeObject clickedObject)
    {
        clickedObjects.Add(clickedObject);
        if (clickedObjects.Count >= populationSize)
        {
            EvolvePopulation();
        }
    }

    // Crossover logic: mix colors of two circles
    public Color MixColors(Color first, Color second)
    {
        float newR = (Random.Range(0, 2) == 0) ? first.r : second.r;
        float newG = (Random.Range(0, 2) == 0) ? first.g : second.g;
        float newB = (Random.Range(0, 2) == 0) ? first.b : second.b;

        if (Random.value < mutationProbability) newR = Random.value;
        if (Random.value < mutationProbability) newG = Random.value;
        if (Random.value < mutationProbability) newB = Random.value;

        return new Color(newR, newG, newB);
    }

    // Evolution logic
    void EvolvePopulation()
    {
        if (clickedObjects.Count < populationSize) return;
        // Sort population based on the click order
        clickedObjects.Sort((a, b) => a.clickOrder.CompareTo(b.clickOrder));
        List<GenerativeObject> newPopulation = new List<GenerativeObject>();

        int startIdx = clickedObjects.Count - 5;

        for (int i = clickedObjects.Count - 1; i >= startIdx; i--)
        {
            for (int j = i - 1; j >= startIdx; j--)
            {
                if (i != j)
                {
                    // Mix the colors of the last 5 clicked circles 
                    Color newColor = MixColors(clickedObjects[i].objectColor, clickedObjects[j].objectColor);
                    Vector2 position = FindValidPosition(newPopulation);

                    GameObject obj = Instantiate(objectPrefab, position, Quaternion.identity);

                    GenerativeObject newObject = obj.GetComponent<GenerativeObject>();
                    newObject.SetColor(newColor);
                    newPopulation.Add(newObject);

                    if (newPopulation.Count >= populationSize)
                    {
                        break;
                    }
                }
            }
            if (newPopulation.Count >= populationSize)
            {
                break;
            }
        }
        // Set new population and reset the counter
        population = newPopulation;
        clickedObjects.Clear();
        GenerativeObject.clickOrderCounter = 0;
    }
}
