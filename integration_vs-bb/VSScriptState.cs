using BB;
using BBUnity.Actions;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[Action("Visual Scripting/State Machine")]
[Help("Executes a VisualScripting StateMachine")]
public class VSScriptState : BasePrimitiveAction
{
	[InParam("ScriptGraphAsset", typeof(ScriptGraphAsset), DefaultBlackboardEntry ="StateMachine")]
	public ScriptGraphAsset scriptGraphAsset;
	//private BBScriptMachine _scriptManchine;
	public override void config(BrickConfigurator c)
	{
		base.config(c);
		//_scriptManchine = 
	}

	public override void OnAbort()
	{
	}

	public override void OnEnd()
	{
		base.OnEnd();
	}

	public override void OnFailedEnd()
	{
		base.OnFailedEnd();
	}

	public override TaskStatus OnLatentEnd()
	{
		return base.OnLatentEnd();
	}

	public override TaskStatus OnLatentFailedEnd()
	{
		return base.OnLatentFailedEnd();
	}

	public override TaskStatus OnLatentStart()
	{
		return base.OnLatentStart();
	}

	public override void OnStart()
	{
	}

	public override TaskStatus OnUpdate()
	{
		BBEventBus.Trigger(EventHooks.Update);
		return TaskStatus.RUNNING;
	}

}
