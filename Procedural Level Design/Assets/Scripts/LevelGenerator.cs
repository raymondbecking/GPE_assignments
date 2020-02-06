using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public enum TileType
{
    Empty = 0,
    Player,
    Enemy,
    Wall,
    Door,
    Key,
    Dagger,
    End
}

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] tiles;

    Node node;
    public List<Node> nodeList;

    int width = 128;
    int height = 128;

    protected void Start()
    {
        TileType[,] grid = new TileType[height, width];

        //List to save nodes, list is used because there are n amount of nodes
        nodeList = new List<Node>();
        CreateNodes(width, height, 128);


        //FillBlock(grid, 0, 0, width, height, TileType.Wall);
        //FillBlock(grid, 26, 26, 12, 12, TileType.Empty);
        //FillBlock(grid, 32, 28, 1, 1, TileType.Player);
        //FillBlock(grid, 30, 30, 1, 1, TileType.Dagger);
        //FillBlock(grid, 34, 30, 1, 1, TileType.Key);
        //FillBlock(grid, 32, 32, 1, 1, TileType.Door);
        //FillBlock(grid, 32, 36, 1, 1, TileType.Enemy);
        //FillBlock(grid, 32, 34, 1, 1, TileType.End);

        Debugger.instance.AddLabel(32, 26, "Room 1");

        //use 2d array (i.e. for using cellular automata)
        CreateTilesFromArray(grid);
    }

    //fill part of array with tiles
    private void FillBlock(TileType[,] grid, int x, int y, int width, int height, TileType fillType)
    {
        for (int tileY = 0; tileY < height; tileY++)
        {
            for (int tileX = 0; tileX < width; tileX++)
            {
                grid[tileY + y, tileX + x] = fillType;
            }
        }
    }

    //use array to create tiles
    private void CreateTilesFromArray(TileType[,] grid)
    {
        int height = grid.GetLength(0);
        int width = grid.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileType tile = grid[y, x];
                if (tile != TileType.Empty)
                {
                    CreateTile(x, y, tile);
                }
            }
        }
    }

    //create a single tile
    private GameObject CreateTile(int x, int y, TileType type)
    {
        int tileID = ((int)type) - 1;
        if (tileID >= 0 && tileID < tiles.Length)
        {
            GameObject tilePrefab = tiles[tileID];
            if (tilePrefab != null)
            {
                GameObject newTile = GameObject.Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                newTile.transform.SetParent(transform);
                return newTile;
            }

        }
        else
        {
            Debug.LogError("Invalid tile type selected");
        }

        return null;
    }

    private void CreateNodes(int width, int height, int maxSplits)
    {
        //Create root node
        node = new Node(0, 0, width, height);
        nodeList.Add(node);
        //Split from root node
        for (int i = 0; i < maxSplits; i++)
        {
            var nodeTuple = nodeList[i].SplitNode();
            nodeList.Add(nodeTuple.Item1);
            nodeList.Add(nodeTuple.Item2);
        }
        ////Split node A
        //nodeList.Add(nodeList[1].SplitNode().Item1);
        //nodeList.Add(nodeList[1].SplitNode().Item2);

        ////Split node B
        //nodeList.Add(nodeList[2].SplitNode().Item1);
        //nodeList.Add(nodeList[2].SplitNode().Item2);

        //node.SplitNode(5);//Split root node with amount of splits

        //Debug.Log("NodeB Split node A");
        //Debug.Log(newNodes2.Item1.x);
        //Debug.Log(newNodes2.Item1.width);
        //Debug.Log("NodeB Split node B");
        //Debug.Log(newNodes2.Item2.x);
        //Debug.Log(newNodes2.Item2.width);
    }

    void OnDrawGizmos()
    {
        //Draw a yellow line around the edges of the rectangle
        foreach (Node node in nodeList) {
            node.DrawNode();
                }

        //if (node.childA != null)
        //{
        //    Gizmos.color = Color.yellow;
        //    //Create gizmo square around the edges of the node
        //    Gizmos.DrawLine(new Vector3(node.childA.x, node.childA.y), new Vector3(node.childA.width, node.childA.y));
        //    Gizmos.DrawLine(new Vector3(node.childA.x, node.childA.height), new Vector3(node.childA.width, node.childA.height));
        //    Gizmos.DrawLine(new Vector3(node.childA.x, node.childA.y), new Vector3(node.childA.x, node.childA.height));
        //    Gizmos.DrawLine(new Vector3(node.childA.width, node.childA.height), new Vector3(node.childA.width, node.childA.y));
        //    if (node.childA.childA != null)
        //    {
        //        Gizmos.color = Color.yellow;
        //        //Create gizmo square around the edges of the node
        //        Gizmos.DrawLine(new Vector3(node.childA.childA.x, node.childA.childA.y), new Vector3(node.childA.childA.width, node.childA.childA.y));
        //        Gizmos.DrawLine(new Vector3(node.childA.childA.x, node.childA.childA.height), new Vector3(node.childA.childA.width, node.childA.childA.height));
        //        Gizmos.DrawLine(new Vector3(node.childA.childA.x, node.childA.childA.y), new Vector3(node.childA.childA.x, node.childA.childA.height));
        //        Gizmos.DrawLine(new Vector3(node.childA.childA.width, node.childA.childA.height), new Vector3(node.childA.childA.width, node.childA.childA.y));
        //    }
        //    if (node.childA.childB != null)
        //    {
        //        Gizmos.color = Color.red;
        //        //Create gizmo square around the edges of the node
        //        Gizmos.DrawLine(new Vector3(node.childA.childB.x, node.childA.childB.y), new Vector3(node.childA.childB.width, node.childA.childB.y));
        //        Gizmos.DrawLine(new Vector3(node.childA.childB.x, node.childA.childB.height), new Vector3(node.childA.childB.width, node.childA.childB.height));
        //        Gizmos.DrawLine(new Vector3(node.childA.childB.x, node.childA.childB.y), new Vector3(node.childA.childB.x, node.childA.childB.height));
        //        Gizmos.DrawLine(new Vector3(node.childA.childB.width, node.childA.childB.height), new Vector3(node.childA.childB.width, node.childA.childB.y));
        //    }
        //}
        //if (node.childB != null)
        //{
        //    Gizmos.color = Color.red;
        //    //Create gizmo square around the edges of the node
        //    Gizmos.DrawLine(new Vector3(node.childB.x, node.childB.y), new Vector3(node.childB.width, node.childB.y));
        //    Gizmos.DrawLine(new Vector3(node.childB.x, node.childB.height), new Vector3(node.childB.width, node.childB.height));
        //    Gizmos.DrawLine(new Vector3(node.childB.x, node.childB.y), new Vector3(node.childB.x, node.childB.height));
        //    Gizmos.DrawLine(new Vector3(node.childB.width, node.childB.height), new Vector3(node.childB.width, node.childB.y));
        //}
    }


}
