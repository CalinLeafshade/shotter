using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NDesk.Options;

namespace Shotter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var useCollector = false;
            var filename = string.Format("screenshot-{0:yyyy-MM-dd_hh-mm-ss-tt}.png", DateTime.Now);
            var show_help = false;
            
            var p = new OptionSet() {
                { "f|filename=", "The filename", v => filename = v },
                { "b|box", "Use a box", (v) => useCollector = v != null },
                { "h|help",  "show this message and exit", v => show_help = v != null },
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("greet: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `greet --help' for more information.");
                return;
            }

            if (show_help)
            {
                ShowHelp(p);
                return;
            }

            if (useCollector)
            {
                var frm = new CollectorForm {Filename = filename};

                Application.Run(frm);
            }
            else
            {

                var ScreenWidth = SystemInformation.VirtualScreen.Width;
                var ScreenHeight = SystemInformation.VirtualScreen.Height;

                var bmpScreenshot = new Bitmap(ScreenWidth, ScreenHeight, PixelFormat.Format32bppArgb);

                using (var gfxScreenshot = Graphics.FromImage(bmpScreenshot))
                {

                    // Take the screenshot from the upper left corner to the right bottom corner.
                    gfxScreenshot.CopyFromScreen(0, 0, 0, 0, new Size(ScreenWidth, ScreenHeight),
                        CopyPixelOperation.SourceCopy);

                }

                bmpScreenshot.Save(filename);

            }
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: shotter [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}
