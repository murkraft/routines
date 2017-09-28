namespace Routines
{
	/// <summary>
	/// Позволяет вернуть значение из Routine.
	/// </summary>
	public class Break
	{
		public object Value { get{ return _value; } }

		private object _value;

		/// <summary>
		/// Прервать выполнение Routine вернув указанное значение
		/// yield return Break.With(some value);
		/// </summary>
		public static Break With(object value)
		{
			return new Break{ _value = value }; 
		}
	}
}

