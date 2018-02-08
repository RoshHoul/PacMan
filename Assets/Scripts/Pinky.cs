using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : BaseGhost {

    public Node startingNode;
    public Node myCornerNode;
 
    public float myReleaseTimer = 5.0f;
    private float ghostReleaseTimer = 0.0f;

    // Use this for initialization
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        targetTile = UpdateTarget();
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
            targTile = pacManTile + (4 * pacManForward);
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
