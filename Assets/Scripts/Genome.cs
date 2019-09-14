using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{

    public int nInputs;
    public int nOutputs;
    public int layers;
    public Node biasNode;
    private List<Node> network;
    public List<Connection> connections;
    public List<Node> nodes;

    public Genome(int nInputs, int nOutputs)
    {
        this.nInputs  = nInputs;
        this.nOutputs = nOutputs;
        layers        = 2;
        nodes         = new List<Node>();
        connections   = new List<Connection>();
        network       = new List<Node>(); 

    }
    #region Start Functions

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
    #endregion

    #region Main Functions

    public void ConnectionMutation()
    {
        if (IsFullyConnected())
            return;

        Node node1;
        Node node2;
        do
        {
            node1 = nodes[Random.Range(0, nodes.Count)];
            node2 = nodes[Random.Range(0, nodes.Count)];
        } while (InvalidNewConnection(node1, node2));

        System.Random rnd = new System.Random();
        double w = rnd.NextDouble();
        if (rnd.Next(0, 2) == 0)
            w *= -1;



        Connection newConnection;

        if (node1.layer > node2.layer)
            newConnection = new Connection(node2, node1, w, true);
        else
        {
            newConnection = new Connection(node2, node1, w, true);
        }
        AddConnectionToDB(newConnection);


    }

    public void NodeMutation()
    {
        if (connections.Count == 0)
        {
            ConnectionMutation();
            return;
        }

        Connection connection = connections[Random.Range(0, connections.Count)];


        Node inputNode  = connection.inputNode;
        Node outputNode = connection.outputNode;

        connection.expressed = false;

        int newNodeLayer = connection.inputNode.layer + 1;

        Node newNode = new Node(newNodeLayer, nodes.Count + 1);

        Connection inToNewConn  = new Connection(inputNode, newNode, 1, true);
        Connection newToOutConn = new Connection(newNode, outputNode, connection.w, true);
        //Connection biasConn     = new Connection(nodes[0], newNode, 0, true);

        AddConnectionToDB(inToNewConn);
        AddConnectionToDB(newToOutConn);

        if (newNode.layer == outputNode.layer)
        {
            foreach(Node n in nodes)
            {
                if (n.layer >= newNode.layer)
                    n.layer++;
            }
            layers++;
        }
        nodes.Add(newNode);


    }

    public void Crossover(Genome parent2)
    {
        Genome child = new Genome(nInputs, nOutputs);


        //Child Nodes
        foreach (Node n in nodes)
            child.nodes.Add(n.Copy());

        child.biasNode = child.nodes[0];
        child.layers = layers;

        //Child Connections 
        List<int> parent2InnovationNumbers = new List<int>();
        Node NodeIn;
        Node NodeOut;
        Connection newConnection;

        foreach (Connection c in parent2.connections)
            parent2InnovationNumbers.Add(c.innovationNumber);

        foreach(Connection c in connections)
        {
            NodeIn = nodes.Find(n => n.id == c.inputNode.id);
            NodeOut = nodes.Find(n => n.id == c.outputNode.id);
            // If both parents have connections with the same In. number
            if (parent2InnovationNumbers.Contains(c.innovationNumber))
            {
                // 50% chance
                int rand = Random.Range(0, 2);

                // Choose randomly which connection will be passed to the child
                if (rand == 0)
                {

                    newConnection = new Connection(NodeIn, NodeOut, c.w, c.expressed);
                    newConnection.innovationNumber = c.innovationNumber;
                }
                else
                {
                    Connection conn2 = parent2.connections.Find(c2 => c2.innovationNumber == c.innovationNumber);
                    newConnection = new Connection(NodeIn, NodeOut, conn2.w, conn2.expressed);
                    newConnection.innovationNumber = conn2.innovationNumber;
                }
                child.connections.Add(newConnection);
            }
            else //Disjoint genes. Fittest parent
            {
                newConnection = new Connection(NodeIn, NodeOut, c.w, c.expressed);
                newConnection.innovationNumber = c.innovationNumber;
            }
        }
    }


    public void Mutation()
    {
        float rand = Random.Range(0, 1);

        if (rand < Config.MUTATE_W_PROB)
        {
            foreach (Connection c in connections)
                c.MutateW();
        }

        if (rand < Config.MUTATE_CONN_PROB)
            ConnectionMutation();

        if (rand < Config.MUTATE_NODE_PROB)
            NodeMutation();
    }



    #endregion

    #region Aux Functions
    


    private bool IsFullyConnected()
    {
        List<int> nodesInLayer = new List<int>();
        for (int layerIndex = 0; layerIndex < layers; layerIndex++)
        {
            int nodeCounter = 0;
            foreach(Node n in nodes)
            {
                if (n.layer == layerIndex)
                    nodeCounter++;
            }
            nodesInLayer.Add(nodeCounter);
        }

        int maxConnections = 0;

        for (int i = 0; i < layers; i++)
        {
            int nodesForward = 0;
            for (int j = i + 1; j < layers; j++)
                nodesForward += nodesInLayer[j];

            maxConnections += nodesInLayer[i] * nodesForward;
        }

        if (maxConnections <= connections.Count)
            return true;
        return false;
    }


    private bool InvalidNewConnection(Node node1, Node node2)
    {
        if (node1.layer == node2.layer)
            return true;

        foreach(Connection c in connections)
        {
            if ((c.inputNode.id == node1.id && c.outputNode.id == node2.id) || 
                c.outputNode.id == node1.id && c.inputNode.id == node2.id)
                return true;
        }

        return false;
    }


    private void AddConnectionToDB(Connection conn)
    {
        int innovationNumber = ConnectionDB.GetCurrentInnovationNumber(conn);
        conn.innovationNumber = innovationNumber;
        connections.Add(conn);
    }

    private void NetworkSetup()
    {
        RefreshConnections();
        network.Clear();

        for (int i = 0; i < layers; i++)
            foreach (Node n in nodes)
                if (n.layer == i)
                    network.Add(n);

    }

    private void RefreshConnections()
    {
        foreach (Node n in nodes)
            n.outConnections.Clear();

        foreach (Node n in nodes)
            foreach (Connection c in connections)
                if (n.id == c.inputNode.id)
                    n.outConnections.Add(c);

    }
    private List<double> FeedForward(List<double> inputValues)
    {
        NetworkSetup();
        nodes[0].outputValue = 1; //Bias Node

        for (int i = 0; i < nInputs; i++)
            nodes[i + 1].outputValue = inputValues[i];

        foreach (Node n in network)
            n.Activate();

        List<double> outputValues = new List<double>();

        for (int i = 0; i < nOutputs; i++)
            outputValues.Add(nodes[nInputs + i + 1].outputValue);

        foreach (Node n in nodes)
        {
            n.inputValue = 0;
            n.outputValue = 0;
        }

        return outputValues;

    }

    #endregion

}


