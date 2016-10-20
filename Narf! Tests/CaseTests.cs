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
      Random rnd = new Random();
      var case1_ = new Case() {
        Date = new DateTime(2016, 7, 17), Duration = (short)new TimeSpan(0, 4, 53).TotalSeconds,
        Substance = "SNDRA", Dose = 12M, Subject = "Pinky", Weight = 246.9M,
        Maze = Maze.Plus, Preview = new byte[10], VideoHash = rnd.Next()
      };
      var case2_ = new Case() {
        Date = new DateTime(2016, 7, 18), Duration = (short)new TimeSpan(0, 4, 53).TotalSeconds,
        Substance = "SRA", Dose = 10M, Subject = "Rat", Weight = 246.1M,
        Maze = Maze.Plus, Preview = new byte[10], VideoHash = rnd.Next()
      };

      Case[] before;
      using (var context = new Entities()) {
        before = context.Cases.ToArray();
        context.Cases.Add(case1_);
        context.SaveChanges();
        context.Cases.Add(case2_);
        context.SaveChanges();
      }
      using (var context = new Entities()) {
        var after = context.Cases.ToArray();
        Case new_ = after.First( c => ! before.Contains(c) );
        case1_.Id = new_.Id;
        Assert.AreEqual(new_.Id, case1_.Id);
      }
    }
    [TestMethod()]
    public void Delete() {
      Case target;
      using (var context = new Entities()) {
        target = context.Cases.Find(4);
        context.Cases.Remove(target);
        context.SaveChanges();
      }
      using (var context = new Entities()) {
        Assert.IsFalse(context.Cases.Contains<Case>(target));
      }
    }
  }
}