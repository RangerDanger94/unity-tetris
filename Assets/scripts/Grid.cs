using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public static int w = 10;
    public static int h = 20;
    public static Transform[,] grid = new Transform[w, h];

    public Vector3 Spawn;
    public Vector3 NextDisplay;

    public BoxCollider2D floor;

    public static Vector2 roundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    private void Start()
    {
        foreach (Transform child in transform)
        {
            Debug.Log("TAG = " + child.tag.ToString());

            switch(child.tag)
            {
                case "Floor":
                    floor = child.GetComponent<BoxCollider2D>();
                    break;
            }
        }
    }

    public static bool insideBorder(Vector2 v)
    {
        return (v.x >= 0 &&
                v.x < w && 
                v.y >= 0);
    }

    public static void DeleteRow(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    public static void DecreaseRow(int y)
    {
        for(int x = 0; x < w; ++x)
        {
            if(grid[x, y] != null)
            {
                // Move one row down
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // Update Block position
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public static void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < h; ++i)
        {
            DecreaseRow(i);
        }
    }

    public static bool IsRowFull(int y)
    {
        for (int x = 0; x < w; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    public static bool Unoccupied()
    {
        for (int x = 0; x < w; ++x)
        {
            for (int y = 0; y < h; ++y)
            {
                if (grid[x, y] != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Delete full rows and return number of rows deleted
    public static int DeleteFullRows()
    {
        int deleted = 0;
        for (int y = 0; y < h; ++y)
        {
            if (IsRowFull(y))
            {
                deleted++;
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                --y;
            }
        }

        return deleted;
    }
}
