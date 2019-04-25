using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMover : MonoBehaviour {
    private Vector3 _target;
    private int _fatigue;
    private float _speed, _hitRadius;
    private float _worldRadius;

    private Vector3 _homeLocation, _farmLocation, _workLocation;

    private GameObject _world;
    public GameObject _homePrefab, _farmPrefab, _factoryPrefab;

	// Use this for initialization
	void Start () {
        _world = GameObject.Find("World");
        _worldRadius = _world.transform.localScale.x * _world.GetComponent<SphereCollider>().radius;

        _homeLocation = randomVector();
        _farmLocation = randomVector();
        _workLocation = randomVector();

        _fatigue = 0;
        _speed = 15.0f;
        _hitRadius = 0.05f;
        _target = _workLocation;

        instantiateObject("HOME", _homeLocation);
        instantiateObject("FARM", _farmLocation);
        instantiateObject("FACTORY", _workLocation);

        StartCoroutine("UpdatePerson");
	}

	// Update is called once per frame
	void Update () {
        _target = getTargetLocation();
        moveTowardsLocation(_target);
	}

    private void moveTowardsLocation(Vector3 target) {
        Vector3 current = transform.position;
        Vector3 direction = target - current;

        if (direction.magnitude > _hitRadius) {  // Stop if close enough to target
            Vector3 tangentDirection = Vector3.Normalize(  // Get vector in direction of `direction` but tangent to the globe at the object
                Vector3.Dot(transform.forward, direction) * transform.forward +  // Forward component
                Vector3.Dot(transform.right, direction) * transform.right        // Right component
            );
            float angle = Vector3.Angle(transform.forward, tangentDirection);
            transform.Rotate(0, angle, 0, Space.Self);  // Look in tangentDirection
            transform.RotateAround(_world.transform.position, transform.right, _speed * Time.deltaTime);  // Move to that direction around globe
        }
    }

    private Vector3 getTargetLocation() {
        Vector3 targetLocation;

        if (_fatigue >= 100) {
            // Return to home
            targetLocation = _homeLocation;
        } else if (_fatigue <= 0) {
            // Ready to work
            if (Random.Range(0, 1) <= 0.5f) {
                targetLocation = _farmLocation;
            } else {
                targetLocation = _workLocation;
            }
        } else {
            targetLocation = _target;
        }

        return targetLocation;
    }

    private Vector3 randomVector() {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        return normalizeVectorToSphere(randomDirection);
    }

    private Vector3 normalizeVectorToSphere(Vector3 vector) {
        return (
            _world.transform.position +
            _worldRadius *
            Vector3.Normalize(vector)
        );
    }

    private void instantiateObject(string modelType, Vector3 location) {
        // modelType should be one of ["HOME", "FARM", "FACTORY"]

        GameObject newItem = null;
        Vector3 normalOffSphere = location - _world.transform.position;
        Quaternion rotation = Quaternion.LookRotation(normalOffSphere);

        if (modelType == "HOME") {
            newItem = Instantiate(_homePrefab, location, rotation);
            _world.GetComponent<GameManager>().addHome(newItem);
        } else if (modelType == "FARM") {
            newItem = Instantiate(_farmPrefab, location, rotation);
            _world.GetComponent<GameManager>().addFarm(newItem);
        } else if (modelType == "FACTORY") {
            newItem = Instantiate(_factoryPrefab, location, rotation);
            _world.GetComponent<GameManager>().addFactory(newItem);
        } else {
            Debug.Log("ERROR: Incorrect modelType in PersonMover script.");
        }

        newItem.transform.Rotate(90, 0, 0);
        newItem.transform.parent = _world.transform;
    }

    IEnumerator RandomizeTarget() {
        for (;;) {
            _target = randomVector();
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator UpdatePerson() {
        for (;;) {
            if (Vector3.Distance(transform.position, _homeLocation) <= _hitRadius) {
                _fatigue = _fatigue <= 0 ? 0 : _fatigue - 10;
            } else if (Vector3.Distance(transform.position, _workLocation) < _hitRadius ||
                       Vector3.Distance(transform.position, _farmLocation) < _hitRadius) {
                _fatigue = _fatigue >= 100 ? 100 : _fatigue + 10;
            }
            yield return new WaitForSeconds(.2f);
        }
    }
}
