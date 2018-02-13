﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    public GameObject[,] board = new GameObject[boardWidth, boardHeigth];
    public GameObject[] ghosts;

    private int level = 1;
    public Text levelUI;
    public Text points;
    public Text life;
    private int lives = 0;

    public bool pacManLost = false;

    //Level variables
    private float ghostSpeed;
    private float ghostTunnelSpeed;
    private float ghostFrightSpeed;
    private float frightDuration;
    private int ElroyDotCount1;
    private int ElroyDotCount2;

    private int releaseCounter = 0;
    private int inkyReleaseCounter, clydeReleaseCounter;

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
        GetLevelStats();

        pacMan = GameObject.Find("Pac Man").GetComponent<Controller>();
        objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach ( GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
            Tile tile = o.GetComponent<Tile>();

            if (o.tag == "Pellets" || o.tag == "PelletsInner" || o.tag == "PelletsSpecial")
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
        Init(false);
        Debug.Log("Pellets are " + pelletsCount);

	}

    void Init(bool reset)
    {
        lives = pacMan.GetComponent<Controller>().life;
        if (reset)
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (ghosts[i].GetComponent<Pinky>() != null)
                {
                    Pinky pinky = ghosts[i].GetComponent<Pinky>();
                    pinky.resetLevel = true;
                    ghosts[i].GetComponent<Pinky>().Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
                }
                else if (ghosts[i].GetComponent<Blinky>() != null)
                {
                    Blinky blinky = ghosts[i].GetComponent<Blinky>();
                    blinky.resetLevel = true;
                    blinky.Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
                }
                else if (ghosts[i].GetComponent<Inky>() != null)
                {
                    Inky inky = ghosts[i].GetComponent<Inky>();
                    inky.resetLevel = true;
                    inky.Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
                }
                else if (ghosts[i].GetComponent<Clyde>() != null)
                {
                    Clyde clyde = ghosts[i].GetComponent<Clyde>();
                    clyde.resetLevel = true;
                    clyde.Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
                }
            }
        } else
        {
            for (int i = 0; i < ghosts.Length; i++)
            {
                if (ghosts[i].GetComponent<Pinky>() != null)
                {
                    Pinky pinky = ghosts[i].GetComponent<Pinky>();
                    pinky.resetLevel = false;
                    ghosts[i].GetComponent<Pinky>().Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
                }
                else if (ghosts[i].GetComponent<Blinky>() != null)
                {
                    Blinky blinky = ghosts[i].GetComponent<Blinky>();
                    blinky.resetLevel = false;
                    blinky.Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
                }
                else if (ghosts[i].GetComponent<Inky>() != null)
                {
                    Inky inky = ghosts[i].GetComponent<Inky>();
                    inky.resetLevel = false;
                    inky.release = inkyReleaseCounter;
                    inky.Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
                }
                else if (ghosts[i].GetComponent<Clyde>() != null)
                {
                    Clyde clyde = ghosts[i].GetComponent<Clyde>();
                    clyde.resetLevel = false;
                    clyde.release = clydeReleaseCounter;
                    clyde.Init(ghostSpeed, ghostFrightSpeed, ghostTunnelSpeed, frightDuration);
                }
            }
        }
        foreach (GameObject o in objects)
        {
            if (!pacManLost)
            {

                if (o.tag == "Pellets" || o.tag == "PelletsInner" || o.tag == "PelletsSpecial")
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
        }

        pacMan.Init(pacManSpeed, frightPacManSpeed, pacManDotsSpeed, frightDuration);
    }
	
    void GetLevelStats()
    {
        if (level == 1)
        {

            ghostSpeed = (25 * ghostsGeneralSpeed) / 100;
            ghostTunnelSpeed = (60 * ghostsGeneralSpeed) / 100;
            ghostFrightSpeed = (50 * ghostsGeneralSpeed) / 100; 
            ElroyDotCount1 = 20;
            ElroyDotCount2 = 10;
            pacManSpeed = (20  * pacManGeneralSpeed) / 100;
            frightPacManSpeed = (21 * pacManGeneralSpeed) / 100;
            pacManDotsSpeed = (29 * pacManGeneralSpeed) / 100;
            frightDuration = 6;
            inkyReleaseCounter = 30;
            clydeReleaseCounter = 50;

        }

        if (level == 2)
        {

            ghostSpeed = (15 * ghostsGeneralSpeed) / 100;
            ghostTunnelSpeed = (55 * ghostsGeneralSpeed) / 100;
            ghostFrightSpeed = (45 * ghostsGeneralSpeed) / 100; 
            ElroyDotCount1 = 40;
            ElroyDotCount2 = 20;
            pacManSpeed = (10  * pacManGeneralSpeed) / 100;
            frightPacManSpeed = (17 * pacManGeneralSpeed) / 100;
            pacManDotsSpeed = (21 * pacManGeneralSpeed) / 100;
            frightDuration = 5;
            inkyReleaseCounter = 30;
            clydeReleaseCounter = 50;
        }

        if (level == 3)
        {

            ghostSpeed = (15 * ghostsGeneralSpeed) / 100;
            ghostTunnelSpeed = (55 * ghostsGeneralSpeed) / 100;
            ghostFrightSpeed = (45 * ghostsGeneralSpeed) / 100;
            ElroyDotCount1 = 40;
            ElroyDotCount2 = 20;
            pacManSpeed = (10 * pacManGeneralSpeed) / 100;
            frightPacManSpeed = (17 * pacManGeneralSpeed) / 100;
            pacManDotsSpeed = (21 * pacManGeneralSpeed) / 100;
            frightDuration = 4;
            inkyReleaseCounter = 30;
            clydeReleaseCounter = 50;
        }

        if (level == 4)
        {

            ghostSpeed = (15 * ghostsGeneralSpeed) / 100;
            ghostTunnelSpeed = (55 * ghostsGeneralSpeed) / 100;
            ghostFrightSpeed = (45 * ghostsGeneralSpeed) / 100;
            ElroyDotCount1 = 40;
            ElroyDotCount2 = 20;
            pacManSpeed = (10 * pacManGeneralSpeed) / 100;
            frightPacManSpeed = (17 * pacManGeneralSpeed) / 100;
            pacManDotsSpeed = (21 * pacManGeneralSpeed) / 100;
            frightDuration = 3;
            inkyReleaseCounter = 30;
            clydeReleaseCounter = 50;
        }

        if (level == 5)
        {

            ghostSpeed = (5 * ghostsGeneralSpeed) / 100;
            ghostTunnelSpeed = (50 * ghostsGeneralSpeed) / 100;
            ghostFrightSpeed = (40 * ghostsGeneralSpeed) / 100;
            ElroyDotCount1 = 40;
            ElroyDotCount2 = 20;
            pacManSpeed = (0 * pacManGeneralSpeed) / 100;
            frightPacManSpeed = (13 * pacManGeneralSpeed) / 100;
            pacManDotsSpeed = (21 * pacManGeneralSpeed) / 100;
            frightDuration = 2;
            inkyReleaseCounter = 30;
            clydeReleaseCounter = 50;
        }

    }

    public int CurrentLevel()
    {
        return level;
    }

	// Update is called once per frame
	void Update () {

        UpdateUI();
		if (IsNewLevel())
        {
            level++;
            GetLevelStats();
            Init(false);
        }

        if (pacManLost)
        {
            if (pacMan.GetComponent<Controller>().life >= 0)
            {
                pacMan.GetComponent<Controller>().life--;
                GetLevelStats();
                Init(true);
                pacManLost = false;
                Debug.Log("Level Reset");
            } else
            {
                //GameOverScreen
            }
        }

	}

    void UpdateUI()
    {
        levelUI.text = level.ToString();
        points.text = pacMan.points.ToString();
        life.text = lives.ToString();
    }

    void ResetLife()
    {
        Debug.Log("DEAAAD");
    }

    bool IsNewLevel()
    {
        return pelletsCount <= pacMan.pCollected;
    }
}
