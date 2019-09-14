using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{

    #region Genome

    public static float MUTATE_W_PROB    = 0.9f;
    public static float MUTATE_CONN_PROB = 0.05f;
    public static float MUTATE_NODE_PROB = 0.03f;

    #endregion

    #region Species
    public static float CROSSOVER_PROB = 0.25f;

    public static float EXCESS_COEFF = 1;
    public static float WEIGHT_DIFF_COEFF = 2;
    public static float COMPATIBILITY_THRESHOLD = 3;
    public static int   SPECIES_STALENESS = 10; 


    #endregion

    #region Population
    #endregion

}
