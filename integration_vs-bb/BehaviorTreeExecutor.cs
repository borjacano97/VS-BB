using BBUnity;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.IO;

[UnitCategory("VS to BB/ Executor")]
[UnitOrder(-1)]
[UnitTitle("BehaviorTreeExecutor")]
[DefaultExecutionOrder(-1)]
public class BehaviorTreeExecutor : Unit
{
	private bool _initialized = false;

	private ControlInput update;
	private ControlOutput running;
	private ControlOutput completed;

	private ValueInput maxTaskPerTickInput;
	private ValueInput pauseValueInput;
	private ValueInput restartWhenFinishedValueInput;

	[SerializeAs(nameof(brickAsset))]
	private BrickAsset brickAsset_ = null;

	[DoNotSerialize]
	[Inspectable, UnitHeaderInspectable("Behaviour")]
	public BrickAsset brickAsset
	{
		get => brickAsset_;
		set => brickAsset_ = value;
	}
	public BrickAsset oldBrickAsset;
	private UnityBlackboard unityBlackboard = new();
	private Pada1.BBCore.Blackboard _blackboard;
	private BrickExecutor executor = new();

	[Serialize]
	public Dictionary<string, ValueInput> _behaviourInputs = new();
	[Serialize]
	public Dictionary<string, ValueOutput> _behaviourOutputs = new();





	private bool seted = false;
	public bool goodDefined = false;


	/// <summary>Set the components of the executor</summary>
	public void SetInternal()
	{
		// Make sure all subbehaviors are registered
		brickAsset_.RegisterSubbehaviors(); 
		//Bake the blackboard
		_blackboard = unityBlackboard.BuildBlackboard(); 
		// Set the beahviour and the memory to use on the executor
		executor.SetBrickAsset(brickAsset_, _blackboard); 
	}

	/// <summary> Update parameters of BB Blacksboard with the definition of the Brick</summary>
	public void UpdateInputParams()
	{
		// Make sure all subbehaviors are registered
		brickAsset_.RegisterSubbehaviors();
		// If the behavior is correctly loaded
		if (brickAsset_.behavior != null)
		{
			// Update the parameters of the blackboard with the values of the brick
			unityBlackboard.updateParams(brickAsset_.behavior.inParamValues);
		} 
			
	}

	protected override void AfterDefine()
	{
		if (brickAsset_ != null)
		{
			_ = System.Threading.Tasks.Task.Run(async () => {

				if (!goodDefined)
				{
					/* [HACK]: This is a hack to avoid the error of the missing behavior
					 * when the behavior is not loaded yet. This is a temporary solution
					 * until we find a better way to handle this.
					 * This happens because of a race condition on the derialization of the
					 * brick and this Node.
					 * Ideally, as we depend on the behavior, we should be serialized after,
					 * but this is not happening. So we are waiting for the behavior to be
					 * loaded before we define the node.

					 * We are waiting until it is loaded, whatever it takes. Maybe we should
					 * add a timeout, or at least a tries counter, to avoid infinite loops.
					 */
					while (brickAsset_.behavior == null)
					{
						await System.Threading.Tasks.Task.Delay(5);
					}
					Define();
				}
			});
		}
		base.AfterDefine();
	}

	protected override void Definition()
	{
		goodDefined = false;
		// The three constant values that are present on the BehaviourExecutorComponent
		pauseValueInput = ValueInput<bool>("Pause", false);
		restartWhenFinishedValueInput = ValueInput<bool>("RestartWhenFinished", false);
		maxTaskPerTickInput = ValueInput<int>("MaxTaskPerTick", 500);

		// Flow input definition
		update = ControlInput("Update", Execute);

		// Flow output definition while the task is running
		running = ControlOutput("Taks RUNNING");
		// Flow output definition when the task is completed
		completed = ControlOutput("Taks COMPLETED");

		// Initialize the parameters of the node 
		// so we can show the parameters on the editor as ports
		InitParameters();
	}

	private void InitParameters()
	{
		// User has not attached a behavior to the node.
		if (brickAsset_ == null) return;

		// if the attached behavior has not been desearialized yet, wait for it.
		goodDefined = brickAsset_.behavior != null;

		// Clear the inputs and outputs lists
		_behaviourInputs.Clear();
		_behaviourOutputs.Clear();

		// Create a new blackboard and executor
		unityBlackboard = new();
		executor = new();


		UpdateInputParams();

		// If the behavior is correctly loaded, create the inputs and outputs
		if (goodDefined)
		{
			// Inputs
			foreach (var input in brickAsset_.behavior.inParamValues.values)
			{
				var valueInput = ValueInput(input.Value.type, input.Key);
				_behaviourInputs.Add(input.Key, valueInput);
			}

			// Outputs
			foreach (var output in brickAsset_.behavior.outParamValues.values)
			{
				var valueOutput = ValueOutput(output.Value.type, output.Key);
				_behaviourOutputs.Add(output.Key, valueOutput);
			}

		}
	}

	/// <summary>Initialize the executor</summary>
	/// <param name="obj">The GameObject that the executor will be attached to</param>
	private void Init(GameObject obj)
	{
		// If the brick asset is not set nor initialized, return
		if (!brickAsset_) return;
		if (_initialized) return;
		_initialized = true;

		// Make sure all subbehaviors are registered
		brickAsset_.RegisterSubbehaviors();
		// Create the executor and the blackboard
		executor = new(obj);
		_blackboard = unityBlackboard.BuildBlackboard();
		// Link the executor with the brick asset and the blackboard
		executor.SetBrickAsset(brickAsset_, _blackboard);
	}


	/// <summary>Execute the behavior tree</summary>
	/// <param name="flow">The flow of the execution</param>
	/// <returns>The control output of the execution</returns>
	private ControlOutput Execute(Flow flow)
	{
		// Get the values from the execution context
		var paused = flow.GetValue<bool>(pauseValueInput);
		var restartWhenFinished = flow.GetValue<bool>(restartWhenFinishedValueInput);
		var maxTaskPerTick = flow.GetValue<int>(maxTaskPerTickInput);

		var gameObject = flow.stack.gameObject;

		if (gameObject != null)
		{
			// If the executor is not initialized, initialize it
			if (!_initialized)
			{
				Init(gameObject);
				return null;
			}
			// If the executor is paused, return null
			if (!paused)
			{
				// If the executor is not finished, tick it
				if (!executor.Finished)
				{
					// Get the input values from the flow and set them on the blackboard
					// so the executor can use them
					foreach (var (portkey, port) in _behaviourInputs)
					{
						_blackboard.Set(portkey, port.type, flow.GetValue(port));
					}

					// Tick the executor
					executor.Tick(maxTaskPerTick);
				}

				// Get the output values from the blackboard and set them on the flow
				foreach (var (portkey, port) in _behaviourOutputs)
				{
					var a = brickAsset_.behavior.outParamValues.values[portkey];
					flow.SetValue(port, _blackboard.Get(portkey, a.type));
				}

				// If the executor is finished, and the restartWhenFinished is true, restart it
				if (executor.Finished)
				{
					if (restartWhenFinished)
					{
						SetInternal();
						seted = false;
					}
					// The executor is finished, return the completed output
					return completed;
				}
				else
				{
					// The executor is running, return the running output
					return running;
				}
			}
		}

		return null;
	}
}