using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Events/BB/Lifecycle")]
[UnitTitle("On BB Failed End")]
[TypeIcon(typeof(Start))]
public class OnBBFailedEnd : MachineEventUnit<EmptyEventArgs>
{
    protected override string hookName => Constants.EventTriggers.onBBFailed;
}
