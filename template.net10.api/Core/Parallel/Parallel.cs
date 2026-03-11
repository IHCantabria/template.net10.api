using System.Diagnostics.CodeAnalysis;
using template.net10.api.Core.Exceptions;
using template.net10.api.Core.Extensions;

namespace template.net10.api.Core.Parallel;

/// <summary>
///     Provides utility methods for executing dependent and independent tasks in parallel with cancellation support.
/// </summary>
[SuppressMessage(
    "ReSharper",
    "UnusedMember.Global",
    Justification = "General-purpose helper methods; not all members are used in every scenario.")]
internal static class ParallelUtils
{
    /// <summary>
    ///     Executes a collection of dependent tasks in parallel, cancelling remaining tasks if any faults.
    /// </summary>
    /// <param name="tasks">The tasks to execute.</param>
    /// <param name="cts">The cancellation token source used to cancel remaining tasks on failure.</param>
    internal static Task ExecuteDependentInParallelAsync(IEnumerable<Task> tasks,
        CancellationTokenSource cts)
    {
        return HandleTaskDependentCompletionAsync(tasks, cts);
    }

    /// <summary>
    ///     Executes a collection of dependent tasks in parallel, returning their results, and cancelling remaining tasks if any faults.
    /// </summary>
    /// <typeparam name="T">The return type of each task.</typeparam>
    /// <param name="tasks">The tasks to execute.</param>
    /// <param name="cts">The cancellation token source used to cancel remaining tasks on failure.</param>
    /// <returns>The collected results from completed tasks.</returns>
    internal static Task<IEnumerable<T>> ExecuteDependentInParallelAsync<T>(IEnumerable<Task<T>> tasks,
        CancellationTokenSource cts)
    {
        return HandleTaskDependentCompletionAsync(tasks, cts);
    }

    /// <summary>
    ///     Executes a collection of dependent Result-wrapped tasks in parallel, cancelling remaining tasks if any faults or returns a faulted result.
    /// </summary>
    /// <typeparam name="T">The success type of the Result.</typeparam>
    /// <param name="tasks">The Result-wrapped tasks to execute.</param>
    /// <param name="cts">The cancellation token source used to cancel remaining tasks on failure.</param>
    /// <returns>A Result containing the collected results or the first encountered exception.</returns>
    /// <exception cref="ResultSuccessInvalidOperationException">
    ///     Result is not a success! Use ExtractException method instead
    ///     and Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractException method
    /// </exception>
    /// <exception cref="ResultFaultedInvalidOperationException">
    ///     Result is not a failure! Use ExtractData method instead and
    ///     Check the state of Result with IsSuccess or IsFaulted before use this method or ExtractData method
    /// </exception>
    internal static async Task<LanguageExt.Common.Result<IEnumerable<T>>> ExecuteDependentInParallelAsync<T>(
        IEnumerable<Task<LanguageExt.Common.Result<T>>> tasks,
        CancellationTokenSource cts)
    {
        var taskResults = await HandleTaskDependentCompletionAsync(tasks, cts).ConfigureAwait(false);
        return taskResults.IsSuccess
            ? new LanguageExt.Common.Result<IEnumerable<T>>(taskResults.ExtractData())
            : new LanguageExt.Common.Result<IEnumerable<T>>(taskResults.ExtractException());
    }

