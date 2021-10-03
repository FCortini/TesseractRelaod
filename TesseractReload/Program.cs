using IronOcr;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.IO;

namespace TessercatTest
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        [STAThread]
        static void Main(string[] args)
        {
            args = new[] { "-help" };
            //args = new[] { @"C:\Users\Francesco\Desktop\Immagine_2021_09_26.jpg" , "-Contrast","-fileOutputDir" , @"C:\Users\Francesco\Downloads" };
            //args = new[] { @"C:\Users\Francesco\Desktop\CV_FC_2021.pdf", "-FromPdf" ,"","0"};
            //args = new[] { @"C:\Users\Francesco\Desktop\CV_FC_2021.pdf", "-AddPdfPage", "9","","0"};

            if (args.Length == 0)
            {
                Console.WriteLine("Error! you should provide at least the file full path");
            }

            string fileOutputDir = Path.ChangeExtension(args[0], ".txt");
            /*
            if (args.Length==0) 
            {
                var handle = GetConsoleWindow();
                // Hide
                ShowWindow(handle, SW_HIDE);
                Application.Run(new Form1());
                return;
            }*/
            List<string> list = args.ToList();

            // PM > Install-Package IronOcr
            // using IronOcr
            var Ocr = new IronTesseract();
            // Hundreds of languages available 
            Ocr.Language = OcrLanguage.English;
            var Input = new OcrInput();

            try {
                SettingInput(ref Input, list, ref fileOutputDir);
            } catch (Exception)
            {
                //lancio eccezione
            }

            try
            {
                IronOcr.OcrResult Result = Ocr.Read(Input);
                Console.WriteLine(Result.Text);
                //string path = @"C:\Users\Francesco\Desktop\MyTest.txt";
                using (StreamWriter sw = File.CreateText(fileOutputDir))
                {
                    sw.WriteLine(Result.Text);
                }
            }
            catch (Exception) 
            {
                //lancio eccezione
            }
            Console.ReadLine();
        }

        private static void SettingInput(ref OcrInput Input, List<string> list, ref string fileOutputDir)
        {
            if (list.Contains("-help"))
            {
                HelpCall();
                return;
            }

            if (list.Contains("-FromPdf"))
            {
                int pos = list.IndexOf("-FromPdf");
                string password = list[pos + 1];
                int DPI = Int32.Parse(list[pos + 2]);
                Input.AddPdf(list[0], password, null, DPI);
            }
            else if (list.Contains("-AddPdfPage"))
            {
                int pos = list.IndexOf("-AddPdfPage");
                int page = Int32.Parse(list[pos + 1]);
                string password = list[pos + 2];
                int DPI = Int32.Parse(list[pos + 3]);
                Input.AddPdfPage(list[0], page, password, null, DPI);
            }
            else if (list.Contains("-AddPdfPages"))
            {
                int pos = list.IndexOf("-AddPdfPages");
                IEnumerable<int> Pages = Array.ConvertAll(list[pos + 1].Split(','), s => int.Parse(s)).ToList();
                string password = list[pos + 2];
                int DPI = Int32.Parse(list[pos + 1]);
                Input.AddPdfPages(list[0], Pages, password, null, DPI);
            }
            else
            {
                //File path to an image file. Supported formats include JPEG, TIFF, GIF, PNG, PDF, BMP.
                Input.AddImage(list[0]);
            }

            /*
            if (list.Contains("-Binarize"))
            {
                Input.Binarize();
                Input.Contrast();
                Input.DeepCleanBackgroundNoise();
                Input.DeNoise();
                Input.Deskew();
                Input.Dilate();
                Input.Dispose();
                Input.EnhanceResolution();
                Input.Erode();
                Input.Invert();
                //Input.ReplaceColor();
                //Input.Rotate();
                //Input.Scale();
                Input.Sharpen();
                Input.ToGrayScale();
            }*/

            if (list.Contains("-Contrast")) { Input.Contrast(); }
            if (list.Contains("-DeepCleanBackgroundNoise")) { Input.DeepCleanBackgroundNoise(); }
            if (list.Contains("-DeNoise")) { Input.DeNoise(); }
            if (list.Contains("-Deskew")) { Input.Deskew(); }
            if (list.Contains("-Dilate")) { Input.Dilate(); }
            if (list.Contains("-Dispose")) { Input.Dispose(); }
            if (list.Contains("-EnhanceResolution")) { Input.EnhanceResolution(); }
            if (list.Contains("-Erode")) { Input.Erode(); }
            if (list.Contains("-Invert")) { Input.Invert(); }

            if (list.Contains("-ReplaceColor"))
            {
                int pos = list.IndexOf("-ReplaceColor");
                string currentColor = list[pos + 1];
                string newColor = list[pos + 2];
                int tollerance = Int32.Parse(list[pos + 3]);
                Input.ReplaceColor(StringToColor(currentColor), StringToColor(newColor), tollerance);
            }
            if (list.Contains("-Rotate"))
            {
                int pos = list.IndexOf("-Rotate");
                Input.Rotate(Int32.Parse(list[pos + 1]));
            }
            if (list.Contains("-Scale"))
            {
                int pos = list.IndexOf("-Scale");
                Input.Scale(Int32.Parse(list[pos + 1]), Int32.Parse(list[pos + 2]));
            }

            if (list.Contains("-Sharpen")) { Input.Sharpen(); }
            if (list.Contains("-ToGrayScale")) { Input.ToGrayScale(); }

            if (list.Contains("-fileOutputDir")) {
                int pos = list.IndexOf("-fileOutputDir");
                fileOutputDir = list[pos + 1] + Path.GetFileNameWithoutExtension(fileOutputDir) + ".txt";
            }
            if (list.Contains("-fileOutputFullPath"))
            {
                int pos = list.IndexOf("-fileOutputFullPath");
                fileOutputDir = list[pos + 1];
            }
            
            if (list.Contains("-help-ColorList"))
            {
                Console.WriteLine("not implemented");
            }
        }

        private static Color StringToColor(string currentColor)
        {
            try
            {
                return Color.FromName(currentColor);
            }
            catch (Exception) { }
            return Color.Black;
        }
        private static void HelpCall()
        {
            Console.WriteLine("-FromPdf                         <password> <DPI>");
            Console.WriteLine("                                 password, \"\" if empty");
            Console.WriteLine("                                 DPI, integer, 0 as Default");
            Console.WriteLine("");
            Console.WriteLine("-AddPdfPage                      <page> <password> <DPI>");
            Console.WriteLine("                                 page, integer number, the page you want, start from 0");
            Console.WriteLine("");
            Console.WriteLine("-AddPdfPages                     <pages> <password> <DPI>");
            Console.WriteLine("                                 pages, integer number dividede with \',\' , with no space, start from 0");
            Console.WriteLine("");
            Console.WriteLine("-Contrast                        Increases contrast automatically. This filter often improves OCR speed and accuracy");
            Console.WriteLine("                                 in low contrast scans. Flattens Alpha channels to white.");
            Console.WriteLine("");
            Console.WriteLine("-DeepCleanBackgroundNoise        Heavy background noise removal. Only use this filter in case extreme document");
            Console.WriteLine("                                 background noise is known, because this filter will also risk reducing OCR accuracy");
            Console.WriteLine("                                 of clean documents, and is very CPU expensive.");
            Console.WriteLine("");
            Console.WriteLine("-DeNoise                         Removes digital noise. This filter should only be used where noise is expected.");
            Console.WriteLine("-Deskew                          Rotates an image so it is the right way up and orthogonal. This is very useful");
            Console.WriteLine("                                   for OCR because Tesseract tolerance for skewed scans can be as low as 5 degrees.");
            Console.WriteLine("");
            Console.WriteLine("-Dilate                          Advanced Morphology. Opposite of IronOcr.OcrInput.Erode(System.Boolean).");
            Console.WriteLine("");
            Console.WriteLine("-Dispose                         OcrInput is IDisposable. For best practice and to avoid memory leaks, remember");
            Console.WriteLine("                                 to dispose, or initialize instances with a \"using\" statement.");
            Console.WriteLine("");
            Console.WriteLine("-EnhanceResolution               Enhances the resolution of low quality images. This filter is not often needed");
            Console.WriteLine("-EnhanceResolution               because IronOcr.OcrInput.MinimumDPI and IronOcr.OcrInput.TargetDPI will automatically");
            Console.WriteLine("-EnhanceResolution               catch and resolve low resolution inputs.");
            Console.WriteLine("-EnhanceResolution               May not work for all images if their metadata is corrupted.");
            Console.WriteLine("");
            Console.WriteLine("-Erode                           Advanced Morphology. Opposite of IronOcr.OcrInput.Dilate(System.Boolean).");
            Console.WriteLine("-Invert                          Inverts every color. E.g. White becomes black : black becomes white.");
            Console.WriteLine("");
            Console.WriteLine("-ReplaceColor                    <currentColor> <newColor> <tollerance>");
            Console.WriteLine("                                 currentColor, based on Color library, use -help-ColorList to get the list");
            Console.WriteLine("                                 newColor, based on Color library, use -help-ColorList to get the list");
            Console.WriteLine("                                 tollerance, integer number");
            Console.WriteLine("");
            Console.WriteLine("-Rotate                          <Degrees>");
            Console.WriteLine("                                 Degrees, integer number");
            Console.WriteLine("                                 Rotates images by a number of degrees clockwise. For anti-clockwise, use negative");
            Console.WriteLine("                                 numbers.");
            Console.WriteLine("");
            Console.WriteLine("-Scale                           <Percentage>");
            Console.WriteLine("                                 Percentage, integer number");
            Console.WriteLine("                                 The percentage scale. 100 = no effect.");
            Console.WriteLine("");
            Console.WriteLine("-Sharpen                         Sharpens blurred OCR Documents. Flattens Alpha channels to white.");
            Console.WriteLine("");
            Console.WriteLine("-ToGrayScale                     This image filter turns every pixel into a shade of grayscale. Unlikely to improve");
            Console.WriteLine("                                 OCR accuracy but may improve speed.");
            Console.WriteLine("");
            Console.WriteLine("-fileOutputDir                   [fileOutputDir]");
            Console.WriteLine("                                 fileOutputDir, full path directory, the new file name is the same");
            Console.WriteLine("-fileOutputFullPath              [fileOutputFullPath]");
            Console.WriteLine("                                 fileOutputFullPath, change the full path, name and extension");
            Console.WriteLine("");
            Console.WriteLine("-langauge                        Not impalemnted yet");
        }
    }
}
