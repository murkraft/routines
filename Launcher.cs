using System;

namespace Routines
{
    /// <summary>
    /// Рутина запускающая заданное действие.
    /// </summary>
    public class Launcher : IRoutine
    {
        private readonly Action _action;
        private bool _started;

        public Launcher(Action action)
        {
            _action = action;
        }

        public bool MoveNext()
        {
            _started = true;
            var a = _action;
            if (a != null)
                a();
            return false;
        }

        public void Reset()
        {
            throw new System.NotSupportedException();
        }

        public object Current { get { return null; } }

        public bool IsStarted { get { return _started; } }

        public bool IsFinished { get { return _started; } }

        public void Break() {}

        public object ExitValue { get { return null; } }
    }
}