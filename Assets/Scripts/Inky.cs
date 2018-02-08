using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : BaseGhost
{

    public Node startingNode;
    public Node myCornerNode;
    private int release = 0;

    // Use this for initialization
    protected override void Start()
    {
    }

    // Update is called once per frame
    protected override void Update()
    {
        targetTile = UpdateTarget();
        if (inGhostHouse)
            ReleaseGhost();
        
        base.Update();

    }

    public override void Init(float speed, float fright, float tunnel, float frightDur, int releaseCounter)
    {
        base.Init(speed, fright, tunnel, frightDur, releaseCounter);
        release = releaseCounter;
        transform.position = startingNode.transform.position;
        currentNode = startingNode;

        direction = Vector2.left;
        nextNode = CanMove();

        prevNode = currentNode;
    }

    public Vector2 UpdateTarget()
    {
        Vector2 targTile = new Vector2();
        if (GetState() == GhostState.Chase)
        {
            Vector2 blinkyPos = GameObject.Find("Blinky").transform.position;
            Vector2 pacManPos = pacMan.transform.position;
            Vector2 pacManForward = pacMan.GetComponent<Controller>().orientation;

            int pacManPosX = Mathf.RoundToInt(pacManPos.x);
            int pacManPosY = Mathf.RoundToInt(pacManPos.y);


            Vector2 pacManTile = new Vector2(pacManPosX, pacManPosY);
            Vector2 tileA = pacManTile + (2 * pacManForward);

            int blinkyPosX = Mathf.RoundToInt(blinkyPos.x);
            int blinkyPosY = Mathf.RoundToInt(blinkyPos.y);
            Vector2 blinkyTile = new Vector2(blinkyPosX, blinkyPosY);

            float distX = (tileA.x - blinkyTile.x) * 2;
            float distY = (tileA.y - blinkyTile.y) * 2;
            targTile = new Vector2(Mathf.RoundToInt(blinkyPosX + distX), Mathf.RoundToInt(blinkyPosY + distY));


            return targTile;
        }

        if (GetState() == GhostState.Scatter)
        {
            Vector2 myCorner = myCornerNode.transform.position;
            targTile = myCorner;
            return targTile;
        }

        return targTile;
    }

    private void ReleaseGhost()
    {
        int scoreSoFar = pacMan.GetComponent<Controller>().pCollected;

        if (scoreSoFar >= release)
        {
            inGhostHouse = false;
            Node tempNode = GetNodeAtPosition(transform.position);
            nextNode = CanMove();
            Move();
            Debug.Log("SCORE:INKY " + scoreSoFar);
        }
    }

}

