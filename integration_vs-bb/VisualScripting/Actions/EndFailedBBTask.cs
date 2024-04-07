using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("BB Task Actions")]
[UnitTitle("End Failed Task")]
public class EndFailedBBTask : Unit
{
	[DoNotSerialize]
	private ControlInput _in;

	[DoNotSerialize]
	private ControlOutput _out;


	protected override void Definition()
	{
		_out = ControlOutput("Then");

		_in = ControlInput("End Failed Task", (flow) =>
		{
			EventHook hook = new EventHook(Constants.EventTriggers.EndTaskFailed, flow.stack.machine);

			EventBus.Trigger(hook);

			return _out;
		});
	}
}
