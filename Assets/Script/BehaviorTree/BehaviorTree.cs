using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
	Success,
	Failure,
	Running
}


[System.Serializable]
public class BehaviorTree
{
	private Dictionary<string, object> data = new Dictionary<string, object>();
	[SerializeField]
	private Leaf topOfTree = null;

	public BehaviorTree(Leaf top)
	{
		topOfTree = top;
	}

	public void Init()
	{
		topOfTree.Init(this);
	}

	public void Process()
	{
		Debug.Log("BT PROCESS");
		topOfTree.Process();
	}
	public Dictionary<string, object> Data
	{
		get { return data; }
		set { data = value; }
	}
}
[System.Serializable]
public abstract class Leaf 
{
	protected BehaviorTree bt;
	public Leaf()
	{

	}

	public virtual void Init(BehaviorTree bt)
	{
		this.bt = bt;
	}
	public abstract ResultType Process();
}

[System.Serializable]
public abstract class Node : Leaf
{
	[SerializeField]
	protected List<Leaf> children = new List<Leaf>();
	public Node(params Leaf[] nodes)
	{
		foreach (var newNode in nodes)
		{
			children.Add(newNode);
		}
	}

	public override void Init(BehaviorTree bt)
	{
		base.Init(bt);
		foreach (var child in children)
		{
			child.Init(bt);
		}
	}
}

[System.Serializable]
public class Sequencer : Node
{
	private int index = 0;

	public Sequencer(params Leaf[] leafs) : base(leafs)
	{

	}
	public override ResultType Process()
	{
		if (children.Count == 0)
			return ResultType.Success;
		switch(children[index].Process())
		{
			case ResultType.Failure:
				index = 0;
				return ResultType.Failure;
			case ResultType.Success:
				index++;
				if (index == children.Count)
				{
					index = 0;
					return ResultType.Success;
				}
				break;
		}
		return ResultType.Running;
	}
}

[System.Serializable]
public class Selector : Node
{
	private int index = 0;
	public Selector(params Leaf[] leafs) : base(leafs)
	{

	}
	public override ResultType Process()
	{
		if (children.Count == 0)
			return ResultType.Success;
		switch (children[index].Process())
		{
			case ResultType.Failure:
				index++;
				if (index == children.Count)
				{
					index = 0;
					return ResultType.Failure;
				}
				break;
			case ResultType.Success:
				index = 0;
				return ResultType.Success;
		}
		return ResultType.Running;
	}
}

[System.Serializable]
public abstract class Decorator : Leaf
{
	[SerializeField]
	protected Leaf child;

	public Decorator(Leaf child)
	{
		this.child = child;
	}

	public override void Init(BehaviorTree bt)
	{
		base.Init(bt);
		child.Init(bt);
	}
}


[System.Serializable]
public class Repeater : Decorator
{
	public override ResultType Process()
	{
		child.Process();
		return ResultType.Running;
	}

	public Repeater(Leaf child) : base(child)
	{
	}
}

[System.Serializable]
public class RepeatUntilFail : Decorator
{
	public override ResultType Process()
	{
		switch (child.Process())
		{
			case ResultType.Failure:
				return ResultType.Failure;
		}

		return ResultType.Running;
	}

	public RepeatUntilFail(Leaf child) : base(child)
	{
	}
}

[System.Serializable]
public class Inverter : Decorator
{
	public override ResultType Process()
	{
		switch (child.Process())
		{
			case ResultType.Failure:
				return ResultType.Success;
			case ResultType.Success:
				return ResultType.Failure;
		}

		return ResultType.Running;
	}

	public Inverter(Leaf child) : base(child)
	{
	}
}