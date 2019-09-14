using System;
using System.Collections;
using System.Collections.Generic;

public class Species
{
    public List<Genome> organisms;
    public int staleness;
    public Genome mainGenome;


    #region Main functions
    public Species(Genome genome)
    {
        staleness = Config.SPECIES_STALENESS;
        organisms = new List<Genome>();
        organisms.Add(genome);
        mainGenome = genome;
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
                    sum += Math.Abs(c1.w - c2.w);
                    break;
                }

        if (matching == 0)
            return 0;

        return sum / matching;

    }

    #endregion
}
