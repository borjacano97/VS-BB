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


	TextAsset _template;


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

	private void OnEnable()
	{
		//FIX: do we load the template from Asserts or just simply embed it in the code?
		const string template_path = "Assets/integration_vs-bb/BehaviourBricks/VSExecuterTEMPLATE.txt";
		_template = AssetDatabase.LoadAssetAtPath<TextAsset>(template_path);
	}

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

		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();

		if (GUILayout.Button("Generate") 
			&& NoDuplicatedParams()
			&& NoEmptyArguments())
		{
			GenerateFile();
			AssetDatabase.Refresh();
		}
	}

	private bool NoEmptyArguments()
	{
		return !string.IsNullOrEmpty(_executorName)
			&& !string.IsNullOrEmpty(_actionString)
			&& !string.IsNullOrEmpty(_helpString)
			&& _inputParametersTypes. TrueForAll(x => !string.IsNullOrEmpty(x))
			&& _inputParametersNames. TrueForAll(x => !string.IsNullOrEmpty(x))
			&& _outputParametersTypes.TrueForAll(x => !string.IsNullOrEmpty(x))
			&& _outputParametersNames.TrueForAll(x => !string.IsNullOrEmpty(x));
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


		new VSExecuterGenerator(_template).
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

}
