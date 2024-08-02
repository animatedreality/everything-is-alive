/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Oculus.Interaction
{
    ///<summary>
    /// Implements a generic Ring Buffer data structure with a fixed capacity.
    /// This buffer overwrites its elements in a circular manner when new elements are added beyond its capacity.
    ///</summary>
    public class RingBuffer<T>
    {
        private T[] _buffer;
        private int _head;
        private int _count;

        /// <summary>
        /// Allocates and initialises the buffer
        /// </summary>
        /// <param name="capacity">The length of the buffer</param>
        /// <param name="defaultValue">The initialisation value</param>
        public RingBuffer(int capacity, T defaultValue)
        {
            _buffer = new T[capacity];
            _count = capacity;
            Clear(defaultValue);
        }

        /// <summary>
        /// Initialises the buffer setting all values to the clear value
        /// </summary>
        /// <param name="clearValue">The value to set in all entries</param>
        public void Clear(T clearValue)
        {
            _head = 0;
            for (int i = 0; i < _count; i++)
            {
                _buffer[i] = clearValue;
            }
        }

        /// <summary>
        /// Inserts a value in the next entry of the buffer.
        /// It overwrites the value already present there.
        /// </summary>
        /// <param name="item">The value to set</param>
        public void Add(T item)
        {
            _buffer[_head] = item;
            _head = (_head + 1) % _count;
        }
        ///<summary>
        /// This indexer method provides read-only access to elements in the buffer.
        ///</summary>
        /// <param name="index">The index of the element to access.</param>
        /// <returns>The element at the specified index in the buffer.</returns>
        public T this[int index] => _buffer[((index % _count) + _count) % _count];

        ///<summary>
        /// Gets the element at a specified offset from the current head of the buffer.
        /// It's useful for looking ahead or behind the buffer without altering its state.
        ///</summary>
        /// <param name="offset">The offset from the head of the buffer. Defaults to 0.</param>
        /// <returns>The element at the calculated position based on the offset.</returns>
        public T Peek(int offset = 0) => _buffer[(((_head + offset) % _count) + _count) % _count];
    }
}
