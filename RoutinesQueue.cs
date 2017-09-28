using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TM.Generic.Monads;
using UnityEngine.Assertions;

namespace Routines
{
	/// <summary>
	/// Очередь рутин выполняемых поочерёдно
	/// </summary>
	public class RoutinesQueue : IRoutine
	{
		public bool IsStarted { get { return _started; } }
		public bool IsFinished { get { return _finished; } }

		public object ExitValue { get { return null; } }
		public object Current { get { return null; } }

		private readonly Queue<IRoutine> _queue = new Queue<IRoutine>();
		private bool _finished;
		private bool _started;

		public RoutinesQueue() {}

		public RoutinesQueue(params IEnumerator[] routines) : this(routines as IEnumerable<IEnumerator>) {}

		public RoutinesQueue(params IRoutine[] routines) : this(routines as IEnumerable<IRoutine>) {}

		public RoutinesQueue(IEnumerable<IEnumerator> routines)
		{
			Enqueue(routines);
		}

		public RoutinesQueue(IEnumerable<IRoutine> routines)
		{
			Enqueue(routines);
		}

		public RoutinesQueue Enqueue(IRoutine routine)
		{
		    Assert.IsNotNull(routine);
			Assert.IsTrue(!_queue.Contains(routine)); // при добавлении рутины дважды, произойдёт не то, что ожидалось. 
			_queue.Enqueue(routine);
			return this;
		}

		public RoutinesQueue Enqueue(IEnumerator routine)
		{
			Enqueue(new Routine(routine));
			return this;
		}

		public void Enqueue(IEnumerable<IRoutine> routines)
		{
            Assert.IsNotNull(routines);
			Assert.IsTrue(!_queue.Intersect(routines).Any());
			Assert.IsTrue(!routines.Contains(this));
			routines.ForEach(x => _queue.Enqueue(x));
		}

		public void Enqueue(IEnumerable<IEnumerator> routines)
		{
			Enqueue(routines.Select(x => new Routine(x)).Cast<IRoutine>());
		}

		public void Clear()
		{
			_queue.Clear();
		}

		public void Break()
		{
			Clear();
		}

		public bool MoveNext()
		{
			_started = true;
			while (_queue.Count > 0)
			{
				_finished = false;
				var current = _queue.Peek();
				if (current.MoveNext())
					break;
				_queue.Dequeue();
			}
			_finished = (_queue.Count == 0);
			return !_finished;
		}

		public void Reset()
		{
			throw new System.NotSupportedException();
		}
	}
}
