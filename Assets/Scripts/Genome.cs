using System.Collections;
using System.Collections.Generic;


public class Genome
{

    public int nInputs;
    public int nOutputs;
    public int layers;
    public Node biasNode;

    public List<Connection> connections;
    public List<Node> nodes;


    public Genome(int nInputs, int nOutputs)
    {
        this.nInputs  = nInputs;
        this.nOutputs = nOutputs;
        layers        = 2;
        nodes         = new List<Node>();
        connections   = new List<Connection>();

    }

    public void AddInitialNodes()
    {
        biasNode = new Node(0, 1);
        nodes.Add(biasNode);

        for (int i = 0; i < nInputs; i++)
            nodes.Add(new Node(0, i + 2));

        for (int i = 0; i < nOutputs; i++)
            nodes.Add(new Node(1, i + nInputs + 2));

    }

    public void SetInitialWeights()
    {
        System.Random rnd = new System.Random();
        foreach (Connection conn in connections)
        {
            conn.w = rnd.NextDouble();
            if (rnd.Next(0, 2) == 0)
                conn.w *= -1;
        }
    }
}


