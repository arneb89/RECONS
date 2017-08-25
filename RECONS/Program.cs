using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RECONS
{
    class Program
    {
        static double[][] obsSpectrums;
        static double[][] modSpectrums;
        static double[][] obsLambda;
        static double[] phases;
        static TSurface srfDopp;
        static double[][] immac_profs;

        static void LoadSpectra(string[] files)
        {
            System.Globalization.CultureInfo usCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.NumberFormatInfo dbNumberFormat = usCulture.NumberFormat;
            dbNumberFormat.NumberDecimalSeparator = ".";

            obsSpectrums = new double[files.Length][];
            obsLambda = new double[files.Length][];
            for (int i = 0; i < files.Length; i++)
            {
                StreamReader sr = new StreamReader(files[i]);
                string text = sr.ReadToEnd();
                string[] lines = text.Split(new string[] { "\n", "\r" }, 
                    StringSplitOptions.RemoveEmptyEntries);
                int n_lines = lines.Length;
                obsSpectrums[i] = new double[n_lines];
                obsLambda[i] = new double[n_lines];
                for (int j = 0; j < n_lines; j++)
                {
                    string[] words = lines[j].Split(new string[] { "\t", " " }, 
                        StringSplitOptions.RemoveEmptyEntries);
                    obsLambda[i][j] = double.Parse(words[0].Replace(",", "."), dbNumberFormat);
                    obsSpectrums[i][j] = double.Parse(words[1].Replace(",", "."), dbNumberFormat);
                }
            }
        }

        static void SaveModelSpectra(string outfile)
        {
            StreamWriter sw = new StreamWriter(outfile+".profs");
            sw.WriteLine(phases.Length.ToString());
            for (int p = 0; p < phases.Length; p++)
            {
                sw.WriteLine(string.Format("{0}\t{1}", obsLambda[p].Length, phases[p]).Replace(",", "."));
                for (int i = 0; i < obsLambda[p].Length; i++)
                {
                    sw.WriteLine(string.Format("{0:0.0000}\t{1:0.0000}\t{2:0.0000}\t{3:0.0000}", 
                        obsLambda[p][i], obsSpectrums[p][i], modSpectrums[p][i], immac_profs[p][i]).Replace(",", "."));
                }
            }
            sw.Flush();
            sw.Close();
        }

        static void RescaleObs(double[] pse_cont)
        {
            for (int p = 0; p < obsSpectrums.Length; p++)
            {
                for (int i = 0; i < obsSpectrums[p].Length; i++)
                {
                    obsSpectrums[p][i] = obsSpectrums[p][i] / pse_cont[p];
                }
            }
        }

        static void SaveMap(string outfile)
        {
            StreamWriter sw = new StreamWriter(outfile+".map");
            sw.Write(srfDopp.ToText().Replace(",", "."));
            sw.Flush();
            sw.Close();
        }

        static void LatDistrib(string path)
        {
            double[] aves = new double[srfDopp.GetNumberOfVisibleBelts()];
            double[] lats = new double[aves.Length];
            for (int i = 0; i < aves.Length; i++)
            {
                double ave = 0;
                for (int j = 0; j < srfDopp.patch[i].Length; j++)
                {
                    ave += srfDopp.teff[i][j];
                }
                ave = ave / srfDopp.teff[i].Length;
                aves[i] = ave;
                lats[i] = 0.5*(srfDopp.patch[i][0].Theta1 + srfDopp.patch[i][0].Theta2);
            }
            StreamWriter sw = new StreamWriter(path+".lat");
            for (int i = 0; i < aves.Length; i++)
                sw.WriteLine(string.Format("{0:0.000}\t{1:0.000}", lats[i], aves[i]).Replace(",", "."));
            sw.Close();
        }

        static double FFTotal()
        {
            double ff = 0;
            double ss = 0;
            double s;
            for (int i = 0; i < srfDopp.GetNumberOfVisibleBelts(); i++)
            {
                for (int j = 0; j < srfDopp.teff[i].Length; j++)
                {
                    s = (srfDopp.patch[i][j].Phi20 - srfDopp.patch[i][j].Phi10) *
                        (Math.Cos(srfDopp.patch[i][j].Theta1) - Math.Cos(srfDopp.patch[i][j].Theta2));
                    ss += s;
                    ff += srfDopp.teff[i][j] * s;
                }
            }
            ff = ff / ss;
            return ff;
        }

        static void Main(string[] args)
        {
            StreamWriter sw = new StreamWriter("out.main");

            Init ini = new Init("RECONS.conf");
            
            int n = ini.NLat;
            int m = ini.NLong;
            double inc = ini.Inc;
            double vsini = ini.VSinI;
            double regpar = ini.RegPar;
            string xscale = ini.XScale;
            phases = ini.Phases;

            Console.WriteLine(" N = {0}", n);
            sw.WriteLine(" N = {0}", n);
            Console.WriteLine(" M = {0}", m);
            sw.WriteLine(" M = {0}", m);
            Console.WriteLine(" Inc = {0}", Math.Round(inc * 180.0 / Math.PI, 3));
            sw.WriteLine(" Inc = {0}", Math.Round(inc * 180.0 / Math.PI, 3));
            Console.WriteLine(" VSin(i) = {0:0.000}", vsini);
            sw.WriteLine(" VSin(i) = {0:0.000}", vsini);
            Console.WriteLine(" RegPar = {0:0.000E000}", regpar);
            sw.WriteLine(" RegPar = {0:0.000E000}", regpar);

            Surface srf = new Surface(n, m, inc);

            LoadSpectra(ini.SpecFiles);

            TSurface tsrf;

            SpectrGrid spectrumGrid = new SpectrGrid(ini.GridFile);
            double[][] phIntLineGrid = spectrumGrid.IntenLineGrid[0];
            double[][] phIntContGrid = spectrumGrid.IntenContGrid[0];
            double[][] spIntLineGrid = spectrumGrid.IntenLineGrid[1];
            double[][] spIntContGrid = spectrumGrid.IntenContGrid[1];

            RescaleObs(ini.PseConts);

            DoppImager di = new DoppImager(srf, phIntLineGrid, spIntLineGrid, phIntContGrid, spIntContGrid,
                spectrumGrid.Mu,
                spectrumGrid.Lambda,
                obsSpectrums,
                ini.Phases,
                obsLambda,
                xscale);

            di.DoppImGo(vsini, regpar, ini.ContCor);
            
            srfDopp = di.RestoredSurface;
            modSpectrums = di.ModelSpectrumGrid;
            immac_profs = di.ImmacProfs;
            SaveModelSpectra(ini.OutFile);
            SaveMap(ini.OutFile);
            LatDistrib(ini.OutFile);

            double ff=FFTotal();
            Console.WriteLine(" FF = {0:0.000E000}", ff);
            sw.WriteLine(" FF = {0:0.000E000}", ff);
            sw.Close();
        }
    }
}
