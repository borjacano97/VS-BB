using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VSExecuterGenerator
{
	private readonly string _template;
	private readonly string _outputDir;
	private ExecutorType _executorType;
	private string _actionString;
	private string _helpString;
	private string _executorName;
	private List<Parameter> _inputParameters;
	private List<Parameter> _outputParameters;

	public enum ExecutorType
	{
		StateMachine,
		ScriptMachine
	}

	public struct Parameter
	{
		public Parameter(string type, string name)
		{
			Type = type;
			Name = name;
		}
		public string Type;
		public string Name;
	}

	public VSExecuterGenerator(string template, string outputDir)
	{
		_template = template;
		_outputDir = outputDir;
	}

	public VSExecuterGenerator SetExecutorType(ExecutorType executorType) 
	{
		_executorType = executorType;
		return this;
	}

    public VSExecuterGenerator SetActionString(string actionString)
    {
		_actionString = actionString;
		return this;
	}

	public VSExecuterGenerator SetHelpString(string helpString)
	{
		_helpString = helpString;
		return this;
	}

	public VSExecuterGenerator SetExecutorName(string executorName)
	{
		_executorName = executorName;
		return this;
	}

	public VSExecuterGenerator SetInputParameters(List<Parameter> parameters)
	{
		_inputParameters = parameters;
		return this;
	}

	public VSExecuterGenerator SetOutputParameters(List<Parameter> parameters)
	{
		_outputParameters = parameters;
		return this;
	}

	public string Generate(bool doWrite = false)
	{
		string inputParameters = "";
		foreach (var p in _inputParameters)
		{
			inputParameters += MakeInputParameter(p) + Environment.NewLine + '\t';
		}

		string outputParameters = "";
		foreach (var p in _outputParameters)
		{
			outputParameters += MakeOutputParameter(p) + Environment.NewLine + '\t';
		}

		string result = _template.
			Replace("$ACTION_PATH$", _actionString).
			Replace("$HELP$", _helpString).
			Replace("$MACHINE_TYPE$", _executorType.ToString()).
			Replace("$CLASS_NAME$", _executorName.Replace(' ', '_')).
			Replace("$INPUT_PARAMETERS$", inputParameters).
			Replace("$OUTPUT_PARAMETERS$", outputParameters);
		
		if(doWrite)
		{
			WriteResult(result);
		}

		return result;
	}

	private void WriteResult(string content)
	{
		string dir_path = Path.Combine(Application.dataPath, _outputDir);
		string file_path = dir_path + "/" + _executorName.Replace(' ', '_') + ".cs";

		Directory.CreateDirectory(dir_path);    // Make sure directory exists
		File.Delete(file_path);                 // Delete old file if exists
		File.WriteAllText(file_path, content);  // Dump string content to file
	}

	private string MakeInputParameter(Parameter param) =>
		$"[InParam(\"{param.Name}\")] public {param.Type} {param.Name};";
	private string MakeOutputParameter(Parameter param) =>
		$"[OutParam(\"{param.Name}\")] public {param.Type} {param.Name};";
}
