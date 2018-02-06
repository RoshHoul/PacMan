using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {

    public GameObject[,] board = new GameObject[boardWidth, boardHeigth];

    private Controller pacMan;
    private int pelletsCount = 0;
    private static int boardWidth = 28;
    private static int boardHeigth = 36;

    
	// Use this for initialization
	void Start () {

        pacMan = GameObject.Find("Pac Man").GetComponent<Controller>();
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach ( GameObject o in objects)
        {
            Vector2 pos = o.transform.position;
            Tile tile = o.GetComponent<Tile>();

            if (o.tag == "Pellets")
            {
                Debug.Log("pellets");
                if (tile != null)
                {
                    Debug.Log("tile");
                    if (tile.isPellet || tile.isSuperPellet) 
                    {
                        Debug.Log("peller or energ");
                        pelletsCount++;
                    }
                }
                board[(int)pos.x, (int)pos.y] = o;
            } else
            {
            }
        }

        Debug.Log("pc is " + pelletsCount);
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log("SCORE: " + pacMan.pCollected);

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
