using System.Collections;

namespace Routines
{
	/// <summary>
	/// Позволяет запустить параллельно выполняемую функцию из Routine.
	/// </summary>
	public class Parallel
	{
		private IRoutine _routine;
		private bool _start;

		public bool IsStart { get { return _start; } }

		public IRoutine Routine { get { return _routine; } }
		
		/// <summary>
		/// Запустить параллельную рутину. Параллельная рутина будет выполнятся вместе с текущей. Её время жизни не больше времени жизни исходной. 
		/// yield return Parallel. Stat(SomeFunc());
		/// Инструкция yeild с данным методом не прерывает выполнение вызывающей его рутины.
		/// </summary>
		public static Parallel Start(IEnumerator routine)
		{
			return Start(new Routine(routine));
		}

		/// <summary>
		/// Запустить параллельную рутину. Параллельная рутина будет выполнятся вместе с текущей. Её время жизни не больше времени жизни исходной. 
		/// yield return Parallel. Stat(SomeFunc());
		/// Инструкция yeild с данным методом не прерывает выполнение вызывающей его рутины.
		/// </summary>
		public static Parallel Start(IRoutine routine)
		{
			return new Parallel {_routine = routine, _start = true};
		}

		/// <summary>
		/// Остановить параллельную рутину
		/// </summary>
		public static Parallel Stop(IRoutine routine)
		{
			return new Parallel { _routine = routine, _start = false };
		}
	}
}
