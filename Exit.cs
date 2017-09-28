namespace Routines
{
	public class Exit
	{
		public object Value { get { return _value; } }

		private object _value;

		/// <summary>
		/// Прервать выполнение Routine и всех родительских рутин вернув указанное значение
		/// yield return Exit.With(some value);
		/// </summary>
		public static Exit With(object value)
		{
			return new Exit { _value = value };
		}
	}
}