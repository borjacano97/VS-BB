
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Events/BB/Lifecycle")]
[UnitOrder(3)]
[UnitTitle("On BB End")]
public sealed class OnBBEnd : MachineEventUnit<EmptyEventArgs>
{
	protected override string hookName => Constants.EventTriggers.onBBEnd;
}