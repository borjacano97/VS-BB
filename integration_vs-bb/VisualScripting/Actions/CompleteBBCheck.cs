using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[UnitCategory("BB Check Conditional")]
[UnitTitle("Complete BB Check")]
public class CompleteBBCheck : Unit
{
	[DoNotSerialize, PortLabelHidden]
	private ControlInput _in;
	[DoNotSerialize, PortLabelHidden]
	private ControlOutput _out;


	private ValueInput _check;

	protected override void Definition()
	{
		_out = ControlOutput("out");
		_check = ValueInput<bool>("Check");
		_in = ControlInput("in", (flow) =>
		{
			EventHook hook = new EventHook(Constants.EventTriggers.CompleteCheck, flow.stack.machine);

			EventBus.Trigger(hook, flow.GetValue<bool>(_check));

			return _out;
		});
	}
}
