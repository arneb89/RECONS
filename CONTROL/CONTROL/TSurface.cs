using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CONTROL
{
    public class TSurface: Surface
    {
        public double[][] teff;

        public TSurface(int N, int M, double inc, double teff0): base(N, M, inc) 
        {
            this.teff = new double[N][];
            for (int i = 0; i < this.patch.Length; i++)
            {
                this.teff[i] = new double[this.patch[i].Length];
            }
            for (int i = 0; i < this.patch.Length; i++)
            {
                for (int j = 0; j < this.patch[i].Length; j++)
                {
                    this.teff[i][j] = teff0;
                }
            }
        }

        public TSurface(string path)
        {
            System.Globalization.CultureInfo usCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.NumberFormatInfo dbNumberFormat = usCulture.NumberFormat;
            dbNumberFormat.NumberDecimalSeparator = ".";

            StreamReader sr = new StreamReader(path);
            string str;
            string[] strMas;
            str = sr.ReadLine();
            // чтение шапки
            string[] stringSeparators = new string[] { " ", "\t" };

            strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            this.N = int.Parse(strMas[0]);
            this.M = int.Parse(strMas[1]);
            this.inc = double.Parse(strMas[2], dbNumberFormat) * Math.PI / 180;

            this.Constructor1(this.N, this.M);

            this.teff = new double[this.N][];
            for (int i = 0; i < this.N; i++)
            {
                this.teff[i] = new double[this.patch[i].Length];
            }

            for (int i = 0; i < this.N; i++)
            {
                str = sr.ReadLine();
                strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < this.patch[i].Length; j++)
                {
                    this.teff[i][j] = double.Parse(strMas[j], dbNumberFormat);
                }
            }
            sr.Close();
        }

        public double[] GetTeffMas()
        {
            int patchNum = this.GetNumberOfPatchesOfVisibleBelts();
            double[] mas = new double[patchNum];
            int k = 0;
            for (int i = 0; i < this.GetNumberOfVisibleBelts(); i++)
            {
                for (int j = 0; j < this.patch[i].Length; j++)
                {
                    mas[k] = this.teff[i][j];
                    k++;
                }
            }
            return mas;
        }

        public double GetMinTeff()
        {
            double teffMin = this.teff[0][0];
            double teffCurr;
            for (int i = 0; i < this.GetNumberOfVisibleBelts(); i++)
            {
                for (int j = 0; j < this.patch[i].Length; j++)
                {
                    teffCurr = this.teff[i][j];
                    if (teffCurr < teffMin) teffMin = teffCurr;
                }
            }
            return teffMin;
        }

        public double GetMaxTeff()
        {
            double teffMax = this.teff[0][0];
            double teffCurr;
            for (int i = 0; i < this.GetNumberOfVisibleBelts(); i++)
            {
                for (int j = 0; j < this.patch[i].Length; j++)
                {
                    teffCurr = this.teff[i][j];
                    if (teffCurr > teffMax) teffMax = teffCurr;
                }
            }
            return teffMax;
        }

        public void AddCircularSpot(double fi, double theta, double radius, double teffSpot)
        {
            double cosDist;
            double cosRadius;
            double thetaP, fiP;

            cosRadius = Math.Cos(radius);

            for (int i = 0; i < this.N; i++)
            {
                for (int j = 0; j < this.patch[i].Length; j++)
                {
                    fiP = patch[i][j].FiCenterOnStart();
                    thetaP = patch[i][j].ThetaCenterOnStart();
                    cosDist = Math.Cos(theta) * Math.Cos(thetaP) +
                        Math.Sin(theta) * Math.Sin(thetaP) * Math.Cos(fiP - fi);

                    if (cosDist >= cosRadius)
                    {
                        this.teff[i][j] = teffSpot;
                    }
                }
            }
        }

        public void AddGaussSpot(double fi, double theta, double radius, double teffSpot)
        {
            double cosDist, dist;
            double cosRadius;
            double thetaP, fiP;

            cosRadius = Math.Cos(radius);


            for (int i = 0; i < this.N; i++)
            {
                for (int j = 0; j < this.patch[i].Length; j++)
                {
                    fiP = patch[i][j].FiCenterOnStart();
                    thetaP = patch[i][j].ThetaCenterOnStart();
                    cosDist = Math.Cos(theta) * Math.Cos(thetaP) +
                        Math.Sin(theta) * Math.Sin(thetaP) * Math.Cos(fiP - fi);
                    dist = Math.Acos(cosDist);

                    if (dist <= 3*radius)
                    {
                        this.teff[i][j] = this.teff[i][j] - teffSpot * Math.Exp(-0.5 * (dist*dist) / Math.Pow(radius, 2));
                    }
                }
            }
        }
        
        public float[][][] GetPatchCoordMas1()
        {
            float[][][] mas;
            int vbn = this.GetNumberOfVisibleBelts();
            int vpn = this.GetNumberOfPatchesOfVisibleBelts();
            int size = vpn * vbn - vbn * (vbn - 1);
                

            mas = new float[2][][];
            mas[0] = new float[size][];
            mas[1] = new float[1][];
            

            for (int i = 0; i < size; i++) mas[0][i] = new float[4];
            mas[1][0] = new float[size];

            float[] phi = new float[vpn - vbn + 2];

            phi[0] = 0.0f;
            phi[phi.Length - 1] = (float)(Math.PI * 2);
            int n = 1;
            for (int i = 0; i < vbn; i++)
            {
                for (int j = 0; j < patch[i].Length-1; j++)
                {
                    phi[n] = (float)patch[i][j].Phi20;
                    n++;
                }
            }

            System.Array.Sort(phi);

            n = 0;
            for (int i = 0; i < vbn; i++)
            {
                for (int j = 0; j < phi.Length-1; j++)
                {
                    mas[0][n][0] = (float)patch[i][0].Theta1;
                    mas[0][n][1] = (float)patch[i][0].Theta2;
                    mas[0][n][2] = (float)phi[j];
                    mas[0][n][3] = (float)phi[j + 1];
                    n++;
                }
            }

            for (int s = 0; s < mas[0].Length; s++)
            {
                float theta0 = (mas[0][s][1] + mas[0][s][0]) * 0.5f;
                float phi0 = (mas[0][s][3] + mas[0][s][2]) * 0.5f;

                for (int i = 0; i < this.GetNumberOfVisibleBelts(); i++)
                {
                    if (theta0 <= patch[i][0].Theta2 && theta0 > patch[i][0].Theta1)
                    {
                        for (int j = 0; j < patch[i].Length; j++)
                        {
                            if (phi0 <= patch[i][j].Phi20 && phi0 > patch[i][j].Phi10)
                            {
                                mas[1][0][s] = (float)this.teff[i][j];
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            return mas;
        }

        public string ToText()
        {
            string str;
            str = string.Format("{0} {1} {2}\r\n", this.GetN(), this.GetM(), this.GetInc() * 180 / Math.PI);
            for (int i = 0; i < this.GetN(); i++)
            {
                for (int j = 0; j < this.teff[i].Length; j++)
                {
                    str = str + this.teff[i][j].ToString() + " ";
                }
                str = str + "\r\n";
            }
            return str;
        }
    }
}
