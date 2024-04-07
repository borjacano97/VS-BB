using BBUnity.Actions;
using Pada1.BBCore;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("BB Task Actions")]
[UnitTitle("Resume BB Task")]
public class ResumeBBTask : Unit
{
	[DoNotSerialize]
	private ControlInput _in;
	[DoNotSerialize]
	private ControlOutput _out;

	private EventHook? _resumeHook;

	protected override void Definition()
	{
		_out = ControlOutput("");
		_in = ControlInput("", Resume);
	}

	private ControlOutput Resume(Flow flow)
	{
		SetHook(flow.stack.machine);
		EventBus.Trigger(_resumeHook.Value);
		return _out;
	}

	private void SetHook(IMachine machine) 
	{
		_resumeHook ??= new EventHook(Constants.EventTriggers.ResumeTask, machine);
	}

}
