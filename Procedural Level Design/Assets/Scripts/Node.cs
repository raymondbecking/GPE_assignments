using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node childA, childB;
    public int x, y, width, height;
    private int minSize = 9;
    private int maxSize;

    public Rect room;
    public int roomSpacing;

    public Node(int nX, int nY, int nWidth, int nHeight)
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
        int nodeSeperation = Random.Range(minSize, maxSize);

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

    public void CreateRoom()
    {
        if (childA == null && childB == null)
        {
            int widthSize = Random.Range(width / 6 + roomSpacing, width - roomSpacing);
            int heightSize = Random.Range(height / 6 + roomSpacing, height - roomSpacing);
            int posX = Random.Range(1, width - widthSize - 1);
            int posY = Random.Range(1, height - heightSize - 1);
            room = new Rect(x + posX, y + posY, widthSize, heightSize);
        }
        else
        {
            if (childA != null)
            {
                childA.CreateRoom();
            }
            if (childB != null)
            {
                childB.CreateRoom();
            }

        }
    }

    public Rect GetRoom()
    {
        if (room != null)
        {
            return room;
        }
        else
        {
            Rect roomA = new Rect(0, 0, 0, 0);
            Rect roomB = new Rect(0, 0, 0, 0);
            if (childA != null)
            {
                roomA = childA.GetRoom();
            }
            if (childB != null)
            {
                roomB = childB.GetRoom();
            }
            if (roomA == null && roomB == null)
            {
                return new Rect(0,0,0,0);
            }
        }

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
