using System.Collections;
using System.Collections.Generic;
using System;
public class Population
{
    public List<Genome> organisms;
    public List<Species> species;
    public int size;
    public int generation;
    public float bestFitness;
    public Genome bestGenome;


    public Population(int size, Genome organism)
    {
        organisms = new List<Genome>();
        organism.AddInitialNodes();
        organism.SetInitialWeights();
        this.size = size;
        generation = 1;
        for(int i = 0; i < size; i++)
        {
            Genome g = organism.Copy();
            g.AddInitialNodes();
            g.SetInitialWeights();

            organisms.Add(g);
        }
    }


    public void NextGeneration()
    {
        RemoveEmptySpecies();
        Speciation();
        SortSpecies();
        RemoveWeak();

        float avgFitnessSum = 0;
        foreach (Species s in species)
            avgFitnessSum += s.avgFitness;

        RemoveStaleSpecies();


        List<Genome> newPopulation = new List<Genome>();

        foreach(Species s in species)
        {
            newPopulation.Add(s.mainGenome.Copy());
            int howManyChildren = (int)Math.Floor(s.avgFitness / avgFitnessSum * size) - 1;

            for (int i = 0; i < howManyChildren; i++)
                newPopulation.Add(s.SpeciesCrossover());
        }

        while(newPopulation.Count < size)
        {
            Species randomSpecie = species[UnityEngine.Random.Range(0, species.Count)];
            newPopulation.Add(randomSpecie.SpeciesCrossover());
        }

        organisms = new List<Genome>(newPopulation);
        generation++;

    }

    #region Aux Functions

    private void Speciation()
    {
        foreach (Species s in species)
            s.organisms.Clear();

        foreach(Genome g in organisms)
        {
            bool newSpeciesFound = true;
            foreach(Species s in species)
            {
                if (s.SameSpecies(g))
                {
                    newSpeciesFound = false;
                    s.organisms.Add(g);
                    break;
                }
            }
            if (newSpeciesFound)
                species.Add(new Species(g));
        }

    }

    private void SortSpecies()
    {
        foreach (Species s in species)
            s.SortOrganisms();

        //Sort by best genome fitness, descending order
        species.Sort((x, y) => y.mainGenome.fitness.CompareTo(x.mainGenome.fitness));
        bestFitness = species[0].mainGenome.fitness;
        bestGenome = species[0].mainGenome;
    }

    private void RemoveEmptySpecies()
    {
        species.RemoveAll(s => s.organisms.Count == 0);
    }

    private void RemoveStaleSpecies()
    {
        List<Species> cleanSpecies = new List<Species>();

        foreach(Species s in species)
        {
            if (s.staleness < Config.SPECIES_STALENESS)
                cleanSpecies.Add(s);
        }
        species = new List<Species>(cleanSpecies);
    }

    private void RemoveWeak()
    {
        foreach(Species s in species)
        {
            s.KillBottom();
            s.FitnessSharing();
            s.AverageFitness();
        }
    }

    #endregion

}
