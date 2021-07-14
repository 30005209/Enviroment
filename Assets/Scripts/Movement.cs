using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [SerializeField]
    [Range(1, 10)]
    private float _speed;
    private Rigidbody2D rb;
    private Vector2 _direction;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float _directionX = Input.GetAxisRaw("Horizontal");
        float _directionY = Input.GetAxisRaw("Vertical");

        _direction = new Vector2(_directionX, _directionY).normalized;

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(_direction.x * _speed, _direction.y * _speed);
        
    }


}
