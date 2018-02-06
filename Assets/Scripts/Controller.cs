using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public float speed = 4.0f;

    public Sprite idleSprite;

    [HideInInspector]
    public int pCollected = 0;

    private Animator anim;
    private Sprite currentSprite;

    private Vector2 direction = Vector2.zero;
    private Vector2 nextDirection;
    private Node currentNode, previousNode, targetNode;
    private GameBoard GM;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentSprite = GetComponent<SpriteRenderer>().sprite;
        GM = GameObject.Find("_GM").GetComponent<GameBoard>();
        Node node = GetNodeAtPosition(transform.localPosition);

        if (node != null)
        {
            currentNode = node;
            Debug.Log(currentNode);
        }

        direction = Vector2.zero;
        ChangePosition(direction);
    }

    void Update () {

        TakeInput();
        Move();
        RotateForward();
        UpdateAnimation();
        ConsumePellet();

	}

    void TakeInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangePosition(Vector2.left);
        } else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangePosition(Vector2.up);
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangePosition(Vector2.right);
        } else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangePosition(Vector2.down);
        }
    }

    void ChangePosition(Vector2 dir)
    {
        if (dir != direction)
        {
            nextDirection = dir;
        }

        if (currentNode != null)
        {
            Node moveToNode = CanMove(dir);

            if (moveToNode != null)
            {
                direction = dir;
                targetNode = moveToNode;
                previousNode = currentNode;
            }
        }
    }

    void Move()
    {
        if (targetNode != currentNode && targetNode != null)
        {
            if (nextDirection == direction * -1)
            {
             //   Debug.Log("Change Direction");

                direction = direction * (-1);
                Node temp = targetNode;
                targetNode = previousNode;
                previousNode = temp;
                currentNode = null;

            }

            if (OverShotTarget())
            {
               // Debug.Log("Overshot" + targetNode.name);
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;

                GameObject otherPortal = GetPortal(currentNode.transform.position);

                if (otherPortal != null)
                {
                    transform.localPosition = otherPortal.transform.position;
                    currentNode = otherPortal.GetComponent<Node>();
                }

                Node moveToNode = CanMove(nextDirection);

                if (moveToNode != null)
                {
                    direction = nextDirection;
                }

                if (moveToNode == null)
                {
                    moveToNode = CanMove(direction);
                }

                if (moveToNode != null)
                {
                    targetNode = moveToNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
                else
                {
                    direction = Vector2.zero;
                     
                }
            }
            else
            {
                transform.localPosition += (Vector3)(direction * speed) * Time.deltaTime;
                currentNode = null;
            }
        }
    }

    void MoveToNode(Vector2 dir)
    {
        Node moveToNode = CanMove(dir);

        if (moveToNode != null)
        {
            transform.localPosition = moveToNode.transform.position;
            currentNode = moveToNode;
        }
    }

    void RotateForward()
    {
        Vector3 temp = transform.localScale;

        if (direction == Vector2.left)
        {
           
            temp.x = -1;
            transform.localScale = temp;
            transform.localRotation = Quaternion.Euler(0, 0, 0);

        } else if (direction == Vector2.right)
        {
            temp.x = 1;
            transform.localScale = temp;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        } else if ( direction == Vector2.up) 
        {

            temp.x = 1;
            transform.localScale = temp;
            transform.localRotation = Quaternion.Euler(0, 0, 90);

        } else if (direction == Vector2.down)
        {
            temp.x = 1;
            transform.localScale = temp;
            transform.localRotation = Quaternion.Euler(0, 0, 270);


        }
    }

    void UpdateAnimation()
    {
        if (direction == Vector2.zero)
        {
            currentSprite = idleSprite;
            anim.enabled = false;
        } else
        {
            anim.enabled = true;
        }
    }

    void ConsumePellet()
    {
        GameObject obj = GetTileAtPosition(transform.position);

        if (obj != null)
        {
            Tile tile = obj.GetComponent<Tile>();

            if (tile != null)
            {
                if (!tile.isConsumed && (tile.isPellet || tile.isSuperPellet))
                {
                    obj.GetComponent<SpriteRenderer>().enabled = false;
                    tile.isConsumed = true;
                    pCollected++;
                }
            }
        }
    }

    Node CanMove(Vector2 dir)
    {
        Node moveTo = null;

        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if (currentNode.possiblePaths[i] == dir)
            {
                moveTo = currentNode.neighbours[i];
                break;
            }
        }

        return moveTo;
    }

    Node GetNodeAtPosition (Vector2 pos)
    {
        GameObject tile = GM.board[(int)pos.x, (int)pos.y];

        if (tile != null)
            return tile.GetComponent<Node>();
        

        return null;
    }

    GameObject GetTileAtPosition (Vector2 pos)
    {
        int tileX = Mathf.RoundToInt(pos.x);
        int tileY = Mathf.RoundToInt(pos.y);

        GameObject tile = GM.board[tileX, tileY];

        if (tile != null)
            return tile;

        return null;
    }

    bool OverShotTarget()
    {
        float nodeToTarget = DistanceFromNode(targetNode.transform.position);
        float nodeToSelf = DistanceFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float DistanceFromNode (Vector2 targetPos)
    {
        Vector2 vec = targetPos - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }
    
    GameObject GetPortal(Vector2 pos)
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
}
