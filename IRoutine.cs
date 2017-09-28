using System.Collections;

namespace Routines
{
	public interface IRoutine : IEnumerator
	{
		/// <summary>
		/// Рутина была запущена
		/// </summary>
		bool IsStarted { get; }
		
		/// <summary>
		/// Рутина завершилась
		/// </summary>
		bool IsFinished { get; }

		/// <summary>
		/// Остановить рутину прям вот щас
		/// </summary>
		void Break();

		/// <summary>
		/// Значение которое вернула рутина с помощью Break.With(значение)
		/// Если рутина завершилась не Break'ом, то значение равно null
		/// </summary>
		object ExitValue { get; }
	}
}

