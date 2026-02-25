
public sealed class ConfigurationActivityContext : IDisposable
{
	private static readonly AsyncLocal<Guid?> _parentActivityLogId = new AsyncLocal<Guid?>();
	private readonly Guid? _previousValue;

	public ConfigurationActivityContext(Guid parentActivityLogId)
	{
		_previousValue = _parentActivityLogId.Value;
		_parentActivityLogId.Value = parentActivityLogId;
	}

	public static Guid? CurrentParentId => _parentActivityLogId.Value;

	public void Dispose()
	{
		_parentActivityLogId.Value = _previousValue;
	}
}


class ConfigurationActivityContextExample
{
	private static Guid parentId1 = Guid.NewGuid();
	private static Guid parentId2 = Guid.NewGuid();

	public static void RunSync()
	{
		Console.WriteLine(new { parentId1, parentId2 });
		Console.WriteLine($"Before task: {ConfigurationActivityContext.CurrentParentId}");

		using (new ConfigurationActivityContext(parentId1))
		{
			Console.WriteLine($"Before subtask: {ConfigurationActivityContext.CurrentParentId}");

			using (new ConfigurationActivityContext(parentId2))
			{
				Console.WriteLine($"Inside subtask: {ConfigurationActivityContext.CurrentParentId}");
			}

			Console.WriteLine($"After subtask: {ConfigurationActivityContext.CurrentParentId}");
		}

		Console.WriteLine($"After task: {ConfigurationActivityContext.CurrentParentId}");
	}

	public static async Task RunAsyncTwoTasks()
	{
		Console.WriteLine(new { parentId1, parentId2 });
		Console.WriteLine($"Before task: {ConfigurationActivityContext.CurrentParentId}");

		using (new ConfigurationActivityContext(parentId1))
		{
			await Task.Delay(100);
			Console.WriteLine($"Before subtask: {ConfigurationActivityContext.CurrentParentId}");

			using (new ConfigurationActivityContext(parentId2))
			{
				await Task.Delay(100);
				Console.WriteLine($"Inside subtask: {ConfigurationActivityContext.CurrentParentId}");
			}

			await Task.Delay(100);
			Console.WriteLine($"After subtask: {ConfigurationActivityContext.CurrentParentId}");
		}

		Console.WriteLine($"After task: {ConfigurationActivityContext.CurrentParentId}");
	}

	public static async Task RunAsyncTwoTasksCallingSubMethod()
	{
		Console.WriteLine(new { parentId1, parentId2 });
		Console.WriteLine($"Before task: {ConfigurationActivityContext.CurrentParentId}");

		using (new ConfigurationActivityContext(parentId1))
		{
			await Task.Delay(100);
			Console.WriteLine($"Before subtask: {ConfigurationActivityContext.CurrentParentId}");

			await SubtaskAsync();

			await Task.Delay(100);
			Console.WriteLine($"After subtask: {ConfigurationActivityContext.CurrentParentId}");
		}

		Console.WriteLine($"After task: {ConfigurationActivityContext.CurrentParentId}");
	}


	private static async Task SubtaskAsync()
	{
		using (new ConfigurationActivityContext(parentId2))
		{
			await Task.Delay(100);
			Console.WriteLine($"Inside subtask: {ConfigurationActivityContext.CurrentParentId}");
		}
	}


	public static Task RunAsyncTwoTasksCallingSubMethod2()
	{
		Console.WriteLine(new { parentId1, parentId2 });
		Console.WriteLine($"Before task: {ConfigurationActivityContext.CurrentParentId}");

		using (new ConfigurationActivityContext(parentId1))
		{
			//await Task.Delay(100);
			Console.WriteLine($"Before subtask: {ConfigurationActivityContext.CurrentParentId}");
			return SubtaskAsync();
			//Console.WriteLine($"After subtask: {ConfigurationActivityContext.CurrentParentId}");
		}
		// Console.WriteLine($"After task: {ConfigurationActivityContext.CurrentParentId}");
	}


	public static async Task LazyTasks()
	{
		Console.WriteLine(new { parentId1, parentId2 });

		IEnumerable<Task<int>>? tasks = null;
		using (new ConfigurationActivityContext(parentId1))
		{
			tasks = EnumerableOfTasks();
		}

		if (tasks is not null)
		{
			foreach (var t in tasks)
			{
				var number = await t;
				Console.WriteLine(new { number });
			}
		}
	}

	private static IEnumerable<Task<int>> EnumerableOfTasks()
	{
		return new int[] { 1, 2, 3 }.Select(GetFromDBAsync);
	}

	private static async Task<int> GetFromDBAsync(int id)
	{
		await Task.Delay(100);
		Console.WriteLine($"Inside task with id {id} ParentId: {ConfigurationActivityContext.CurrentParentId}");
		// <- Copilot :) this will be null because the async local value is captured when the task is created, and at that time the ConfigurationActivityContext has already been disposed 
		return id * 10;
	}
}



