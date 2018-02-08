using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    public GameObject[,] board = new GameObject[boardWidth, boardHeigth];
    public GameObject[] ghosts;

    private Controller pacMan;
    private int pelletsCount = 0;
    private static int boardWidth = 28;
    private static int boardHeigth = 36;

    
	// Use this for initialization
	void Start () {

        ghosts = GameObject.FindGameObjectsWithTag("Ghosts");
        Init();


        pacMan = GameObject.Find("Pac Man").GetComponent<Controller>();
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

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

	}

    void Init()
    {
        for (int i = 0; i < ghosts.Length; i++)
        {
            if (ghosts[i].GetComponent<Pinky>() != null)
            {
                ghosts[i].GetComponent<Pinky>().Init();
            }
            else if (ghosts[i].GetComponent<Blinky>() != null)
            {
                ghosts[i].GetComponent<Blinky>().Init();
            }
            else if (ghosts[i].GetComponent<Inky>() != null)
            {
                ghosts[i].GetComponent<Inky>().Init();
            }
            else if (ghosts[i].GetComponent<Clyde>() != null)
            {
                ghosts[i].GetComponent<Clyde>().Init();
            }

        }
    }
	
	// Update is called once per frame
	void Update () {

		if (IsGameOver())
        {
            Debug.Log("Congratz!");
        }
	}

    bool IsGameOver()
    {
        return pelletsCount <= pacMan.pCollected;
    }
}
