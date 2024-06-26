
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SearchService;

[UnitCategory("Events/BB/Lifecycle")]
[UnitOrder(3)]
[UnitTitle("On BB Update")]
[TypeIcon(typeof(Update))]
public sealed class OnBBUpdate : MachineEventUnit<UpdateArguments>
{
	protected override string hookName => Constants.EventTriggers.onBBUpdate;

	private List<ValueOutput> _valueOutputs = new();

	protected override void AssignArguments(Flow flow, UpdateArguments args)
	{
		for (int i = 0; i < args.arguments.Length; i++)
			flow.SetValue(_valueOutputs[i], args.arguments[i]);
	}

	[SerializeAs(nameof(ArgsCount))]
	private int _argsCount = 0;


	/// <summary> The number of arguments that this event will receive. </summary>
	[DoNotSerialize]
	[Inspectable, UnitHeaderInspectable("Aguments Count")]
	public int ArgsCount
	{
		get => _argsCount;
		// Clamp the value between 0 and the maximum number of arguments.
		set => _argsCount = Math.Clamp(value, 0, Constants.MAX_ARGS);
	}

	protected override void Definition() 
	{
		base.Definition();

		// If the number of arguments has changed, update the outputs.
		if (ArgsCount != _valueOutputs.Count) 
		{
			valueOutputs.Clear();
			_valueOutputs.Clear();
			// Create an output for each argument.
			for(int i = 0; i < ArgsCount; i++)
			{
				var output = ValueOutput<object>($"Arg{i}");
				_valueOutputs.Add(output);
			}
		}
	}

}
