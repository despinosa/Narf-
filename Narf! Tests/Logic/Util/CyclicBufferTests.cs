using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narf.Logic.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Logic.Util.Tests {
  [TestClass()]
  public class CyclicBufferTests {

    static int Modulo(int x, int m) {
      int r = x % m;
      return r < 0 ? r + m : r;
    }

    [TestMethod()]
    bool _finished = false;
    bool _te = true;
    public void FinishedTest() {
        _finished = true;
      Assert.AreEqual(_finished, _te);
    }

    [TestMethod()]
    public void HasBackTest() {
      int _Size = 100;
      int _TotalWritten = 50;
      int _ReadIndex = 10; 
      int _cont = 0;
    bool _resultado = 0;
       _resultado = 0 < ReadIndex && ReadIndex < TotalWritten &&
          Modulo(ReadIndex, Size) != Modulo(TotalWritten, Size);
      Assert.AreEqual(_cont, _resultado);
    }

    [TestMethod()]
    public void HasFrontTest() {
      int _Size = 100;
      int _TotalWritten = 50;
      int _ReadIndex = 10;
      int _cont = 0;
      bool _resultado = 0;
      _resultado = Modulo(ReadIndex, Size) != Modulo(TotalWritten - 1, Size)
          && 0 <= ReadIndex && ReadIndex <= TotalWritten;
      Assert.AreEqual(_cont, _resultado);
    }

    /*[TestMethod()]
    public void HasForwardTest() {
      Assert.Fail();
    }

    [TestMethod()]
    public void ForwardReadTest() {
      Assert.Fail();
    }

    [TestMethod()]
    public void WriteTest() {
      Assert.Fail();
    }*/
  }
}