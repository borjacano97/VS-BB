/***********************************************************************************
 * DO NOT EDIT THIS FILE MANUALLY                                                  *
 * This file was generated by Behaviour Bricks Visual Scripting Executor Generator *
 **********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Pada1.BBCore;
using Unity.VisualScripting;
[Action("Examples/Move Agent")]
[Help("set the destination for the agent in a Nav Mesh")]
public class MoveAgent : VSExecuter<MoveAgent>
{
	[InParam("Machine")] ScriptMachine _machine;
	[InParam("positiion")] public Vector3 positiion;
	
	
	protected override IMachine Machine => _machine.GetReference().machine;

}