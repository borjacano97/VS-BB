using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;
using System.CodeDom;
using System.IO;

public class VSExecutorEditor : EditorWindow
{
	[MenuItem("Window/Behavior Bricks/Create New Visual Scripting Executor")]
	public static void CreateNewVSExecutor()
	{
		EditorWindow.GetWindow(typeof(VSExecutorEditor));
	}

	private const int MAX_ARGS = 8;

	private VSExecuterGenerator.ExecutorType _executorType = VSExecuterGenerator.ExecutorType.ScriptMachine;
	private string _executorName = "New Executor";
	private int _inputParametersCount = 0;
	private int _outputParametersCount = 0;

	private string _actionString = "";
	private string _helpString = "";
	
	private readonly List<string> _inputParametersTypes  = new();
	private readonly List<string> _inputParametersNames  = new();

	private readonly List<string> _outputParametersTypes = new();
	private readonly List<string> _outputParametersNames = new();

	private string _outputDir = "BehaviorBricks/VisualScripting/Executors";

	private void OnGUI()
	{ 
		GUILayout.Label("Create New Visual Scripting Executor", EditorStyles.boldLabel);
		EditorGUILayout.Separator();


		GUILayout.BeginHorizontal();
		_executorType = (VSExecuterGenerator.ExecutorType)EditorGUILayout.EnumPopup(_executorType, 
			GUILayout.ExpandWidth(false),
			GUILayout.Width(110)
			);
		_executorName = EditorGUILayout.TextArea(_executorName, GUILayout.ExpandWidth(true));
		GUILayout.EndHorizontal();

		_actionString = EditorGUILayout.TextField("Action path", _actionString);
		_helpString = EditorGUILayout.TextField("Help", _helpString);

		EditorGUILayout.Separator();
		int tmp_inputParametersCount = EditorGUILayout.IntField("Input parameters: ", _inputParametersCount);
		_inputParametersCount = Math.Clamp(tmp_inputParametersCount, 0, MAX_ARGS);

		while (_inputParametersCount > _inputParametersTypes.Count) 
		{
			_inputParametersTypes.Add("");
			_inputParametersNames.Add("");
		}

		while (_inputParametersCount < _inputParametersTypes.Count)
		{
			_inputParametersTypes.RemoveAt(_inputParametersTypes.Count - 1);
			_inputParametersNames.RemoveAt(_inputParametersNames.Count - 1);
		}

		EditorGUILayout.BeginVertical();
		for (int i = 0; i < _inputParametersCount; i++)
		{
			EditorGUILayout.BeginHorizontal();
			_inputParametersTypes[i] = EditorGUILayout.TextField("Type:", _inputParametersTypes[i]).Trim();
			_inputParametersNames[i] = EditorGUILayout.TextField("Name:", _inputParametersNames[i]).Trim();
			EditorGUILayout.EndHorizontal();

		}
		EditorGUILayout.EndVertical();


		EditorGUILayout.Separator();
		int tmp_outputParametersCount = EditorGUILayout.IntField("Output parameters: ", _outputParametersCount);
		_outputParametersCount = Math.Clamp(tmp_outputParametersCount, 0, MAX_ARGS);

		while (_outputParametersCount > _outputParametersTypes.Count)
		{
			_outputParametersTypes.Add("");
			_outputParametersNames.Add("");
		}

		while (_outputParametersCount < _outputParametersTypes.Count)
		{
			_outputParametersTypes.RemoveAt(_outputParametersTypes.Count - 1);
			_outputParametersNames.RemoveAt(_outputParametersNames.Count - 1);
		}

		EditorGUILayout.BeginVertical();
		for (int i = 0; i < _outputParametersCount; i++)
		{
			EditorGUILayout.BeginHorizontal();	
			_outputParametersTypes[i] = EditorGUILayout.TextField("Type:", _outputParametersTypes[i]).Trim();
			_outputParametersNames[i] = EditorGUILayout.TextField("Name:", _outputParametersNames[i]).Trim();
			EditorGUILayout.EndHorizontal();

		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical();
		_outputDir = EditorGUILayout.TextField("Output directory:", _outputDir);
		EditorGUILayout.EndVertical();

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		if (GUILayout.Button("Generate")) 
		{
			bool noEmptyArgs = NoEmptyArguments();
			bool noDuplicatedParams = NoDuplicatedParams();

			if (noEmptyArgs && noDuplicatedParams)
			{
				GenerateFile();
				AssetDatabase.Refresh();
			}
		}
	}

	private bool NoEmptyArguments()
	{
		bool no_empty = true;
		if (string.IsNullOrEmpty(_executorName))
		{
			Debug.LogError("Executor name is empty");
			no_empty = false;
		}

		if (string.IsNullOrEmpty(_actionString))
		{
			Debug.LogError("Action string is empty");
			no_empty = false;
		}

		if (string.IsNullOrEmpty(_helpString))
		{
			Debug.LogError("Help string is empty");
			no_empty = false;
		}

		for (int i = 0; i < _inputParametersTypes.Count; i++)
		{
			string type = _inputParametersTypes[i];
			if (string.IsNullOrEmpty(type))
			{
				Debug.LogError($"Input parameter {i+1} type is empty");
				no_empty = false;
			}
		}

		for (int i = 0; i < _inputParametersNames.Count; i++)
		{
			string name = _inputParametersNames[i];
			if (string.IsNullOrEmpty(name))
			{
				Debug.LogError($"Input parameter {i+1} name is empty");
				no_empty = false;
			}
		}

		for (int i = 0; i < _outputParametersTypes.Count; i++)
		{
			string type = _outputParametersTypes[i];
			if (string.IsNullOrEmpty(type))
			{
				Debug.LogError($"Output parameter {i+1} type is empty");
				no_empty = false;
			}
		}

		for (int i = 0; i < _outputParametersNames.Count; i++)
		{
			string name = _outputParametersNames[i];
			if (string.IsNullOrEmpty(name))
			{
				Debug.LogError($"Output parameter {i+1} name is empty");
				no_empty = false;
			}
		}

		return no_empty;
	}

	private void GenerateFile() 
	{
		var input = new List<VSExecuterGenerator.Parameter>(_inputParametersCount);
		for(int i = 0; i < _inputParametersCount; i++)
		{
			input.Add(new VSExecuterGenerator.Parameter(_inputParametersTypes[i], _inputParametersNames[i]));
		}
		var output = new List<VSExecuterGenerator.Parameter>(_outputParametersCount);
		for (int i = 0; i < _outputParametersCount; i++)
		{
			output.Add(new VSExecuterGenerator.Parameter(_outputParametersTypes[i], _outputParametersNames[i]));
		}


		new VSExecuterGenerator(TEMPLATE, _outputDir).
			SetExecutorType(_executorType).
			SetExecutorName(_executorName).
			SetActionString(_actionString).
			SetHelpString(_helpString).
			SetInputParameters(input).
			SetOutputParameters(output).
			Generate(true);
		Debug.Log($@"Visual Scripting Executor ""{_executorName}"" generated!");

	}



	private bool NoDuplicatedParams()
	{
		bool no_duplicated = true;
		// Check duplicated withing input parameters
		for (int i = 0; i < _inputParametersCount; i++)
		{
			for (int j = i + 1; j < _inputParametersCount; j++)
			{
				if (_inputParametersNames[i] == _inputParametersNames[j])
				{
					Debug.LogError($"Duplicated input parameter name: {_inputParametersNames[i]}");
					no_duplicated = false;
				}
			}
		}

		// Check duplicated withing output parameters
		for (int i = 0; i < _outputParametersCount; i++)
		{
			for (int j = i + 1; j < _outputParametersCount; j++)
			{
				if (_outputParametersNames[i] == _outputParametersNames[j])
				{
					Debug.LogError($"Duplicated output parameter name: {_outputParametersNames[i]}");
					no_duplicated = false;
				}
			}
		}

		// Check duplicated between input and output parameters
		for (int i = 0; i < _inputParametersCount; i++)
		{
			for (int j = 0; j < _outputParametersCount; j++)
			{
				if (_inputParametersNames[i] == _outputParametersNames[j])
				{
					Debug.LogError($"Duplicated parameter name: {_inputParametersNames[i]}. Input and output parameters must not share name");
					no_duplicated = false;
				}
			}
		}

		return no_duplicated;
	}


	private const string TEMPLATE = 
@"/***********************************************************************************
 * DO NOT EDIT THIS FILE MANUALLY                                                  *
 * This file was generated by Behaviour Bricks Visual Scripting Executor Generator *
 **********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Pada1.BBCore;
using Unity.VisualScripting;
[Action(""$ACTION_PATH$"")]
[Help(""$HELP$"")]
public class $CLASS_NAME$ : VSExecuter<$CLASS_NAME$>
{
	[InParam(""Machine"")] $MACHINE_TYPE$ _machine;
	$INPUT_PARAMETERS$
	$OUTPUT_PARAMETERS$
	protected override IMachine Machine => _machine.GetReference().machine;

}";

}
