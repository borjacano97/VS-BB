public readonly struct UpdateArguments
{
	/// <summary> The arguments of the update </summary>
	public readonly object[] arguments;

	/// <summary> Create a new instance of the arguments </summary>
	public UpdateArguments(params object[] arguments)
	{
		this.arguments = arguments;
	}
	/// <summary> Emptry arguments instance</summary>
	public static readonly UpdateArguments Empty = new();

	/// <summary> Get the length of the arguments </summary>
	public int Length => arguments.Length;
	/// <summary> Get the argument at the specified index </summary>
	/// <param name="index">The index of the argument</param>
	/// <returns>The argument at the specified index</returns>
	public object this[int index] => arguments[index];
}
