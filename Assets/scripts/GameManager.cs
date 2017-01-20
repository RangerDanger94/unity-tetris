using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public string fileGravity;
    public string fileGrading;

    private SortedList<int, int> gravity = new SortedList<int, int>();
    private SortedList<string, int> grading = new SortedList<string, int>();

    public int areDelay;
    public int dasDelay;
    public int lockDelay;
    public int clearDelay;
    public int level;
    private float gFrames;

    private Spawner bag;
    private GameObject activePiece;
    private GameObject nextPiece;

    private List<GameObject> field = new List<GameObject>(); // Store tetrominos that have been added to the field
   
    private Grid board;

    // GameManager singleton
    private static GameManager instance = null;
    public static GameManager getInstance()
    {
        return instance;
    }
    
    // Manage Singleton and set framerate
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }   
        else if (instance != this)
        {
            Destroy(gameObject);
        }
            
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;
    }

    private Tetromino ActiveTetromino()
    {
        return activePiece.GetComponent<Tetromino>();
    }

    // Use this for initialization
    void Start()
    {
        LoadGravity();
        LoadGrading();
         
        bag = FindObjectOfType<Spawner>();
        board = FindObjectOfType<Grid>();

        activePiece = Instantiate(bag.SelectNext(), board.Spawn, Quaternion.identity);
        nextPiece = Instantiate(bag.SelectNext(), board.NextDisplay, Quaternion.identity);
    }

    private void Update()
    {
        Tetromino active = ActiveTetromino();
        ApplyGravity();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            active.ShiftLeftSafe();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            active.ShiftRightSafe();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            active.RotateClockwiseSafe();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            active.RotateCounterClockwiseSafe();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            active.Drop(true);
        }

        if (active.IsLocked())
        {
            field.Add(activePiece);
            activePiece = Instantiate(nextPiece, board.Spawn, Quaternion.identity);
            Destroy(nextPiece);
            nextPiece = Instantiate(bag.SelectNext(), board.NextDisplay, Quaternion.identity);
            Grid.deleteFullRows();


            // Cleanup
            for (int i = 0; i < field.Count; ++i)
            {
                GameObject terrain = field[i];
                foreach (Transform child in terrain.GetComponentsInChildren<Transform>(false))
                {
                    if (child.tag == "Rotation" && child.childCount == 0)
                    {
                        Debug.Log("Cleaning up " + terrain);
                        field.Remove(terrain);
                        Destroy(terrain);
                    }
                }
            }
        }
    }

    public void ApplyGravity()
    {
        // Get current gravity based on level value
        float result = gravity[0];
        foreach (int key in gravity.Keys)
        {
            if(key > level)
            {
                break;
            }
            result = gravity[key] / 256.0f;
        }
        
        gFrames += result;

        while(gFrames >= 1)
        {
            ActiveTetromino().Drop(false);
            gFrames--;
        }  
    }

    public int GetLockDelay()
    {
        return lockDelay;
    }

    public int GetClearDelay()
    {
        return clearDelay;
    }

    public int GetLevel()
    {
        return level;
    }

    public int NextLevel(bool cleared)
    {
        if (level + 1 % 100 == 0 || level == 998)  
        {
            if (cleared)
            {
                level++;
            }
        }
        else
        {
            level++;
        }

        return level;
    }

    private void LoadGravity()
    {
        var reader = new StreamReader(fileGravity, Encoding.Default);
        using (reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] split = line.Split();
                gravity[System.Convert.ToInt32(split[0])] = System.Convert.ToInt32(split[1]);
            }
        }
    }

    private void LoadGrading()
    {
        var reader = new StreamReader(fileGrading, Encoding.Default);
        using (reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] split = line.Split();
                grading[split[0]] = System.Convert.ToInt32(split[1]);
            }
        }
    } 
}
