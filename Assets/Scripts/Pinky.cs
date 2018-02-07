using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : BaseGhost {

    public Node startingNode;
    public Node myCornerNode;
    private float ghostReleaseTimer = 0.0f;
    public float myReleaseTimer = 5.0f;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        Node node = GetNodeAtPosition(transform.localPosition);
        if (node != null)
        {
            currentNode = node;
        } else
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
        base.Update();
        Debug.Log("currentState " + GetState());
        //Move();
        if (inGhostHouse)
        {
            ReleaseGhost();
        }

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
            targTile = pacManTile + (4 * pacManForward);
            Debug.Log("PINKY targ " + targTile + " pacman pos " + pacManTile);
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
        ghostReleaseTimer += Time.deltaTime;

        if (ghostReleaseTimer > myReleaseTimer && inGhostHouse)
        {
            inGhostHouse = false;
        }
    }
}