    /// <summary>
    ///     Handles dependent task completion by awaiting tasks one-by-one and cancelling all on first fault.
    /// </summary>
    /// <param name="tasks">The tasks to monitor.</param>
    /// <param name="cts">The cancellation token source.</param>
    private static async Task HandleTaskDependentCompletionAsync(IEnumerable<Task> tasks,
        CancellationTokenSource cts)
    {
        var tasksList = tasks.ToList();
        while (tasksList.Count > 0)
        {
            // Wait for any task to complete
            var completedTask = await Task.WhenAny(tasksList).ConfigureAwait(false);

            // Remove the completed task from the list
            tasksList.Remove(completedTask);

            // Check if the completed task is Faulted
            if (completedTask.IsFaulted)
                // cancel all other tasks
                await cts.CancelAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Handles dependent task completion with typed results, cancelling all remaining tasks on first fault.
    /// </summary>
    /// <typeparam name="T">The return type of each task.</typeparam>
    /// <param name="tasks">The tasks to monitor.</param>
    /// <param name="cts">The cancellation token source.</param>
    /// <returns>The collected results from completed tasks.</returns>
    private static async Task<IEnumerable<T>> HandleTaskDependentCompletionAsync<T>(IEnumerable<Task<T>> tasks,
        CancellationTokenSource cts)
    {
        var completedResults = new List<T>();
        var tasksList = tasks.ToList();
        while (tasksList.Count > 0)
        {
            // Wait for any task to complete
            var completedTask = await Task.WhenAny(tasksList).ConfigureAwait(false);

            // Remove the completed task from the list
            tasksList.Remove(completedTask);

            var taskResult = await completedTask.ConfigureAwait(false);

            // Check if the completed task is Faulted
            if (completedTask.IsFaulted)
            {
                // cancel all other tasks
                await cts.CancelAsync().ConfigureAwait(false);
                return completedResults;
            }

            completedResults.Add(taskResult);
        }

        return completedResults;
    }

    /// <summary>
    ///     Handles dependent Result-wrapped task completion, cancelling all remaining tasks if any task faults or returns a faulted Result.
    /// </summary>
    /// <typeparam name="T">The success type of the Result.</typeparam>
    /// <param name="tasks">The Result-wrapped tasks to monitor.</param>
    /// <param name="cts">The cancellation token source.</param>
    /// <returns>A Result containing the collected results or the first encountered exception.</returns>
    private static async Task<LanguageExt.Common.Result<IEnumerable<T>>> HandleTaskDependentCompletionAsync<T>(
        IEnumerable<Task<LanguageExt.Common.Result<T>>> tasks,
        CancellationTokenSource cts)
    {
        var completedResults = new List<T>();
        var tasksList = tasks.ToList();
        while (tasksList.Count > 0)
        {
            // Wait for any task to complete
            var completedTask = await Task.WhenAny(tasksList).ConfigureAwait(false);

            // Remove the completed task from the list
            tasksList.Remove(completedTask);

            var taskResult = await completedTask.ConfigureAwait(false);

            // Check if the completed task is Faulted or the result of the completed task is Faulted
            if (completedTask.IsFaulted || taskResult.IsFaulted)
            {
                // cancel all other tasks
                await cts.CancelAsync().ConfigureAwait(false);
                return new LanguageExt.Common.Result<IEnumerable<T>>(completedTask.IsFaulted
                    ? completedTask.Exception
                    : taskResult.ExtractException());
            }

            completedResults.Add(taskResult.ExtractData());
        }

        return completedResults;
    }

    /// <summary>
    ///     Executes a collection of independent tasks in parallel, allowing all tasks to complete regardless of individual faults.
    /// </summary>
    /// <param name="tasks">The tasks to execute.</param>
    internal static Task ExecuteIndependentInParallelAsync(IEnumerable<Task> tasks)
    {
        return HandleTaskIndependentCompletionAsync(tasks);
    }

    /// <summary>
    ///     Executes a collection of independent Result-wrapped tasks in parallel, collecting all individual results regardless of faults.
    /// </summary>
    /// <typeparam name="T">The success type of the Result.</typeparam>
    /// <param name="tasks">The Result-wrapped tasks to execute.</param>
    /// <returns>A collection of Result values, one per completed task.</returns>
    internal static Task<IEnumerable<LanguageExt.Common.Result<T>>> ExecuteIndependentInParallelAsync<T>(
        IEnumerable<Task<LanguageExt.Common.Result<T>>> tasks)
    {
        return HandleTaskIndependentCompletionAsync(tasks);
    }

    /// <summary>
    ///     Handles independent task completion by awaiting all tasks regardless of faults.
    /// </summary>
    /// <param name="tasks">The tasks to monitor.</param>
    private static async Task HandleTaskIndependentCompletionAsync(IEnumerable<Task> tasks)
    {
        var tasksList = tasks.ToList();
        while (tasksList.Count > 0)
        {
            // Wait for any task to complete
            var completedTask = await Task.WhenAny(tasksList).ConfigureAwait(false);

            // Remove the completed task from the list
            tasksList.Remove(completedTask);
        }
    }

    /// <summary>
    ///     Handles independent Result-wrapped task completion by awaiting all tasks and collecting their results.
    /// </summary>
    /// <typeparam name="T">The success type of the Result.</typeparam>
    /// <param name="tasks">The Result-wrapped tasks to monitor.</param>
    /// <returns>A collection of Result values, one per completed task.</returns>
    private static async Task<IEnumerable<LanguageExt.Common.Result<T>>> HandleTaskIndependentCompletionAsync<T>(
        IEnumerable<Task<LanguageExt.Common.Result<T>>> tasks)
    {
        var completedResults = new List<LanguageExt.Common.Result<T>>();
        var tasksList = tasks.ToList();
        while (tasksList.Count > 0)
        {
            // Wait for any task to complete
            var completedTask = await Task.WhenAny(tasksList).ConfigureAwait(false);

            // Remove the completed task from the list
            tasksList.Remove(completedTask);

            var taskResult = await completedTask.ConfigureAwait(false);


            completedResults.Add(taskResult);
        }

        return completedResults;
    }
}