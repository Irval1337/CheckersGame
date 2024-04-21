// See https://aka.ms/new-console-template for more information
using CheckersGame;
using System.Linq;

Game g = new Game(8, 8);

while (true)
{
    for(int i = 0; i < 8; i++)
    {
        for(int j = 0; j < 8; j++)
        {
            Console.Write(g.getBoard()[i][j].playerId);
        }
        Console.WriteLine();
    }
    try
    {
        int x, y, x1, y1;
        var a = Console.ReadLine().Split(' ').ToList().Select(x => Convert.ToInt32(x)).ToList();
        var b = g.move(new Move((a[0], a[1]), (a[2], a[3])));
        Console.WriteLine(b);
    } catch (Exception ex) { Console.WriteLine(ex.Message); }
}