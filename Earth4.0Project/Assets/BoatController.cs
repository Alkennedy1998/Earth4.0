using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour {

    private const float _HIT_RADIUS = 0.05f;
    private const float _SPEED = 10.0f;

    private GameObject _targetObject;
    private GameObject _ocean;
    private float _oceanRadius = 0.09308953f;    


	// Use this for initialization
	void Start () {
        _ocean = GameObject.Find("Ocean");
        //SphereCollider sphere = _ocean.GetComponent<SphereCollider>();
        //_oceanRadius = sphere.radius;

        setNewRandomTarget();
    }
	
	// Update is called once per frame
	void Update () {
        int count = 0;
        count++;

        if (count > 100)
        {
            Debug.Log(count);
            count = 0;
            setNewRandomTarget();
        }
    }

    private void moveTowardsLocation(Vector3 target)
    {
        Vector3 current = transform.position;
        Vector3 direction = target - current;

        if (direction.magnitude > _HIT_RADIUS)
        {  // Stop if close enough to target
            Vector3 tangentDirection = Vector3.Normalize(  // Get vector in direction of `direction` but tangent to the globe at the object
                Vector3.Dot(transform.forward, direction) * transform.forward +  // Forward component
                Vector3.Dot(transform.right, direction) * transform.right        // Right component
            );
            float angle = Vector3.Angle(transform.forward, tangentDirection);
            transform.Rotate(0, angle, 0, Space.Self);  // Look in tangentDirection
            transform.RotateAround(_ocean.transform.position, transform.right, _SPEED * Time.deltaTime);  // Move to that direction around globe
        }
    }

    private bool atTarget()
    {
        if (_targetObject == null)
            return false;
        return Vector3.Distance(transform.position, _targetObject.transform.position) <= 10 * _HIT_RADIUS;
    }

    private void setNewRandomTarget()
    {
        GameObject.Destroy(_targetObject);

        _targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //_targetObject.transform.localScale = new Vector3(.5f, 5f, .5f);
        _targetObject.transform.position = randomVector();
    }

    private Vector3 randomVector()
    {
        Vector3 position;
        position = Random.onUnitSphere;
        position *= _oceanRadius;
        position += _ocean.transform.position;
        return position;
    }

    private Vector3 normalizeVectorToSphere(Vector3 vector)
    {
        return (
            _ocean.transform.position +
            _oceanRadius *
            Vector3.Normalize(vector)
        );
    }
}
