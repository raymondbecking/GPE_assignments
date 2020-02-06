using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node childA;
    public Node childB;
    public int x, y, width, height;

    public Node(int nX, int nY, int nWidth, int nHeight)
    {
        this.width = nWidth;
        this.height = nHeight;
        this.x = nX;
        this.y = nY;
    }

    public (Node, Node) SplitNode()
    {//Give level of how many splits are needed     
        //Temporary hardcoded split
        int nodeSeperation = width / 2;
        //Create child node from origin to split
        childA = new Node(x, y, nodeSeperation, height);
        //Create child node from split to end
        childB = new Node((x + nodeSeperation), y, width - nodeSeperation, height);
        return (childA, childB);

    }

    public void DrawNode()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(this.x, this.y), new Vector3(this.width, this.y));
        Gizmos.DrawLine(new Vector3(this.x, this.height), new Vector3(this.width, this.height));
        Gizmos.DrawLine(new Vector3(this.x, this.y), new Vector3(this.x, this.height));
        Gizmos.DrawLine(new Vector3(this.width, this.height), new Vector3(this.width, this.y));
    }



}
