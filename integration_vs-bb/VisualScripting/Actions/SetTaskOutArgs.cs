using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("BB Task Actions")]
[UnitTitle("Set Task Output Arguments")]
public class SetTaskOutArgs : Unit
{
	[DoNotSerialize, PortLabelHidden]
	private ControlInput _in;
	[DoNotSerialize, PortLabelHidden]
	private ControlOutput _out;


	[SerializeAs(nameof(ArgsCount))]
	private int _argsCount = 0;

	[DoNotSerialize]
	[Inspectable, UnitHeaderInspectable("Arguments Count")]
	public int ArgsCount
	{
		get => _argsCount;
		set => _argsCount = Mathf.Clamp(value, 0, Constants.MAX_ARGS);
	}


	private List<ValueInput> _valueInputs = new();

	protected override void Definition()
	{

		if (ArgsCount != _valueInputs.Count)
		{
			valueInputs.Clear();
			_valueInputs.Clear();
			for (int i = 0; i < ArgsCount; i++)
			{
				var input = ValueInput<object>($"Arg{i}");
				_valueInputs.Add(input);
			}
		}
		_out = ControlOutput("out");

		_in = ControlInput("in", (flow) =>
		{
			object[] args = new object[ArgsCount];
			for (int i = 0; i < ArgsCount; i++)
			{
				args[i] = flow.GetValue<object>(_valueInputs[i]);
			}

			EventHook hook = new EventHook(Constants.EventTriggers.SetOutTaskArgs, flow.stack.machine);

			EventBus.Trigger(hook, new UpdateArguments(args));

			return _out;
		});
	}
}
