using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingPlacementValidator : MonoBehaviour
{

    private GameObject world;
    // Use this for initialization
    void Start()
    {
        world = GameObject.Find("hand_right");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag == "Building")
            world.GetComponent<BuildingPlacerRay>()._buildingCollisions++;
    }
    void OnTriggerExit(Collider obj)
    {
        if (obj.tag == "Building")
            world.GetComponent<BuildingPlacerRay>()._buildingCollisions--;
    }
}
