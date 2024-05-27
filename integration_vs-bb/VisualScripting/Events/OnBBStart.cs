
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Events/BB/Lifecycle")]
[UnitOrder(3)]
[UnitTitle("On BB Start")]
[TypeIcon(typeof(Start))]
public sealed class OnBBStart : MachineEventUnit<EmptyEventArgs>
{
	protected override string hookName => Constants.EventTriggers.onBBStart;
}
