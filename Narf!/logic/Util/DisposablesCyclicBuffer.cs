using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Narf.Logic.Util {
  class DisposablesCyclicBuffer<T> where T:IDisposable {
    public int Size { get; }
    public int TotalWritten { get; private set; }
    int ReadIndex { get; set; }
    object _lock = new object();
    Semaphore UnreadCount { get; }
    T[] Content { get; }

    public DisposablesCyclicBuffer(int size) {
      Size = size;
      Content = new T[Size];
      UnreadCount = new Semaphore(Size / 2, Size * 2 / 3);
    }

    public bool HasBackward() {
      return TotalWritten > ReadIndex && ReadIndex != TotalWritten % Size;
    }

    public T BackwardRead() {
      if (!HasBackward()) return default(T);
      lock (_lock) {
        ReadIndex = (ReadIndex - 1) % Size;
        return Content[ReadIndex];
      }
    }

    public bool HasForward() {
      return ReadIndex != (TotalWritten - 1) % Size && TotalWritten > ReadIndex;
    }

    public T ForwardRead() {
      if (!HasForward()) return default(T);
      UnreadCount.Release();
      lock (_lock) {
        var retVal = Content[ReadIndex];
        ReadIndex = (ReadIndex + 1) % Size;
        return retVal;
      }
    }

    public void Write(T item) {
      UnreadCount.WaitOne();  // ensure read is at least Size/3 elements ahead
      lock (_lock) {
        if (Content[TotalWritten % Size] != null) {
          Content[TotalWritten % Size].Dispose();
        }
        Content[TotalWritten++ % Size] = item;
      }
    }
  }
}
