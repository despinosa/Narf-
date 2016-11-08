using Narf.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Narf.Util {
  public class CyclicBuffer<T> : IDisposable {
    bool _finished = false;
    public bool Finished {
      get {
        return _finished;
      }
      set {
        _finished = true;
      }
    }
    public bool HasBack {
      get {
        return 0 < ReadIndex && ReadIndex < TotalWritten &&
          Modulo(ReadIndex, Size) != Modulo(TotalWritten, Size);
      }
    }
    public bool HasFront {
      get {
        return Modulo(ReadIndex, Size) != Modulo(TotalWritten - 1, Size)
          && 0 <= ReadIndex && ReadIndex <= TotalWritten;
      }
    }
    public int Size { get; }
    public int MaxAhead { get; }
    public int TotalWritten { get; private set; }
    bool TIsDisposable { get; }
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
      TIsDisposable = typeof(T).IsAssignableFrom(typeof(IDisposable));
      Content = new T[Size];
      Lock = new object();
      MaxAhead = (int)(Size * Settings.Default.BufferAheadRatio);
      DejaVu = 0;
      Ahead = new Semaphore(MaxAhead, MaxAhead);
    }

    public T ReadBack() {
      if (!HasBack) return default(T);
      lock (Lock) {
        ++DejaVu;
        return Content[Modulo(--ReadIndex, Size)];
      }
    }

    public T Read() {
      if (Finished && !HasFront) return default(T);
      lock (Lock) {
        if (DejaVu > 0) --DejaVu;
        else if (!Finished) Ahead.Release();
        // Ahead.Release();
        return Content[Modulo(ReadIndex++, Size)];
      }
    }

    public void Write(T item) {
      if (Finished) return;
      Ahead.WaitOne();
      lock (Lock) {
        if (Content[Modulo(TotalWritten, Size)] != null
            && TIsDisposable) {
          ((IDisposable)Content[Modulo(TotalWritten, Size)]).Dispose();
        }
        Content[Modulo(TotalWritten++, Size)] = item;
      }
    }

    public void Dispose() {
      Finished = true;
      Ahead.Dispose();
      if (TIsDisposable) {
        foreach (var item in Content) ((IDisposable)item).Dispose();
      }
    }
  }
}
