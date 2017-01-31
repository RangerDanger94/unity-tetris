using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour {
    const int MAX_LEVEL = 999;

    public string fileGravity;
    public string fileGrading;

    private SortedList<int, int> gravity = new SortedList<int, int>();
    private SortedList<string, int> grading = new SortedList<string, int>();

    public GameObject scoreDisplay;
    public GameObject levelDisplay;

    public int areDelay;
    public int dasDelay;
    public int lockDelay;
    public int clearDelay;
    public int level;
    private int score;
    private int combo;
    

    // Frame counters
    private int dasFrames;
    private int lockFrames;
    private int softFrames;
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
        level = 1;
    }

    private void UpdateGUI()
    {
        levelDisplay.GetComponent<GUIText>().text = level.ToString().PadLeft(3, '0');
        scoreDisplay.GetComponent<GUIText>().text = score.ToString().PadLeft(6, '0'); 
    }

    private void ClockwiseRotation(Tetromino active)
    {
        if (!active.RotateClockwiseSafe())
        {
            if (!(active.ShiftRightSafe() && active.RotateClockwiseSafe()))
            {
                active.ShiftLeftSafe();
                active.RotateClockwiseSafe();
            }
        };
    }

    private void CounterClockwiseRotation(Tetromino active)
    {
        if (!active.RotateCounterClockwiseSafe())
        {
            if (!(active.ShiftRightSafe() && active.RotateCounterClockwiseSafe()))
            {
                active.ShiftLeftSafe();
                active.RotateCounterClockwiseSafe();
            }
        }
    }

    private void Update()
    {
        Tetromino active = ActiveTetromino();
        bool softDrop = false;
        
        float shift = Input.GetAxisRaw("Horizontal");
        // Update DAS
        dasFrames = shift != 0 ? dasFrames += 1 : dasFrames = 0;

        bool shiftRight = (dasFrames == 1 || dasFrames > dasDelay) && shift == 1;
        bool shiftLeft = (dasFrames == 1 || dasFrames > dasDelay) && shift == -1;

        if (shiftRight)
        {
            active.ShiftRightSafe();
        }
        else if (shiftLeft)
        {
            active.ShiftLeftSafe();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ClockwiseRotation(active);
        }
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    CounterClockwiseRotation(active);
        //}
        else if (Input.GetAxisRaw("Vertical") == -1)
        {
            active.Drop(true);
            softDrop = true;
            softFrames++;
        }

        ApplyGravity();
        if (active.IsLocked()) // Drop has collision
        {
            lockFrames++;

            if (lockFrames >= lockDelay || softDrop)
            {
                UpdateScore(Grid.DeleteFullRows());
                softFrames = 0;

                field.Add(activePiece);
                SpawnNextPiece();
                CleanField();
            }
        }
        else
        {
            lockFrames = 0;
        }

        UpdateGUI();
    }

    private void SpawnNextPiece()
    {
        activePiece = Instantiate(nextPiece, board.Spawn, Quaternion.identity);
        Destroy(nextPiece);
        nextPiece = Instantiate(bag.SelectNext(), board.NextDisplay, Quaternion.identity);

        // Increment if !99->100 && !998->999 
        if (level+1 % 100 != 0 && level+1 != MAX_LEVEL)
        {
            level++;
        } 
    }

    private void UpdateScore(int lines)
    {
        level += lines;
        if (lines > 0)
        {
            int bravo = Grid.Unoccupied() ? 4 : 1;
            float bScore = Mathf.Ceil((level + lines) / 4) + softFrames;
            combo += (2 * lines) - 2;
            score += (int)bScore * lines * ((2 * lines) - 1) * combo * bravo;
        } 
        else
        {
            combo = 1;
        }
    }

    private void CleanField()
    {
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
