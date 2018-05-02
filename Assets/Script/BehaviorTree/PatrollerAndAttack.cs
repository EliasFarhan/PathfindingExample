using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Patrol : Leaf
{
	public override ResultType Process()
	{
		return ResultType.Failure;
	}
}

class MoveToPosition : Leaf
{
	private string dataKey;
	public MoveToPosition(string dataKey)
	{
		this.dataKey = dataKey;
	}
	public override ResultType Process()
	{
		Vector3 position = (Vector3) bt.Data[dataKey];
		return ResultType.Failure;
	}
}

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

class PlayerIsSpotted : Leaf
{
	private Transform player;
	private Transform bot;
	private float scope;
	public PlayerIsSpotted(Transform player, Transform bot, float scope = 2.5f)
	{
		this.scope = scope;
		this.player = player;
		this.bot = bot;
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
	private PlayerCharacter pChar;

	private BehaviorTree bt;
	// Use this for initialization
	void Start ()
	{
		bt = new BehaviorTree(
			new Repeater(
				new Sequencer(
					new Selector(
						new PlayerIsSpotted(pChar.transform, transform), 
						new Inverter(
							new Patrol()
						)
					), 
					new Sequencer(
						new MoveToPosition("SpottedPlace"),
						new Selector(
							new PlayerIsSpotted(pChar.transform, transform),
							new Inverter(
								new MoveToPosition("BotPosition")
							)
						),
						new Attack(pChar)
					)
				)
			)
		);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
