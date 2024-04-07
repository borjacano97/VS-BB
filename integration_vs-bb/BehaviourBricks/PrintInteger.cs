using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public struct cosas
//{
//	int a;
//	int b;
//}

[Action("Basic/PrintInteger")]
[Help("Prints an integer")]
public class PrintInteger : BasePrimitiveAction
{
	[InParam("Integer")]
	public int integer;
	//[InParam("fritos")]
	//public cosas cositas;
	public override TaskStatus OnUpdate()
	{
		Debug.Log(integer);
		return TaskStatus.COMPLETED;
	}
}
