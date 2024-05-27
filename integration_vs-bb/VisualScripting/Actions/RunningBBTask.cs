using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[UnitCategory("BB Task Actions")]
[UnitTitle("Running BB Task")]
public class RunningBBTask : Unit
{
	[DoNotSerialize]
	private ControlInput _in;

	[DoNotSerialize]
	private ControlOutput _out;

	protected override void Definition()
	{
		_out = ControlOutput("Then");

		_in = ControlInput("Abort Task", (flow) =>
		{
			EventHook hook = new EventHook(Constants.EventTriggers.AbortTask, flow.stack.machine);

			EventBus.Trigger(hook);

			return _out;
		});
	}
}
