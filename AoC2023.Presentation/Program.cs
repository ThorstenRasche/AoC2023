using AoC2023.Application;
using AoC23.Presentation;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {

        Console.WriteLine("Advent of Code 2023");
        Console.WriteLine("Bitte wählen Sie den Tag (1-24):");
        int day = int.Parse(Console.ReadLine());
        Console.WriteLine("Bitte wählen Sie den Teil (1-2):");
        int part = int.Parse(Console.ReadLine());
        FileDialogService fileDialogService = new();
        string filePath = fileDialogService.GetFilePath();
        //string filePath = String.Empty;
        //using OpenFileDialog openFileDialog = new OpenFileDialog
        //{
        //    Title = "Wählen Sie die Eingabedatei aus",
        //    Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
        //    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        //};

        //if (openFileDialog.ShowDialog() == DialogResult.OK)
        //{
        //    filePath = openFileDialog.FileName;
        //}



        var calculator = DayCalculatorFactory.GetCalculator(day);
        if (calculator is null)
            Console.WriteLine("Nicht verfügbar!");
        var result = part == 1 ? calculator!.CalculatePart1(filePath) : calculator!.CalculatePart2(filePath);
        Console.WriteLine($"Ergebnis für Tag {day}, Teil {part}: {result}");

    }
}
