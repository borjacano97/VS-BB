public readonly struct UpdateArguments
{
	public readonly object[] arguments;
	public UpdateArguments(params object[] arguments)
	{
		this.arguments = arguments;
	}
	public static readonly UpdateArguments Empty = new();

	public int Length => arguments.Length;
	public object this[int index] => arguments[index];
}
