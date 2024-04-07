using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BB
{
	public class BBScriptMachine : EventMachine<FlowGraph, ScriptGraphAsset>
	{
		public override FlowGraph DefaultGraph()
		{
			return FlowGraph.WithStartUpdate();
		}

		protected override void OnEnable()
		{
			if (hasGraph)
			{
				graph.StartListening(reference);
			}

			base.OnEnable();
		}

		protected override void OnInstantiateWhileEnabled()
		{
			if (hasGraph)
			{
				graph.StartListening(reference);
			}

			base.OnInstantiateWhileEnabled();
		}

		protected override void OnUninstantiateWhileEnabled()
		{
			base.OnUninstantiateWhileEnabled();

			if (hasGraph)
			{
				graph.StopListening(reference);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			if (hasGraph)
			{
				graph.StopListening(reference);
			}
		}

		[ContextMenu("Show Data...")]
		protected override void ShowData()
		{
			base.ShowData();
		}
	}
}