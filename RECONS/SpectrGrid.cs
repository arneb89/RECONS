using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RECONS
{
    class SpectrGrid
    {
        private double logg, vturb, abund;
        private double lambda0, deltaLambda;
        private int lambdaNumber;
        private double[] lambdaSet;
        private int teffNumber;
        private double[] teffSet;
        private int muNumber;
        private double[] muSet;
        private double[][][] spectrLine;
        private double[][][] spectrCont;
        private double lambdaRange;
        private double teffRange;

        public SpectrGrid(string path)
        {
            System.Globalization.CultureInfo usCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.NumberFormatInfo dbNumberFormat = usCulture.NumberFormat;
            dbNumberFormat.NumberDecimalSeparator = ".";

            StreamReader sr = new StreamReader(path);
            string str;
            string[] strMas;
            // чтение шапки
            str = sr.ReadLine();
            string[] stringSeparators = new string[] { " ", "\t" };
            strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            this.logg = double.Parse(strMas[0], dbNumberFormat);
            this.vturb = double.Parse(strMas[1], dbNumberFormat);
            this.abund = double.Parse(strMas[2], dbNumberFormat);
            str = sr.ReadLine();
            strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            this.lambda0 = double.Parse(strMas[0], dbNumberFormat);
            this.deltaLambda = double.Parse(strMas[1], dbNumberFormat);
            this.lambdaNumber = int.Parse(strMas[2], dbNumberFormat);
            str = sr.ReadLine();
            strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            this.teffNumber = int.Parse(strMas[0], dbNumberFormat);
            //
            this.teffSet = new double[this.teffNumber];
            for (int i = 0; i < this.teffNumber; i++)
            {
                this.teffSet[i] = double.Parse(strMas[i + 1], dbNumberFormat);
            }
            str = sr.ReadLine();
            strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            //
            this.muNumber = Convert.ToInt32(strMas[0]);
            this.muSet = new double[this.muNumber];
            for (int i = 0; i < this.muNumber; i++)
            {
                this.muSet[i] = double.Parse(strMas[i + 1], dbNumberFormat);
            }
            //
            this.spectrCont=new double[this.teffNumber][][];
            this.spectrLine=new double[this.teffNumber][][];
            for(int t=0; t<this.teffNumber; t++)
            {
                this.spectrCont[t] = new double[this.muNumber][];
                this.spectrLine[t] = new double[this.muNumber][];
                for(int m=0; m<this.muNumber; m++)
                {
                    this.spectrCont[t][m] = new double[this.lambdaNumber];
                    this.spectrLine[t][m] = new double[this.lambdaNumber];
                }
            }
            //
            for (int t = 0; t < this.teffNumber; t++)
            {
                for (int l = 0; l < this.lambdaNumber; l++)
                {
                    str = sr.ReadLine();
                    strMas = str.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    for (int m = 0; m < this.muNumber; m++)
                    {
                        this.spectrCont[t][m][l] = double.Parse(strMas[m + 1], dbNumberFormat);
                    }
                    for (int m = 0; m < this.muNumber; m++)
                    {
                        this.spectrLine[t][m][l] = double.Parse(strMas[m + 1 + this.muNumber], dbNumberFormat);
                    }
                }
            }

            this.lambdaSet = new double[this.lambdaNumber];
            for (int i = 0; i < this.lambdaNumber; i++)
            {
                this.lambdaSet[i] = lambda0 + i * deltaLambda;
            }

            this.lambdaRange = this.lambdaSet[this.lambdaNumber - 1]-this.lambdaSet[0];
            this.teffRange = this.teffSet[this.TeffNumber - 1] - this.teffSet[0];

            sr.Close();
        }

        public double InterpContIntensityPPP(double teff, double mu, double lambda)
        {
            double[] lambdaGrid = new double[3];
            double[] teffGrid = new double[3];
            double[] muGrid = new double[3];
            double[][][] value = new double[3][][];
            for (int i = 0; i < 3; i++)
            {
                value[i] = new double[3][];
                for (int j = 0; j < 3; j++)
                {
                    value[i][j] = new double[3];
                }
            }

            // Инициализация аргументов сетки
            int nearestIndexOnTeff, nearestIndexOnLambda;
            nearestIndexOnLambda = NearestIndexOnLambda(lambda);
            lambdaGrid[0] = this.lambdaSet[nearestIndexOnLambda - 1];
            lambdaGrid[1] = this.lambdaSet[nearestIndexOnLambda];
            lambdaGrid[2] = this.lambdaSet[nearestIndexOnLambda + 1];
            nearestIndexOnTeff = NearestIndexOnTeff(teff);
            teffGrid[0] = this.teffSet[nearestIndexOnTeff - 1];
            teffGrid[1] = this.teffSet[nearestIndexOnTeff];
            teffGrid[2] = this.teffSet[nearestIndexOnTeff + 1];
            muGrid[0] = this.muSet[0];
            muGrid[1] = this.muSet[1];
            muGrid[2] = this.muSet[2];
            // Инициализация значений сетки
            for (int t = 0; t < 3; t++)
            {
                for (int m = 0; m < 3; m++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        value[t][m][l] = this.spectrCont[nearestIndexOnTeff - 1 + t][m][nearestIndexOnLambda - 1 + l];
                    }
                }
            }
            // Объявление матрицы и вектора свободных членов, входящие в состая СЛАУ определения параметров параболы
            double[][] mat = new double[3][];
            double[][] mat1 = new double[3][]; ;
            for (int i = 0; i < 3; i++)
            {
                mat[i] = new double[3];
                mat1[i] = new double[3];
            }
            double[] b = new double[3];
            //
            double[] c = new double[3];
            //
            double[][] y1 = new double[3][];
            double[] y2 = new double[3];
            for (int i = 0; i < 3; i++)
            {
                y1[i] = new double[3];
            }
            // Интерполяция по Lambda
            mat[0][0] = Math.Pow(lambdaGrid[0], 2);
            mat[1][0] = Math.Pow(lambdaGrid[1], 2);
            mat[2][0] = Math.Pow(lambdaGrid[2], 2);
            mat[0][1] = lambdaGrid[0];
            mat[1][1] = lambdaGrid[1];
            mat[2][1] = lambdaGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;
            for (int t = 0; t < 3; t++)
            {
                for (int m = 0; m < 3; m++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        b[i] = value[t][m][i];
                    }
                    for (int j = 0; j < 3; j++)
                        for (int k = 0; k < 3; k++)
                            mat1[j][k] = mat[j][k];
                    c = this.GaussSolver(mat1, b);
                    y1[t][m] = c[0] * lambda * lambda + c[1] * lambda + c[2];
                }
            }

            // Интерполяция по Mu
            mat[0][0] = Math.Pow(muGrid[0], 2);
            mat[1][0] = Math.Pow(muGrid[1], 2);
            mat[2][0] = Math.Pow(muGrid[2], 2);
            mat[0][1] = muGrid[0];
            mat[1][1] = muGrid[1];
            mat[2][1] = muGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;

            for (int t = 0; t < 3; t++)
            {
                for (int i = 0; i < 3; i++)
                {
                    b[i] = y1[t][i];
                }
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        mat1[j][k] = mat[j][k];
                c = this.GaussSolver(mat1, b);
                y2[t] = c[0] * mu * mu + c[1] * mu + c[2];
            }

            // Интерполяция по Teff
            mat[0][0] = Math.Pow(teffGrid[0], 2);
            mat[1][0] = Math.Pow(teffGrid[1], 2);
            mat[2][0] = Math.Pow(teffGrid[2], 2);
            mat[0][1] = teffGrid[0];
            mat[1][1] = teffGrid[1];
            mat[2][1] = teffGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;

            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    mat1[j][k] = mat[j][k];
            c = this.GaussSolver(mat1, y2);


            return c[0] * teff * teff + c[1] * teff + c[2];
        }

        public double InterpLineIntensityPPP(double teff, double mu, double lambda)
        {
            double[] lambdaGrid = new double[3];
            double[] teffGrid = new double[3];
            double[] muGrid = new double[3];
            double[][][] value = new double[3][][];
            for (int i = 0; i < 3; i++)
            {
                value[i] = new double[3][];
                for (int j = 0; j < 3; j++)
                {
                    value[i][j] = new double[3];
                }
            }

            // Инициализация аргументов сетки
            int nearestIndexOnTeff, nearestIndexOnLambda;
            nearestIndexOnLambda = NearestIndexOnLambda(lambda);
            lambdaGrid[0] = this.lambdaSet[nearestIndexOnLambda - 1];
            lambdaGrid[1] = this.lambdaSet[nearestIndexOnLambda];
            lambdaGrid[2] = this.lambdaSet[nearestIndexOnLambda + 1];
            nearestIndexOnTeff = NearestIndexOnTeff(teff);
            teffGrid[0] = this.teffSet[nearestIndexOnTeff - 1];
            teffGrid[1] = this.teffSet[nearestIndexOnTeff];
            teffGrid[2] = this.teffSet[nearestIndexOnTeff + 1];
            muGrid[0] = this.muSet[0];
            muGrid[1] = this.muSet[1];
            muGrid[2] = this.muSet[2];
            // Инициализация значений сетки
            for (int t = 0; t < 3; t++)
            {
                for (int m = 0; m < 3; m++)
                {
                    for (int l = 0; l < 3; l++)
                    {
                        value[t][m][l] = this.spectrLine[nearestIndexOnTeff - 1 + t][m][nearestIndexOnLambda - 1 + l];
                    }
                }
            }
            // Объявление матрицы и вектора свободных членов, входящие в состая СЛАУ определения параметров параболы
            double[][] mat = new double[3][];
            double[][] mat1 = new double[3][]; ;
            for (int i = 0; i < 3; i++)
            {
                mat[i] = new double[3];
                mat1[i] = new double[3];
            }
            double[] b = new double[3];
            //
            double[] c = new double[3];
            //
            double[][] y1 = new double[3][];
            double[] y2 = new double[3];
            for (int i = 0; i < 3; i++)
            {
                y1[i] = new double[3];
            }
            // Интерполяция по Lambda
            mat[0][0] = Math.Pow(lambdaGrid[0], 2);
            mat[1][0] = Math.Pow(lambdaGrid[1], 2);
            mat[2][0] = Math.Pow(lambdaGrid[2], 2);
            mat[0][1] = lambdaGrid[0];
            mat[1][1] = lambdaGrid[1];
            mat[2][1] = lambdaGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;
            for (int t = 0; t < 3; t++)
            {
                for (int m = 0; m < 3; m++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        b[i] = value[t][m][i];
                    }
                    for (int j = 0; j < 3; j++)
                        for (int k = 0; k < 3; k++)
                            mat1[j][k] = mat[j][k];
                    c = this.GaussSolver(mat1, b);
                    y1[t][m] = c[0] * lambda * lambda + c[1] * lambda + c[2];
                }
            }

            // Интерполяция по Mu
            mat[0][0] = Math.Pow(muGrid[0], 2);
            mat[1][0] = Math.Pow(muGrid[1], 2);
            mat[2][0] = Math.Pow(muGrid[2], 2);
            mat[0][1] = muGrid[0];
            mat[1][1] = muGrid[1];
            mat[2][1] = muGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;

            for (int t = 0; t < 3; t++)
            {
                for (int i = 0; i < 3; i++)
                {
                    b[i] = y1[t][i];
                }
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        mat1[j][k] = mat[j][k];
                c = this.GaussSolver(mat1, b);
                y2[t] = c[0] * mu * mu + c[1] * mu + c[2];
            }

            // Интерполяция по Teff
            mat[0][0] = Math.Pow(teffGrid[0], 2);
            mat[1][0] = Math.Pow(teffGrid[1], 2);
            mat[2][0] = Math.Pow(teffGrid[2], 2);
            mat[0][1] = teffGrid[0];
            mat[1][1] = teffGrid[1];
            mat[2][1] = teffGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;

            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    mat1[j][k] = mat[j][k];
            c = this.GaussSolver(mat1, y2);


            return c[0] * teff * teff + c[1] * teff + c[2];
        }

        public double InterpContIntensityPPL(double teff, double mu, double lambda)
        {
            double[] lambdaGrid = new double[2];
            double[] teffGrid = new double[3];
            double[] muGrid = new double[3];
            double[][][] value = new double[3][][];
            for (int i = 0; i < 3; i++)
            {
                value[i] = new double[3][];
                for (int j = 0; j < 3; j++)
                {
                    value[i][j] = new double[2];
                }
            }

            // Инициализация аргументов сетки
            int nearestIndexOnTeff, firstIndexOnLambda;
            firstIndexOnLambda = (int)((lambda - this.lambdaSet[0]) * (this.lambdaNumber - 1) / this.lambdaRange);
            if (firstIndexOnLambda < 0) firstIndexOnLambda = 0;
            if (firstIndexOnLambda >= this.lambdaNumber - 1) firstIndexOnLambda = this.lambdaNumber - 2;
            lambdaGrid[0] = this.lambdaSet[firstIndexOnLambda];
            lambdaGrid[1] = this.lambdaSet[firstIndexOnLambda + 1];
            nearestIndexOnTeff = NearestIndexOnTeff(teff);
            teffGrid[0] = this.teffSet[nearestIndexOnTeff - 1];
            teffGrid[1] = this.teffSet[nearestIndexOnTeff];
            teffGrid[2] = this.teffSet[nearestIndexOnTeff + 1];
            muGrid[0] = this.muSet[0];
            muGrid[1] = this.muSet[1];
            muGrid[2] = this.muSet[2];
            // Инициализация значений сетки
            for (int t = 0; t < 3; t++)
            {
                for (int m = 0; m < 3; m++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        value[t][m][l] = this.spectrCont[nearestIndexOnTeff - 1 + t][m][firstIndexOnLambda + l];
                    }
                }
            }
            // Объявление матрицы и вектора свободных членов, входящие в состая СЛАУ определения параметров параболы
            double[][] mat = new double[3][];
            double[][] mat1 = new double[3][]; ;
            for (int i = 0; i < 3; i++)
            {
                mat[i] = new double[3];
                mat1[i] = new double[3];
            }
            double[] b = new double[3];
            //
            double[] c = new double[3];
            //
            double[][] y1 = new double[3][];
            double[] y2 = new double[3];
            for (int i = 0; i < 3; i++)
            {
                y1[i] = new double[3];
            }
            // Интерполяция по Lambda
            for (int t = 0; t < 3; t++)
            {
                for (int m = 0; m < 3; m++)
                {
                    y1[t][m] = value[t][m][0] + (value[t][m][1] - value[t][m][0]) *
                        (lambda - lambdaGrid[0]) / (lambdaGrid[1] - lambdaGrid[0]);
                }
            }

            // Интерполяция по Mu
            mat[0][0] = Math.Pow(muGrid[0], 2);
            mat[1][0] = Math.Pow(muGrid[1], 2);
            mat[2][0] = Math.Pow(muGrid[2], 2);
            mat[0][1] = muGrid[0];
            mat[1][1] = muGrid[1];
            mat[2][1] = muGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;

            for (int t = 0; t < 3; t++)
            {
                for (int i = 0; i < 3; i++)
                {
                    b[i] = y1[t][i];
                }
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        mat1[j][k] = mat[j][k];
                c = this.GaussSolver(mat1, b);
                y2[t] = c[0] * mu * mu + c[1] * mu + c[2];
            }

            // Интерполяция по Teff
            mat[0][0] = Math.Pow(teffGrid[0], 2);
            mat[1][0] = Math.Pow(teffGrid[1], 2);
            mat[2][0] = Math.Pow(teffGrid[2], 2);
            mat[0][1] = teffGrid[0];
            mat[1][1] = teffGrid[1];
            mat[2][1] = teffGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;

            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    mat1[j][k] = mat[j][k];
            c = this.GaussSolver(mat1, y2);


            return c[0] * teff * teff + c[1] * teff + c[2];
        }

        public double InterpLineIntensityPPL(double teff, double mu, double lambda)
        {
            double[] lambdaGrid = new double[2];
            double[] teffGrid = new double[3];
            double[] muGrid = new double[3];
            double[][][] value = new double[3][][];
            for (int i = 0; i < 3; i++)
            {
                value[i] = new double[3][];
                for (int j = 0; j < 3; j++)
                {
                    value[i][j] = new double[2];
                }
            }

            // Инициализация аргументов сетки
            int nearestIndexOnTeff, firstIndexOnLambda;
            firstIndexOnLambda = (int)((lambda - this.lambdaSet[0]) * (this.lambdaNumber - 1) / this.lambdaRange);
            if (firstIndexOnLambda < 0) firstIndexOnLambda = 0;
            if (firstIndexOnLambda >= this.lambdaNumber - 1) firstIndexOnLambda = this.lambdaNumber - 2;
            lambdaGrid[0] = this.lambdaSet[firstIndexOnLambda];
            lambdaGrid[1] = this.lambdaSet[firstIndexOnLambda + 1];
            nearestIndexOnTeff = NearestIndexOnTeff(teff);
            teffGrid[0] = this.teffSet[nearestIndexOnTeff - 1];
            teffGrid[1] = this.teffSet[nearestIndexOnTeff];
            teffGrid[2] = this.teffSet[nearestIndexOnTeff + 1];
            muGrid[0] = this.muSet[0];
            muGrid[1] = this.muSet[1];
            muGrid[2] = this.muSet[2];
            // Инициализация значений сетки
            for (int t = 0; t < 3; t++)
            {
                for (int m = 0; m < 3; m++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        value[t][m][l] = this.spectrLine[nearestIndexOnTeff - 1 + t][m][firstIndexOnLambda + l];
                    }
                }
            }
            // Объявление матрицы и вектора свободных членов, входящие в состая СЛАУ определения параметров параболы
            double[][] mat = new double[3][];
            double[][] mat1 = new double[3][]; ;
            for (int i = 0; i < 3; i++)
            {
                mat[i] = new double[3];
                mat1[i] = new double[3];
            }
            double[] b = new double[3];
            //
            double[] c = new double[3];
            //
            double[][] y1 = new double[3][];
            double[] y2 = new double[3];
            for (int i = 0; i < 3; i++)
            {
                y1[i] = new double[3];
            }
            // Интерполяция по Lambda
            for (int t = 0; t < 3; t++)
            {
                for (int m = 0; m < 3; m++)
                {
                    y1[t][m] = value[t][m][0] + (value[t][m][1] - value[t][m][0]) *
                        (lambda - lambdaGrid[0]) / (lambdaGrid[1] - lambdaGrid[0]);
                }
            }

            // Интерполяция по Mu
            mat[0][0] = Math.Pow(muGrid[0], 2);
            mat[1][0] = Math.Pow(muGrid[1], 2);
            mat[2][0] = Math.Pow(muGrid[2], 2);
            mat[0][1] = muGrid[0];
            mat[1][1] = muGrid[1];
            mat[2][1] = muGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;

            for (int t = 0; t < 3; t++)
            {
                for (int i = 0; i < 3; i++)
                {
                    b[i] = y1[t][i];
                }
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        mat1[j][k] = mat[j][k];
                c = this.GaussSolver(mat1, b);
                y2[t] = c[0] * mu * mu + c[1] * mu + c[2];
            }

            // Интерполяция по Teff
            mat[0][0] = Math.Pow(teffGrid[0], 2);
            mat[1][0] = Math.Pow(teffGrid[1], 2);
            mat[2][0] = Math.Pow(teffGrid[2], 2);
            mat[0][1] = teffGrid[0];
            mat[1][1] = teffGrid[1];
            mat[2][1] = teffGrid[2];
            mat[0][2] = 1;
            mat[1][2] = 1;
            mat[2][2] = 1;

            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    mat1[j][k] = mat[j][k];
            c = this.GaussSolver(mat1, y2);


            return c[0] * teff * teff + c[1] * teff + c[2];
        }

        private int NearestIndexOnLambda(double lambda)
        {
            int index=(int)((lambda-this.lambdaSet[0])*(this.lambdaNumber-1)/this.lambdaRange);
            if (index < 0) index = 1;
            if (index >= this.lambdaNumber - 1) index = this.lambdaNumber - 2;
            if ((lambda - this.lambdaSet[index]) > (this.lambdaSet[index + 1] - lambda)) index++;
            if (index == 0) index++;
            if (index == this.lambdaNumber - 1) index--;
            return index;
        }

        private int NearestIndexOnTeff(double teff)
        {
            int index = (int)((teff - this.teffSet[0]) * (this.teffNumber - 1) / this.teffRange);
            if (index < 0) index = 1;
            if (index >= this.teffNumber - 1) index = this.teffNumber - 2;
            if ((teff - this.teffSet[index]) > (this.teffSet[index + 1] - teff)) index++;
            if (index == 0) index++;
            if (index == this.teffNumber - 1) index--;
            return index;
        }

        private double[] GaussSolver(double[][] m, double[] l)
        {
            int N = l.Length;
            double[] x = new double[N];

            // Приведение матрицы m к треугольному виду
            for (int s = 0; s <= N - 2; s++)
            {
                double k1 = m[s][s];

                for (int c = s; c <= N - 1; c++)
                {
                    m[s][c] = m[s][c] / k1;
                }

                l[s] = l[s] / k1;
                for (int s1 = s + 1; s1 <= N - 1; s1++)
                {
                    double k2 = m[s1][s];
                    for (int c1 = s; c1 <= N - 1; c1++)
                    {
                        m[s1][c1] = -m[s][c1] * k2 + m[s1][c1];
                    }
                    l[s1] = -l[s] * k2 + l[s1];
                }
            }

            // обратный ход
            x[N - 1] = l[N - 1] / m[N - 1][N - 1];
            for (int i = N - 2; i >= 0; i--)
            {
                double w = 0;
                for (int j = N - 1; j > i; j--)
                {
                    w = w + x[j] * m[i][j];
                }
                x[i] = (l[i] - w);
            }
            return x;
        }

        public double[] GetContIntensitys(int iTeff, int iMu)
        {
            return this.spectrCont[iTeff][iMu];
        }

        public double[] GetLineIntensitys(int iTeff, int iMu)
        {
            return this.spectrLine[iTeff][iMu];
        }

        public int TeffNumber
        {
            get
            {
                return this.teffNumber;
            }
        }

        public double[] Teff
        {
            get
            {
                return this.teffSet;
            }
        }

        public int MuNumber
        {
            get
            {
                return this.muNumber;
            }
        }

        public double[] Mu
        {
            get
            {
                return this.muSet;
            }
        }

        public int LambdaNumber
        {
            get
            {
                return this.lambdaNumber;
            }
        }

        public double[] Lambda
        {
            get
            {
                return this.lambdaSet;
            }
        }

        public double LogG
        {
            get
            {
                return this.logg;
            }
        }

        public double VTurb
        {
            get
            {
                return this.vturb;
            }
        }

        public double Abund
        {
            get
            {
                return this.abund;
            }
        }

        public double[][][] IntenLineGrid
        {
            get
            {
                return this.spectrLine;
            }
        }

        public double[][][] IntenContGrid
        {
            get
            {
                return this.spectrCont;
            }
        }
    }
}
