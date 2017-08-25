using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RECONS
{
    class Init
    {
        int n_lat, n_long;
        double inc;
        double vsini;
        double reg_par;
        string grid_file;
        string outfile;
        string xscale;
        bool cont_corr;
        double[] sigmas;
        string[] spec_files;
        double[] phases;
        double[] pse_conts;

        public Init(string path)
        {
            System.Globalization.CultureInfo usCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.NumberFormatInfo dbNumberFormat = usCulture.NumberFormat;
            dbNumberFormat.NumberDecimalSeparator = ".";

            StreamReader sr = new StreamReader(path);
            string text = sr.ReadToEnd();
            string[] lines = text.Split(new string[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string[] str_mas = lines[i].Split(new string[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (str_mas.Length < 2) continue;
                string id = str_mas[0];
                string value = str_mas[1];
                switch (id)
                {
                    case "NLAT":
                        n_lat = int.Parse(value);
                        break;
                    case "NLONG":
                        n_long = int.Parse(value);
                        break;
                    case "INC":
                        inc = double.Parse(value.Replace(",", "."), dbNumberFormat);
                        inc = inc * Math.PI / 180;
                        break;
                    case "VSINI":
                        vsini = double.Parse(value.Replace(",", "."), dbNumberFormat);
                        break;
                    case "REGPAR":
                        reg_par = double.Parse(value.Replace(",", "."), dbNumberFormat);
                        break;
                    case "GRID":
                        grid_file = @value;
                        break;
                    case "OUTFILE":
                        outfile = @value;
                        break;
                    case "XSCALE":
                        xscale = @value;
                        break;
                    case "CONTCOR":
                        if (value.ToLower() == "yes") cont_corr = true;
                        else cont_corr = false;
                        break;
                    case "PROFS":
                        int n_profs = int.Parse(value);
                        sigmas = new double[n_profs];
                        spec_files = new string[n_profs];
                        phases = new double[n_profs];
                        pse_conts = new double[n_profs];
                        for (int p = 0; p < n_profs; p++)
                        {
                            i++;
                            str_mas = lines[i].Split(new string[] { " ", "\t" }, 
                                StringSplitOptions.RemoveEmptyEntries);
                            phases[p] = double.Parse(str_mas[0].Replace(",", "."), dbNumberFormat);
                            spec_files[p] = str_mas[1];
                            sigmas[p] = double.Parse(str_mas[2].Replace(",", "."), dbNumberFormat);
                            pse_conts[p] = double.Parse(str_mas[3].Replace(",", "."), dbNumberFormat);
                        }
                        break;
                    default:
                        break;
                }
            }
            sr.Close();
        }

        public int NLat { get { return n_lat; } }
        public int NLong { get { return n_long; } }
        public double Inc { get { return inc; } }
        public double VSinI { get { return vsini; } }
        public double RegPar { get { return reg_par; } }
        public double[] Phases { get { return phases; } }
        public double[] Sigmas { get { return sigmas; } }
        public string[] SpecFiles { get { return spec_files; } }
        public string GridFile { get { return grid_file; } }
        public string OutFile { get { return outfile; } }
        public string XScale { get { return xscale; } }
        public double[] PseConts { get { return pse_conts; } }
        public bool ContCor { get { return cont_corr; } }
    }
}
