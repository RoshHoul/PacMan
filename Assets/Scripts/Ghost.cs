using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    public int speed = 5;
    public enum GhostState { Chase,
                             Scatter,
                             Frightened};

    public GhostState currentState = GhostState.Scatter;
    public GhostState prevState;

    public int ScatterModeTimer1 = 7;
    public int ScatterModeTimer2 = 5;
    public int ChaseModeTimer = 20;

    private int modeChangeIteration = 0;
    private float modeChangeTimer = 0f;

    private GameBoard GM;
    public GameObject pacMan;

    public Node currentNode, nextNode,previousNode;
    public Vector2 direction, nextDirection;

	// Use this for initialization
	public void Start () {
        pacMan = GameObject.Find("Pac Man");
        GM = GameObject.Find("_GM").GetComponent<GameBoard>();
        Node node = GetNodeAtPosition(transform.localPosition);
        if (node != null)
            currentNode = node;
        previousNode = currentNode;
        currentState = GhostState.Chase;

        direction = Vector2.left;

	}
	
	// Update is called once per frame
	void Update () {
        Move();
	}

    void UpdateState()
    {
        if (currentState != GhostState.Frightened)
        {
            modeChangeTimer += Time.deltaTime; 

            if (modeChangeIteration < 2)
            {
                if (GetState() == GhostState.Scatter && modeChangeTimer > ScatterModeTimer1)
                {
                    SetState(GhostState.Chase);
                    modeChangeTimer = 0;
                }

                if (GetState() == GhostState.Chase && modeChangeTimer > ChaseModeTimer)
                {
                    modeChangeIteration++;
                    SetState(GhostState.Scatter);
                    modeChangeTimer = 0;
                }
            } else if (modeChangeIteration < 4)
            {
                if (GetState() == GhostState.Scatter && modeChangeTimer > ScatterModeTimer2)
                {
                    SetState(GhostState.Chase);
                    modeChangeTimer = 0;
                } 

                if (GetState() == GhostState.Chase && modeChangeTimer > ChaseModeTimer)
                {
                    modeChangeIteration++;
                    SetState(GhostState.Scatter);
                    modeChangeTimer = 0;
                }
            } else
            {
                if (GetState() == GhostState.Scatter )
                {
                    SetState(GhostState.Chase);
                    modeChangeTimer = 0;
                } 
            }
        } else
        {
            //TODO:
        }
    }

    void Move()
    {
        if (nextNode != currentNode && nextNode != null)
        {
            Debug.Log("Ghost in 1st if");
            if (OverShotTarget())
            {
                Debug.Log("Ghost in OST");
                currentNode = nextNode;
                transform.localPosition = currentNode.transform.position;

                GameObject portal = GetPortal(currentNode.transform.position);

                if (portal != null)
                {
                    transform.localPosition = portal.transform.position;
                    currentNode = portal.GetComponent<Node>();
                }

                nextNode = CanMove();

                previousNode = currentNode;
                currentNode = null;

            }
            else
            {
                transform.localPosition += (Vector3)direction * speed * Time.deltaTime;
            }
        }
    }

    public GameObject GetPortal(Vector2 pos)
    {
        GameObject tile = GM.board[(int)pos.x, (int)pos.y];

        if (tile.GetComponent<Tile>() != null)
        {

            if (tile.GetComponent<Tile>().isPortal)
            {
                GameObject otherPortal = tile.GetComponent<Tile>().portalReciever;
                return otherPortal;
            }
        }
        return null;
    }

    public Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GM.board[(int)pos.x, (int)pos.y];

        if (tile != null)
            return tile.GetComponent<Node>();


        return null;
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

    public bool OverShotTarget()
    {
        float nodeToTarget = DistanceFromNode(nextNode.transform.position);
        float nodeToSelf = DistanceFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float DistanceFromNode(Vector2 targetPos)
    {
        Vector2 vec = targetPos - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    float DistanceBetweenTwoNodes(Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        return distance;
    }

    void SetState(GhostState newState)
    {
        prevState = currentState;
        currentState = newState;
    }

    GhostState GetState()
    {
        return currentState;
    }
}
