using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FollowerMovement
{
	Lerp,
	SeekAndArrive,
	PursueLerp,
	PursueSeekAndArrive
}

public class Follower : MonoBehaviour
{

	[SerializeField] private FollowerMovement followerMovement = FollowerMovement.Lerp;
	protected Rigidbody2D m_Body;
	protected PlayerCharacter pChar = null;
	[SerializeField]
	private float speed = 4.0f;
	[SerializeField]
	private float force = 4.0f;

	[SerializeField] private float capedSpeed = 4.0f;
	// Use this for initialization
	void Start ()
	{
		m_Body = GetComponent<Rigidbody2D>();
		pChar = FindObjectOfType<PlayerCharacter>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		Vector3 direction = pChar.transform.position - transform.position;
		
		switch (followerMovement)
		{
			case FollowerMovement.Lerp:
				direction.Normalize();
				m_Body.velocity = speed * direction;
				break;
			case FollowerMovement.SeekAndArrive:
				direction.Normalize();
				m_Body.AddForce(force*direction);
				if (m_Body.velocity.magnitude > capedSpeed)
				{
					m_Body.velocity = m_Body.velocity.normalized * capedSpeed;
				}
				break;
			case FollowerMovement.PursueLerp:
			{
				float t = direction.magnitude / speed;
				Vector3 newCharPosition = pChar.transform.position.to2() + pChar.Body.velocity * t;
				direction = newCharPosition - transform.position;

				direction.Normalize();
				m_Body.velocity = speed * direction;
			}
				break;
			case FollowerMovement.PursueSeekAndArrive:
			{
				float t = direction.magnitude / capedSpeed;
				Vector3 newCharPosition = pChar.transform.position.to2() + pChar.Body.velocity * t;
				direction = newCharPosition - transform.position;
				direction.Normalize();
				m_Body.AddForce(force * direction);
				if (m_Body.velocity.magnitude > capedSpeed)
				{
					m_Body.velocity = m_Body.velocity.normalized * capedSpeed;
				}
			}
				break;
		}
	}
}
