using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants 
{
	public const int MAX_ARGS = 10;
	public static class EventTriggers
	{
		public const string onBBUpdate  = "OnBBUpdate";
		public const string onBBStart   = "OnBBStart";
		public const string onBBEnd     = "OnBBEnd";
		public const string onBBAborted = "OnBBAborted";
		public const string onBBFailed  = "OnBBFailed";
		public const string onBBCheck = "OnBBCheck";

		public const string SetOutTaskArgs = "SetOutTaskArgs";

		public const string ResumeTask    = "ResumeTask";
		public const string SuspendTask   = "SuspendTask";
		public const string CompleteTask = "CompletedTask";
		public const string CompleteCheck = "CompletedCheck";
		public const string EndTaskFailed    = "FailedTask";
		public const string AbortTask     = "AbortTask";
	}
}