using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour {
    private List<GameObject> rotations = new List<GameObject>();
    private GameObject piece;

    private bool Locked = false;

    // Use this for initialization
    private void Start () {

        // Add all rotations to a list
        foreach (Transform child in transform)
        {
            if(child.tag == "Rotation")
            {
                rotations.Add(child.gameObject);
                if (child.gameObject.activeSelf)
                {
                    piece = child.gameObject;
                }
            }
        }
    }

    public bool IsLocked()
    {
        return Locked;
    }
    
    private void ShiftRight()
    {
        // Shift all rotations 1 unit to the right
        foreach (GameObject rotation in rotations)
        {
            rotation.transform.Translate(1, 0, 0);
        }
    }

    public void ShiftRightSafe()
    {
        // Modify position
        ShiftRight();

        // See if valid
        if (IsValidGridPos())
        {
            UpdateGrid();
        }
        else
        {
            ShiftLeft();
        }
    }

    private void ShiftLeft()
    {
        // Shift all rotations 1 unit to the left
        foreach (GameObject rotation in rotations)
        {
            rotation.transform.Translate(-1, 0, 0);
        }
    }

    public void ShiftLeftSafe()
    {
        // Modify position
        ShiftLeft();

        // See if valid
        if (IsValidGridPos())
        {
            UpdateGrid();
        }
        else
        {
            ShiftRight();
        }
    }

    private void RotateClockwise()
    {
        // Get next rotation
        int next = rotations.IndexOf(piece) + 1;

        // Remove last rotation from the grid and deactivate it 
        RemoveFromGrid();
        piece.SetActive(false);

        // Set next rotation
        piece = (next >= rotations.Count) ? rotations[0] : rotations[next];
        piece.SetActive(true);
    }

    public void RotateClockwiseSafe()
    {
        // Modify position
        RotateClockwise();

        // See if valid
        if (IsValidGridPos())
        {
            UpdateGrid();
        }
        else
        {
            RotateCounterClockwise();
        }
    }

    private void RotateCounterClockwise()
    {
        // Get next rotation
        int next = rotations.IndexOf(piece) - 1;

        // Remove last rotation from the grid and deactivate it 
        RemoveFromGrid();
        piece.SetActive(false);

        // Set next rotation
        piece = (next < 0) ? rotations[rotations.Count - 1] : rotations[next];
        piece.SetActive(true);
    }

    public void RotateCounterClockwiseSafe()
    {
        // Modify position
        RotateCounterClockwise();

        // See if valid
        if (IsValidGridPos())
        {
            UpdateGrid();
        }
        else
        {
            RotateClockwise();
        }
    }


    // Returns false if drop position is not valid
    public void Drop(bool hold)
    {
        // Modify position
        transform.position += new Vector3(0, -1, 0);

        // See if valid
        if (IsValidGridPos())
        {
            UpdateGrid();
        }
        else
        {
            Locked = true;
            transform.position += new Vector3(0, 1, 0);
        }
    }

    // Remove old children from grid
    private void RemoveFromGrid()
    {
        for (int y = 0; y < Grid.h; ++y)
        {
            for (int x = 0; x < Grid.w; ++x)
            {
                if (Grid.grid[x, y] != null)
                {
                    if (Grid.grid[x, y].parent == piece.transform)
                    {
                        Grid.grid[x, y] = null;
                    }
                }
            }
        }
    }

    // Add new children to grid
    private void AddToGrid()
    {
        foreach (Transform child in piece.transform)
        {
            Vector2 v = Grid.roundVec2(child.position);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }
    }

    private void UpdateGrid()
    {
        RemoveFromGrid();
        AddToGrid();
    }

    private bool IsValidGridPos()
    {
        foreach (Transform child in piece.transform)
        {
            Vector2 v = Grid.roundVec2(child.position);

            // Not inside border?
            if(!Grid.insideBorder(v))
            {
                Debug.Log("I am outside the border!");
                return false;
                
            }

            // Block in grid cell (and not part of same group)?
            if (Grid.grid[(int)v.x, (int)v.y] != null && 
                Grid.grid[(int)v.x, (int)v.y].parent != piece.transform)
            {
                Debug.Log("I am colliding with another piece");
                return false;
            }
        }

        return true;
    }
}