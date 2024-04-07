using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("BB Task Actions")]
[UnitTitle("Suspend BB Task")]
public class SuspendBBTask : Unit
{
	[DoNotSerialize]
	private ControlInput _in;
	[DoNotSerialize]
	private ControlOutput _out;

	private EventHook? _suspendHook;

	protected override void Definition()
	{
		_out = ControlOutput("");
		_in = ControlInput("", Suspend);
	}

	private ControlOutput Suspend(Flow flow)
	{
		SetHook(flow.stack.machine);
		EventBus.Trigger(_suspendHook.Value);
		return _out;
	}

	private void SetHook(IMachine machine)
	{
		_suspendHook ??= new EventHook(Constants.EventTriggers.SuspendTask, machine);
	}
}
