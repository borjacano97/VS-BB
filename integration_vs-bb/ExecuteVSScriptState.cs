using Pada1.BBCore.Framework;
using Pada1.BBCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pada1.BBCore.Tasks;
using BB;
using Unity.VisualScripting;
using BBUnity.Actions;

[Action("VS Executors/ExecuteVSScriptState")]
[Help("Executes a VS Script State")]
public class ExecuteVSScriptState : GOAction
{
	[InParam("ScriptMachine", typeof(ScriptMachine))]
	public ScriptMachine scriptMachine;

	//[InParam("event name", typeof(string))]
	//public string event_name = "MyEvent";

	EventHook hook, hook2;
	private bool finished = false;
	TaskStatus status, response;

	public override void OnStart()
	{
		hook = new EventHook("OpenGate" + scriptMachine.GetInstanceID(), scriptMachine.gameObject);
		hook2 = new EventHook("TaskStatus" + scriptMachine.GetInstanceID(), scriptMachine.gameObject);
		BBEventBus.Register<TaskStatus>(hook2, 
			(status) =>
            {
				finished = true;
                this.status = status;
				BBEventBus.Trigger(hook, false);
                resume();
			});
	}
	public override TaskStatus OnUpdate()
	{
		if (finished)
        {
			finished = !finished;
			return status;
        }
        BBEventBus.Trigger(hook, true);
		return TaskStatus.SUSPENDED;

	}
}
