using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GIS.Model
{
    public class CoorTransferer
    {
        int degree;     // 六度带带号
        public CoorTransferer(int degree)
        {
            this.degree=degree;
        }
        private double Degree2Radian(double degree)
        {
            return degree*Math.PI/180.0;
        }

        private double Radian2Degree(double radian)
        {
            return radian/Math.PI*180.0;
        }

        public double[] Nautica2xy(double lng,double lat)
        {
            double B=Degree2Radian(lat);    //纬度
            double L=Degree2Radian(lng);    //经度

            double X0=6378245.0*(1-0.006693421623)*(1.0050517739*B-0.00506237764*Math.Sin(2*B)/2.0+0.00001062451*Math.Sin(4*B)/4.0-0.0000002081*Math.Sin(6*B)/6.0);
            double l=L-Degree2Radian(6.0*this.degree-3.0);
            double N=6378245/Math.Sqrt(1-0.006693421623*Math.Pow(Math.Sin(B),2));
            double t=Math.Tan(B);
            double R=180.0/Math.PI;
            double sita=0.006738525415*Math.Pow(Math.Cos(B),2);
            double sita4=5-Math.Pow(t,2)+9*sita+4*Math.Pow(sita,2);
            double V=61.0-58.0*Math.Pow(t,2)+Math.Pow(t,4);
            double K=R*l*Math.Sin(B)+Math.Sin(B)*Math.Pow(Math.Cos(B),2)*(1+3*sita+2*Math.Pow(sita,2))*Math.Pow(l,3)+Math.Sin(B)*Math.Pow(Math.Cos(B),4)*(2-Math.Pow(t,2))*Math.Pow(l,5);

            double X=X0+Math.Pow(l,2)*N*Math.Sin(B)*Math.Cos(B)/2+Math.Pow(l,4)*N*Math.Sin(B)*Math.Pow(Math.Cos(B),3)*sita4/24.0+Math.Pow(l,6)*N*Math.Sin(B)*Math.Pow(Math.Cos(B),5)*V/720.0;
            double Y=this.degree*1000000.0+500000.0+l*N*Math.Cos(B)*(1+Math.Pow(l*Math.Cos(B),2)*(1-Math.Pow(t,2)+sita)/6.0+Math.Pow(l*Math.Cos(B),4)*(5-18*Math.Pow(t,2)+Math.Pow(t,4)+14*sita-58*sita*Math.Pow(t,2))/120.0);

            double[] result=new double[2]{X,Y};

            return result;
        }

        public double[] xy2Nautica(double x,double y)
        {
            double B=x;
            double C=y;
            double K=this.degree*6-3;
            double L=B/(6378245*(1-0.006693421623)*1.0050517739);
            double M=L+(0.00506237764*Math.Sin(2*L)/2.0-0.00001062451*Math.Sin(4*L)/4.0+0.0000002081*Math.Sin(6*L)/6.0)/1.0050517739;
            double N=L+(0.00506237764*Math.Sin(2*M)/2.0-0.00001062451*Math.Sin(4*M)/4.0+0.0000002081*Math.Sin(6*M)/6.0)/1.0050517739;
            double O=L+(0.00506237764*Math.Sin(2*N)/2.0-0.00001062451*Math.Sin(4*N)/4.0+0.0000002081*Math.Sin(6*N)/6.0)/1.0050517739;
            double P=L+(0.00506237764*Math.Sin(2*O)/2.0-0.00001062451*Math.Sin(4*O)/4.0+0.0000002081*Math.Sin(6*O)/6.0)/1.0050517739;
            double Q=L+(0.00506237764*Math.Sin(2*P)/2.0-0.00001062451*Math.Sin(4*P)/4.0+0.0000002081*Math.Sin(6*P)/6.0)/1.0050517739;
            double R=L+(0.00506237764*Math.Sin(2*Q)/2.0-0.00001062451*Math.Sin(4*Q)/4.0+0.0000002081*Math.Sin(6*Q)/6.0)/1.0050517739;
            double S=Math.Tan(R);
            double T=0.006738525415*Math.Pow(Math.Cos(R),2);
            double U=6378245/Math.Sqrt(1-0.006693421623*Math.Pow(Math.Sin(R),2));
            double V=6378245*(1-0.006693421623)/Math.Pow((Math.Sqrt((1-0.006693421623*Math.Pow(Math.Sin(R),2)))),3);
            double W=5+3*Math.Pow(S,2)+T-9*T*Math.Pow(S,2);
            double X=61+90*Math.Pow(S,2)+45*Math.Pow(S,4);
            double Y=1+2*Math.Pow(S,2)+Math.Pow(T,2);
            double Z=5+28*Math.Pow(S,2)+24*Math.Pow(S,4)+6*T+8*T*Math.Pow(S,2);
            double AA=(180/Math.PI)*(R-Math.Pow((C-degree*1000000-500000),2)*S/(2*V*U)+Math.Pow((C-degree*1000000-500000),4)*W/(24*Math.Pow(U,3)*V)-Math.Pow(C-degree*1000000-500000,6)*X/(7200*Math.Pow(U,5)*V));
            double AB=(180/Math.PI)*(C-degree*1000000-500000)*(1-Math.Pow(C-degree*1000000-500000,2)*Y/(6*Math.Pow(U,2))+Math.Pow((C-degree*1000000-500000),4)*Z/(120*Math.Pow(U,4)))/(U*Math.Cos(P));

            double lng=K+AB;
            double lat=AA;

            double[] result=new double[2]{lng,lat};

            return result;
        }
    }
}