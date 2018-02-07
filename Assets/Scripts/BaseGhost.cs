using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGhost : MonoBehaviour
{
    public enum GhostState
    {
        Chase,
        Scatter,
        Frightened
    };

    public int speed = 5;
    public GhostState currentState = GhostState.Scatter;
    public GhostState prevState;

    protected Node prevNode, currentNode, nextNode;

    public float myReleaseTimer = 5.0f;

    public int ScatterModeTimer1 = 7;
    public int ScatterModeTimer2 = 5;
    public int ChaseModeTimer = 20;

    public bool inGhostHouse;

    protected Vector2 direction;
    protected Vector2 nextDirection;
    protected Vector2 targetTile;

    private int modeChangeIteration = 0;
    private float modeChangeTimer = 0f;

    private float ghostReleaseTimer = 0.0f;

    private GameBoard GM;
    public GameObject pacMan;
    private bool stateChange = false;

    protected virtual void Start()
    {
        pacMan = GameObject.Find("Pac Man");
        GM = GameObject.Find("_GM").GetComponent<GameBoard>();
        currentState = GhostState.Scatter;
    }

    protected virtual void Update()
    {
        ReleaseGhost();
        UpdateState();
        Move();

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

    void Move()
    {
        if (nextNode != currentNode && nextNode != null && !inGhostHouse)
        {
            Debug.Log(transform.gameObject.name + " Is ready");
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
        } // TUK PRAVIM ELSE IF ZA SMQNA NA STATE-A
    }

    public Node CanMove()
    {
        Node moveTo = null;
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

    public bool OverShotTarget(Node nextNode, Node prevNode)
    {
        float nodeToTarget = DistanceFromNode(nextNode.transform.position, prevNode);
        float nodeToSelf = DistanceFromNode(transform.position, prevNode);

        return nodeToSelf > nodeToTarget;
    }

    protected float DistanceFromNode(Vector2 targetPos, Node prevNode)
    {
        Vector2 vec = targetPos - (Vector2)prevNode.transform.position;
        return vec.sqrMagnitude;
    }

    protected float DistanceBetweenTwoNodes(Vector2 posA, Vector2 posB)
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt(dx * dx + dy * dy);

        return distance;
    }

    private void ReleaseGhost()
    {
        ghostReleaseTimer += Time.deltaTime;

        if (ghostReleaseTimer > myReleaseTimer && inGhostHouse)
        {
            inGhostHouse = false;
        }
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
            }
            else if (modeChangeIteration < 4)
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
            }
            else
            {
                if (GetState() == GhostState.Scatter)
                {
                    SetState(GhostState.Chase);
                    modeChangeTimer = 0;
                }
            }
        }
        else
        {
            //TODO:
        }
    }

    void SetState(GhostState newState)
    {
            prevState = currentState;
            currentState = newState;
    }

    public GhostState GetState()
    {
        return currentState;

    }


}