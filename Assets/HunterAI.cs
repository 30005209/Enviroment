using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


[RequireComponent(typeof(ColourController))]
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody2D))]
public class HunterAI : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _speed = 200f;

    [SerializeField]
    private float _nextWaypointDistance = 3.0f;

    private Path _path;

    private int _currentWaypoint = 0;

    private bool _reachedEndOfPath = false;

    private Seeker _seeker;

    private Rigidbody2D _rb;


    // Start is called before the first frame update
    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("UpdatePath", 0f, 0.25f);
    }

    void UpdatePath()
    {
        if(_seeker.IsDone())
        {
            _seeker.StartPath(_rb.position, _target.position, OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if(_path != null)
        {
            _reachedEndOfPath =_currentWaypoint >= _path.vectorPath.Count;
            
            Vector2 _direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
            Vector2 _force = _direction * _speed * Time.deltaTime;

            _rb.AddForce(_force);

            float _distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);

            if(_distance < _nextWaypointDistance)
            {
                _currentWaypoint++;
            }
        
        }
    }
}
