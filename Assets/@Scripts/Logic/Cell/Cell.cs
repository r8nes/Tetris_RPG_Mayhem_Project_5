namespace TetrisMayhem.Logic
{
    public class Cell 
    {
		public int X { get; }
		public int Y { get; }
		public static float Width { get; set; }
		public CellElement Element { get; set; }
		public Cell(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
