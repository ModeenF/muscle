
///<summary>
/// This class implements a templated double-ended queue data structure.
/// Adding or removing items from the head or tail of a Queue is (on average) an O(1) operation.
/// </summary>
namespace muscle.queue {
    public class Queue
    {        
        private Object _queue[] = null;  // demand-allocated object array
        private int _elementCount = 0;  // number of valid elements in the array
        private int _headIndex = -1;   // index of the first filled slot, or -1 
        private int _tailIndex = -1;   // index of the last filled slot, or -1

        public Queue()
        {
            // empty
        }

        public Queue(Queue copyMe)
        {
            setEqualTo(copyMe);
        }

        ///<summary>Make (this) a shallow copy of the passed-in queue.</summary>
        ///<param name="setTo">What to set this queue equal to.</param>
        public void setEqualTo(Queue setTo)
        {
            removeAllElements();
            int len = setTo.size();
            ensureSize(len);
            
            for (int i=0; i<len; i++)
                appendElement(setTo.elementAt(i));
        }

        ///<summary>Appends (obj) to the end of the queue.  Queue size grows by one.</summary>
        ///<param name="obj">The element to append.</param>
        public void appendElement(Object obj)
        {
            ensureSize(_elementCount+1);
            if (_elementCount == 0)
                _headIndex = _tailIndex = 0;
            else
                _tailIndex = nextIndex(_tailIndex);
            
            _queue[_tailIndex] = obj;
            _elementCount++;
        }

        ///<summary>Prepends (obj) to the head of the queue.  Queue size grows by one.</summary>
        ///<param name="obj">The Object to prepend. </param>
        public void prependElement(Object obj)
        {
            ensureSize(_elementCount+1);
            
            if (_elementCount == 0)
                _headIndex = _tailIndex = 0;
            else
                _headIndex = prevIndex(_headIndex);
            
            _queue[_headIndex] = obj;
            _elementCount++;
        }

        ///<summary>Removes and returns the element at the head of the queue.</summary>
        ///<rereturns>The removed Object on success, or null if the Queue was empty.</rereturns>
        public Object removeFirstElement()
        {
            Object ret = null;
            if (_elementCount > 0)
            {
                ret = _queue[_headIndex];
                _queue[_headIndex] = null;   // allow garbage collection
                _headIndex = nextIndex(_headIndex);
                _elementCount--;
            }
            return ret;
        }

        ///<summary>Removes and returns the element at the tail of the Queue.</summary>
        ///<returns>The removed Object, or null if the Queue was empty.</returns>
        public Object removeLastElement()
        {
            Object ret = null;
            if (_elementCount > 0)
            {       
                ret = _queue[_tailIndex];
                _queue[_tailIndex] = null;   // allow garbage collection
                _tailIndex = prevIndex(_tailIndex);
                _elementCount--;
            }
            return ret;
        }


        ///<summary>removes the element at the (index)'th position in the queue.</summary>
        ///<param name="idx">Which element to remove--can range from zero 
        /// (head of the queue) to CountElements()-1 (tail of the queue).</param>
        ///<returns>The removed Object, or null if the Queue was empty.
        /// Note that this method is somewhat inefficient for indices that
        /// aren't at the head or tail of the queue (i.e. O(n) time)</returns>
        ///<exception cref="IndexOutOfBoundsException">if (idx) isn't a valid index.</exception>
        public Object removeElementAt(int idx)
        {
            if (idx == 0)
                return removeFirstElement();
      
            int index = internalizeIndex(idx);
            Object ret = _queue[index];   

            while(index != _tailIndex)
            {
                int next = nextIndex(index);
                _queue[index] = _queue[next];
                index = next;
            }

            _queue[_tailIndex] = null;    // allow garbage collection 
            _tailIndex = prevIndex(_tailIndex); 
            _elementCount--;
            return ret;
        }


        ///<summary>Copies the (index)'th element into (returnElement).</summary>
        ///<param name="index">Which element to get--can range from zero 
        /// (head of the queue) to (CountElements()-1) (tail of the queue).</param>
        ///<returns>The Object at the given index.</returns>
        ///<exception cref="IndexOutOfBoundsException">if (index) isn't a valid index.</exception>
        public Object elementAt(int index) {
            return _queue[internalizeIndex(index)];
        }

