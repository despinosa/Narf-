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
    public void CaseTest() {
      var case_ = new Case() {
        Date = new DateTime(2016, 7, 17), Duration = 4 * 60 + 53,
        Substance = "SNDRA", Dose = 12M, Subject = "Pinky", Weight = 246.9M,
        Maze = Maze.Cross
      };
      using (var context = new Entities()) {
        context.Cases.Add(case_);
        context.SaveChanges();
      }
      using (var context = new Entities()) {
        var weight = from c in context.Cases where c.Subject == case_.Subject
                     & c.Maze == case_.Maze select c.Weight;
        Assert.Equals(weight, case_.Weight);
      }
    }
  }
}