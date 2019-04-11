namespace PM
{
	public enum StopStatus
	{
		/// <summary>
		///     The compiler was stopped by user via pressing the stop button.
		/// </summary>
		UserForced,

		/// <summary>
		///     The compiler was stopped by code via e.g. PMWrapper.
		/// </summary>
		CodeForced,

		/// <summary>
		///     The compiler finished successfully.
		/// </summary>
		Finished,

		/// <summary>
		///     The compiler had an error during runtime. For example some missing variable or syntax error.
		/// </summary>
		RuntimeError,

		/// <summary>
		///     The compiler was stopped due to task error. For example user submitted wrong answer or uncompleted task.
		/// </summary>
		TaskError
	}
}