
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnBBAborted : MachineEventUnit<EmptyEventArgs>
{
	protected override string hookName => Constants.EventTriggers.onBBAborted;
}
