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
      Assert.IsFalse(buffer.Finished);
      buffer.Write('o');
      buffer.Write('l');
      buffer.Write('a');
      buffer.Write('k');
      buffer.Write('e');
      buffer.Write('a');
      buffer.Write('s');
      buffer.Dispose();
      Assert.IsTrue(buffer.Finished);
      buffer.Write('e');
      var builder = new StringBuilder();
      char read;
      do {
        read = buffer.Read();
        builder.Append(read);
      } while (read != default(char));
      Assert.AreEqual(builder.ToString(), "olakeas");
    }

    [TestMethod()]
    public void HasBack() {
      var buffer = new CyclicBuffer<char>(10);
      buffer.Write('o');
      buffer.Write('l');
      buffer.Write('i');
      buffer.Write('t');
      buffer.Write('a');
      char read;
      read = buffer.Read();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      read = buffer.Read();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      read = buffer.Read();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      read = buffer.Read();
      Assert.IsFalse(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      read = buffer.ReadBack();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      read = buffer.ReadBack();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      var builder = new StringBuilder();
      builder.Append(read);

      Assert.AreEqual(builder.ToString(), "i");
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