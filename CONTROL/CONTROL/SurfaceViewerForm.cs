using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CONTROL
{
    public partial class SurfaceViewerForm : Form
    {

        // массив прамоугольников разбиения поверхности
        public Rectangle[] rcs;
        public double[][] rmas;
        // массив интенсивностей соотв-их rcs
        public double[] inten;

        private int xBegin = 60, yBegin = 30;

        public Color color0, color1;

        private int plotHeight, plotWidth;

        private bool showLines = false;

        private Bitmap bitmap = null;

        public SurfaceViewerForm()
        {
            InitializeComponent();
            this.plotHeight = 314;
            this.plotWidth = 628;
        }

        public void SetColor0(Color color0)
        {
            this.color0 = color0;
        }

        public void SetColor1(Color color1)
        {
            this.color1 = color1;
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X <= this.rcs[this.rcs.Length - 1].Right + xBegin && e.X >= this.xBegin
                && e.Y >= this.yBegin && e.Y <= this.rcs[this.rcs.Length - 1].Bottom+this.yBegin)
            {
                int h = this.rcs[this.rcs.Length - 1].Bottom;
                int w = this.rcs[this.rcs.Length - 1].Right;
                txtLat.Text = (-(e.Y-this.yBegin)*180/((double) h)+90).ToString();
                txtLong.Text = ((e.X - this.xBegin)*360 / (double)w).ToString();
                int i;
                for ( i = 0; i < this.rcs.Length; i++)
                {
                    if (e.X <= this.rcs[i].Right + xBegin && e.X >= this.rcs[i].Left + xBegin
                    && e.Y >= this.rcs[i].Top + yBegin && e.Y <= this.rcs[i].Bottom + this.yBegin) break;
                }
                if (i < this.inten.Length)
                {
                    txtValue.Text = this.inten[i].ToString();
                }
                else
                {
                    txtValue.Text = "Unknown";
                }
            }
        }

        public void Init(double[][] rmas, double[] vals)
        {
            int rcNumber = rmas.Length;
            this.rcs = new Rectangle[rcNumber];
            this.inten = vals;
            int x, y, h, w;

            this.rmas = rmas;
            
            for (int i = 0; i < this.rcs.Length; i++)
            {
                 x = Convert.ToInt16(rmas[i][2] * 100);
                 y = Convert.ToInt16(rmas[i][0] * 100);
                 h = Convert.ToInt16((rmas[i][1]*100 - y));
                 w = Convert.ToInt16((rmas[i][3]*100 - x));
                 rcs[i] = new Rectangle(x, y, w, h);
            }
        }

        private void DrawLatitAxis(Graphics g, int x, int ymin, int ymax)
        {
            int axisWidth = ymax - ymin;
            int pointsNumber = 7;
            Pen p=new Pen(Color.Black);
            g.DrawLine(p, x, ymin, x, ymax);
            Font tFont = new Font("Arial", 12, FontStyle.Regular);
            for (int i = 0; i < pointsNumber; i++)
            {
                g.DrawLine(p, x - 5, ymin + i * axisWidth / (pointsNumber - 1), x + 5, ymin + i * axisWidth / (pointsNumber - 1));
                g.DrawString(string.Format("{0}", 90-i * 180.0 / (pointsNumber - 1)), tFont, Brushes.Black, new PointF(x - 30, ymin + i * axisWidth / (pointsNumber - 1)));
            }
        }

        private void DrawLongitAxis(Graphics g, int y, int xmin, int xmax)
        {
            int axisWidth = xmax - xmin;
            int pointsNumber = 13;
            Pen p=new Pen(Color.Black);
            g.DrawLine(p, xmin, y, xmax, y);
            Font tFont = new Font("Arial", 12, FontStyle.Italic);
            for (int i = 0; i < pointsNumber; i++)
            {
                g.DrawLine(p, xmin + i * axisWidth / (pointsNumber - 1), y - 5, xmin + i * axisWidth / (pointsNumber - 1), y + 5);
                g.DrawString(string.Format("{0}", i * 360.0 / (pointsNumber - 1)), tFont, Brushes.Black, new PointF(xmin + i * axisWidth / (pointsNumber - 1)-5, y + 7));
            }           
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // определяем максимальную и минимальную интенсивность
            double iMax = inten.Max();
            double iMin = inten.Min();

            Pen p;

            Graphics g = e.Graphics;

            int R1, G1, B1, R0, G0, B0;

            R1 = color1.R; G1 = color1.G; B1 = color1.B;
            R0 = color0.R; G0 = color0.G; B0 = color0.B;

            // определение точки начала рисования поверхности
            g.TranslateTransform(this.xBegin,this.yBegin);

            int R, G, B;
            double kR, kB, kG;
            if (iMin != iMax)
            {
                kR = (R1 - R0) / (iMax - iMin);
                kG = (G1 - G0) / (iMax - iMin);
                kB = (B1 - B0) / (iMax - iMin);
            }
            else
            {
                kR = 0;
                kG = 0;
                kB = 0;
            }

            // рисование закрашенных ячеек поверхности
            for (int i = 0; i < inten.Length; i++)
            {
                R = Convert.ToInt16(kR * (inten[i] - iMin) + R0);
                G = Convert.ToInt16(kG * (inten[i] - iMin) + G0);
                B = Convert.ToInt16(kB * (inten[i] - iMin) + B0);
                
                SolidBrush sb = new SolidBrush(Color.FromArgb(R, G, B));
                g.FillRectangle(sb, rcs[i]);  
            }

            // Рисование границ ячеек
            if (this.showLines)
            {
                p = new Pen(Color.Black, 1);
                g.DrawRectangles(p, rcs);
                g.DrawRectangle(p, new Rectangle(0, 0, 628, 314));
            }

            g.DrawRectangle(new Pen(Color.Black, 1.0f),
                new Rectangle(0, 0, this.plotWidth, this.plotHeight));

            this.DrawLatitAxis(g, -20, 0, this.rcs[this.rcs.Length-1].Bottom);
            this.DrawLongitAxis(g, this.rcs[this.rcs.Length - 1].Bottom + 20, this.rcs[0].Left, this.rcs[this.rcs.Length - 1].Right); 

            // Title
            g.TranslateTransform(250, -30);
            Font tFont = new Font("Arial", 16, FontStyle.Italic);
            g.DrawString("Surface", tFont, Brushes.Black, new PointF(25, 2));

            // Цветовая шкала
            g.TranslateTransform(385, 40);
            Rectangle ColorLine = new Rectangle(0, 0, 45, 295);
            LinearGradientBrush lgb = new LinearGradientBrush(ColorLine, color1, color0, 90);
            g.FillRectangle(lgb, ColorLine);

            Font iFont = new Font("Arial", 8, FontStyle.Italic);
            g.DrawString(string.Format("{0:E2}", iMax), iFont, Brushes.Black, new PointF(-5, -15));
            g.DrawString(string.Format("{0:E2}", iMin), iFont, Brushes.Black, new PointF(-5, 295));

            

            base.OnPaint(e);
        }

        private void SurfaceViewerForm_Load(object sender, EventArgs e)
        {
            this.MouseClick += new MouseEventHandler(this.MouseDown);
        }

        private void btnMaxColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            this.color1 = colorDialog1.Color;
            this.Refresh();
        }

        private void btnMinColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            this.color0 = colorDialog1.Color;
            this.Refresh();
        }

        private void btnShowLines_Click(object sender, EventArgs e)
        {
            if (this.showLines)
            {
                this.showLines = false;
                this.btnShowLines.Text = "Show Lines";
            }
            else
            {
                this.showLines = true;
                this.btnShowLines.Text = "Hide Lines";
            }
            this.Refresh();
        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            Graphics gr;
            this.bitmap = new Bitmap(this.plotWidth, this.plotHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            gr = Graphics.FromImage(this.bitmap);
            gr.CopyFromScreen((int)g.VisibleClipBounds.Left /*+ this.xBegin*/,
                (int)g.VisibleClipBounds.Top /*+ this.yBegin*/, 
                0, 0, new Size(this.plotWidth, this.plotHeight), CopyPixelOperation.SourceCopy);
            bitmap.Save(@"D:\\saved.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private void btnSaveMapData_Click(object sender, EventArgs e)
        {

        }
    }
}
