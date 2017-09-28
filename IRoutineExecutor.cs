using System.Collections;

namespace Routines
{
	/// <summary>
	/// Интерфейс класса, через который можно запускать и выполнять рутины
	/// </summary>
	public interface IRoutineExecutor
	{
		/// <summary>
		/// Запустить новую рутину
		/// </summary>
		IRoutine StartRoutine(IEnumerator routine);

		/// <summary>
		/// Заппустить новую рутину
		/// </summary>
		IRoutine StartRoutine(IRoutine cr);

		/// <summary>
		/// Очстановить рутину
		/// </summary>
		void StopRoutine(IRoutine cr);

		/// <summary>
		/// Шаг в секундах между вызовами Routine.Execute
		/// Файтически - UnityEngine.Time.deltaTime
		/// </summary>
		float ExecuteRoutineStep { get; }
	}
}