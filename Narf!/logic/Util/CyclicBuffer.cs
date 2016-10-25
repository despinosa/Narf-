using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic.Util {
  class CyclicBuffer<T> where T:IDisposable {
    public int Size { get; }
    T[] Content { get; }
    int Written { get; set; }
    int Buffered { get; set; }
    public CyclicBuffer(int size) {
      Size = size;
      Content = new T[Size];
    }
    public bool HasBackward() {
      return Buffered > 0 && Buffered <= Size;
    }
    public T BackwardRead() {
      if (Buffered >= Size) return default(T);
      Buffered++;
      var index = (Written % Size - Buffered) % Size;
      return Content[index];
    }
    public bool HasForward() {
      return Buffered > 0;
    }
    public T ForwardRead() {
      if (Buffered < 1) return default(T);
      Buffered--;
      var index = (Written % Size - Buffered) % Size;
      return Content[index];
    }
    public void Write(T item) {
      if (Content[Written % Size] != null) {
        Content[Written % Size].Dispose();
      }
      Content[Written % Size] = item;
      Written++;
    }
  }
}
