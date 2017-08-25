using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathLib;
using System.Threading.Tasks;

namespace RECONS
{
    class GNHelperForDI
    {
        private double[][] rline, rcont;
        private double[] bline, bcont;
        private double[] obsSpec;
        private int pldim, vpn;
        private Matrix D;
        private Matrix F;

        public GNHelperForDI() { }

        public GNHelperForDI(double[][] rline, 
                             double[][] rcont, 
                             double[] bline, 
                             double[] bcont, 
                             double[][] obsSpec, 
                             int pldim,
                             int vpn)
        {
            this.pldim = pldim;
            this.vpn = vpn;
            this.rline = rline;
            this.rcont = rcont;
            this.bline = bline;
            this.bcont = bcont;
            //this.obsSpec = obsSpec;
            this.obsSpec = new double[pldim];
            int k = 0;
            for (int i = 0; i < obsSpec.Length; i++)
            {
                for (int j = 0; j < obsSpec[i].Length; j++)
                {
                    this.obsSpec[k] = obsSpec[i][j];
                    k++;
                }
            }
            this.D = new Matrix(pldim + vpn, vpn);
            this.F = new Matrix(pldim + vpn, 1);
        }

        public Matrix GetJacobyMatrix(Matrix x)
        {
            double suml = 0, sumc = 0;
            for (int i = 0; i < pldim; i++)
            {
                suml = 0;
                for (int k = 0; k < vpn; k++)
                {
                    suml = suml + rline[i][k] * 1.0 / (1.0 + Math.Exp(-x[k]));
                }
                sumc = 0;
                for (int k = 0; k < vpn; k++)
                {
                    sumc = sumc + rcont[i][k] * 1.0 / (1.0 + Math.Exp(-x[k]));
                }
                for (int j = 0; j < vpn; j++)
                {
                    this.D[i, j] = (rline[i][j] * (sumc + bcont[i]) - rcont[i][j] * (suml + bline[i])) *
                    (Math.Exp(-x[j]) / (Math.Pow(1.0 + Math.Exp(-x[j]), 2))) / Math.Pow(sumc + bcont[i], 2);
                }
            }
            for (int i = 0; i < vpn; i++) D[i + pldim, i] = 0.001;
            
            return this.D;
        }

        public Matrix GetVectorFunction(Matrix x)
        {
            double suml = 0, sumc = 0;

            for (int i = 0; i < pldim; i++)
            {
                suml = 0;
                for (int k = 0; k < vpn; k++)
                {
                    suml = suml + rline[i][k] * 1.0 / (1.0 + Math.Exp(-x[k]));
                }
                sumc = 0;
                for (int k = 0; k < vpn; k++)
                {
                    sumc = sumc + rcont[i][k] * 1.0 / (1.0 + Math.Exp(-x[k]));
                }
                this.F[i] = (suml + this.bline[i]) / (sumc + this.bcont[i]) - this.obsSpec[i];
            }
            
            return this.F;
        }


