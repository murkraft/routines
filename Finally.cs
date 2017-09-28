using System;

namespace Routines
{
    /// <summary>
    /// Даёт возможность выполнить определённый код при окончании или прерывании рутины
    /// </summary>
	public class Finally
	{
		private Action _action;

		public void Execute()
		{
			if(_action!=null)
				_action();
		}

		public static Finally Action(Action action)
		{
			return new Finally { _action = action };
		}
	}
}
