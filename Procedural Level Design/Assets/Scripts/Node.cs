using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node childA, childB;
    public float x, y, width, height;
    private float minSize = 9f;
    private float maxSize;

    public Node(float nX, float nY, float nWidth, float nHeight)
    {
        this.width = nWidth;
        this.height = nHeight;
        this.x = nX;
        this.y = nY;
    }

    public bool SplitNode()
    {
        //Prevent splitting a node that has already been split
        if (this.childA != null || this.childB != null)
        {
            return false;
        }

        //Randomly split height or width
        bool splitWidth = (Random.value > 0.5f);
        if (splitWidth)
        {
            maxSize = width - minSize;
        }
        else
        {
            maxSize = height - minSize;
        }
        //Prevent splitting from creating nodes that are too small (also causes more even splitting)
        if (maxSize < minSize)
        {
            return false;
        }
        //Make sure splitting is even
        if (width > height && percentage(width, height) > 25) splitWidth = true;
        else if (height > width && percentage(height, width) > 25) splitWidth = false;

        
        //Random split location
        float nodeSeperation = Random.Range(minSize, maxSize);

        if (splitWidth)
        {
            //Create child node A from origin to split (width)
            childA = new Node(x, y, nodeSeperation, height);
            //Create child node B from split to end (width)
            childB = new Node((x + nodeSeperation), y, (width - nodeSeperation), height);
        }
        else
        {
            //Create child node A from origin to split (height)
            childA = new Node(x, y, width, nodeSeperation);
            //Create child node B from split to end (height)
            childB = new Node(x, (y + nodeSeperation), width, (height - nodeSeperation));
        }
        return true;

    }

    public void DrawNode()
    {
        Gizmos.color = new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f));
        //Draw line around the edges of the node
        Gizmos.DrawWireCube(new Vector3(((this.width / 2) + this.x), ((this.height / 2) + this.y)), new Vector3((this.width), (this.height)));
    }

    public float percentage(float val1, float val2)
    {
        return ((val1 / val2 * 100) - 100);
    }

}
