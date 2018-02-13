using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGhost : MonoBehaviour
{
    public enum GhostState
    {
        Chase,
        Scatter,
        Frightened,
        Consumed
    };

    public Node ghostHouse;

    public Sprite eyesUp;
    public Sprite eyesDown;
    public Sprite eyesLeft;
    public Sprite eyesRight;

    public bool resetLevel;

    private float ghostSpeed;
    private float ghostTunnelSpeed;
    private float ghostFrightSpeed;

    private bool inTunnel = false;

    public float speed, consumedSpeed;
    private float maxSpeed = 5;
    public GhostState currentState = GhostState.Scatter;
    public GhostState prevState;

    public Node prevNode, currentNode, nextNode;


    public int ScatterModeTimer1 = 7;
    public int ScatterModeTimer2 = 5;
    public float ScatterModeTimer3 = 1 / 60;
    public int ChaseModeTimer = 20;

    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController ghostBlue;
    public RuntimeAnimatorController ghostWhite;

    public bool inGhostHouse;

    protected Vector2 direction;
    protected Vector2 nextDirection;
    protected Vector2 targetTile;

    private int modeChangeIteration = 0;
    private float modeChangeTimer = 0f;

    private float frightenedModeTimer = 0f;
    private float blinkTimer = 0;

    protected float frightenedModeDuration = 3;
    protected float startBlinkingAt = 7;

    private bool frightenedModeIsWhite = false;


    private GameBoard GM;
    public GameObject pacMan;
    private Controller pacManScript;

    protected virtual void Start()
    {
        //Init();
    }

    protected virtual void Update()
    {
        Move();
        CheckCollision();
        UpdateState();
        CheckIsInGhostHouse();
    }

    void CheckIsInGhostHouse()
    {
        if (GetState() == GhostState.Consumed)
        {
            GameObject tile = GetTileAtPos(transform.position);

            if (tile != null)
            {
                if (tile.GetComponent<Tile>() != null)
                {
                    if (tile.transform.GetComponent<Tile>().isHouse)
                    {
                        speed = ghostSpeed;
                        Node node = GetNodeAtPosition(transform.position);

                        if (node != null)
                        {
                            currentNode = node;
                            direction = Vector2.up;
                            nextNode = currentNode.neighbours[0];

                            prevNode = currentNode;
                            currentNode = null;
                            SetState(GhostState.Chase);
                            inGhostHouse = false;
                            UpdateAnimatorController();
                        }
                    }
                }
            }
        }
    }

    public virtual void Init(float defaultSpeed, float defFrightSpeed, float defTunnelSpeed, float frightDur)
    {

        pacMan = GameObject.Find("Pac Man");
        pacManScript = pacMan.GetComponent<Controller>();
        GM = GameObject.Find("_GM").GetComponent<GameBoard>();


        UpdateAnimatorController();
        modeChangeIteration = 0;

        consumedSpeed = 15;
        ghostSpeed = maxSpeed - defaultSpeed;
        ghostFrightSpeed = maxSpeed - defFrightSpeed;
        ghostTunnelSpeed = maxSpeed - defTunnelSpeed;
        speed = ghostSpeed;
        frightenedModeDuration = frightDur;
        startBlinkingAt = frightenedModeDuration - 1.5f;
        SetState(GhostState.Scatter);
        Debug.Log("me:" + transform.name +  " cn " + currentNode + " pn " + prevNode + " nn " + nextNode);
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

    GameObject GetTileAtPos(Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GM.GetComponent<GameBoard>().board[tileX, tileY];

        return tile;
    }

    public Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GM.board[(int)pos.x, (int)pos.y];

        if (tile != null)
            return tile.GetComponent<Node>();


        return null;
    }

    public void Move()
    {

        if (nextNode != currentNode && nextNode != null && !inGhostHouse)
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

                if (nextNode != null)
                {

                    GameObject nextPortal = GetPortal(nextNode.transform.position);

                    if (nextPortal != null)
                    {
                        speed = ghostTunnelSpeed;
                        inTunnel = true;
                    }
                    else if ((nextPortal == null) && (portal == null) && inTunnel)
                    {
                        inTunnel = false;
                        speed = ghostSpeed;
                    }
                    prevNode = currentNode;
                    currentNode = null;

                    UpdateAnimatorController();
                }
            }
            else
            {
                transform.localPosition += (Vector3)direction * speed * Time.deltaTime;
            }
        }

        if ((!inGhostHouse && nextNode == null))
        {
            nextNode = SwitchDirection();
            direction = direction * -1;
            transform.localPosition += (Vector3)direction * speed * Time.deltaTime;
        }

        //if (GetState() == GhostState.Frightened && !inGhostHouse)
        //{
        //    targetTile = RandomMovement();
        //}

        //if (GetState() == GhostState.Consumed)
        //{
        //    targetTile = ghostHouse.transform.position;
        //}

    }

    
    public Vector2 RandomMovement()
    {
        int x = Random.Range(0, 28);
        int y = Random.Range(0, 36);

        return new Vector2(x, y);
    }

    public Node SwitchDirection()
    {
        Node moveTo = null;
        for (int i = 0; i < prevNode.neighbours.Length; i++)
        {
            if (prevNode.possiblePaths[i] == direction * -1)
            {
                moveTo = prevNode.neighbours[i];
            }
        }
        return moveTo;
    }

    public Node CanMove()
    {
        if (GetState() == GhostState.Frightened && !inGhostHouse)
        {
            targetTile = RandomMovement();
        }

        else if (GetState() == GhostState.Consumed)
        {
            targetTile = ghostHouse.transform.position;
        }

            Node moveTo = null;
        Node[] foundNodes = new Node[4];
        Vector2[] possiblePaths = new Vector2[4];
        int nodeCounter = 0;

        if (currentNode == null)
        {
            currentNode = GetNodeAtPosition(transform.position);
        }

        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if (currentNode.possiblePaths[i] != direction * -1)
            {
                if (currentNode.tag == "PelletsSpecial" && currentNode.possiblePaths[i] == Vector2.up)
                    continue;

                if (GetState() != GhostState.Consumed)
                {
                    GameObject tile = GetTileAtPos(currentNode.transform.position);
                    if (tile.transform.GetComponent<Tile>().isHouseEntry)
                    {
                        if (currentNode.possiblePaths[i] != Vector2.down)
                        {
                            foundNodes[nodeCounter] = currentNode.neighbours[i];
                            possiblePaths[nodeCounter] = currentNode.possiblePaths[i];
                            nodeCounter++;
                        }
                    } else
                    {
                        foundNodes[nodeCounter] = currentNode.neighbours[i];
                        possiblePaths[nodeCounter] = currentNode.possiblePaths[i];
                        nodeCounter++;

                    }
                } else
                {
                    foundNodes[nodeCounter] = currentNode.neighbours[i];
                    possiblePaths[nodeCounter] = currentNode.possiblePaths[i];
                    nodeCounter++;
                }

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

    void CheckCollision()
    {
        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);
        Rect pacManRect = new Rect(pacMan.transform.position, pacMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);

        if (ghostRect.Overlaps(pacManRect))
        {
            if (GetState() == GhostState.Frightened || GetState() == GhostState.Consumed)
            {
                pacMan.GetComponent<Controller>().points += 200;

                SetState(GhostState.Consumed);

                return;
            }
            else
            {
             //   GM.pacManLost = true;
                return;
            }
        }
    }


    void UpdateState()
    {
        if (GetState() != GhostState.Frightened && GetState() != GhostState.Consumed)
        {
            if (!inTunnel)
            {
                speed = ghostSpeed;
            }
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
                    Debug.Log("Tuk 4<");
                }
            }
            else if (modeChangeIteration >= 4)
            {
                Debug.Log("tuk");
                if (GetState() == GhostState.Scatter )
                {
                    SetState(GhostState.Chase);

                    modeChangeTimer = 0;
                }

            }
        }
        else if (GetState() == GhostState.Frightened)
        {
            frightenedModeTimer += Time.deltaTime;
            speed = ghostFrightSpeed;

            if (frightenedModeTimer >= frightenedModeDuration)
            {
                frightenedModeTimer = 0;
                SetState(prevState);
            }

            if (frightenedModeTimer >= startBlinkingAt)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= 0.1f)
                {
                    blinkTimer = 0;
                    if (frightenedModeIsWhite)
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
                        frightenedModeIsWhite = false;
                    }
                    else
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostWhite;
                        frightenedModeIsWhite = true;
                    }

                }
            }

        }
        else if (GetState() == GhostState.Consumed)
        {
            speed = consumedSpeed;
        }
    }

    public void SetToFrightened()
    {
        SetState(GhostState.Frightened);
    }

    void SetState(GhostState newState)
    {
        if (newState == GhostState.Frightened )
        {
            speed = ghostFrightSpeed;
            prevState = currentState;

        }

        if (newState == GhostState.Consumed)
        {
            speed = consumedSpeed;
        }

        if (newState != GhostState.Frightened && newState != GhostState.Consumed)
        {
            speed = ghostSpeed;

            prevState = currentState;
        }

        //Change direction conditions
        if (!inGhostHouse || !(prevState == GhostState.Frightened) || !(newState == GhostState.Consumed))
        {
            prevNode = nextNode;
            nextNode = null;
        }

        currentState = newState;
        UpdateAnimatorController();

    }

    public GhostState GetState()
    {
        return currentState;

    }

    void UpdateAnimatorController()
    {
        if (GetState() != GhostState.Frightened && GetState() != GhostState.Consumed)
        {
            if (direction == Vector2.up)
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;
            if (direction == Vector2.down)
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
            if (direction == Vector2.left)
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            if (direction == Vector2.right)
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
        }

        else if (GetState() == GhostState.Frightened)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostBlue;
        } 
        else if (GetState() == GhostState.Consumed)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = null;
            if (direction == Vector2.up)
                transform.GetComponent<SpriteRenderer>().sprite = eyesUp;
            if (direction == Vector2.left)
                transform.GetComponent<SpriteRenderer>().sprite = eyesLeft;
            if (direction == Vector2.down)
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            if (direction == Vector2.right)
                transform.GetComponent<SpriteRenderer>().sprite = eyesRight;
        }
    }


}