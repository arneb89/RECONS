using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathLib;
using MathLib.Optimization;
using System.Threading.Tasks;
using System.Threading;
/*
 * 17.08.16 Решение СЛАУ нужно проводить для очень малого TOL (~1e-15). В противном
 * случае программа ходит около минимума, отсутствует монотонное снижение невязок от
 * итерации к итерации.
 */

namespace RECONS
{
    /// <summary>
    /// Class for filling factor imaging of spotted stars.
    /// </summary>
    class DoppImager
    {
        private Surface srf = null;
        private double[][] phIntLineGrid, spIntLineGrid, phIntContGrid, spIntContGrid, obsSpec, masObsLambda;
        private double[] masMu, masLambda, masObsPhase;
        private string xscale;
        private double[][] immac_profs;
        
        private double[][] calcSpec;
        private TSurface calcSrf;

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <param name="srf">
        /// init surface;</param>
        /// <param name="phIntLineGrid">
        /// grid of photosphere spectra with lines;</param>
        /// <param name="spIntLineGrid">
        /// grid of spot spectra with lines;</param>
        /// <param name="phIntContGrid">
        /// grid of photosphere spectra without lines;</param>
        /// <param name="spIntContGrid">
        /// grid of spot spectra without lines;</param>
        /// <param name="masMu">
        /// array of mu values (mu = cos(gamma), where gamma --- angle between 
        /// normal to the surface and line of sight);</param>
        /// <param name="masLambda">
        /// array of values of wavelengths for witch model spectra was calculated;</param>
        /// <param name="obsSpec">
        /// array of observed spectra;</param>
        /// <param name="masObsPhase">
        /// array of values of phases for witch spectra was observed;</param>
        /// <param name="masObsLambda">
        /// array of values of wavelengths for witch spectra was observed;</param>
        public DoppImager(Surface srf, 
            double[][] phIntLineGrid,  
            double[][] spIntLineGrid,
            double[][] phIntContGrid,
            double[][] spIntContGrid,
            double[] masMu,
            double[] masLambda,
            double[][] obsSpec,
            double[] masObsPhase,
            double[][] masObsLambda,
            string xscale)
        {
            this.srf = srf;
            this.phIntLineGrid = phIntLineGrid;
            this.spIntLineGrid = spIntLineGrid;
            this.phIntContGrid = phIntContGrid;
            this.spIntContGrid = spIntContGrid;
            this.masMu = masMu;
            this.masLambda = masLambda;
            this.obsSpec = obsSpec;
            this.masObsPhase = masObsPhase;
            this.masObsLambda = masObsLambda;
            this.calcSpec = new double[obsSpec.Length][];
            for (int i = 0; i < obsSpec.Length; i++) this.calcSpec[i] = new double[obsSpec[i].Length];
            this.immac_profs = new double[obsSpec.Length][];
            for (int i = 0; i < obsSpec.Length; i++) this.immac_profs[i] = new double[obsSpec[i].Length];
            this.calcSrf = new TSurface(this.srf.GetN(), this.srf.GetM(), this.srf.GetInc(), 0.0);
            this.xscale = xscale;
        }

        private void ATmultA_PL(ref double[][] aTr, ref double[][] res, int t, int threadNum)
        {
            int n = res.Length;
            int upElNum = (n * n - n) / 2;
            int begin = (upElNum / threadNum) * t;
            int end = upElNum / threadNum * (t + 1);
            if (t == threadNum - 1) end = upElNum;

            int beginRow = 0, beginCol = 0;

            int k = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    if (k == begin)
                    {
                        beginRow = i;
                        beginCol = j;
                        k++;
                    }
                }
            }

