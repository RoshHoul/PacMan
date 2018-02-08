using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    public GameObject[,] board = new GameObject[boardWidth, boardHeigth];
    public GameObject[] ghosts;

    private int level = 1;
    public Text levelUI;
    public Text points;

    //Level variables
    private float ghostSpeed;
    private float ghostTunnelSpeed;
    private float ghostFrightSpeed;
    private float frightDuration;
    private int ElroyDotCount1;
    private int ElroyDotCount2;


    private float pacManSpeed;
    private float frightPacManSpeed;
    private float pacManDotsSpeed;


    private float ghostsGeneralSpeed = 5;
    private float pacManGeneralSpeed = 5.1f;
    private Controller pacMan;
    private int pelletsCount = 0;
    private static int boardWidth = 28;
    private static int boardHeigth = 36;
    private Object[] objects;

    // Use this for initialization
    void Start () {

        ghosts = GameObject.FindGameObjectsWithTag("Ghosts");
        IncreaseLevel();

        pacMan = GameObject.Find("Pac Man").GetComponent<Controller>();
        objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach ( GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
            Tile tile = o.GetComponent<Tile>();

            if (o.tag == "Pellets" || o.tag == "PelletsInner")
            {
                if (tile != null)
                {
                    if (tile.isPellet || tile.isSuperPellet) 
                    {
                        pelletsCount++;
                    }
                }
                board[(int)pos.x, (int)pos.y] = o;
            } else
            {
            }
        }
        Init();
        Debug.Log("Pellets are " + pelletsCount);

	}

    void Init()
    {
      //  IncreaseLevel();
        
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (ghosts[i].GetComponent<Pinky>() != null)
            {
                ghosts[i].GetComponent<Pinky>().Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
            }
            else if (ghosts[i].GetComponent<Blinky>() != null)
            {
                ghosts[i].GetComponent<Blinky>().Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
            }
            else if (ghosts[i].GetComponent<Inky>() != null)
            {
                ghosts[i].GetComponent<Inky>().Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
            }
            else if (ghosts[i].GetComponent<Clyde>() != null)
            {
                ghosts[i].GetComponent<Clyde>().Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
            }
        }

        foreach (GameObject o in objects)
        {


            if (o.tag == "Pellets" || o.tag == "PelletsInner")
            {
                Tile tile = o.GetComponent<Tile>();
                if (tile != null)
                {
                    if (tile.isPellet || tile.isSuperPellet)
                    {
                        tile.GetComponent<SpriteRenderer>().enabled = true;
                        tile.isConsumed = false;
                    }
                }
            }
        }

        pacMan.Init();
    }
	
    void IncreaseLevel()
    {
        if (level == 1)
        {

            Debug.Log("level1");
            ghostSpeed = (25 * ghostsGeneralSpeed) / 100;
            ghostTunnelSpeed = (60 * ghostsGeneralSpeed) / 100;
            ghostFrightSpeed = (50 * ghostsGeneralSpeed) / 100; 
            Debug.Log("ghost values " + ghostSpeed + " " + ghostTunnelSpeed + " " + ghostFrightSpeed);
            ElroyDotCount1 = 20;
            ElroyDotCount2 = 10;
            pacManSpeed = (20 / 100) * pacManGeneralSpeed;
            frightPacManSpeed = (10 / 100) * pacManGeneralSpeed;
            pacManDotsSpeed = (25 / 100) * pacManGeneralSpeed;
            frightDuration = 6;

        }
    }

    public int CurrentLevel()
    {
        return level;
    }

	// Update is called once per frame
	void Update () {

        UpdateUI();
		if (IsGameOver())
        {
            Init();
        }

	}

    void UpdateUI()
    {
        levelUI.text = level.ToString();
        points.text = pacMan.points.ToString();
    }

    bool IsGameOver()
    {
        return pelletsCount <= pacMan.pCollected;
    }
}
