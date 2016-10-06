using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narf.Model {
  // public enum MazeType { None, Cross }
  public class MazeType {
    private static MazeType _none;
    private static MazeType _cross;
    public string Name { get; }
    private MazeType(string name) {
      Name = name;
    }
    public static MazeType None {
      get {
        if(_none == null) _none = new MazeType("Sin laberinto");
        return _none;
      }
    }
    public static MazeType Cross {
      get {
        if(_cross == null) _cross = new MazeType("Laberinto en cruz");
        return _cross;
      }
    }
    override public string ToString() {
      return Name;
    }
  }

  class Case {
    public DateTime Date { get; set; }
    public string Substance { get; set; }
    public string Rat { get; set; }
    public decimal Weight { get; set; }
    public string Notes { get; set; }
    public MazeType MazeType { get; set; }
  }
}
