using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NPlot;

namespace CONTROL
{
    public partial class Form1 : Form
    {
        double[][] intes_obs;
        double[][] intes_mod;
        double[][] intes_imm;
        double[][] lambs;
        double[] phases;

        public Form1()
        {
            InitializeComponent();
            lbSpecs.SelectedIndexChanged += new EventHandler(lbSpecs_SelectedIndexChanged);
        }

        void lbSpecs_SelectedIndexChanged(object sender, EventArgs e)
        {
            int n = lbSpecs.SelectedIndex;
            LinePlot lpObs = new LinePlot();
            lpObs.AbscissaData = lambs[n];
            lpObs.OrdinateData = intes_obs[n];
            lpObs.Color = Color.Black;
            LinePlot lpMod = new LinePlot();
            lpMod.AbscissaData = lambs[n];
            lpMod.OrdinateData = intes_mod[n];
            lpMod.Color = Color.Red;
            LinePlot lpImm = new LinePlot();
            lpImm.AbscissaData = lambs[n];
            lpImm.OrdinateData = intes_imm[n];
            lpImm.Color = Color.Green;
            plot.Clear();
            plot.Add(lpObs);
            plot.Add(lpMod);
            plot.Add(lpImm);
            plot.Refresh();
        }

        

        private void btnLoad_Click(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo usCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.NumberFormatInfo dbNumberFormat = usCulture.NumberFormat;
            dbNumberFormat.NumberDecimalSeparator = ".";

            openFileDialog1.ShowDialog();
            string file = openFileDialog1.FileName;
            int n_phases;
            StreamReader sr=new StreamReader(file);
            string line;
            line = sr.ReadLine();
            n_phases = int.Parse(line);

            phases = null;
            lambs = null;
            intes_mod = null;
            intes_obs = null;

            phases = new double[n_phases];
            lambs = new double[n_phases][];
            intes_obs = new double[n_phases][];
            intes_mod = new double[n_phases][];
            intes_imm = new double[n_phases][];

            for (int p = 0; p < phases.Length; p++)
            {
                line = sr.ReadLine();
                string[] str_mas = line.Split(new string[] { " ", "\t" },
                    StringSplitOptions.RemoveEmptyEntries);
                phases[p] = double.Parse(str_mas[1], dbNumberFormat);
                int nn = int.Parse(str_mas[0]);
                lambs[p] = new double[nn];
                intes_obs[p] = new double[nn];
                intes_mod[p] = new double[nn];
                intes_imm[p] = new double[nn];
                for (int i = 0; i < nn; i++)
                {
                    line = sr.ReadLine();
                    str_mas = line.Split(new string[] { " ", "\t" },
                    StringSplitOptions.RemoveEmptyEntries);
                    lambs[p][i] = double.Parse(str_mas[0].Replace(",", "."), dbNumberFormat);
                    intes_obs[p][i] = double.Parse(str_mas[1].Replace(",", "."), dbNumberFormat);
                    intes_mod[p][i] = double.Parse(str_mas[2].Replace(",", "."), dbNumberFormat);
                    intes_imm[p][i] = double.Parse(str_mas[3].Replace(",", "."), dbNumberFormat);
                }
            }

            lbSpecs.Items.Clear();
            for (int p = 0; p < phases.Length; p++)
            {
                lbSpecs.Items.Add(string.Format("PHASE {0:0.000}", phases[p]).Replace(",", "."));
            }
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            System.Globalization.CultureInfo usCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.NumberFormatInfo dbNumberFormat = usCulture.NumberFormat;
            dbNumberFormat.NumberDecimalSeparator = ".";

            openFileDialog1.ShowDialog();
            string file = openFileDialog1.FileName;

            TSurface tsrf=new TSurface(file);

            SurfaceViewerForm svf = new SurfaceViewerForm();
            svf.Init(tsrf.GetPatchCoordMas(), tsrf.GetTeffMas());
            svf.color0 = Color.White;
            svf.color1 = Color.Black;
            svf.ShowDialog();
        }
    }
}
