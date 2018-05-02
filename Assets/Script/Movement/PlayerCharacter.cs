using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
	public Rigidbody2D Body { get; private set; }

	[SerializeField] private float speed;
	// Use this for initialization
	void Start ()
	{
		Body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		float horizontal = Input.GetAxisRaw("Horizontal");
		float vertical = Input.GetAxisRaw("Vertical");
		Vector2 direction = new Vector2(horizontal, vertical);
		if (direction.magnitude > 1.0f)
		{
			direction = direction.normalized;
		}
		Body.velocity = speed * direction;

	}

}
