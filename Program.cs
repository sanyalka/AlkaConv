using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PdfiumViewer;
using System.Windows.Forms;
using Microsoft.Win32;


class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // GUI режим
        if (args.Length == 0)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            return;
        }

        // HELP режим
        if (args.Length == 1)
        {
            string a = args[0].ToLower();
            if (a == "-h" || a == "-help" || a == "/?")
            {
                ShowHelp();
                return;
            }
        }

        // CLI режим
        RunCli(args);
    }

    static void RunCli(string[] args)
    {
        string pdfPath = "";
        int dpi = 100;
        string? outputDir = null;
        string? outputPrefix = null;
        bool useSubfolder = false;
        bool deleteAfter = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-dpi":
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out int dpiValue))
                    {
                        dpi = dpiValue;
                        i++;
                    }
                    break;

                case "-o":
                    if (i + 1 < args.Length)
                    {
                        outputDir = args[i + 1];
                        i++;
                    }
                    break;

                case "-prefix":
                    if (i + 1 < args.Length)
                    {
                        outputPrefix = args[i + 1];
                        i++;
                    }
                    break;

                case "-subfolder":
                    useSubfolder = true;
                    break;

                case "-delete":
                    deleteAfter = true;
                    break;

                default:
                    if (pdfPath == "")
                        pdfPath = args[i];
                    break;
            }
        }

        if (string.IsNullOrEmpty(pdfPath))
        {
            Console.WriteLine("Error: PDF file not specified");
            return;
        }

        if (!File.Exists(pdfPath))
        {
            Console.WriteLine($"Error: File not found: {pdfPath}");
            return;
        }

        string pdfDir = Path.GetDirectoryName(pdfPath) ?? Environment.CurrentDirectory;
        string pdfName = Path.GetFileNameWithoutExtension(pdfPath);

        if (useSubfolder)
            outputDir = Path.Combine(pdfDir, pdfName);
        else if (string.IsNullOrEmpty(outputDir))
            outputDir = pdfDir;

        if (!Directory.Exists(outputDir))
            Directory.CreateDirectory(outputDir);

        if (string.IsNullOrEmpty(outputPrefix))
            outputPrefix = pdfName;

        try
        {
            Console.WriteLine($"Converting: {pdfPath}");
            Console.WriteLine($"Output: {outputDir}");

            using (var document = PdfDocument.Load(pdfPath))
            {
                int totalPages = document.PageCount;

                for (int page = 0; page < totalPages; page++)
                {
                    using var image = document.Render(page, dpi, dpi, true);
                    string path = Path.Combine(outputDir, $"{outputPrefix}-{page + 1:D4}.jpg");
                    image.Save(path, ImageFormat.Jpeg);
                }
            }

            if (deleteAfter)
                File.Delete(pdfPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine(@"
PDF to Image Converter

Usage:
  PdfToImage.exe <file.pdf> [options]

Options:
  -dpi <number>     DPI (default 100)
  -o <dir>          Output folder
  -prefix <name>    Output file prefix
  -subfolder        Create subfolder
  -delete           Delete PDF after conversion

Help:
  -h, -help, /?     Show help

Examples:
  PdfToImage.exe file.pdf
  PdfToImage.exe file.pdf -dpi 300 -subfolder
  PdfToImage.exe file.pdf -o C:\Images -delete
");
    }
}