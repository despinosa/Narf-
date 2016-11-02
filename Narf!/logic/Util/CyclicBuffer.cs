using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Narf.Logic.Util {
  class CyclicBuffer<T> {
    bool _finished = false;
    public bool Finished {
      get {
        return _finished;
      }
      set {
        _finished = true;
      }
    }
    public bool HasBackward {
      get {
        return TotalWritten > ReadIndex &&
          ReadIndex != Modulo(TotalWritten, Size);
      }
    }
    public bool HasForward {
      get {
        return ReadIndex != Modulo(TotalWritten - 1, Size)
          && TotalWritten > ReadIndex;
      }
    }
    public int Size { get; }
    public int MaxAhead { get; }
    public int TotalWritten { get; private set; }
    bool IsDisposable { get; }
    int ReadIndex { get; set; }
    int DejaVu { get; set; }
    object Lock { get; }
    Semaphore Ahead { get; }
    T[] Content { get; }

    static int Modulo(int x, int m) {
      int r = x % m;
      return r < 0 ? r + m : r; 
    }

    public CyclicBuffer(int size) {
      Size = size;
      IsDisposable = typeof(T).IsAssignableFrom(typeof(IDisposable));
      Content = new T[Size];
      Lock = new object();
      MaxAhead = Size / 2;
      DejaVu = 0;
      Ahead = new Semaphore(MaxAhead, MaxAhead);
    }

    public T BackwardRead() {
      if (!HasBackward) return default(T);
      lock (Lock) {
        ++DejaVu;
        return Content[Modulo(--ReadIndex, Size)];
      }
    }

    public T ForwardRead() {
      if (Finished && !HasForward) return default(T);
      lock (Lock) {
        Ahead.Release();
        if (DejaVu > 0) DejaVu--;
        // else if (!Finished) Ahead.Release();
        return Content[Modulo(ReadIndex++, Size)];
      }
    }

    public void Write(T item) {
      if (Finished) return;
      Ahead.WaitOne();
      lock (Lock) {
        if (Content[Modulo(TotalWritten, Size)] != null
            && IsDisposable) {
          ((IDisposable)Content[Modulo(TotalWritten, Size)]).Dispose();
        }
        Content[Modulo(TotalWritten++, Size)] = item;
      }
    }
  }
}
