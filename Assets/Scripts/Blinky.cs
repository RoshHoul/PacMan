﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : BaseGhost {

    public int speed = 5;
    private Node prevNode, currentNode, nextNode;
    private Vector2 direction, nextDirection;

	// Use this for initialization
	protected override void Start () {
        base.Start();

        Node node = GetNodeAtPosition(transform.localPosition);
        if (node != null)
        {
            currentNode = node;
        } 

        prevNode = currentNode;
        direction = Vector2.left;

        Vector2 pacManPos = pacMan.transform.position;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacManPos.x), Mathf.RoundToInt(pacManPos.y));
        //  nextNode = GetNodeAtPosition(targetTile);
        nextNode = CanMove();
        

    }
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
        Debug.Log("currentState " + currentState);
        Move();
	}

    void Move()
    {
        if (nextNode != currentNode && nextNode != null)
        {
            if (OverShotTarget(nextNode, prevNode))
            {
                currentNode = nextNode;
                transform.localPosition = currentNode.transform.position;

                GameObject portal = GetPortal(currentNode.transform.position);

                if (portal != null)
                {
                    transform.localPosition = portal.transform.position;
                    currentNode = portal.GetComponent<Node>();
                }

                nextNode = CanMove();
              //  direction = 
                prevNode = currentNode;
                currentNode = null;

            }
            else
            {
                transform.localPosition += (Vector3)direction * speed * Time.deltaTime;
            }
        }
    }

    public Node CanMove()
    {
        Node moveTo = null;
        Vector2 pacManPos = pacMan.transform.position;
        Vector2 targetTile = new Vector2(Mathf.RoundToInt(pacManPos.x), Mathf.RoundToInt(pacManPos.y));
        Node[] foundNodes = new Node[4];
        Vector2[] possiblePaths = new Vector2[4];
        int nodeCounter = 0;


        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if (currentNode.possiblePaths[i] != direction * -1)
            {
                foundNodes[nodeCounter] = currentNode.neighbours[i];
                possiblePaths[nodeCounter] = currentNode.possiblePaths[i];
                nodeCounter++;
            }
        }

        if (foundNodes.Length == 1)
        {
            moveTo = foundNodes[0];
            direction = possiblePaths[0];
        }

        if (foundNodes.Length > 1)
        {
            float shortest = 100000;
            for (int i = 0; i < foundNodes.Length; i++)
            {
                if (foundNodes[i] != null)
                {
                    if (DistanceBetweenTwoNodes(foundNodes[i].transform.position, targetTile) < shortest)
                    {
                        shortest = DistanceBetweenTwoNodes(foundNodes[i].transform.position, targetTile);
                        moveTo = foundNodes[i];
                        direction = possiblePaths[i];
                    }
                }
            }
        }


        return moveTo;
    }
}
