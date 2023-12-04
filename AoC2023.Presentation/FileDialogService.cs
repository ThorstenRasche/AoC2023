namespace AoC23.Presentation;

public class FileDialogService
{

    public FileDialogService()
    {
        
    }
    public string GetFilePath()
    {
#if WINDOWS
        using OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Title = "Wählen Sie die Eingabedatei aus",
            Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            return openFileDialog.FileName;
        }

        return string.Empty;
#else
Console.WriteLine("Bitte geben Sie den vollständigen Pfad zur Datei ein:");
    return Console.ReadLine();
#endif
    }
}