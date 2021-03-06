﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : BaseGhost
{

    public Node startingNode;
    public Node myCornerNode;
    public int release = 0;

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

    public override void Init(float speed, float fright, float tunnel, float frightDur)
    {
        base.Init(speed, fright, tunnel, frightDur);
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

        else if (GetState() == GhostState.Scatter)
        {
            targTile = myCornerNode.transform.position;
            return targTile;
        }

        return targTile;
    }

    private void ReleaseGhost()
    {
        int scoreSoFar = pacMan.GetComponent<Controller>().pCollected;

        if (scoreSoFar >= release || resetLevel == true)
        {
            inGhostHouse = false;
            Node tempNode = GetNodeAtPosition(transform.position);
            nextNode = CanMove();
            prevNode = currentNode;
            Move();
        }
    }


}

