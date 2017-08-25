using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RECONS
{
    public class InputObsSpectra
    {
        private double[][] obsSpectrums;
        private double[][] modSpectrums;
        private double[] obsPhase;
        private double[][] obsLambda;

        private bool error;
        private string errorString;

        public InputObsSpectra(string path)
        {
            StreamReader sr = new StreamReader(path);
            int n, m;
            double phase;
            string str;
            string[] strMas;
            string[] stringSeparators = new string[] { " ", "\t" };
            this.errorString="";

            try
            {
                str = sr.ReadLine();
                n = int.Parse(str);
            }
            catch
            {
                this.error = true;
                this.errorString = this.errorString + "Cannot read the number of observed phases. ";
                return;
            }
            if (n <= 0)
            {
                this.error = true;
                this.errorString = this.errorString + "Wrong number of observed phases. ";
                return;
            }

            this.obsSpectrums = new double[n][];
            this.obsPhase = new double[n];
            this.obsLambda = new double[n][];

            for (int i = 0; i < n; i++)
            {
                try
                {
                    str = sr.ReadLine();
                    strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    phase = double.Parse(strMas[0]);
                    m = int.Parse(strMas[1]);
                    this.obsSpectrums[i] = new double[m];
                    this.obsPhase[i] = phase;
                    this.obsLambda[i] = new double[m];
                }
                catch
                {
                    this.error = true;
                    this.errorString = this.errorString + "Something wrong with number of observed wavelengths. ";
                    return;
                }
                for (int j = 0; j < m; j++)
                {
                    try
                    {
                        str = sr.ReadLine();
                        strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                        this.obsLambda[i][j] = double.Parse(strMas[0]);
                        this.obsSpectrums[i][j] = double.Parse(strMas[1]);
                    }
                    catch
                    {
                        this.error = true;
                        this.errorString = this.errorString + "Cannot read observed wavelengths or intensityes. ";
                        return;
                    }
                }
            }
            this.error = false;
            this.errorString = "All right !";
            sr.Close();
        }

        public double[][] ObsSpectra
        {
            get
            {
                return this.obsSpectrums;
            }
        }

        public double[][] ModSpectra
        {
            get
            {
                return this.modSpectrums;
            }
        }

        public double[] ObsPhases
        {
            get
            {
                return this.ObsPhases;
            }
        }

        public double[][] Lambda
        {
            get
            {
                return this.Lambda;
            }
        }

        public bool Errors
        {
            get
            {
                return this.error;
            }
        }

        public string ErrorString
        {
            get
            {
                return this.errorString;
            }
        }
    }
}