        ///<summary>Replaces the (index)'th element in the queue with (newElement).</summary>
        ///<param name="index">Which element to replace--can range from zero 
        /// (head of the queue) to (CountElements()-1) (tail of the queue).</param>
        ///<param name="newElement">The element to place into the queue at the (index)'th position.</param>
        ///<returns>The Object that was previously in the (index)'th position.</returns>
        ///<exception cref="IndexOutOfBoundsException">if (index) isn't a valid index.</exception>
        public Object setElementAt(int index, Object newElement)
        {
            int iidx = internalizeIndex(index);
            Object ret = _queue[iidx];
            _queue[iidx] = newElement;
            return ret;
        }
 
        ///<summary>Inserts (element) into the (nth) slot in the array.  InsertElementAt(0)
        /// is the same as addHead(element), InsertElementAt(CountElements()) is the same
        /// as addTail(element).  Other positions will involve an O(n) shifting of contents.</summary>
        ///<param name="index">The position at which to insert the new element.</param>
        ///<param name="newElement">The element to insert into the queue.</param>
        ///<exception cref="IndexOutOfBoundsException">if (index) isn't a valid index.</exception>
        public void insertElementAt(int index, Object newElement)
        {
            if (index == 0)
                prependElement(newElement);
            else
            {
                // Harder case:  inserting into the middle of the array
                appendElement(null);  // just to allocate the extra slot
                int lo = (int) index;
                for (int i = _elementCount-1; i > lo; i--)
                    _queue[internalizeIndex(i)] = _queue[internalizeIndex(i-1)];
                _queue[internalizeIndex(index)] = newElement;
            }
        }

        ///<summary>Removes all elements from the queue.</summary>
        public void removeAllElements() 
        {
            if (_elementCount > 0)
            {
                // clear valid array elements to allow immediate garbage collection
                if (_tailIndex >= _headIndex)
                    java.util.Arrays.fill(_queue, _headIndex, _tailIndex+1, null);
                else
                {
                    java.util.Arrays.fill(_queue, 0, _headIndex+1,  null);
                    java.util.Arrays.fill(_queue, _tailIndex, _queue.length, null);
                }
                _elementCount = 0;
                _headIndex    = -1;
                _tailIndex    = -1; 
            }
        }

        ///<summary>Returns the number of elements in the queue.  (This number does not include pre-allocated space)</summary>
        public int size() {
            return _elementCount;
        }
        
        ///<summary>Returns true iff their are no elements in the queue.</summary>
        public boolean isEmpty() {
            return (_elementCount == 0);
        }
        
        ///<summary>Returns the head element in the queue.  You must not call this when the queue is empty!</summary>
        public Object firstElement() {
            return elementAt(0);
        }
        
        ///<summary>Returns the tail element in the queue.  You must not call this when the queue is empty!</summary>
        public Object lastElement() {
            return elementAt(_elementCount-1);
        }

        /// <summary>
        /// Makes sure there is enough space preallocated to hold at least
        /// (numElements) elements.  You only need to call this if
        /// you wish to minimize the number of array reallocations done. 
        /// </summary>
        /// <param name="numElements">the minimum amount of elements to pre-allocate space for in the Queue.</param>
        public void ensureSize(int numElements)
        {
            if ((_queue == null) || (_queue.length < numElements))
            {
                Object newQueue[] = new Object[(numElements < 5) ? 5 : numElements*2];    
                if (_elementCount > 0)
                {
                    for (int i = 0; i < _elementCount; i++)
                        newQueue[i] = elementAt(i);
                    _headIndex = 0;
                    _tailIndex = _elementCount-1;
                }
                _queue = newQueue;
            }
        }

        /// <summary>
        /// Returns the last index of the given (element), or -1 if (element) is
        /// not found in the list.  O(n) search time.
        /// </summary>
        /// <param name="element">The element to look for.</param>
        /// <returns>The index of (element), or -1 if no such element is present.</returns>       
        public int indexOf(Object element)
        {
            if (_queue != null)
                for (int i = size()-1; i >= 0; i--)
                    if (elementAt(i) == element)
                        return i;
            return -1;
        }
   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int nextIndex(int idx) {
            return (idx >= getArraySize()-1) ? 0 : idx+1;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int prevIndex(int idx) {
            return (idx <= 0) ? getArraySize()-1 : idx-1;
        }

        /// <summary>
        /// Translates a user-index into an index into the _queue array.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int internalizeIndex(int idx) 
        {
            if ((idx < 0) || (idx >= _elementCount))
                throw new IndexOutOfBoundsException("bad index " + idx + " (queuelen=" + _elementCount + ")");
            return (_headIndex + idx) % getArraySize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int getArraySize() {
            return (_queue != null) ? _queue.length : 0;
        }
    }
}
