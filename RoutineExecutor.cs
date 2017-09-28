#if UNITY_5 || UNITY_5_4_OR_NEWER
using System.Collections;
using UnityEngine;

namespace Routines
{
	/// <summary>
	/// Отдельный MonoBehaviour, на коором можно запускать и выполнять рутины
	/// </summary>
	public class RoutineExecutor : MonoBehaviour, IRoutineExecutor
	{
		private readonly RoutinesSet _routines = new RoutinesSet();

		private static RoutineExecutor _instance;

		public static RoutineExecutor Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<RoutineExecutor>();

					if (_instance == null)
						throw new MissingComponentException("Routine Executor is missing in scene!");
				}

				return _instance;
			}
		}

		public float ExecuteRoutineStep { get { return Time.deltaTime; } }

		/// <summary>
		/// Запустить новую рутину синглтон-вей
		/// </summary>
		public static void StartNewRoutine(IEnumerator routine)
		{
			Instance.StartRoutine(routine);
		}

		/// <summary>
		/// Запустить новую рутину
		/// </summary>
		public IRoutine StartRoutine(IEnumerator cr)
		{
			return StartRoutine(Routine.Run(cr));
		}

		public IRoutine StartRoutine(IRoutine cr)
		{
			return _routines.Add(cr);
		}

		public void StopRoutine(IRoutine cr)
		{
			_routines.Remove(cr);
		}

		public void Update()
		{
			_routines.Execute();
		}

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}
#endif