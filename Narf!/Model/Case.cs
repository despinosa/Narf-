using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Model {
  // public enum Maze { None, Cross }
  public class Maze {
    private static Maze _none;
    private static Maze _cross;
    private static Maze[] _all;
    public string Name { get; }
    public Type Type { get; }

    public static Maze[] All {
      get {
        if (_all == null) _all = new Maze[] { None, Cross };
        return _all;
      }
    }
    public static Maze None {
      get {
        if (_none == null) {
          _none = new Maze("Sin laberinto", typeof(Case));
        }
        return _none;
      }
    }
    public static Maze Cross {
      get {
        if (_cross == null) {
          _cross = new Maze("Laberinto en cruz", typeof(Case));
        }
        return _cross;
      }
    }
    private Maze(string name, Type type) {
      Name = name;
      Type = type;
    }

    override public string ToString() {
      return Name;
    }
  }


  public class Case {
    public DateTime Date { get; set; }
    public decimal Dose { get; set; }
    public TimeSpan Duration { get; set; }
    public Maze Maze { get; set; }
    public string Notes { get; set; }
    public Mat Preview { get; set; }
    public string Subject { get; set; }
    public string Substance { get; set; }
    public int VideoHash { get; set; }
    public decimal Weight { get; set; }
  }
}
