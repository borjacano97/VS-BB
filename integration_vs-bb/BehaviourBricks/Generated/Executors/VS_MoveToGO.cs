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
[Action("Examples/VS MoveToGO")]
[Help("move to gameobejct")]
public class VS_MoveToGO : VSExecuter<VS_MoveToGO>
{
	[InParam("Machine")] ScriptMachine _machine;
	[InParam("target")] public GameObject target;
	
	
	protected override IMachine Machine => _machine.GetReference().machine;

}