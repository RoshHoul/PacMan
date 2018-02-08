using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : BaseGhost {

    public Node startingNode;
    public Node myCornerNode;
    public bool cruiseLeroy = false;
    public int cruiseLeroyCount;
    


	// Use this for initialization
	protected override void Start () {
        base.Start();
    }
	
	// Update is called once per frame
	protected override void Update () {
        targetTile = UpdateTarget();
        base.Update();
	}

    public override void Init(float speed, float fright, float tunnel, float frightDur, int releaseCounter)
    {
        base.Init(speed, fright, tunnel, frightDur, releaseCounter);
        
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
            targTile = new Vector2(Mathf.RoundToInt(pacManPos.x), Mathf.RoundToInt(pacManPos.y));
        }

        else if (GetState() == GhostState.Scatter)
        {
            Vector2 myCorner = myCornerNode.transform.position;
            targTile = myCorner;
        }
        //else if (GetState() == GhostState.Frightened)
        //{
        //    targetTile = RandomMovement();
        //}

        //else if (GetState() == GhostState.Consumed)
        //{
        //    Debug.Log("BLABLBALBALBA");
        //    targTile = ghostHouse.transform.position;
        //    Debug.Log(targTile);
        //}

        
        return targTile;
    }

}
