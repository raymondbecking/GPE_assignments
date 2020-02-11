using System;
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
    TileType[,] grid;

    Node node;
    //Array to store nodes to loop through
    Node[] nodeArr = new Node[1];

    //Array to store rooms
    Rect[] roomArr = new Rect[1];

    public int width = 128;
    public int height = 128;

    public int maxSplits = 15;
    public int minNodeSize = 20;

    bool hasSplit = true;
    int splitCounter = 0;


    protected void Start()
    {
        grid = new TileType[height, width];
        FillBlock(grid, 0, 0, width, height, TileType.Wall);
        CreateNodes(width, height, maxSplits);
        //Fill room with walls
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

        nodeArr[0] = node;

        //Split from root node
        while (hasSplit)
        {
            hasSplit = false;
            //Control amount of splits
            if (splitCounter < maxSplits)
            {
                foreach (Node n in nodeArr)
                {
                    //Only split if it has no children yet
                    if (n.childA == null && n.childB == null)
                    {
                        //Prevent nodes from becoming too small or too big
                        if (n.width > minNodeSize || n.height > minNodeSize)
                        {
                            if (n.SplitNode())
                            {
                                //Resize array to make space for 2 new children
                                Array.Resize(ref nodeArr, nodeArr.Length + 2);
                                //Resize room array
                                Array.Resize(ref roomArr, roomArr.Length + 2);

                                nodeArr[nodeArr.Length - 2] = n.childA;
                                nodeArr[nodeArr.Length - 1] = n.childB;

                                splitCounter++;
                                hasSplit = true;
                            }
                        }
                    }
                }
            }
        }
        node.CreateRoom();
        Debug.Log(nodeArr.Length);
    }
    

    void CreateHallways()
    {
        for (int i = 0; i < (roomArr.Length -1); i++)//Create hallways for all rooms
        {
            Rect roomA = roomArr[i];
            Rect roomB = roomArr[i + 1];
            Vector2 randomPosA = new Vector2(UnityEngine.Random.Range(roomA.x + 2, roomA.width - 1), UnityEngine.Random.Range(roomA.y + 2, roomA.height - 1));
            Vector2 randomPosB = new Vector2(UnityEngine.Random.Range(roomB.x + 2, roomB.width - 1), UnityEngine.Random.Range(roomB.y + 2, roomB.height - 1));
            int xDistance = (int)randomPosB.x - (int)randomPosA.x;
            int yDistance = (int)randomPosB.y - (int)randomPosA.y;

            if (xDistance > 0)
            {
                if (yDistance > 0)
                {
                    FillBlock(grid, (int)randomPosB.x, (int)randomPosA.y, 1, Math.Abs(yDistance), TileType.Empty);
                    FillBlock(grid, (int)randomPosA.x, (int)randomPosA.y, Math.Abs(xDistance), 1, TileType.Empty);
                }
                else if (yDistance < 0)
                {
                    FillBlock(grid, (int)randomPosA.x, (int)randomPosB.y, 1, Math.Abs(yDistance), TileType.Empty);
                    FillBlock(grid, (int)randomPosA.x, (int)randomPosB.y, Math.Abs(xDistance), 1, TileType.Empty);
                }
                else if (yDistance == 0)
                {
                    FillBlock(grid, (int)randomPosB.x, (int)randomPosB.y, Math.Abs(xDistance), 1, TileType.Empty);
                }
            }
            if (xDistance < 0)
            {
                if (yDistance > 0)
                {
                    FillBlock(grid, (int)randomPosA.x, (int)randomPosA.y, 1, Math.Abs(yDistance), TileType.Empty);
                    FillBlock(grid, (int)randomPosB.x, (int)randomPosB.y, Math.Abs(xDistance), 1, TileType.Empty);
                }
                else if (yDistance < 0)
                {
                    FillBlock(grid, (int)randomPosB.x, (int)randomPosA.y, Math.Abs(xDistance), 1, TileType.Empty);
                    FillBlock(grid, (int)randomPosB.x, (int)randomPosB.y, 1, Math.Abs(yDistance), TileType.Empty);
                }
                else if(yDistance == 0)
                {
                    FillBlock(grid, (int)randomPosA.x, (int)randomPosA.y, Math.Abs(xDistance), 1, TileType.Empty);
                }
            }
            if (xDistance == 0)
            {
                if (yDistance > 0)
                {
                    FillBlock(grid, (int)randomPosB.x, (int)randomPosB.y, 2, Mathf.Abs(yDistance), TileType.Empty);
                }
                else if(yDistance < 0)
                {
                    FillBlock(grid, (int)randomPosA.x, (int)randomPosA.y, 1, Mathf.Abs(yDistance), TileType.Empty);
                }
            }


        }
    }

    void OnDrawGizmos()
    {
        //Draw a cube around the edges of each node
        foreach (Node n in nodeArr)
        {
            n.DrawNode();
        }
        //Debug.Log(nodeList.Count);
    }


}
