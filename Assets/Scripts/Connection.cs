using UnityEngine;

public class Connection
{

    public bool expressed;
    public Node inputNode;
    public Node outputNode;
    public double w;
    public int innovationNumber;

    public Connection()
    {
        expressed = true;
    }
    public void MutateW()
    {
        System.Random rnd = new System.Random();
        int number = rnd.Next(100);

        if (number < 90)
        {
            w += rnd.Next(-5, 5) / 100;
        }
        else
        {
            w = rnd.NextDouble();
            if (rnd.Next(0, 2) == 0)
                w *= -1;
            
        }

        if (w > 1)
            w = 1;
        else if (w < -1)
            w = -1;

    }

}
