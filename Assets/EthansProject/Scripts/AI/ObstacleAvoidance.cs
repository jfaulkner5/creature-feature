using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    public float avoidRange = 8f;
    public float avoidanceStrength = 10;

    Transform agent;

    public Vector3 newDir;

    public LayerMask obstacleMask;

    public List<GameObject> currentObstacles = new List<GameObject>();
    // Use this for initialization
    void Start()
    {
        agent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 avoidanceVector = CalculateSeperationVector() * avoidanceStrength;

        if(currentObstacles.Count > 0)
            agent.position += (avoidanceVector) * Time.deltaTime;

        Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + avoidanceVector, Color.blue);

    }

    Vector3 CalculateSeperationVector()
    {
        Vector3 startCenter = Vector3.zero;

        for (int i = 0; i < currentObstacles.Count; i++)
        {
            startCenter += currentObstacles[i].gameObject.transform.position;
        }

        startCenter /= currentObstacles.Count;

        Vector3 seperationVector = (startCenter - transform.position);

        return seperationVector.normalized;
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.layer == obstacleMask)
            currentObstacles.Add(coll.gameObject);
    }

    private void OnCollisionExit(Collision coll)
    {
        if (coll.gameObject.layer == obstacleMask)
            currentObstacles.Remove(coll.gameObject);
    }


}
