using System;
using System.Collections.Generic;
using System.Text;

namespace CONTROL
{
    [Serializable]

    public class Surface
    {
        // Угол наклона оси вращения к лучу зрения
        protected double inc;
        // 3.1415926
        protected const double pi = 3.1415926;
        // N-кол-во разбиений по меридиану; M-кол-во разбиений по экватору;
        protected int N, M;
        // Массив площадок, на которые разбита поверхность
        public Patch[][] patch;

        // Конструктор класса
        public Surface(int N, int M, double inc)
        {
            this.N = N;
            this.M = M;
            this.inc = inc;
            this.Constructor1(N, M);
        }

        public Surface()
        {
            this.N = 0;
            this.M = 0;
            this.inc = 0;
        }

        /// <summary>
        /// Subdiviation of the sphere into array of sperical triangles with approximately equal areas.
        /// </summary>
        /// <param name="N">the number of subdiviation belts.</param>
        /// <param name="M">the number of near-equatorial elements.</param>
        protected void Constructor1(int N, int M)
        {
            // The area of near-equatorial elements;
            double areaEqElem;
            // The area of near-equatorial belt;
            double areaEqBelt;

            // Calculation of the area of near-equatorial elements;
            // The case of even amount of subdiviation belts;
            if (N % 2 == 0)
            {
                areaEqBelt = 2 * Math.PI * Math.Sin(Math.PI / N);
                areaEqElem = areaEqBelt / M;
            }
            // The case of odd amount of subdiviation belts;
            else
            {
                areaEqBelt = 2 * Math.PI * Math.Sin(0.5 * Math.PI / N);
                areaEqElem = areaEqBelt / M;
            }

            this.patch = new Patch[N][];

            double theta1, theta2;
            for (int i = 0; i < N; i++)
            {
                theta1 = i * Math.PI / N;
                theta2 = (i + 1) * Math.PI / N;
                int size;
                
                // The "if"s is need to exclude errors in subdiviation of near-equatorial
                // belts, that can appear due to limited precision of computing;
                if (N % 2 == 0 && (i == N / 2 || i == N/2-1))
                {
                    size = M;
                }
                else
                {
                    if (N % 2 != 0 && i == N / 2)
                    {
                        size = M;
                    }
                    else
                    {
                        size = (int)(2 * Math.PI * (Math.Cos(theta1) - Math.Cos(theta2)) / areaEqElem);
                    }
                }
                
                if (size == 0) size = 1;
                this.patch[i] = new Patch[size];
                double dphi = 2 * Math.PI / size;
                for (int j = 0; j < size; j++)
                {
                    this.patch[i][j] = new Patch(j * dphi, (j + 1) * dphi, theta1, theta2);
                }
            }

            //double dTheta, dFi, patchSquare;
            //int[] size = new int[N];
            //double[] square = new double[N];

            //patch = new Patch[N][];

            //dTheta = pi / N;
            //dFi = 2 * pi / M;
            //patchSquare = 2 * pi * Math.Cos((N / 2 - 1) * dTheta) / M;

            //size[N / 2 - 1] = M;
            //size[N / 2] = M;
            //square[N / 2 - 1] = patchSquare;
            //square[N / 2] = patchSquare;

            //double ringSquare;
            //for (int i = N / 2 - 2; i >= 0; i--)
            //{
            //    ringSquare = 2 * pi * (Math.Cos(dTheta * i) - Math.Cos(dTheta * (i + 1)));
            //    size[i] = (int)(ringSquare / patchSquare);
            //    square[i] = ringSquare / size[i];
            //}

            //for (int i = N - 1; i > N / 2; i--)
            //{
            //    size[i] = size[N - 1 - i];
            //    square[i] = square[N - 1 - i];
            //}

            //for (int i = 0; i < N; i++)
            //{
            //    patch[i] = new Patch[size[i]];
            //}

            //for (int i = 0; i < N; i++)
            //{
            //    dFi = 2 * pi / patch[i].Length;
            //    for (int j = 0; j < patch[i].Length; j++)
            //    {
            //        patch[i][j] = new Patch(j * dFi, (j + 1) * dFi, i * dTheta, 
            //            (i + 1) * dTheta);
            //    }
            //}
        }

        /// <summary>
        /// Gets a number of visible belts.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfVisibleBelts()
        {
            int i = 0;
            while (patch[i][0].MayBeVisible(this.inc))
            {
                i++;
                if (i == this.N)
                {
                    break;
                }
            }
            return i;
        }

        /// <summary>
        /// Gets a number of observable patches;
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfPatchesOfVisibleBelts()
        {
            int Ns, VPN;
            Ns = this.GetNumberOfVisibleBelts();
            VPN = 0;
            for (int i = 0; i < Ns; i++)
            {
                VPN = VPN + patch[i].Length;
            }
            return VPN;
        }

        /// <summary>
        /// Gets the total number of patches.
        /// </summary>
        /// <returns></returns>
        public int GetPatchNumber()
        {
            int Ns, PN;
            Ns = N;
            PN = 0;
            for (int i = 0; i < Ns; i++)
            {
                PN = PN + patch[i].Length;
            }
            return PN;
        }

        public double[][] GetPatchCoordMas()
        {
            int patchNum = this.GetPatchNumber();
            double[][] mas = new double[patchNum][];
            for (int i = 0; i < patchNum; i++)
            {
                mas[i] = new double[4];
            }

            int k = 0;
            for (int i = 0; i < patch.Length; i++)
            {
                for (int j = 0; j < patch[i].Length; j++)
                {
                    mas[k][0] = patch[i][j].Theta1;
                    mas[k][1] = patch[i][j].Theta2;
                    mas[k][2] = patch[i][j].Phi10;
                    mas[k][3] = patch[i][j].Phi20;
                    k++;
                }
            }
            return mas;
        }

        public int LocatedInNeighbourhoodOfPatch(double fi, double theta, double fiP, double thetaP, double radius)
        {
            double cosDist;
            double cosRadius;

            cosRadius = Math.Cos(radius);

            cosDist = Math.Cos(theta) * Math.Cos(thetaP) +
                        Math.Sin(theta) * Math.Sin(thetaP) * Math.Cos(fiP - fi);
            if (cosDist >= cosRadius)
            {
                return 1;
            }

            return 0;
        }

        public int NumberOfPathesInNeighbourhood(double fi, double theta, double radius)
        {
            double cosDist;
            double cosRadius;
            double thetaP, fiP;

            cosRadius = Math.Cos(radius);
            int m = 0;
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
                        m++;
                    }
                }
            }

            return m;
        }

        // Возвращает количество широтных разбиений
        public int GetN()
        {
            return this.N;
        }

        public int GetM()
        {
            return this.M;
        }

        public double GetInc()
        {
            return this.inc;
        }

        public void SetInc(double inc)
        {
            this.inc = inc;
        }
    }
}
