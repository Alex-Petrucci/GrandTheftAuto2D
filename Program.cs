namespace Gta2D
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Count() > 1)
            {
                Console.WriteLine("Too many arguments");
                return;
            }

            if (args.Count() == 0)
            {
                Console.WriteLine("Running the game.");
                Game.Run();
                return;
            }

            if (args[0] == "editor")
            {
                Console.WriteLine("Running editor");
                Editor.Run();
            }
            else
            {
                Console.WriteLine("Unknown arguments");
            }
        }
    }
}
