using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narf.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Util.Tests {
  [TestClass()]
  public class CyclicBufferTests {
    [TestMethod()]
    public void FinishedTest() {
      var buffer = new CyclicBuffer<char>(10);
      bool finished = buffer.Finished;
      Assert.IsFalse(finished);
      buffer.Write('o');
      buffer.Write('l');
      buffer.Write('a');
      buffer.Write('k');
      buffer.Write('e');
      buffer.Write('a');
      buffer.Write('s');
      buffer.Dispose();
      finished = buffer.Finished;
      Assert.IsTrue(finished);
      buffer.Write('e');
      var builder = new StringBuilder();
      char read;
      do {
        read = buffer.Read();
        builder.Append(read);
      } while (read != default(char));
      Assert.AreEqual(builder.ToString(), "olakeas");
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