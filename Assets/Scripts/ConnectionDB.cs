using System.Collections;
using System.Collections.Generic;

public class ConnectionDB
{

    List<Connection> connectionList = new List<Connection>();

    public bool CheckIfConnectionExists(Connection conn)
    {
        foreach(Connection c in connectionList)
        {
            if (c.inputNode.id == conn.inputNode.id && c.outputNode.id == conn.outputNode.id)
                return true;
        }
    
        return false;
    }

    public bool AddConnection(Connection conn)
    {
        if (!CheckIfConnectionExists(conn))
        {
            connectionList.Add(conn);
            return true;
        }
        else return false;
    }

}
