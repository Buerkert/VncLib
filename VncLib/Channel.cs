using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VncLib
{
    internal class Channel<T>
    {
        private readonly int _bufferSize;
        private readonly Queue<T> _inner = new Queue<T>();
        private readonly object lockObj = new object();

        public Channel(int bufferSize)
        {
            _bufferSize = bufferSize;
        }

        public Channel()
        {
            _bufferSize = 0;
        }

        /// <summary>
        /// Send a value to the channel
        /// </summary>
        public void Send(T item)
        {
            lock (_inner)
            {
                _inner.Enqueue(item);
                Monitor.Pulse(_inner);
            }
        }

        /// <summary>
        /// Counts the number of elements in the inner Queue
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int Count()
        {
            int count;
            lock (_inner)
            {
                count = _inner.Count;
            }

            return count;
        }

        /// <summary>
        /// Get a value from the channel
        /// </summary>
        public T Receive()
        {
            T item = default(T);
            //SpinWait.SpinUntil(() => TryReceive(out item));

            lock (_inner)
            {
                if (_inner.Count == 0)
                    Monitor.Wait(_inner);
                if (_inner.Count > 0)
                    item = _inner.Dequeue();
            }

            return item;
        }

        /// <summary>
        /// This method is used be <see cref="Select"/> to handle channel receives. It is internal because it returns a lockObject that needs to be released at some point.
        /// </summary>
        /// <param name="lockObject">the lockObject for this chanel, if you received this, you have the responsibility to release it</param>
        internal bool TryEnterExclusive(out object lockObject)
        {
            Monitor.Enter(_inner);

            lockObject = _inner;
            if (_inner.Any())
            {
                Monitor.Exit(_inner);
                return true;
            }

            lockObject = null;
            Monitor.Exit(_inner);
            return false;
        }

        public void Clear()
        {
            lock (_inner)
            {
                _inner.Clear();
            }
        }
    }
}