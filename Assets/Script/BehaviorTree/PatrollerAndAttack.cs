using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
class Patrol : Leaf
{
	private PatrollerAndAttack bot;
	Vector3[] points;
	private int index = 0;
	private float radius;
	public Patrol(Vector3[] points)
	{
		this.points = points;
	}
	public override ResultType Process()
	{
		Debug.Log("Coucou, tu veux voir mon cours de physique");
		if (points.Length == 0)
		{
			return ResultType.Success;
		}

		bt.Data["PatrolNextPos"] = points[index];
		Debug.Log("Next Pos "+points[index]);
		Debug.Log("Dict Next Pos: "+((Vector3)bt.Data["PatrolNextPos"]));
		index++;
		if (index == points.Length)
		{
			index = 0;
		}
		return ResultType.Success;
	}
}

[System.Serializable]
class MoveToPosition : Leaf
{
	private string dataKey;
	private Transform botTransform;
	private float speed;
	private float radius;
	public MoveToPosition(string dataKey, float radius = 0.5f)
	{
		this.radius = radius;
		this.dataKey = dataKey;
		
	}

	public override void Init(BehaviorTree bt)
	{
		base.Init(bt);
		botTransform = ((PatrollerAndAttack)bt.Data["Bot"]).transform;
		speed = ((PatrollerAndAttack)bt.Data["Bot"]).Speed;
	}

	public override ResultType Process()
	{
		Debug.Log(bt.Data.Keys.Aggregate((key, text) => text +" "+ key.ToString() ));
		Debug.Log("Data Key: "+dataKey);
		Vector3 position = (Vector3) bt.Data[dataKey];
		Vector3 deltaPos = (position - botTransform.position);

		Debug.Log("DeltaPos: " + deltaPos);
		if (deltaPos.magnitude < radius)
		{
			return ResultType.Success;
		}
		botTransform.position += deltaPos.normalized * speed*Time.deltaTime;
		return ResultType.Failure;
	}
}

[System.Serializable]
class Attack : Leaf
{
	private PlayerCharacter pCharacter = null;
	public Attack(PlayerCharacter pChar)
	{
		pCharacter = pChar;
	}
	public override ResultType Process()
	{
		if (pCharacter == null)
			return ResultType.Failure;

		return ResultType.Failure;
	}
}

[System.Serializable]
class PlayerIsSpotted : Leaf
{
	private Transform player;
	private Transform bot;
	private float scope;
	public PlayerIsSpotted(Transform player,  float scope = 2.5f)
	{
		this.scope = scope;
		this.player = player;
	}

	public override void Init(BehaviorTree bt)
	{
		base.Init(bt);

		bot = ((PatrollerAndAttack)bt.Data["Bot"]).transform;
	}

	public override ResultType Process()
	{
		if ((player.position - bot.position).magnitude < scope)
		{
			bt.Data["SpottedPlace"] = player.position;
			bt.Data["BotPosition"] = bot.position;
			return ResultType.Success;
		}
		return ResultType.Failure;
	}
}

public class PatrollerAndAttack : MonoBehaviour
{
	[SerializeField]
	private PlayerCharacter pChar;
	[SerializeField]
	private BehaviorTree bt;
	[SerializeField] private float speed = 4.0f;
	[SerializeField] private Transform[] patrolPoints;

	public float Speed
	{
		get { return speed; }
		set { speed = value; }
	}

	// Use this for initialization
	void Start ()
	{
		
		bt = new BehaviorTree(
			new Repeater(
				new Sequencer(
					new Selector(
						//new PlayerIsSpotted(pChar.transform, transform), 
						new Inverter(
							new Sequencer(
								new MoveToPosition("PatrolNextPos"), 
								new Patrol(patrolPoints.Select(patrolPointObject => patrolPointObject.position).ToArray())
							)
						)
					)
				/*, new Sequencer(
					new MoveToPosition("SpottedPlace"),
					new Selector(
						new PlayerIsSpotted(pChar.transform),
						new Inverter(
							new MoveToPosition("BotPosition")
						)
					),
					new Attack(pChar),
					new MoveToPosition("OriginPos")
				)*/
				)

			)
		);
		bt.Data["OriginPos"] = transform.position;
		bt.Data["PatrolNextPos"] = transform.position;
		bt.Data["Bot"] = this;

		bt.Init();
	}
	
	// Update is called once per frame
	void Update ()
	{
		bt.Process();
	}
}
