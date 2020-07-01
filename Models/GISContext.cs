using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using System.Collections.Generic;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace GIS.Model
{
    public class GisContext:DbContext
    {
        public GisContext(DbContextOptions<GisContext> options)
            : base(options){
            //this.Database.EnsureDeleted();
            this.Database.EnsureCreated(); //自动建库建表

            pointsXY=new List<PollutedPoint>();

            this.Initialize();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PollutedPointMap>().HasKey(t => new { t.lng, t.lat });
        }

        private int PointExist(int x,int y)
        {
            for(int i=0;i<this.pointsXY.Count;i++)
            {
                PollutedPoint point=this.pointsXY[i];

                if(x==(int)point.X && y==(int)point.Y)
                {
                    return i;
                }
            }
            
            return -1;
        }

        private void Initialize()
        {
            this.pointsXY.Clear();
            foreach (var point in this.PollutedPoints)
                 this.PollutedPoints.Remove(point);

            this.SaveChanges();
        }

        public void mock(AirCondition airCondition)
        {
            this.Initialize();
            CoorTransferer coorTransferer=new CoorTransferer(20);

            foreach(PollutionSource pollutionSource in this.PollutionSources)
            {
                double lng=pollutionSource.X;
                double lat=pollutionSource.Y;

                double[] source=coorTransferer.Nautica2xy(lng,lat);

                pollutionSource.X=source[0];
                pollutionSource.Y=source[1];

                int x=(int)source[0];
                int y=(int)source[1];

                int width=500;
                int step=50;

                for(int dx=x-width;dx<=x+width;dx+=step)
                {
                    for(int dy=y-width;dy<=y+width;dy+=step)
                    {
                        // 加载外部 python 脚本文件.
                        ScriptRuntime pyRumTime = Python.CreateRuntime();
                        dynamic obj = pyRumTime.UseFile("t.py");
                        PollutedPoint newPoint;

                        if(PointExist(dx,dy)<0)
                        {
                            newPoint=new PollutedPoint(dx,dy);
                            obj.GetArbitraryC(newPoint, airCondition, pollutionSource);    // python脚本计算污染点的浓度值，并将计算结果返回给对象的污染物浓度属性
                            pointsXY.Add(newPoint);
                        }
                        else
                        {
                            newPoint=pointsXY[PointExist(dx,dy)];
                            obj.GetArbitraryC(newPoint, airCondition, pollutionSource);    // python脚本计算污染点的浓度值，并将计算结果返回给对象的污染物浓度属性
                        }
                    }
                }

                // 使用后还原
                pollutionSource.X=lng;
                pollutionSource.Y=lat;
            }

            foreach(PollutedPoint point in pointsXY)
            {
                PollutedPointMap newPointMap=new PollutedPointMap(point);
                PollutedPoints.Add(newPointMap);  
            }

            this.SaveChanges();         
        }

        public double risk(double lng,double lat,AirCondition airCondition)
        {
            CoorTransferer coorTransferer=new CoorTransferer(20);

            double[] coor=coorTransferer.Nautica2xy(lng,lat);

            PollutedPoint pollutedPoint=new PollutedPoint((int)coor[0],(int)coor[1]);

            foreach(PollutionSource pollutionSource in this.PollutionSources)
            {
                double sourceLng=pollutionSource.X;
                double sourceLat=pollutionSource.Y;

                double[] source=coorTransferer.Nautica2xy(sourceLng,sourceLat);

                pollutionSource.X=source[0];
                pollutionSource.Y=source[1];

                // 加载外部 python 脚本文件.
                ScriptRuntime pyRumTime = Python.CreateRuntime();
                dynamic obj = pyRumTime.UseFile("t.py");
                obj.GetArbitraryC(pollutedPoint, airCondition, pollutionSource);    // python脚本计算污染点的浓度值，并将计算结果返回给对象的污染物浓度属性

                // 使用后还原
                pollutionSource.X=sourceLng;
                pollutionSource.Y=sourceLat;
            }

            return pollutedPoint.Concentration;
        }

        public DbSet<AirCondition> AirConditions { get; set; }
        public DbSet<PollutionSource> PollutionSources { get; set; }
        public DbSet<PollutedPointMap> PollutedPoints { get; set; }
        private List<PollutedPoint> pointsXY;
    }
}