using System;
using System.Collections.Generic;

public class Node
{
    public int layer;
    public int id;
    public double outputValue;
    public double inputValue;
    public List<Connection> outConnections;

    public Node(int layer, int id)
    {
        this.layer     = layer;
        this.id        = id;
        outputValue    = 0;
        inputValue     = 0;
        outConnections = new List<Connection>();

    }


    public void Activate()
    {
        if (layer != 0)
        {
            outputValue = Sigmoid(inputValue);
        }

        foreach (Connection conn in outConnections) {
            if (conn.expressed)
                conn.outputNode.inputValue += conn.w * outputValue;
        }
    }

    private double Sigmoid(double x)
    {
        return 1 / (1 + Math.Pow(Math.E, -4.9 * x));
    }


    public Node Copy()
    {
        return new Node(layer, id);
    }
}
