using System.Collections;
using System.Collections.Generic;

public static class ConnectionDB
{

    static List<Connection> connectionList = new List<Connection>();

    public static bool CheckIfConnectionExists(Connection conn)
    {
        foreach(Connection c in connectionList)
        {
            if (c.inputNode.id == conn.inputNode.id && c.outputNode.id == conn.outputNode.id)
                return true;
        }
    
        return false;
    }

    public static bool AddConnection(Connection conn)
    {
        if (!CheckIfConnectionExists(conn))
        {
            connectionList.Add(conn);
            return true;
        }
        else return false;
    }

    public static int GetCurrentInnovationNumber(Connection conn)
    {
        foreach (Connection c in connectionList)
        {
            if (c.inputNode.id == conn.inputNode.id && c.outputNode.id == conn.outputNode.id)
            {
                return c.innovationNumber;
            }

        }
        conn.innovationNumber = connectionList.Count;
        connectionList.Add(conn);
        return conn.innovationNumber;
    }
}
