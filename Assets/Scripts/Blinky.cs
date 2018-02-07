using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : BaseGhost {

    public Node startingNode;
    public Node myCornerNode;
    public bool inStartingPosition;

	// Use this for initialization
	protected override void Start () {
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

        if (inStartingPosition)
        {
            direction = Vector2.left;
            nextNode = CanMove();
        }

        prevNode = currentNode;

    }
	
	// Update is called once per frame
	protected override void Update () {
        targetTile = UpdateTarget();
        base.Update();
        Debug.Log("currentState " + GetState());
	}

    public Vector2 UpdateTarget()
    {
        Vector2 targTile = new Vector2();
        if (GetState() == GhostState.Chase)
        {
            Vector2 pacManPos = pacMan.transform.position;
            targTile = new Vector2(Mathf.RoundToInt(pacManPos.x), Mathf.RoundToInt(pacManPos.y));
        }

        if (GetState() == GhostState.Scatter)
        {
            Debug.Log("Blabla");
            Vector2 myCorner = myCornerNode.transform.position;
            targTile = myCorner;
        }

        return targTile;
    }

}
