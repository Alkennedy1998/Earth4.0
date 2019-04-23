using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMover : MonoBehaviour {
    private Vector3 _target;
    private int _fatigue;
    private float _speed;

    private Vector3 _homeLocation, _workLocation;

    private GameObject _world;
    public Transform _pillar;
    public Transform _factory;

	// Use this for initialization
	void Start () {
        _world = GameObject.Find("World");

        _homeLocation = randomVector();
        _workLocation = randomVector();

        _fatigue = 0;
        _speed = 100.0f;
        _target = _workLocation;

        Instantiate(_pillar, _homeLocation, Quaternion.identity);
        Instantiate(_factory, _workLocation, Quaternion.identity);

        StartCoroutine("UpdatePerson");
        // StartCoroutine("RandomizeTarget");
	}

	// Update is called once per frame
	void Update () {
        _target = getTargetLocation();
        moveTowardsLocation(_target);
	}

    private void moveTowardsLocation(Vector3 target) {
        Vector3 current = transform.position;
        Vector3 direction = target - current;

        if (direction.magnitude > 0.1f) {  // Stop if close enough to target
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
            targetLocation = _homeLocation;
        } else if (_fatigue <= 0) {
            targetLocation = _workLocation;
        } else {
            targetLocation = new Vector3(0.0f, 0.0f, 0.0f);
        }

        return targetLocation;
    }

    private Vector3 randomVector() {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        return (
            _world.transform.position +
            // _world.GetComponent<SphereCollider>().radius *
            2.6f *
            Vector3.Normalize(randomDirection)
        );
    }

    IEnumerator RandomizeTarget() {
        for (;;) {
            _target = randomVector();
            // Instantiate(Pillar, target, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator UpdatePerson() {
        for (;;) {
            if (Vector3.Distance(transform.position, _homeLocation) < 0.2f) {
                _fatigue = _fatigue <= 0 ? 0 : _fatigue - 10;
            } else if (Vector3.Distance(transform.position, _workLocation) < 0.2f) {
                _fatigue = _fatigue >= 100 ? 100 : _fatigue + 10;
            }
            yield return new WaitForSeconds(.2f);
        }
    }
}
