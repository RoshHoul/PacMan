using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : BaseGhost
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
            Vector2 pacManPos = pacMan.transform.position;
            Vector2 pacManForward = pacMan.GetComponent<Controller>().orientation;

            int pacManPosX = Mathf.RoundToInt(pacManPos.x);
            int pacManPosY = Mathf.RoundToInt(pacManPos.y);

            Vector2 pacManTile = new Vector2(pacManPosX, pacManPosY);
            int dist = Mathf.RoundToInt(DistanceBetweenTwoNodes(transform.position, pacManTile));

            if (dist > 8)
            {
                return pacManTile;
            } else
            {
                targTile = myCornerNode.transform.position;
                return targTile;
            }
        }

        if (GetState() == GhostState.Scatter)
        {
            targTile = myCornerNode.transform.position;
            return targTile;
        }

        return targTile;
    }

    private void ReleaseGhost()
    {
        int scoreSoFar = pacMan.GetComponent<Controller>().pCollected;

        if (scoreSoFar >= 50)
        {
            inGhostHouse = false;
        }
    }


}

