
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Species
{
    public List<Genome> organisms;
    public int staleness;
    public Genome mainGenome;
    public float avgFitness;
    public float bestFitness;

    #region Main functions
    public Species(Genome genome)
    {
        staleness = Config.SPECIES_STALENESS;
        organisms = new List<Genome>();
        organisms.Add(genome);
        mainGenome = genome;
        bestFitness = genome.fitness;
        avgFitness = 0;
    }


    public bool SameSpecies(Genome genomeB)
    {
        int excessAndDisjoint = GetExcessAndDisjoint(mainGenome, genomeB);
        double avgWeightDiff  = GetAverageWeightDiff(mainGenome, genomeB);

        int N;

        if (mainGenome.connections.Count <= 20 && genomeB.connections.Count <= 20)
            N = 1;
        else
        {
            if (mainGenome.connections.Count > genomeB.connections.Count)
                N = mainGenome.connections.Count;
            else
                N = genomeB.connections.Count;
        }

        double compatibility = (Config.EXCESS_COEFF * excessAndDisjoint / N) + (Config.WEIGHT_DIFF_COEFF * avgWeightDiff);

        return Config.COMPATIBILITY_THRESHOLD > compatibility;


    }

    public Genome SpeciesCrossover()
    {
        float rand = Random.Range(0, 1);

        Genome child;
        if (rand < Config.CROSSOVER_PROB)
            child = SelectOrganism().Copy();
        else
        {
            // Crossover

            Genome genomeA = SelectOrganism();
            Genome genomeB = SelectOrganism();

            if (genomeA.fitness >= genomeB.fitness)
                child = genomeA.Crossover(genomeB);
            else child = genomeB.Crossover(genomeA);
        }

        child.Mutation();

        return child;

    }

    public Genome SelectOrganism()
    {
        if (organisms.Count == 1)
            return organisms[0];

        float fitnessSum = organisms.Sum(g => g.fitness);

        float rand = Random.Range(0, fitnessSum);

        float intervalSum = 0;

        foreach(Genome g in organisms)
        {
            intervalSum += g.fitness;

            if (intervalSum >= rand)
                return g;
        }
        return null;
    }

    public void SortOrganisms()
    {
        //Sort by fitness, descending order
        organisms.Sort((x, y) => y.fitness.CompareTo(x.fitness));

        if(organisms[0].fitness > bestFitness)
        {
            staleness = 0;
            bestFitness = organisms[0].fitness;
            mainGenome = organisms[0];
        }
        else
        {
            staleness++;
        }
    }

    public void AverageFitness()
    {
        float sum = organisms.Sum(g => g.fitness) / organisms.Count;

    }

    public void KillBottom()
    {
 
        if (organisms.Count > 1)
            organisms = organisms.Take((int)Mathf.Round(organisms.Count / 2)).ToList();
    }

    public void FitnessSharing()
    {
        foreach (Genome g in organisms)
            g.fitness /= organisms.Count;
    }
    #endregion

    #region Aux Functions

    private int GetExcessAndDisjoint(Genome genomeA, Genome genomeB)
    {
        int matching = 0;

        foreach(Connection c1 in genomeA.connections)
            foreach (Connection c2 in genomeB.connections)
                if(c1.innovationNumber == c2.innovationNumber)
                {
                    matching++;
                    break;
                }

        return genomeA.connections.Count + genomeB.connections.Count - (2 * matching);
    }

    private double GetAverageWeightDiff(Genome genomeA, Genome genomeB)
    {
        if (genomeA.connections.Count == 0 || genomeB.connections.Count == 0)
            return 0;

        int matching = 0;
        double sum = 0;

        foreach (Connection c1 in genomeA.connections)
            foreach (Connection c2 in genomeB.connections)
                if (c1.innovationNumber == c2.innovationNumber)
                {
                    matching++;
                    sum += System.Math.Abs(c1.w - c2.w);
                    break;
                }

        if (matching == 0)
            return 0;

        return sum / matching;

    }




    #endregion
}
