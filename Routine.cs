#if UNITY_5 || UNITY_5_4_OR_NEWER
#define UNITY
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
#if UNITY
using UnityEngine;
#endif
namespace Routines
{
	public class Routine : IRoutine
	{
		public bool IsStarted { get { return _started; } }
		public bool IsFinished { get { return _finished; } }
		public object ExitValue { get { return _exitValue; } }

		private readonly Stack<Entry> _stack = new Stack<Entry>();
		private bool _finished;
		private bool _started;
		private object _exitValue;
		private RoutinesSet _coroutines;

		/// <summary>
		/// Создать пустую рутину
		/// </summary>
		public Routine()
		{}

		/// <summary>
		/// Создать рутину НЕ запуская её.
		/// </summary>
		public Routine(IEnumerator routine)
		{
			Assert.IsNotNull(routine);
			_stack.Push(new Entry(routine));
		}

		/// <summary>
		/// Создаёт рутину и сразу запускает её (т.е. выполняет до первого yield).
		/// Тоже самое что последовательный запуск new и MoveNext
		/// </summary>
		public static Routine Run(IEnumerator routine)
		{
			var cr = new Routine(routine);
			cr.MoveNext();
			return cr;
		}

		/// <summary>
		/// Останавливает рутину на текущем шаге с вызовом всех Finally и бряканием всех параллельных рутин
		/// </summary>
		public void Break()
		{
			if (!_finished)
			{
				if (_started)
				{
					while (_stack.Count > 0)
					{
						var entry = _stack.Pop();
						var routine = entry.Enumerator as IRoutine;
						if (routine != null)
							routine.Break();
						var @finally = entry.Finally;
						if (@finally != null)
							foreach (var f in @finally)
								f.Execute();
					}
					if (_coroutines != null)
						foreach (var coroutine in _coroutines)
							coroutine.Break();
				}
				_coroutines = null;
				_finished = true;
			}
		}

		/// <summary>
		/// Выполнить рутину до следующего yeild. Возвращает false, если рутина и все её корутины завершилась.
		/// </summary>
		public bool MoveNext()
		{
			_started = true;
			if (!_finished)
				_finished = (Execution() == false);
			if (_coroutines != null && !_coroutines.IsFinished)
				_finished = false;
			return !_finished;
		}
		
		public object Current { get { return null; } }

		public void Reset()
		{
			throw new System.NotSupportedException();
		}
		
		public override string ToString()
		{
			return base.ToString() + (_stack.Count > 0 ? _stack.Peek().ToString() : "");
		}

		private bool Execution()
		{
			_coroutines.Execute();
			while (_stack.Count > 0)
			{
				var entry = _stack.Peek();
				var enumerator = entry.Enumerator;
				if (enumerator.MoveNext())
				{
					if (enumerator.Current == null)
					{
						return true;
					}
#if UNITY
					if (enumerator.Current is AsyncOperation)
					{
						Push(Wait.Until(() => ((AsyncOperation) enumerator.Current).isDone));
					}
					else if (enumerator.Current is WWW)
					{
						Push(Wait.Until(() => ((WWW) enumerator.Current).isDone));
					}
#endif
					else if (enumerator.Current is IEnumerator)
					{
						Push((IEnumerator) enumerator.Current);
						// вложенные корутины запускаются сразу! поэтому тут не return true; 
					}
					else if (enumerator.Current is Break)
					{
						_exitValue = ((Break) enumerator.Current).Value;
						Pop();
					}
					else if (enumerator.Current is Exit)
					{
						_exitValue = ((Exit)enumerator.Current).Value;
						while(_stack.Count>0)
							Pop();
					}
					else if (enumerator.Current is Finally)
					{
						if( entry.Finally == null )
							entry.Finally = new List<Finally>();
						entry.Finally.Add((Finally)enumerator.Current);
					}
					else if (enumerator.Current is Parallel)
					{
						var parallel = (Parallel) enumerator.Current;
						if (parallel.IsStart)
							(_coroutines ?? (_coroutines = new RoutinesSet())).Add(parallel.Routine);
						else if (_coroutines != null)
							_coroutines.Remove(parallel.Routine);
						// Parallel не прерывает исполнения рутины! поэтому тут не return true; }
					}
				}
				else
				{
					Pop();
				}
			}
			return false;
		}

		private void Push(IEnumerator enumerator)
		{
			_stack.Push(new Entry(enumerator));
		}

		private void Pop()
		{
			var @finally = _stack.Pop().Finally;
			if (@finally != null)
				foreach (var f in @finally)
					f.Execute();
		}

		private class Entry
		{
			public readonly IEnumerator Enumerator;
			public List<Finally> Finally;

			public Entry(IEnumerator enumerator)
			{
				Enumerator = enumerator;
			}
		}
	}
}
