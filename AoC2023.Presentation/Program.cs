using AoC23.Application;
using AoC23.Presentation;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Console.WriteLine("Advent of Code 2023");
        Console.WriteLine("Bitte wählen Sie den Tag (1-4):");
        var user_day = Console.ReadLine();
        if(!user_day.TryParseToInt(out int day, false))
            Console.WriteLine("Keine Zahl");
        var calculator = DayCalculatorFactory.GetCalculator(day);
        if (calculator is null)
        {
            Console.WriteLine("Nicht verfügbar!");
            Console.ReadKey();
            return;
        }
        Console.WriteLine("Bitte wählen Sie den Teil (1-2):");
        var user_part = Console.ReadLine();        
        if (!user_part.TryParseToInt(out int part, true))
        {
            Console.WriteLine("Nicht verfügbar!");
            Console.ReadKey();
            return;
        }

        FileDialogService fileDialogService = new();
        string filePath = fileDialogService.GetFilePath();       
        
        var result = part == 1 ? calculator!.CalculatePart1(filePath) : calculator!.CalculatePart2(filePath);
        Console.WriteLine($"Ergebnis für Tag {day}, Teil {part}: {result}");
        Console.ReadKey();
    }
}
