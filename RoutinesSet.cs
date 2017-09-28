using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TM.Generic.Extensions;
using UnityEngine.Assertions;

namespace Routines
{
	/// <summary>
	/// Множество рутин выполняемых "одновременно"
	/// </summary>
	public class RoutinesSet : IRoutine, IEnumerable<IRoutine>
	{
		public bool IsStarted { get { return _isStarted; } }
		public bool IsFinished { get { return _isFinished; } }

		public object ExitValue { get { return null; } }
		public object Current { get { return null; } }
		public bool IsEmpty { get { return _routines.Count == 0; } }

		private readonly List<IRoutine> _routines = new List<IRoutine>();
		private bool _isStarted;
		private bool _isFinished;

		public RoutinesSet() {}

		public RoutinesSet(params IEnumerator[] routines) : this(routines as IEnumerable<IEnumerator>) {}

		public RoutinesSet(params IRoutine[] routines) : this(routines as IEnumerable<IRoutine>) {}
		
		public RoutinesSet(IEnumerable<IEnumerator> routines)
		{
			Add(routines);
		}

		public RoutinesSet(IEnumerable<IRoutine> routines)
		{
			Add(routines);
		}

		public IRoutine Add(IRoutine routine)
		{
			Assert.IsTrue(!_routines.Contains(routine));
			Assert.IsTrue(routine != this);
			_routines.Add(routine);
			return routine;
		}

		public IRoutine Add(IEnumerator routine)
		{
			var r = new Routine(routine);
			Add(r);
			return r;
		}

		public void Add(IEnumerable<IRoutine> routines)
		{
			Assert.IsTrue(!_routines.Intersect(routines).Any());
			Assert.IsTrue(!routines.Contains(this));
			_routines.AddRange(routines);
		}

		public void Add(IEnumerable<IEnumerator> routines)
		{
			Add(routines.Select(x => new Routine(x)).Cast<IRoutine>());
		}

		public void Break()
		{
		    for (int i = 0, cnti = _routines.Count; i < cnti; ++i)
		    {
		        var routine = _routines[i];
		        if (routine != null)
		            routine.Break();
		    }
		    Clear();
		}

		public void Remove(IRoutine routine)
		{
            // так как этот метод может быть вызван во ходе выполнения MoveNext, то не удаляем рутину из списка сразу, а убираем ссылку на неё
            var idx = _routines.IndexOf(routine);
		    if (idx != -1)
		        _routines[idx] = null;
		}

		public void Clear()
		{
            // опять же. не очищаем список, а лишь обнуляем его элементы
			_routines.Fill((IRoutine)null);
		}

		public bool MoveNext()
		{
			_isStarted = true;

            int count = _routines.Count;
		    if (count > 0)
		    {
		        int end = count;
		        for (int i = end - 1; i >= 0; --i)
		        {
		            var routine = _routines[i];
		            if (routine == null || !routine.MoveNext())
		                _routines[i] = _routines[--end];
		        }

		        // count, в этот момент, может быть не равен _routines.Count, в случае, если выполняемые рутины добавляли новые рутины в это множество
		        // и так как новые рутины всегда добавляются в конец списка, то рутины предназначенные для удаления будут находится между оставшимися и новыми
		        // и именно этот диапазон и нужно удалить
		        if (end < count)
		            _routines.RemoveRange(end, count - end);

		        _isFinished = _routines.Count == 0;
		    }
		    else
		        _isFinished = true;
            return !_isFinished;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public IEnumerator<IRoutine> GetEnumerator()
		{
			return _routines.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _routines.GetEnumerator();
		}
	}
}
