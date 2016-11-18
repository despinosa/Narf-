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
      var buffer = new CyclicBuffer<char>(8);
      var builder = new StringBuilder();
      buffer.Write('a');
      buffer.Write('b');
      buffer.Write('c');
      buffer.Write('d');
      buffer.Write('e');
      char read;
      buffer.Read();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      buffer.Read();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      buffer.Read();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      buffer.Read();
      Assert.IsFalse(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      buffer.ReadBack();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);

      read = buffer.ReadBack();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);
      builder.Append(read);
      Assert.AreEqual(builder.ToString(), "c");

      read = buffer.ReadBack();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsTrue(buffer.HasBack);
      builder.Append(read);
      Assert.AreEqual(builder.ToString(), "cb");


      read = buffer.ReadBack();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsFalse(buffer.HasBack, "No HasBack");
      builder.Append(read);
      Assert.AreEqual(builder.ToString(), "cba");


      buffer.Write('f');

      read = buffer.Read();
      //Assert.IsTrue(buffer.HasFront, "NO HasFront");
      //Assert.IsFalse(buffer.HasBack, "No HasBack");
      builder.Append(read);
      Assert.AreEqual(builder.ToString(), "cbaa");
      buffer.Read();
      buffer.Read();
      buffer.Read();
      buffer.Read();
      read = buffer.Read();
      Assert.IsTrue(buffer.HasFront, "NO HasFront");
      Assert.IsFalse(buffer.HasBack, "No HasBack");
      builder.Append(read);
      Assert.AreEqual(builder.ToString(), "cbaaf");

      buffer.Write('g');
      buffer.Write('h');
      buffer.Write('i');
      buffer.Write('j');
      buffer.Write('k');
      buffer.Read();
      buffer.Write('l');


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