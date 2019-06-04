using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRotater : MonoBehaviour
{
    GameObject _AStar;
    Pathfinding.NavMeshGraph graph;

    float _TICK_TIME = 0.5f;
    float currentTickTime = 0.0f;
    bool needsScanning = false;

    // Use this for initialization
    void Start()
    {
        _AStar = GameObject.Find("A*");
        graph = _AStar.GetComponent<AstarPath>().graphs[0] as Pathfinding.NavMeshGraph;
    }

    // Update is called once per frame
    void Update()
    {
        currentTickTime += Time.deltaTime;
        rotateWorld();


        if (currentTickTime >= _TICK_TIME && needsScanning)
        {
            currentTickTime = 0.0f;
            _AStar.GetComponent<AstarPath>().Scan();
            needsScanning = false;
        }
    }

    void rotateWorld()
    {
        Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        float leftRotate = Time.deltaTime * leftStick.x * -30;
        float rightRotate = Time.deltaTime * leftStick.y * 30;

        if (leftRotate != 0.0f || rightRotate != 0.0f) {
            Vector3 before = transform.rotation.eulerAngles;
            transform.Rotate(0, leftRotate, rightRotate, Space.World);
            Vector3 after = transform.rotation.eulerAngles;

            // TODO TIM: fix so it rotates correctly. Obviously the graph reads rotation in a (somewhat) different way
            //graph.rotation += new Vector3(-rightRotate, leftRotate, 0);
            graph.rotation += after - before;
            needsScanning = true;
        }
    }
}
