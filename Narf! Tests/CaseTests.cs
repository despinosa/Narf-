using Microsoft.VisualStudio.TestTools.UnitTesting;
using Narf.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Model.Test {
  [TestClass()]
  public class CaseTests {
    [TestMethod()]
    public void Create() {
      var case_ = new Case() {
        Date = new DateTime(2016, 7, 17), Duration = (short)new TimeSpan(0, 4, 53).TotalSeconds,
        Substance = "SNDRA", Dose = 12M, Subject = "Pinky", Weight = 246.9M,
        Maze = Maze.Plus, Preview = new byte[10], VideoHash = new Random().Next()
      };
      Case[] before;
      using (var context = new Entities()) {
        before = context.Cases.ToArray();
        context.Cases.Add(case_);
        context.SaveChanges();
      }
      using (var context = new Entities()) {
        var after = context.Cases.ToArray();
        Case new_ = after.First( c => ! before.Contains(c) );
        case_.Id = new_.Id;
        Assert.AreEqual(new_, case_);
      }
    }
    [TestMethod()]
    public void Delete() {
      Case target;
      using (var context = new Entities()) {
        target = context.Cases.Find(1);
        context.Cases.Remove(target);
        context.SaveChanges();
      }
      using (var context = new Entities()) {
        Assert.IsFalse(context.Cases.Contains<Case>(target));
      }
    }
  }
}