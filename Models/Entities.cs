using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GIS.Model
{
    // 包含各种实体类

    // 空气稳定度
    public enum AirStability
    {
        None = 0,
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6
    }

    // 白天太阳辐射率
    public enum SolarRadiation
    {
        None = 0,
        Strong = 1,
        Medium = 2,
        Weak = 3,
        Cloudy = 4
    }

    // 白天或晚上
    public enum TimeDomain
    {
        None = 0,
        Day = 1,
        Night = 2
    }

    // 晚上云量情况
    public enum Cloudage
    {
        None = 0,
        Cloudful = 1,
        Cloudless = 2
    }

    // 空气状况类
    public class AirCondition
    {
        [Key]
        public string Name { get; set; }            // 主键
        public double WindSpeedGround { get; set; } // 地面风速
        public double WindDirection { get; set; }   // 风向
        public TimeDomain TimeDomain { get; set; }  // 白天或夜晚
        public SolarRadiation SolarRadiation { get; set; }  // 白天太阳辐射率
        public Cloudage Cloudage { get; set; }  // 夜晚云量
        public double EnvTem { get; set; }  // 环境温度

        public AirCondition()
        {}

        public AirCondition(string name, double windSpeedGround, double windDirection, TimeDomain timeDomain, SolarRadiation solarRadiation, double envTem)
        {
            this.Name = name;
            this.WindDirection = windDirection;
            this.WindSpeedGround = windSpeedGround;
            this.TimeDomain = timeDomain;
            this.SolarRadiation = solarRadiation;
            this.EnvTem = envTem;
        }

        public AirCondition(string name, double windSpeedGround, double windDirection, TimeDomain timeDomain, Cloudage cloudage, double envTem)
        {
            this.Name = name;
            this.WindDirection = windDirection;
            this.WindSpeedGround = windSpeedGround;
            this.TimeDomain = timeDomain;
            this.Cloudage = cloudage;
            this.EnvTem = envTem;
        }
    }

    // 污染源类，一个对象表示一个污染源
    public class PollutionSource
    {
        [Key]
        public string Name { get; set; }  // 主键
        public double X { get; set; }   // X坐标
        public double Y { get; set; }   // Y坐标
        public double Height { get; set; }  // 高度
        public double EmissionRatio { get; set; }   // 污染物排放率
        public double SmokeOutSpeed { get; set; }   // 污染物排出速率
        public double SmokeOutTem { get; set; }     // 排出口温度
        public double ChiSize { get; set; }         // 烟囱出口内径

        public PollutionSource(){}

        public PollutionSource(string name, double x, double y, double height, double emissionRatio, double smokeOutSpeed, double smokeOutTem, double chiSize)
        {
            this.Name=name;
            this.X = x;
            this.Y = y;
            this.Height = height;
            this.EmissionRatio = emissionRatio;
            this.SmokeOutSpeed = smokeOutSpeed;
            this.SmokeOutTem = smokeOutTem;
            this.ChiSize = chiSize;
        }
    }

    // 污染点类，每个点看作一个格网坐标
    public class PollutedPoint
    {
        public int X { get; set; }      // X坐标
        public int Y { get; set; }      // Y坐标
        public double Concentration { get; set; }       // 污染物浓度

        public PollutedPoint(){}

        public PollutedPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;

            this.Concentration = 0;
        }

        // 坐标值相同，表示是同一个点
        public override bool Equals(object obj)
        {
            return obj is PollutedPoint point &&
                   X == point.X &&
                   Y == point.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

    public class PollutedPointMap
    {
        public double lng {get;set;}
        public double lat {get;set;}

        public double Count { get; set; }       // 污染物浓度

        public PollutedPointMap(){}

        public PollutedPointMap(PollutedPoint point)
        {
            CoorTransferer coor=new CoorTransferer(20); 

            this.lng = coor.xy2Nautica((double)point.X,(double)point.Y)[0];
            this.lat = coor.xy2Nautica((double)point.X,(double)point.Y)[1];
            this.Count=point.Concentration;
        }
    }
}
