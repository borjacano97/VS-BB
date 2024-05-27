
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



[UnitCategory("Events/BB/Lifecycle")]
[UnitOrder(3)]
[UnitTitle("On BB Abort")]
public class OnBBAborted : MachineEventUnit<EmptyEventArgs>
{
	protected override string hookName => Constants.EventTriggers.onBBAborted;
}
