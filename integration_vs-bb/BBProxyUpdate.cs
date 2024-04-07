using BB;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BBProxyUpdate : Unit
{


	//[Inspectable, UnitHeaderInspectable("event name")]
	//public string event_name;

	public BBProxyUpdate() : base()
	{
	}

	public ControlInput _update;
	public ControlOutput _updateOut;

	public bool Passthrough { get; set; } = false;

	private EventHook? hook = null;
	

	protected override void Definition()
	{
		_updateOut = ControlOutput("");

		_update = ControlInput("Update", (flow) => 
		{
			if (hook is null) // Only register once
			{
				GameObject gameObject = flow.stack.gameObject;
				ScriptMachine scriptMachine = gameObject.GetComponent<ScriptMachine>();
				if(scriptMachine is null)
				{
					Debug.LogError("BBProxyUpdate: ScriptMachine is null");
					return null;
				}
				flow.stack.EnsureDebugDataAvailable();
				int uniqueID = gameObject.GetComponent<ScriptMachine>().GetInstanceID();
				hook = new EventHook("OpenGate" + uniqueID, gameObject);
				BBEventBus.Register<bool>(hook.Value, (v) => Passthrough = v);
			}

			return Passthrough ? _updateOut : null;
		});
	}
}