            double sum;
            bool stop = false;
            k = 0;
            for (int i = beginRow; i < n; i++)
            {
                for (int j = beginCol; j < n; j++)
                {
                    sum = 0;
                    for (int s = 0; s < aTr[0].Length; s++)
                    {
                        sum = sum + aTr[i][s] * aTr[j][s];
                    }
                    res[i][j] = sum;
                    res[j][i] = sum;
                    k++;
                    if (k == end)
                    {
                        stop = true;
                        break;
                    }
                }
                if (stop) break;
                beginCol = i;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vSinI"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public void DoppImGo(double vSinI, double lambda, bool autoCont)
        {
            int vpn = this.srf.GetNumberOfPatchesOfVisibleBelts();

            SpectrumInterpPL siplPhLine = new SpectrumInterpPL(this.phIntLineGrid, masMu, masLambda);

            SpectrumInterpPL siplPhCont = new SpectrumInterpPL(this.phIntContGrid, masMu, masLambda);
            
            SpectrumInterpPL siplSpLine = new SpectrumInterpPL(this.spIntLineGrid, masMu, masLambda);
            
            SpectrumInterpPL siplSpCont = new SpectrumInterpPL(this.spIntContGrid, masMu, masLambda);

            int pldim = 0;
            for (int p = 0; p < this.masObsPhase.Length; p++)
                for (int l = 0; l < this.masObsLambda[p].Length; l++)
                    pldim++;

            double[] bline = new double[pldim];
            double[] bcont = new double[pldim];
            double[][] rline = new double[pldim][];
            double[][] rcont = new double[pldim][];

            for (int i = 0; i < pldim; i++)
            {
                rline[i] = new double[vpn];
                rcont[i] = new double[vpn];
            }

            double vr;
            double deltaX = 0;
            double prArea;
            double sumLine, sumCont;
            int m = 0;
            for (int p = 0; p < this.masObsPhase.Length; p++)
            {
                for (int l = 0; l < this.masObsLambda[p].Length; l++)
                {
                    sumLine = 0;
                    sumCont = 0;
                    for (int i = 0; i < this.srf.GetNumberOfVisibleBelts(); i++)
                    {
                        for (int j = 0; j < this.srf.patch[i].Length; j++)
                        {
                            if (this.srf.patch[i][j].Visible(this.masObsPhase[p], this.srf.GetInc())!=0)
                            {
                                vr = vSinI * Math.Sin(this.srf.patch[i][j].ThetaCenterOnStart()) *
                                    Math.Sin(this.srf.patch[i][j].FiCenterOnStart() + 2 * Math.PI * this.masObsPhase[p] - 0.5 * Math.PI);
                                if (xscale == "Lambda") deltaX = this.masObsLambda[p][l] * vr / 300000;
                                if (xscale == "Vel") deltaX = vr;
                                prArea = this.srf.patch[i][j].ProjectedArea(this.masObsPhase[p], this.srf.GetInc());
                                sumLine = sumLine + prArea * siplPhLine.Interp(
                                    this.srf.patch[i][j].Mu(this.masObsPhase[p],
                                    this.srf.GetInc()),
                                    this.masObsLambda[p][l] - deltaX);
                                sumCont = sumCont + prArea * siplPhCont.Interp(
                                    this.srf.patch[i][j].Mu(this.masObsPhase[p],
                                    this.srf.GetInc()),
                                    this.masObsLambda[p][l]);   
                            }
                        }
                    }
                    bline[m] = sumLine;
                    bcont[m] = sumCont;
                    m++;
                }
            }

            int k;
            m = 0;
            for (int p = 0; p < this.masObsPhase.Length; p++)
            {
                for (int l = 0; l < this.masObsLambda[p].Length; l++)
                {
                    k = 0;
                    for (int i = 0; i < this.srf.GetNumberOfVisibleBelts(); i++)
                    {
                        for (int j = 0; j < this.srf.patch[i].Length; j++)
                        {
                            if (this.srf.patch[i][j].Visible(this.masObsPhase[p], this.srf.GetInc()) != 0)
                            {
                                vr = vSinI * Math.Sin(this.srf.patch[i][j].ThetaCenterOnStart()) *
                                    Math.Sin(this.srf.patch[i][j].FiCenterOnStart() + 2 * Math.PI * this.masObsPhase[p] - 0.5 * Math.PI);
                                if (xscale == "Lambda") deltaX = this.masObsLambda[p][l] * vr / 300000;
                                if (xscale == "Vel") deltaX = vr;
                                prArea = this.srf.patch[i][j].ProjectedArea(this.masObsPhase[p], this.srf.GetInc());
                                rline[m][k] = prArea * (siplSpLine.Interp(
                                    this.srf.patch[i][j].Mu(this.masObsPhase[p], this.srf.GetInc()),
                                    this.masObsLambda[p][l] - deltaX)-
                                    siplPhLine.Interp(
                                    this.srf.patch[i][j].Mu(this.masObsPhase[p], this.srf.GetInc()),
                                    this.masObsLambda[p][l] - deltaX));
                                rcont[m][k] = prArea * ( 
                                    siplSpCont.Interp(
                                    this.srf.patch[i][j].Mu(this.masObsPhase[p], this.srf.GetInc()),
                                    this.masObsLambda[p][l])-
                                    siplPhCont.Interp(
                                    this.srf.patch[i][j].Mu(this.masObsPhase[p], this.srf.GetInc()),
                                    this.masObsLambda[p][l]));
                            }
                            k++;
                        }
                    }
                    m++;
                }
            }

            double[] obsSpec1 = new double[pldim];
            
            k = 0;
            for (int i = 0; i < obsSpec.Length; i++)
            {
                for (int j = 0; j < obsSpec[i].Length; j++)
                {
                    obsSpec1[k] = obsSpec[i][j];
                    k++;
                }
            }

            GNHelperForDI gnhdi = new GNHelperForDI();

            double[] x0 = new double[vpn];
            
            double[] x = new double[vpn];

            double[][] jacobyTr = new double[vpn][];
            
            for (int i = 0; i < vpn; i++) jacobyTr[i] = new double[pldim+vpn];

            double[][] hessian = new double[vpn][];
            
            for (int i = 0; i < vpn; i++) hessian[i] = new double[vpn];

            double[] b = new double[vpn];

            double[] f = new double[pldim + vpn];
            
            double[] h = new double[vpn];

            for (int i = 0; i < vpn; i++) x0[i] = 0.5; //0.0 ----
            
            for (int i = 0; i < vpn; i++) x[i] = x0[i];

            double sqrtLambda = Math.Sqrt(lambda);

            double[] ff = new double[vpn];

            
            /******************************************************************/
            /********************* Gauss-Newton cycle; ************************/
            /******************************************************************/

            int iter = 0;
            int iterMax = 50;
            int threadNum = 4;
            double crit;
            
            do
            {
                Console.Write(" == ITER {0:00} ", iter);

                for (int i = 0; i < vpn; i++) x0[i] = x[i];
                for (int i = 0; i < vpn; i++) ff[i] = x0[i]; //1.0 / (1.0 + Math.Exp(-x0[i]));

                // Initialization of the transponse Jacoby matrix;
                Parallel.For(0, threadNum, t =>
                {
                    gnhdi.GetJacobyTrPL(ref rline,
                        ref rcont,
                        ref bline,
                        ref bcont,
                        ref jacobyTr,
                        ref x0,
                        sqrtLambda,
                        pldim,
                        vpn,
                        t,
                        threadNum
                        );
                });

                Parallel.For(0, threadNum, t =>
                {
                    gnhdi.GetVectorFunctionPL(ref rline,
                        ref rcont,
                        ref bline,
                        ref bcont,
                        ref obsSpec1,
                        ref x0,
                        ref f,
                        sqrtLambda,
                        pldim,
                        vpn,
                        t,
                        threadNum
                        );
                });

                double fnorm = Norm2(f);
                Console.Write("CHI = {0:0.000E000} ", fnorm);
                Console.Write("FMAX = {0:0.000} FMIN = {1:0.000}  \r\n", ff.Max(), ff.Min());

                // Initialization of the Hessian matrix;
                Parallel.For(0, vpn, i =>
                {
                    double sum;
                    for (int j = i; j < vpn; j++)
                    {
                        sum = 0;
                        for (int l = 0; l < pldim /*+ vpn*/; l++)
                        {
                            sum = sum + jacobyTr[i][l] * jacobyTr[j][l];
                        }
                        if (i == j) sum = sum + Math.Pow(jacobyTr[i][i + pldim], 2);
                        hessian[i][j] = sum;
                        hessian[j][i] = sum;
                    }
                });

                MathLib.Basic.AMultB(ref jacobyTr, ref f, ref b);

                MathLib.Basic.VAMultSC(ref b, -1.0);

                ConvGradMethodPL(ref hessian, ref b, ref h, 1e-20);

                //MathLib.Basic.VAMultSC(ref h, 0.1);

                //MathLib.Basic.VAplusVB(ref x0, ref h, ref x);

                LineSearch(ref rcont, ref rline, ref bline, ref bcont, ref obsSpec1, ref x,
                            ref f, sqrtLambda,pldim, vpn, ref h);

                for (int i = 0; i < vpn; i++)
                {
                    if (x[i] < 0) x[i] = 0;
                    if (x[i] > 1) x[i] = 1;
                }

                //if (autoCont) 
                //    AutoScale(ref rline, ref rcont, ref bline, ref bcont, ref x);

                iter++;
                if (iter == iterMax) { Console.WriteLine("Max itearations exceed...\r\n"); break; }

                double[] diff = new double[vpn];
                for (int i = 0; i < vpn; i++) diff[i] = x[i] - x0[i];
                crit = Norm2(diff)/Math.Sqrt(vpn);
                
            } while (crit > 0.0005);

            /************************************************************************/
            /********************* End of Gauss-Newton cycle;************************/
            /************************************************************************/

            
            k = 0;
            for (int i = 0; i < srf.GetNumberOfVisibleBelts(); i++)
                for (int j = 0; j < srf.patch[i].Length; j++)
                {
                    this.calcSrf.teff[i][j] = x[k]; // 1.0 / (1.0 + Math.Exp(-x[k])); ----
                    k++;
                }
            

            this.calcSpec = CalcModelProfs(ref rline, ref rcont, ref bline, ref bcont, ref x);

            k = 0;
            for (int i = 0; i < this.calcSpec.Length; i++)
            {
                for (int j = 0; j < this.calcSpec[i].Length; j++)
                {
                    this.immac_profs[i][j] = bline[k] / bcont[k];
                    k++;
                }
            }
        }

        private void LineSearch(ref double[][] rcont, 
            ref double[][] rline,
            ref double[] bline,
            ref double[] bcont,
            ref double[] obsSpec1,
            ref double[] x,
            ref double[] f,
            double sqrtLambda,
            int pldim,
            int vpn,
            ref double[] h
            )
        {
            GNHelperForDI gnhdi = new GNHelperForDI();

            double[] x0 = new double[x.Length];
            for (int i = 0; i < x0.Length; i++) x0[i] = x[i];
            double fnorm = 0;
            double fnorm1 = 0;
            for (int i = 0; i < 1000 + 1; i++)
            {
                for (int j = 0; j < x.Length; j++) x[j] = x0[j] + ((double)i / 1000) * h[j];
                for (int j = 0; j < x.Length; j++) if (x[j] > 1) x[j] = 1;
                for (int j = 0; j < x.Length; j++) if (x[j] < 0) x[j] = 0;
                gnhdi.GetVectorFunctionPL(ref rline, ref rcont, ref bline, ref bcont,
                    ref obsSpec1, ref x, ref f, sqrtLambda, pldim, vpn, 0, 1);

                fnorm1 = Norm2(f);

                if (i == 0) { fnorm = fnorm1; }
                else
                {
                    if (fnorm < fnorm1) break;
                    fnorm = fnorm1;
                }
            }
        }

        private void ConvGradMethodPL(ref double[][] a, ref double[] b, ref double[] x, double error)
        {
            //Прибижённое решение
            double[] temp = MathLib.Basic.VectorConstructor(Basic.RowCount(ref a));
            double[] rNew = MathLib.Basic.VectorConstructor(Basic.RowCount(ref a));
            //Невязка
            double[] r = MathLib.Basic.VectorConstructor(Basic.RowCount(ref a));
            MathLib.Basic.VAminusVCmultMB(ref b, ref a, ref x, ref r);
            //Базисный вектор
            double[] p = MathLib.Basic.VectorConstructor(Basic.RowCount(ref r));
            MathLib.Basic.CopyVector(ref r, ref p);
            double rSquare = MathLib.Basic.AMultB(ref r, ref r);
            int numIter = 0;
            while (rSquare > error)
            {
                numIter++;
                MathLib.Basic.AMultB(ref a, ref p, ref temp);
                double alpha = rSquare / MathLib.Basic.AMultB(ref temp, ref p);
                MathLib.Basic.VAplusSCmultVB(ref x, ref p, alpha, ref x);
                MathLib.Basic.VAplusSCmultVB(ref r, ref temp, -alpha, ref rNew);
                double rNewSquare = MathLib.Basic.AMultB(ref rNew, ref rNew);
                double beta = rNewSquare / rSquare;
                r = rNew;
                rSquare = rNewSquare;
                MathLib.Basic.VAplusSCmultVB(ref r, ref p, beta, ref p);
            }
        }

        private double Norm2(double[] x)
        {
            double res=0;
            for (int i = 0; i < x.Length; i++) res += x[i] * x[i];
            return Math.Sqrt(res);
        }

        private void AutoScale(ref double[][] rline, ref double[][] rcont,
            ref double[] bline, ref double[] bcont, ref double[] x)
        {
            double[] obs_ave_levels = new double[obsSpec.Length];
            double[] mod_ave_levels = new double[obsSpec.Length];
            
            for (int i = 0; i < obs_ave_levels.Length; i++)
            {
                obs_ave_levels[i] = obsSpec[i].Average();
            }

            double[][] profs_mod = CalcModelProfs(ref rline, ref rcont, ref bline, ref bcont, ref x);
            
            for (int i = 0; i < mod_ave_levels.Length; i++)
            {
                mod_ave_levels[i] = profs_mod[i].Average();
            }

            int k = 0;
            for (int p = 0; p < obsSpec.Length; p++)
            {
                double c = obs_ave_levels[p] / mod_ave_levels[p];
                for (int i = 0; i < obsSpec[p].Length; i++)
                {
                    bline[k] *= c;
                    for (int j = 0; j < rline[k].Length; j++)
                    {
                        rline[k][j] *= c;
                    }
                    k++;
                }
            }
        }

        private double[][] CalcModelProfs(ref double[][] rline, ref double[][] rcont,
            ref double[] bline, ref double[] bcont, ref double[] x)
        {
            double[][] spec = new double[obsSpec.Length][];
            for (int i = 0; i < obsSpec.Length; i++) spec[i] = new double[obsSpec[i].Length];

            double suml, sumc;
            int k = 0;
            for (int i = 0; i < spec.Length; i++)
            {
                for (int j = 0; j < spec[i].Length; j++)
                {
                    suml = 0;
                    for (int s = 0; s < x.Length; s++)
                    {
                        suml = suml + rline[k][s] * x[s]; //1.0 / (1.0 + Math.Exp(-x[s]));
                    }
                    sumc = 0;
                    for (int s = 0; s < x.Length; s++)
                    {
                        sumc = sumc + rcont[k][s] * x[s]; //  1.0 / (1.0 + Math.Exp(-x[s])); ----
                    }
                    spec[i][j] = (suml + bline[k]) / (sumc + bcont[k]);
                    k++;
                }
            }
            return spec;
        }

        /// <summary>
        /// Gets the model spectrum set;
        /// </summary>
        public double[][] ModelSpectrumGrid { get { return this.calcSpec; } }

        /// <summary>
        /// Gets the restored surface;
        /// </summary>
        public TSurface RestoredSurface { get { return this.calcSrf; } }

        public double[][] ImmacProfs { get { return this.immac_profs; } }
    }
}