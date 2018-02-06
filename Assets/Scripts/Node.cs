using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public Node[] neighbours;
    public Vector2[] possiblePaths;

    private void Start()
    {
        possiblePaths = new Vector2[neighbours.Length];

        for (int i = 0; i < neighbours.Length; i++)
        {
            Node neighbour = neighbours[i];
            Vector2 tempVector = neighbour.transform.localPosition - transform.localPosition;

            possiblePaths[i] = tempVector.normalized;
        }
    }
    // Update is called once per frame
    void Update () {
		
	}
}
