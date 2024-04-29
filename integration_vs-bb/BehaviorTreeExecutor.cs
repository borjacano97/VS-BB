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



	public void SetInternal()
	{
		brickAsset_.RegisterSubbehaviors();
		_blackboard = unityBlackboard.BuildBlackboard();
		executor.SetBrickAsset(brickAsset_, _blackboard);
	}

	public void UpdateInputParams()
	{
		brickAsset_.RegisterSubbehaviors();
		if (brickAsset_.behavior != null)
			unityBlackboard.updateParams(brickAsset_.behavior.inParamValues);
	}

	protected override void AfterDefine()
	{
		if (brickAsset_ != null)
		{
			_ = System.Threading.Tasks.Task.Run(async () => {

				if (!goodDefined)
				{
					// Espera a que se defina el behavior, por favor
					// tal vez restreingirlo a un numero máximo de intentos sea buena idea,
					// pero puede ser malo si tu procesador es muy malo,
					// y si justo es que no has llegado a cargarlo??
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
		pauseValueInput = ValueInput<bool>("Pause", false);
		restartWhenFinishedValueInput = ValueInput<bool>("RestartWhenFinished", false);
		maxTaskPerTickInput = ValueInput<int>("MaxTaskPerTick", 500);


		update = ControlInput("Update", Execute);

		running = ControlOutput("Taks RUNNING");
		completed = ControlOutput("Taks COMPLETED");
		InitParameters();
	}

	private void InitParameters()
	{

		//Debug.Log("el behavior check = " + brickAsset_.CheckBehavior());

		//Debug.Log(UnityEngine.StackTraceUtility.ExtractStackTrace());

		if (brickAsset_ == null) return;

		//	Debug.Log("Definition :D");

		if (brickAsset_.behavior == null)
		{
			// Debug.Log("no hay behavior :D");



		}
		else
		{
			//Debug.Log("Definido good :D");
			goodDefined = true;
		}

		_behaviourInputs.Clear();
		_behaviourOutputs.Clear();

		unityBlackboard = new();
		executor = new();

		//StartBehavior();

		UpdateInputParams();

		if (brickAsset_.behavior != null)
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

	private void Init(GameObject obj)
	{
		if (!brickAsset_) return;
		if (_initialized) return;
		_initialized = true;

		brickAsset_.RegisterSubbehaviors();
		executor = new(obj);
		_blackboard = unityBlackboard.BuildBlackboard();
		executor.SetBrickAsset(brickAsset_, _blackboard);
	}


	private ControlOutput Execute(Flow flow)
	{
		var paused = flow.GetValue<bool>(pauseValueInput);
		var restartWhenFinished = flow.GetValue<bool>(restartWhenFinishedValueInput);
		var gameObject = flow.stack.gameObject;
		var maxTaskPerTick = flow.GetValue<int>(maxTaskPerTickInput);

		if (gameObject != null)
		{
			if (!_initialized)
			{
				Init(gameObject);
				return null;
			}
			if (!paused)
			{

				if (!executor.Finished)
				{

					//Comentado porque creemos que no hace falta con la implementacion actual, en su momento hacia falta al reiniciar 

					//if (!seted)
					//               {
					//	foreach (var (portkey, port) in _behaviourInputs)
					//	{
					//		unityBlackboard.SetBehaviorParam(portkey, flow.GetValue(port));
					//	}
					//	_blackboard = unityBlackboard.BuildBlackboard();
					//	executor.SetBrickAsset(brickAsset_, _blackboard);
					//	//SetInternal();
					//	seted = true;
					//               }

					foreach (var (portkey, port) in _behaviourInputs)
					{
						_blackboard.Set(portkey, port.type, flow.GetValue(port));
					}

					executor.Tick(maxTaskPerTick);
				}



				foreach (var (portkey, port) in _behaviourOutputs)
				{
					var a = brickAsset_.behavior.outParamValues.values[portkey];
					flow.SetValue(port, _blackboard.Get(portkey, a.type));
				}

				if (executor.Finished)
				{
					if (restartWhenFinished)
					{
						SetInternal();
						seted = false;
					}
					return completed;
				}
				else
				{
					return running;
				}
			}
		}

		return null;
	}
}