using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RECONS
{
    class InputDI
    {
        private int n, m;
        private double inc, vsini;
        private string pathMod, pathObs;
        private bool error;
        private string errorString;

        public InputDI(string path)
        {
            this.n = -1;
            this.m = -1;
            this.inc = -1;
            this.vsini = -1;
            this.pathMod = null;
            this.pathObs = null;
            this.error = false;
            this.errorString = "";

            StreamReader sr = new StreamReader(path);
            string str;
            string[] strMas;
            string[] stringSeparators = new string[] { " ", "\t" };
            str = sr.ReadLine();
            while (str != null)
            {
                strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                switch (strMas[0])
                {
                    case "N":
                        try
                        {
                            this.n = int.Parse(strMas[1]);
                        }
                        catch
                        {
                            this.error = true;
                            this.errorString = this.errorString + "Reading error of N parameter. ";
                        }
                        break;
                    case "M":
                        try
                        {
                            this.m = int.Parse(strMas[1]);
                        }
                        catch
                        {
                            this.error = true;
                            this.errorString = this.errorString + "Reading error of M parameter. ";
                        }
                        break;
                    case "INC":
                        try
                        {
                            this.inc = double.Parse(strMas[1].Replace(".", ",")) * Math.PI / 180.0;
                        }
                        catch
                        {
                            this.error = true;
                            this.errorString = this.errorString + "Reading error of INC parameter. ";
                        }
                        break;
                    case "VSINI":
                        try
                        {
                            this.vsini = double.Parse(strMas[1].Replace(".", ","));
                        }
                        catch
                        {
                            this.error = true;
                            this.errorString = this.errorString + "Reading error of VSINI parameter. ";
                        }
                        break;
                    case "PATH_MOD":
                        try
                        {
                            this.pathMod = strMas[1];
                        }
                        catch
                        {
                            this.error = true;
                            this.errorString = this.errorString + "Reading error of PATH_MOD parameter. ";
                        }
                        break;
                    case "PATH_OBS":
                        try
                        {
                            this.pathObs = strMas[1];
                        }
                        catch
                        {
                            this.error = true;
                            this.errorString = "Reading error of PATH_OBS parameter. ";
                        }
                        break;
                }
                str = sr.ReadLine();
            }

            if (this.n == -1)
            {
                this.error = true;
                this.errorString = this.ErrorString + "Parameter N is not initialised. ";
            }
            if(this.m == -1)
            {
                this.error = true;
                this.errorString = this.ErrorString + "Parameter M is not initialised. ";
            }
            if(this.inc == -1)
            {
                this.error = true;
                this.errorString = this.ErrorString + "Parameter INC is not initialised. ";
            }
            if(this.vsini == -1)
            {
                this.error = true;
                this.errorString = this.ErrorString + "Parameter VSINI is not initialised. ";
            }
            if (this.pathMod == null)
            {
                this.error = true;
                this.errorString = this.ErrorString + "Parameter PATH_MOD is not initialised. ";
            }
            if(this.pathObs == null)
            {
                this.error = true;
                this.errorString = this.ErrorString + "Parameter PATH_OBS is not initialised. ";
            }

            if (this.error == false) this.errorString = "All right ! ";

            sr.Close();
        }

        public int N
        {
            get { return this.n; }
        }

        public int M
        {
            get { return this.m; }
        }

        public double Inc
        {
            get { return this.inc; }
        }

        public double VSinI
        {
            get { return this.vsini; }
        }

        public string PathMod
        {
            get { return this.pathMod; }
        }

        public string PathObs
        {
            get { return this.pathObs; }
        }

        public bool Errors
        {
            get { return this.error; }
        }

        public string ErrorString
        {
            get { return this.errorString; }
        }
    }
}
