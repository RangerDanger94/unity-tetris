using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject[] tetrominos;
    public string[] history = { "Z", "Z", "Z", "Z" };
    public int retries = 4;

    private GameObject selectedPiece;
    private bool first = true;

    private void UpdateHistory()
    {
        System.Array.Copy(history, 0, history, 1, history.Length - 1);
        history[0] = selectedPiece.tag;
    }

    private bool InHistory(string t)
    {
        foreach(string piece in history)
        {
            if (piece == t)
            {
                return true;
            }
        }

        return false;
    }

    private void FirstSelection()
    {
        do
        {
            Selection();
        } // Never start the game with an O, Z or S piece
        while (
               selectedPiece.CompareTag("O") ||
               selectedPiece.CompareTag("Z") ||
               selectedPiece.CompareTag("S"));

        first = false;
    }

    private void Selection()
    {
        int r = Random.Range(0, tetrominos.Length);

        // Attempt up pick out a piece that is not in the history up to the retry limit
        for (int i = 0; i < retries - 1; ++i)
        {
            if (InHistory(tetrominos[r].tag))
            {
                r = Random.Range(0, tetrominos.Length);
            }
            else
            {
                break;
            }
        }

        selectedPiece = tetrominos[r];
    }

    public GameObject SelectNext()
    {
        if (first)
        {
            FirstSelection();
        } 
        else
        {
            Selection();
        }

        UpdateHistory();
        return selectedPiece;
    }
}
