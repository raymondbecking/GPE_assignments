using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private Node[] childs;
    private int x, y, width, height;
    public Node(int nX, int nY, int nWidth, int nHeight)
    {
        this.width = nWidth;
        this.height = nHeight;
    }

    public void SplitNode(int level=0) {
        childs[level] = this.width / 2;
}
    
}
