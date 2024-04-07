using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[UnitCategory("BB Task Actions")]
[UnitTitle("Complete BB Task")]
public class CompleteBBTask : Unit
{
	[DoNotSerialize, PortLabelHidden]
	private ControlInput _in;
	[DoNotSerialize, PortLabelHidden]
	private ControlOutput _out;



	protected override void Definition()
	{
		_out = ControlOutput("out");
		_in = ControlInput("in", (flow) =>
		{
			EventHook hook = new EventHook(Constants.EventTriggers.CompleteTask, flow.stack.machine);

			EventBus.Trigger(hook);

			return _out;
		});
	}
}
