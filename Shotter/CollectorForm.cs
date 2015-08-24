using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Shotter
{
    public partial class CollectorForm : Form
    {

        private int ScreenWidth
        {
            get { return SystemInformation.VirtualScreen.Width; }
        }

        private int ScreenHeight
        {
            get { return SystemInformation.VirtualScreen.Height; }
        }

        private Point? topLeft;
        private Point? bottomRight;
        private Bitmap shot;
        private int mx, my;
        private Brush dimBrush = new SolidBrush(Color.FromArgb(128,0,0,0));
        private Brush lightBrush = new SolidBrush(Color.FromArgb(128,255,255,255));

        public String Filename { get; set; }

        public CollectorForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
            TopMost = true;
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            var bmpScreenshot = new Bitmap(ScreenWidth, ScreenHeight, PixelFormat.Format32bppArgb);

            using (var gfxScreenshot = Graphics.FromImage(bmpScreenshot))
            {

                // Take the screenshot from the upper left corner to the right bottom corner.
                gfxScreenshot.CopyFromScreen(0, 0, 0, 0, new Size(ScreenWidth, ScreenHeight),
                    CopyPixelOperation.SourceCopy);

            }

            BackgroundImage = bmpScreenshot;
            shot = bmpScreenshot;
            Invalidate();
        }

        private void CollectorForm_Load(object sender, EventArgs e)
        {


        }

        private Bitmap crop(Bitmap src, Rectangle cropRect)
        {

            var target = new Bitmap(cropRect.Width, cropRect.Height);

            using (var  g = Graphics.FromImage(target))
            {
                g.DrawImage(src, new Rectangle(0, 0, target.Width, target.Height),
                                 cropRect,
                                 GraphicsUnit.Pixel);
            }

            return target;

        }

        private void takeShot()
        {
            
            var w = Math.Abs(topLeft.Value.X - bottomRight.Value.X);
            var h = Math.Abs(topLeft.Value.Y - bottomRight.Value.Y);
            var x = Math.Min(topLeft.Value.X, bottomRight.Value.X);
            var y = Math.Min(topLeft.Value.Y, bottomRight.Value.Y);

            var final = crop(shot, new Rectangle(x, y, w, h));

            if (!String.IsNullOrWhiteSpace(Filename))
                final.Save(Filename);

            Close();
        }

        private void CollectorForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (!topLeft.HasValue)
            {
                topLeft = e.Location;
            }
            else
            {
                bottomRight = e.Location;
                takeShot();
            }
        }

        private void CollectorForm_Paint(object sender, PaintEventArgs e)
        {

            e.Graphics.FillRectangle(dimBrush, Bounds);

            if (!topLeft.HasValue) return;
            var tlx = topLeft.Value.X;
            var tly = topLeft.Value.Y;
            var w = Math.Abs(tlx - mx);
            var h = Math.Abs(tly - my);
            tlx = Math.Min(tlx, mx);
            tly = Math.Min(tly, my);
            
            e.Graphics.FillRectangle(lightBrush, tlx, tly, w, h);
        }

        private void CollectorForm_MouseMove(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y;
            Invalidate();
        }
    }
}
