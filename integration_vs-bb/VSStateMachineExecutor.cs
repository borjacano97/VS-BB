using BB;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Action("Visual Scripting/Script Machine")]
[Help("Executes a VisualScripting Script Machine")]
public class VSStateMachineExecutor : BasePrimitiveAction
{
	[InParam("State Machine", typeof(BBScriptMachine), DefaultBlackboardEntry = "StateMachine")]
	public BBStateMachine stateMachine;
}
