# -*- coding: utf-8 -*-

from utils import *
import math


class AirPollutionAnalyser():
    sourceX = 0.  # 污染源X坐标
    sourceY = 0.  # 污染源Y坐标
    windDirection = 0.  # 风向
    airStability = 0.  # 大气稳定度
    windSpeedGround = 0.  # 高度10m处风速
    sourceHeight = 0.  # 污染源高度
    timeDomain = 0.  # 时域（白天或夜晚）
    solarRadiation = 0.  # 白天日照辐射度
    cloudage = 0.  # 夜晚云量
    smokeOutSpeed = 0.  # 烟流出口速度
    smokeOutTem = 0.  # 烟流出口温度
    envTem = 0.  # 环境平均温度
    chiSize = 0.  # 烟囱出口内径
    chiEfficiency = 0.  # 烟囱排热效率
    ER = 0.  # 污染物排放率

    def __init__(self, sourceX, sourceY, sourceHeight, ER, windSpeedGround, windDirection, smokeOutSpeed, smokeOutTem=0,
                 envTem=0, chiSize=0, chiEffiency=0, timeDomain=0, solarRadiation=0, cloudage=0):
        self.sourceX = sourceX
        self.sourceY = sourceY
        self.sourceHeight = sourceHeight
        self.ER = ER
        self.windSpeedGround = windSpeedGround
        self.windDirection = windDirection
        self.timeDomain = timeDomain
        self.solarRadiation = solarRadiation
        self.cloudage = cloudage
        self.airStability = self.GetAirStability()
        self.smokeOutSpeed = smokeOutSpeed
        self.smokeOutTem = smokeOutTem
        self.envTem = envTem
        self.chiSize = chiSize
        self.chiEfficiency = chiEffiency

    # 计算高度指数
    def GetExponent(self):
        if self.airStability == AirStability.A:
            return 0.1
        elif self.airStability == AirStability.B:
            return 0.15
        elif self.airStability == AirStability.C:
            return 0.2
        elif self.airStability == AirStability.D:
            return 0.25
        else:
            return 0.3

    # 计算平均风速
    def GetAveWindSpeed(self):

        return self.windSpeedGround * pow(self.sourceHeight / 10, self.GetExponent())

    # 计算y,z方向上房差方差
    def GetSigmas(self, x):
        if self.airStability in [AirStability.A, AirStability.B]:
            sigmaY = 0.32 * x * pow(1 + 0.0004 * x, -0.5)
            sigmaZ = 0.24 * x * pow(1 + 0.0001 * x, -0.5)
        elif self.airStability == AirStability.C:
            sigmaY = 0.22 * x * (1 + 0.0004 * x, -0.5)
            sigmaZ = 0.2 * x
        elif self.airStability == AirStability.D:
            sigmaY = 0.16 * x * (1 + 0.0004 * x, -0.5)
            sigmaZ = 0.14 * x * (1 + 0.0003 * x, -0.5)
        elif self.airStability in [AirStability.E, AirStability.F]:
            sigmaY = 0.11 * x * (1 + 0.0004 * x, -0.5)
            sigmaZ = 0.08 * x * (1 + 0.0015 * x, -0.5)
        else:
            sigmaY = sigmaZ = None

        return sigmaY, sigmaZ

    # 计算大气稳定度
    def GetAirStability(self):
        airStability = None

        # 风速小于2
        if self.windSpeedGround < 2:
            if self.timeDomain == TimeDomain.Day:
                if self.solarRadiation == SolarRadiation.Strong:
                    airStability = AirStability.A
                elif self.solarRadiation in [SolarRadiation.Medium, SolarRadiation.Weak]:
                    airStability = AirStability.B
                else:
                    airStability = AirStability.D
            else:
                airStability = AirStability.D

        # 风速在2-3之间
        elif self.windSpeedGround >= 2 and self.windSpeedGround < 3:
            if self.timeDomain == TimeDomain.Day:
                if self.solarRadiation == SolarRadiation.Strong:
                    airStability = AirStability.A
                elif self.solarRadiation == SolarRadiation.Medium:
                    airStability = AirStability.B
                elif self.solarRadiation == SolarRadiation.Weak:
                    airStability = AirStability.C
                elif self.solarRadiation == SolarRadiation.Cloudy:
                    airStability = AirStability.D
            elif self.timeDomain == TimeDomain.Night:
                if self.cloudage == Cloudage.Cloudful:
                    airStability = AirStability.E
                elif self.cloudage == Cloudage.Cloudless:
                    airStability = AirStability.F

        # 风速在3-5之间
        elif self.windSpeedGround >= 3 and self.windSpeedGround < 5:
            if self.timeDomain == TimeDomain.Day:
                if self.solarRadiation == SolarRadiation.Strong:
                    airStability = AirStability.B
                elif self.solarRadiation in [SolarRadiation.Medium, SolarRadiation.Weak]:
                    airStability = AirStability.C
                elif self.solarRadiation == SolarRadiation.Cloudy:
                    airStability = AirStability.D
            elif self.timeDomain == TimeDomain.Night:
                if self.cloudage == Cloudage.Cloudful:
                    airStability = AirStability.D
                elif self.cloudage == Cloudage.Cloudless:
                    airStability = AirStability.E

        # 风速在5-6之间
        elif self.windSpeedGround >= 5 and self.windSpeedGround < 6:
            if self.timeDomain == TimeDomain.Day:
                if self.solarRadiation == SolarRadiation.Strong:
                    airStability = AirStability.C
                elif self.solarRadiation in [SolarRadiation.Medium, SolarRadiation.Weak, SolarRadiation.Cloudy]:
                    airStability = AirStability.D
            elif self.timeDomain == TimeDomain.Night:
                airStability = AirStability.D

        # 风速在6以上
        elif self.windSpeedGround >= 6:
            if self.timeDomain == TimeDomain.Day and self.solarRadiation == SolarRadiation.Strong:
                airStability = AirStability.C
            else:
                airStability = AirStability.D

        return airStability

    # Holland公式计算deltaH
    def Holland(self):
        aveWindSpeed = self.GetAveWindSpeed()

        deltaH = 1.5 + 2.7 * self.chiSize * (self.smokeOutTem - self.envTem) / self.smokeOutTem
        deltaH = deltaH * self.smokeOutSpeed * self.chiSize / aveWindSpeed

        return deltaH

    # 归一化坐标系中计算浓度
    def NormalizedC(self, x, y):
        if(x<=0):
            return 0
        sigmaY, sigmaZ = self.GetSigmas(x)

        H = self.sourceHeight + self.Holland()
        aveWindSpeed = self.GetAveWindSpeed()

        c = self.ER / (math.pi * aveWindSpeed * sigmaZ * sigmaY)

        c = c * math.exp(-pow(y, 2) / (2 * pow(sigmaY, 2))) * math.exp(-pow(H, 2) / (2 * pow(sigmaZ, 2)))

        return c*1000000

    def ArbitraryC(self, x, y):
        rotation = -self.windDirection * math.pi / 180

        centerX = x - self.sourceX
        centerY = y - self.sourceY

        newX = centerX * math.cos(rotation) - centerY * math.sin(rotation)
        newY = centerX * math.sin(rotation) + centerY * math.cos(rotation)

        c = self.NormalizedC(newX, newY)

        return c


def GetArbitraryC(pollutedPoint, airCondition, pollutionSource):
    airPollutionAnalyser = AirPollutionAnalyser(sourceX=pollutionSource.X,
                                                sourceY=pollutionSource.Y,
                                                sourceHeight=pollutionSource.Height,
                                                ER=pollutionSource.EmissionRatio,
                                                windSpeedGround=airCondition.WindSpeedGround,
                                                windDirection=airCondition.WindDirection,
                                                smokeOutSpeed=pollutionSource.SmokeOutSpeed,
                                                smokeOutTem=pollutionSource.SmokeOutTem,
                                                envTem=airCondition.EnvTem,
                                                chiSize=pollutionSource.ChiSize,
                                                timeDomain=int(airCondition.TimeDomain),
                                                solarRadiation=int(airCondition.SolarRadiation),
                                                cloudage=int(airCondition.Cloudage)
                                                )

    pollutedPoint.Concentration += airPollutionAnalyser.ArbitraryC(pollutedPoint.X, pollutedPoint.Y)

