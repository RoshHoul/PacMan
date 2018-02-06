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

    public GhostState currentState = GhostState.Scatter;
    public GhostState prevState;

    public int ScatterModeTimer1 = 7;
    public int ScatterModeTimer2 = 5;
    public int ChaseModeTimer = 20;

    private int modeChangeIteration = 0;
    private float modeChangeTimer = 0f;


    private GameBoard GM;
    public GameObject pacMan;

    protected virtual void Start()
    {
        pacMan = GameObject.Find("Pac Man");
        GM = GameObject.Find("_GM").GetComponent<GameBoard>();
        currentState = GhostState.Scatter;
    }

    protected virtual void Update()
    {
        UpdateState();
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

    GhostState GetState()
    {
        return currentState;

    }
}