using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organism : MonoBehaviour
{

    public int nInputs;
    public int nOutputs;
    public Genome genome;
    public float fitness = 0;
    public List<double> inputValues;
    public bool isDead = false;


    public Organism(int nInputs, int nOutputs)
    {
        genome = new Genome(nInputs, nOutputs);
        
    }

    /*
    public void ReceiveData(List<double> data)
    {
        inputValues = data;
    }

    public List<double> ProcessData()
    {
        return genome.FeedForward(inputValues);
    }

    public void Crossover(Organism parent2)
    {
 
    }
    */
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
