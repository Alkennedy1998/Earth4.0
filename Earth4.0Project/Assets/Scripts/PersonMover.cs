using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMover : MonoBehaviour {
    private Vector3 target;
    private int fatigue;
    private Vector3 homeLocation;
    private Vector3 workLocation;
    public Transform Pillar;
    public Transform Factory;

	// Use this for initialization
	void Start () {
        homeLocation = randomVector();
        Instantiate(Pillar, homeLocation, Quaternion.identity);
        workLocation = randomVector();
        Instantiate(Factory, workLocation, Quaternion.identity);

        fatigue = 0;
        target = workLocation;
        StartCoroutine("UpdatePerson");
        // StartCoroutine("RandomizeTarget");
	}
	
	// Update is called once per frame
	void Update () {
        if (fatigue >= 100)
        {
            target = homeLocation;
        }
        else if (fatigue <= 0)
        {
            target = workLocation;
        }

        Vector3 current = transform.position;
        Vector3 direction = target - current;

        if (direction.magnitude > 0.1f)  // Stop if close enough to target
        {
            Vector3 tangentDirection = Vector3.Normalize(  // Get vector in direction of `direction` but tangent to the globe at the object
                Vector3.Dot(transform.forward, direction) * transform.forward +  // Forward component
                Vector3.Dot(transform.right, direction) * transform.right        // Right component
            );
            float angle = Vector3.Angle(transform.forward, tangentDirection);
            transform.Rotate(0, angle, 0, Space.Self);  // Look in tangentDirection
            transform.RotateAround(Vector3.zero, transform.right, 100 * Time.deltaTime);  // Move to that direction around globe
        }
        
	}
    
    private Vector3 randomVector()
    {
        return 2.6f * Vector3.Normalize(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
    }

    IEnumerator RandomizeTarget()
    {
        for (;;)
        {
            target = randomVector();
            // Instantiate(Pillar, target, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator UpdatePerson()
    {
        for (;;)
        {
            if (Vector3.Distance(transform.position, homeLocation) < 0.2f)
            {
                fatigue = fatigue <= 0 ? 0 : fatigue - 10;
            } else if (Vector3.Distance(transform.position, workLocation) < 0.2f) {
                fatigue = fatigue >= 100 ? 100 : fatigue + 10;
            }
            yield return new WaitForSeconds(.2f);
        }
    }
}
