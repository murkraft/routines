namespace Routines
{
	public static class RoutineExecute
	{
		/// <summary>
		/// Выполнить рутину до следующего yeild. Возвращает false routine, если корутина завершилась.
		/// </summary>
		public static bool Execute(this IRoutine routine)
		{
			return routine != null && routine.MoveNext();
		}

		/// <summary>
		/// Проолжить выполнение корутины. Возвращает false и обнуляет routine, если корутина завершилась.
		/// </summary>
		public static bool Execute(ref IRoutine routine)
		{
			if (routine == null)
				return false;
			if (!routine.MoveNext())
			{
				routine = null;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Получить знаение которое вернула корутина
		/// </summary>
		public static T GetExitValue<T>(this Routine routine)
		{
			if (!routine.IsFinished)
				throw new System.Exception("Routine is not finished");
			return routine.ExitValue != null ? (T) routine.ExitValue : default(T);
		}

		/// <summary>
		/// Проверка что рутыны нет или она завершилась
		/// </summary>
		public static bool IsNullOrFinished(this IRoutine routine)
		{
			return routine == null || routine.IsFinished;
		}
	}
}