        /// <summary>
        /// Gets a part of transposed Jacoby matrix.
        /// </summary>
        /// <param name="rline"></param>
        /// <param name="rcont"></param>
        /// <param name="bline"></param>
        /// <param name="bcont"></param>
        /// <param name="jacobyTr"></param>
        /// <param name="x"></param>
        /// <param name="sqrtLambda"></param>
        /// <param name="pldim"></param>
        /// <param name="vpn"></param>
        /// <param name="t"></param>
        /// <param name="threadNum"></param>
        public void GetJacobyTrPL(ref double[][] rline,
                             ref double[][] rcont,
                             ref double[] bline,
                             ref double[] bcont,
                             ref double[][] jacobyTr,
                             ref double[] x,
                             double sqrtLambda,
                             int pldim,
                             int vpn,
                             int t,
                             int threadNum)
        {
            int bvpn = (vpn / threadNum) * t;
            int evpn = vpn / threadNum * (t + 1);
            if (t == threadNum - 1) evpn = vpn;

            int bpldim = (pldim / threadNum) * t;
            int epldim = pldim / threadNum * (t + 1);
            if (t == threadNum - 1) epldim = pldim;

            for (int i = bpldim; i < epldim; i++)
            {
                double suml = 0;
                double sumc = 0;

                double v;
                for (int k = 0; k < vpn; k++)
                {
                    v = 1.0 / (1.0 + Math.Exp(-x[k]));
                    suml = suml + rline[i][k] * x[k]; //*v
                    sumc = sumc + rcont[i][k] * x[k]; //*v
                }
                for (int j = 0; j < vpn; j++)
                {
                    jacobyTr[j][i] = (rline[i][j] * (sumc + bcont[i]) - rcont[i][j] * (suml + bline[i])) /**
                    (Math.Exp(-x[j]) / (Math.Pow(1.0 + Math.Exp(-x[j]), 2))) */ / Math.Pow(sumc + bcont[i], 2);
                }
            }

            for (int i = bvpn; i < evpn; i++)
            {
                jacobyTr[i][i + pldim] = sqrtLambda /* * Math.Exp(-x[i]) / Math.Pow((1.0 + Math.Exp(-x[i])), 2)*/;
            }

            //for (int i = bvpn; i < evpn; i++)
            //{
            //    double oneToVPN = 1.0 / (double)vpn;
            //    double expx;
            //    for (int j = 0; j < vpn; j++)
            //    {
            //        if (i == j)
            //        {
            //            expx = Math.Exp(-x[j]);
            //            jacobyTr[j][i + pldim] = sqrtLambda * (1.0 - oneToVPN) * (expx) / Math.Pow(1.0 + expx, 2);
            //        }
            //        else
            //        {
            //            expx = Math.Exp(-x[j]);
            //            jacobyTr[j][i + pldim] = sqrtLambda * oneToVPN * expx / Math.Pow(1.0 + expx, 2);
            //        }
            //    }
            //}
        }

        public void GetVectorFunctionPL(ref double[][] rline,
                             ref double[][] rcont,
                             ref double[] bline,
                             ref double[] bcont,
                             ref double[] obsSpec,
                             ref double[] x,
                             ref double[] f,
                             double lambdaSqrt,
                             int pldim,
                             int vpn,
                             int t,
                             int threadNum)
        {
            int bvpn = (vpn / threadNum) * t;
            int evpn = vpn / threadNum * (t + 1);
            if (t == threadNum - 1) evpn = vpn;

            int bpldim = (pldim / threadNum) * t;
            int epldim = pldim / threadNum * (t + 1);
            if (t == threadNum - 1) epldim = pldim;

            double suml = 0, sumc = 0;
            double oneToVPN = 1.0 / (double)vpn;
            double v;
            for (int i = bpldim; i < epldim; i++)
            {
                suml = 0;
                sumc = 0;
                for (int k = 0; k < vpn; k++)
                {
                    v = 1.0 / (1.0 + Math.Exp(-x[k]));
                    suml = suml + rline[i][k] * x[k]; //v;
                    sumc = sumc + rcont[i][k] * x[k]; //v;
                }
                f[i] = (suml + bline[i]) / (sumc + bcont[i]) - obsSpec[i];
            }

            for (int i = bvpn; i < evpn; i++)
            {
                f[pldim + i] = lambdaSqrt; // *(1.0 / (1.0 + Math.Exp(-x[i])) - 0.0);
            }

            //double sum;
            //double term;
            //for (int i = bvpn; i < evpn; i++)
            //{
            //    sum = 0;
            //    for (int j = 0; j < vpn; j++)
            //    {
            //        //double dfdx = Math.Exp(-x[j]) / Math.Pow((1.0 + Math.Exp(-x[j])), 2);
            //        double dfdx = 1.0 / (1.0 + Math.Exp(-x[j]));
            //        if (i == j)
            //        {
            //            term = (1 - oneToVPN) * dfdx;
            //        }
            //        else
            //        {
            //            term = -oneToVPN * dfdx;
            //        }
            //        sum = sum + term;
            //    }
            //    f[pldim + i] = lambdaSqrt * sum;
            //}
        }
    }
}