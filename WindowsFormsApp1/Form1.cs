using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    

    public partial class Form1 : Form
    {
        double xMin = -2.5;
        double xMax = 1.5;
        double yMin = -1;
        double yMax = 1;
        private double zoom = 1;
        private Point startPoint;
        private bool isDragging;
        double xStep;
        double yStep;

        public Form1()
        {
            
            InitializeComponent();

            DrawMandelbrot(this, xMin, xMax, yMin, yMax, pictureBox1);
            pictureBox1.Invalidate();
            //this.MouseWheel += new MouseEventHandler(Mandelbrot_MouseWheel);
            pictureBox1.MouseUp += new MouseEventHandler(pictureBox_MouseUp);
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox_MouseDown);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
        }


        private void DrawMandelbrot(object sender, double xMin, double xMax, double yMin, double yMax, PictureBox pictureBox)
        {
            //double xMin = -2.5;
            //double xMax = 1.5;
            //double yMin = -1;
            //double yMax = 1;

            int maxIterations = 1500;

            // Color palette
            Color[] palette = 
                {
                    Color.FromArgb(66, 30, 15),
                    Color.FromArgb(25, 7, 26),
                    Color.FromArgb(9, 1, 47),
                    Color.FromArgb(4, 4, 73),
                    Color.FromArgb(0, 7, 100),
                    Color.FromArgb(12, 44, 138),
                    Color.FromArgb(24, 82, 177),
                    Color.FromArgb(57, 125, 209),
                    Color.FromArgb(134, 181, 229),
                    Color.FromArgb(211, 236, 248),
                    Color.FromArgb(241, 233, 191),
                    Color.FromArgb(248, 201, 95),
                    Color.FromArgb(255, 170, 0),
                    Color.FromArgb(204, 128, 0),
                    Color.FromArgb(153, 87, 0),
                    Color.FromArgb(106, 52, 3)
                };

            Color[] gradient = { Color.DarkBlue, Color.Blue, Color.Cyan, Color.Lime, Color.Yellow, Color.Red };

            double xStep = (xMax - xMin) / pictureBox1.Width;
            double yStep = (yMax - yMin) / pictureBox1.Height;

            Graphics g = pictureBox.CreateGraphics();
            Bitmap mandelbrot = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            for (int px = 0; px < pictureBox1.Width; px++)
            {
                for (int py = 0; py < pictureBox1.Height; py++)
                {
                    double x = 0;
                    double y = 0;
                    double x0 = xMin + px * xStep;
                    double y0 = yMin + py * yStep;

                    int iterations = 0;
                    
                    double xTemp = 0;

                    while (x * x + y * y < 4 && iterations < maxIterations)
                    {
                        xTemp = x * x - y * y + x0;
                        y = 2 * x * y + y0;
                        x = xTemp;
                        iterations++;
                    }

                    if (iterations == maxIterations)
                    {
                        mandelbrot.SetPixel(px, py, Color.Black);
                    }
                    else
                    {
                        //var brush = new SolidBrush(Color.FromArgb(iterations % 256, 0, 0));
                        //g.FillRectangle(brush, Convert.ToSingle(px), Convert.ToSingle(py), 1, 1);
                        //int colorIndex = iterations % palette.Length;
                        //mandelbrot.SetPixel(px, py, palette[colorIndex]);

                        double colorIndex = (double)iterations / maxIterations;
                        int index1 = (int)(colorIndex * (gradient.Length - 1));
                        int index2 = index1 + 1;
                        double interpolation = colorIndex * (gradient.Length - 1) - index1;
                        Color color = Lerp(gradient[index1], gradient[index2], (float)interpolation);
                        mandelbrot.SetPixel(px, py, color);
                    }

                    pictureBox1.Image = mandelbrot;
                    pictureBox1.Invalidate();
                }
            }
        }

        private Color Lerp(Color start, Color end, float t)
        {
            return Color.FromArgb(
                (int)(start.A + (end.A - start.A) * t),
                (int)(start.R + (end.R - start.R) * t),
                (int)(start.G + (end.G - start.G) * t),
                (int)(start.B + (end.B - start.B) * t)
                );
        }

        private void Mandelbrot_MouseWheel(object sender, MouseEventArgs e)
        {

            if (e.Delta > 0)
            {
                xMin /= (1 + zoom);
                xMax /= (1 + zoom);
                yMin /= (1 + zoom);
                yMax /= (1 + zoom);
            }
            else
            {
                xMin *= (1 + zoom);
                xMax *= (1 + zoom);
                yMin *= (1 + zoom);
                yMax *= (1 + zoom);
            }
            DrawMandelbrot(this, xMin, xMax, yMin, yMax, pictureBox1);
            
        }
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                isDragging = true;
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Change the cursor to a hand while dragging
                Cursor = Cursors.Hand;

                // Draw a rectangle to show the zoomed area
                using (Graphics g = pictureBox1.CreateGraphics())
                {
                    g.DrawRectangle(Pens.Red,
                        Math.Min(startPoint.X, e.X),
                        Math.Min(startPoint.Y, e.Y),
                        Math.Abs(e.X - startPoint.X),
                        Math.Abs(e.Y - startPoint.Y));
                }

                pictureBox1.Invalidate();
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
                Cursor = Cursors.Default;

                // Get the zoomed area
                Rectangle zoomArea = new Rectangle(
                    Math.Min(startPoint.X, e.X),
                    Math.Min(startPoint.Y, e.Y),
                    Math.Abs(e.X - startPoint.X),
                    Math.Abs(e.Y - startPoint.Y));

                xStep = (xMax - xMin) / pictureBox1.Width;
                yStep = (yMax - yMin) / pictureBox1.Height;

                // Update the xMin, xMax, yMin, yMax values based on the zoomed area
                xMin += zoomArea.X * xStep;
                xMax = xMin + zoomArea.Width * xStep;
                yMin += zoomArea.Y * yStep;
                yMax = yMin + zoomArea.Height * yStep;

                // Redraw the Mandelbrot set
                
                DrawMandelbrot(this, xMin, xMax, yMin, yMax, pictureBox1);
            }
        }

    }
}
