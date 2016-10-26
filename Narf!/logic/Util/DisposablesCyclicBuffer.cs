using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic.Util {
  class DisposablesCyclicBuffer<T> where T:IDisposable {
    public int Size { get; }
    T[] Content { get; }
    int Written { get; set; }
    int Buffered { get; set; }
    public DisposablesCyclicBuffer(int size) {
      Size = size;
      Content = new T[Size];
    }

    public bool HasBackward() {
      return Written > 0 && Buffered < Size;
    }

    public T BackwardRead() {
      if (!HasBackward()) return default(T);
      int index = (Written % Size - ++Buffered) % Size;
      return Content[index];
    }

    public bool HasForward() {
      return Buffered > 0;
    }

    public T ForwardRead() {
      if (!HasForward()) return default(T);
      int index = (Written % Size - --Buffered) % Size;
      return Content[index];
    }

    public void Write(T item) {
      if (Content[Written % Size] != null) {
        Content[Written % Size].Dispose();
      }
      Content[Written % Size] = item;
      Written++;
      if (HasBackward() && HasForward()) Buffered++;
    }
  }
}
