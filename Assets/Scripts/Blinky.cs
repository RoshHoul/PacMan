using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinky : Ghost {

	// Use this for initialization
	void Start () {
		
	}


	
	// Update is called once per frame
	void Update () {
		
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
}
