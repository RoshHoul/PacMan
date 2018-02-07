using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : BaseGhost
{

    public Node startingNode;
    public Node myCornerNode;


    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        Node node = GetNodeAtPosition(transform.localPosition);
        if (node != null)
        {
            currentNode = node;
        }
        else
        {
            transform.position = startingNode.transform.position;
            currentNode = startingNode;
        }

        direction = Vector2.left;
        nextNode = CanMove();

        prevNode = currentNode;

    }

    // Update is called once per frame
    protected override void Update()
    {
        targetTile = UpdateTarget();
        if (inGhostHouse)
            ReleaseGhost();
        
        base.Update();

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

        if (scoreSoFar >= 30)
        {
            inGhostHouse = false;
        }
    }

}

