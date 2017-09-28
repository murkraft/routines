#if UNITY_5 || UNITY_5_4_OR_NEWER
#define UNITY
#endif
using System;
using System.Collections;
#if UNITY
using UnityEngine;
#endif

namespace Routines
{
	public delegate bool Predicate();

	public class Wait : IEnumerator
	{
		private Predicate _predicate;
		private bool _value;

		public static readonly Wait Null = new Wait {_predicate = () => true, _value = true};

		/// <summary>
		/// Ожидание пока предикат не станет истинным
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		public static Wait Until(Predicate predicate)
		{
			return new Wait {_predicate = predicate, _value = true};
		}

		/// <summary>
		/// Ожидание пока предикат истинный
		/// </summary>
		public static Wait While(Predicate predicate)
		{
			return new Wait {_predicate = predicate, _value = false};
		}

		/// <summary>
		/// Ожидает this потом next
		/// </summary>
		public Wait Then(IEnumerator next)
		{
			return Wait.Sequence(this, next);
		}

		/// <summary>
		/// Ожидание в течении заданного времени с кастомным таймером
		/// </summary>
		public static Wait Seconds(float t, Func<float> timer)
		{
			var time = timer() + t;
			return new Wait {_predicate = () => timer() < time, _value = false};
		}

#if UNITY
		/// <summary>
		/// Ожидание в течении заданного времени без учёта TimeScale
		/// </summary>
		public static Wait Seconds(float t)
		{
			var time = Time.unscaledTime + t;
			return new Wait {_predicate = () => Time.unscaledTime < time, _value = false};
		}

		/// <summary>
		/// Ожидание в течение заданного времени с учетом TimeScale
		/// </summary>
		public static Wait ScaledSeconds(float t)
		{
			var time = Time.time + t;
			return new Wait {_predicate = () => Time.time < time, _value = false};
		}
#endif

		/// <summary>
		/// Ожидание обнуления счётчика
		/// </summary>
		public static Wait Counts(int t)
		{
			return new Wait {_predicate = () => --t >= 0, _value = false};
		}

		/// <summary>
		/// Ждёт когда закончат выполнятся все перечисленные энумераторы
		/// </summary>
		public static Wait All(params IEnumerator[] enums)
		{
			return new Wait
			{
				_predicate = () =>
				{
					bool status = false;
					for (int i=0; i<enums.Length; ++i)
					{
						if (enums[i] != null)
						{
							if (enums[i].MoveNext())
								status = true;
							else
								enums[i] = null;
						}
					}
					return status;
				},
				_value = false
			};
		}

		/// <summary>
		/// Ждёт когда закончит выполнятся любой из перечисленных энумераторов
		/// </summary>
		public static Wait Any(params IEnumerator[] enums)
		{
			return new Wait
			{
				_predicate = () =>
				{
					bool rv = false;
					foreach (var e in enums)
						rv |= !e.MoveNext();
					return rv;
				},
				_value = true
			};
		}

		/// <summary>
		/// Ждёт завершения всех инумераторов выполняемых последовательно
		/// </summary>
		public static Wait Sequence(params IEnumerator[] enums)
		{
			object current = (object) 0;
			return new Wait
			{
				_predicate = () =>
				{
					int c = (int) current;
					if (!enums[c].MoveNext())
					{
						++c;
						if (c < enums.Length)
							current = c;
						else
							return true;
					}
					return false;
				},
				_value = true
			};
		}

		private Wait() {}

		public bool MoveNext()
		{
			return _predicate() != _value;
		}

		public object Current { get { return null; } }

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}
}
