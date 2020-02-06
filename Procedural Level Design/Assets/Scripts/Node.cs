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

    public void SplitNode()
    {//Give level of how many splits are needed
        //Temporary hardcoded split
        int nodeSeperation = width / 2;
        Debug.Log(nodeSeperation);
        //Create child node from origin to split
        childA = new Node(x, y, nodeSeperation, height);
        //Create child node from split to end
        childB = new Node((x + nodeSeperation), y, width, height);
        

    }



}
