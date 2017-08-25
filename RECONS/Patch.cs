using System;
using System.Collections.Generic;
using System.Text;

namespace RECONS
{
    [Serializable]

    public class Patch
    {
        private double theta1, theta2;
        private double phi10, phi20;

        /* Конструктор класса */
        public Patch(double fi1, double fi2, double theta1, double theta2)
        {
            this.phi10 = fi1;
            this.phi20 = fi2;
            this.theta1 = theta1;
            this.theta2 = theta2;
        }

        /* Функция для вычисления площади проекции сегмента на картинную плоскость */
        public double ProjectedArea(double phase, double inc)
        {
            double sinI = Math.Sin(inc);
            double cosI = Math.Cos(inc);
            double x;
            x= ((Math.Sin(2 * theta1) - Math.Sin(2 * theta2)) * 0.25 + 0.5 * (theta2 - theta1)) *
                (Math.Cos(Phi1(phase)) - Math.Cos(Phi2(phase))) * sinI +
                0.5 * (Math.Cos(theta1) * Math.Cos(theta1) -
                Math.Cos(theta2) * Math.Cos(theta2)) * cosI * (Phi2(phase) - Phi1(phase));
            return x;
        }

        /* Возвращает 0 если элемент не виден и 1 если элемент виден */
        public int Visible(double phase, double inc)
        {
            double sinI = Math.Sin(inc);
            double cosI = Math.Cos(inc);
            double theta=0.5*(theta2+theta1);
            double fi=0.5*(Phi2(phase)+Phi1(phase));
            double s = Math.Sin(theta) * Math.Sin(fi) * sinI + Math.Cos(theta) * cosI;
            if (s >= 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /* Возвращает координату fi1 на момент времени time */
        public double Phi1(double phase)
        {
            return phi10 + 2*phase*Math.PI;
        }

        /* Возвращает координату fi2 на момент времени time */
        public double Phi2(double phase)
        {
            return phi20 + 2 * phase * Math.PI;
        }

        public double Mu(double phase, double inc)
        {
            double sinI = Math.Sin(inc);
            double cosI = Math.Cos(inc);
            double theta;
            double fi;
            theta=0.5*(theta1+theta2);
            fi=0.5*(Phi1(phase)+Phi2(phase));
            return Math.Sin(theta) * Math.Sin(fi) * sinI + Math.Cos(theta) * cosI;
        }

        public bool MayBeVisible(double inc)
        {
            bool mayBeVisible=false;
            double theta = this.ThetaCenterOnStart();
            if (Math.PI/2.0 + inc > theta) mayBeVisible = true;
            else mayBeVisible = false;
            return mayBeVisible;
        }

        public double FiCenterOnStart()
        {
            return 0.5 * (phi10 + phi20);
        }

        public double ThetaCenterOnStart()
        {
            return 0.5 * (theta1 + theta2);
        }

        public double Theta1
        {
            get { return this.theta1; }
        }

        public double Theta2
        {
            get { return this.theta2; }
        }

        public double Phi10
        {
            get { return this.phi10; }
        }

        public double Phi20
        {
            get { return this.phi20; }
        }
    }
}
