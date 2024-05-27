
using BBUnity.Actions;
using Pada1.BBCore;
using Pada1.BBCore.Framework;
using Pada1.BBCore.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class VSExecuter<T>: GOAction where T : VSExecuter<T>
{
	protected abstract IMachine Machine { get; }



	// Flags
	private bool finished = false;
	private bool started = false;
	private bool suspended = false;

	// Ending task status
	private TaskStatus response = TaskStatus.NONE;

	// Actions
	private EventHook resumeHook;
	private EventHook suspendHook;

	private EventHook onAbortTaskHook;
	private EventHook onCompletedTaskHook;
	private EventHook onFailedTaskHook;

	private EventHook setOutTaskArgsHook;

	private EventHook onStartHook;
	private EventHook onUpdateHook;
	private EventHook onAbortedHook;
	private EventHook onEndedHook;
	private EventHook onFailedEndHook;

	// Main class method, invoked by the execution engine.

	public override void OnStart()
	{
		/// ACTIONS FROM THE VS TO THE EXECUTER
		// Suspend / Resume hooks
		suspendHook = new EventHook(Constants.EventTriggers.SuspendTask, Machine);
		EventBus.Register<EmptyEventArgs>(suspendHook, OnTaskSuspend);

		resumeHook = new EventHook(Constants.EventTriggers.ResumeTask, Machine);
		EventBus.Register<EmptyEventArgs>(resumeHook, OnTaskResume);

		// Ending task hooks
		onAbortTaskHook = new EventHook(Constants.EventTriggers.AbortTask, Machine);
		EventBus.Register<EmptyEventArgs>(onAbortTaskHook, OnTaskAbort);

		onCompletedTaskHook = new EventHook(Constants.EventTriggers.CompleteTask, Machine);
		EventBus.Register<EmptyEventArgs>(onCompletedTaskHook, OnTaskCompleted);

		onFailedTaskHook = new EventHook(Constants.EventTriggers.EndTaskFailed, Machine);
		EventBus.Register<EmptyEventArgs>(onFailedTaskHook, OnTaskFailed);

		// Set output params hook
		setOutTaskArgsHook = new EventHook(Constants.EventTriggers.SetOutTaskArgs, Machine);
		EventBus.Register<UpdateArguments>(setOutTaskArgsHook, OnSetOutputParams);


		/// EVENTS FROM THE EXECUTER TO THE VS
		onStartHook     = new EventHook(Constants.EventTriggers.onBBStart,   Machine);
		onUpdateHook    = new EventHook(Constants.EventTriggers.onBBUpdate,  Machine);
		onAbortedHook   = new EventHook(Constants.EventTriggers.onBBAborted, Machine);
		onEndedHook     = new EventHook(Constants.EventTriggers.onBBEnd,     Machine);
		onFailedEndHook = new EventHook(Constants.EventTriggers.onBBFailed,  Machine);
	}
	public override TaskStatus OnUpdate()
	{

		if (!finished)
		{
			if (!started) // First execution
			{
				started = true;
				EventBus.Trigger(onStartHook);
			}
			// Trigger the update event
			EventBus.Trigger(onUpdateHook, GetInputParams());

			return suspended ? TaskStatus.SUSPENDED : TaskStatus.RUNNING;
		}
		else
		{
			// Reset the flags
			started = false;
			finished = false;
			suspended = false;

			// return the given TaskStatus ending state (COMPLETED, FAILED, ABORTED)
			return response;
		}
	}

	private void OnTaskSuspend(EmptyEventArgs _)
	{
		suspended = true;
	}

	private void OnTaskResume(EmptyEventArgs _)
	{
		suspended = false;
		resume();
	}

	private void OnTaskAbort(EmptyEventArgs _)
	{
		finished = true;
		response = TaskStatus.ABORTED;
	}

	private void OnTaskCompleted(EmptyEventArgs _)
	{
		finished = true;
		response = TaskStatus.COMPLETED;
	}

	private void OnTaskFailed(EmptyEventArgs _)
	{
		finished = true;
		response = TaskStatus.FAILED;
	}

	public override void OnAbort()
	{
		EventBus.Trigger(onAbortedHook);
	}

	public override void OnEnd()
	{
		EventBus.Trigger(onEndedHook);
	}

	public override void OnFailedEnd()
	{
		EventBus.Trigger(onFailedEndHook);
	}

	private void OnSetOutputParams(UpdateArguments args) 
	{
		var _out = typeof(T).GetFields().Where(m => m.HasAttribute<OutParamAttribute>());
		if(_out.Count() != args.Length)
		{
			Debug.LogError($"The number of output parameters defined in the {nameof(T)} is different from the number of parameters the VS is trying to set."
			+ $"The VS is trying to set {args.Length} parameters, but the {nameof(T)} has {_out.Count()} output parameters defined.");
			return; // We return early to avoid a out of range exception
		}

		// Check Types
		bool typeMismatch = false;
		for (int i = 0; i < args.Length; i++)
		{
			if (_out.ElementAt(i).FieldType != args[i].GetType())
			{
				Debug.LogError($"The type of the output parameter {_out.ElementAt(i).Name} defined in the {nameof(T)} is different from the type of the parameter the VS is trying to set."
				+ $"The VS is trying to set a parameter of type {args[i].GetType()}, but the {_out.ElementAt(i).Name} is of type {_out.ElementAt(i).FieldType}.");
				typeMismatch = true; // We set the flag to true to avoid a type mismatch exception
			}
		}
		// If there is a type mismatch we return early to avoid a type mismatch exception
		if(typeMismatch) return; 

		// At this point we know that the number of parameters and the types match, so we can set the values
		for (int i = 0; i < args.Length; i++)
		{
			// Set the output parameter value
			_out.ElementAt(i).SetValue((T)this, args[i]);
		}
	}

	private UpdateArguments GetInputParams()
	{
		var _in = typeof(T).GetFields().Where(m => m.HasAttribute<InParamAttribute>());
		if (!_in.Any()) return UpdateArguments.Empty;

		object[] _inParams = _in.Select(m => m.GetValue((T)this)).ToArray();
		return new UpdateArguments(_inParams);

	}
}
