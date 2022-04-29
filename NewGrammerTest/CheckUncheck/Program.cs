// check uncheck
//uint i = unchecked((uint)-1);
//byte b = 200;
//try
//{

//    b = checked((byte)(b + 200));
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//}
//Console.WriteLine(i);
//Console.WriteLine(b);

// box unbox
//int v = 5;
//object o = v;
//v = 123;
//Console.WriteLine(v + "," + (int)o);

Point p = new Point(1, 1);
Console.WriteLine(p);
p.Change(2, 2);
Console.WriteLine(p);
object o = p;
Console.WriteLine(o);
((Point)o).Change(3, 3);
Console.WriteLine(o);

string newLine = "hi" + Environment.NewLine + "there";
Console.WriteLine(newLine);
struct Point
{
    private int m_x, m_y;
    public Point(int x, int y)
    {
        m_x = x; m_y = y; 
    }

    public void Change(int x, int y)
    {
        m_x = x; m_y = y;
    }

    public override string ToString()
    {
        return $"{m_x}, {m_y}";
    }
}

