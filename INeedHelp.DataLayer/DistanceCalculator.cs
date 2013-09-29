using System;
using INeedHelp.Models;

namespace INeedHelp.DataLayer
{
    public static class DistanceCalculator
    {
        public static double ToRad(this double val)
        {
            return val*Math.PI/180;
        }

        public static double CalculateDistance(Coordinates point1, Coordinates point2)
        {
            double lat1 = point1.Latitude;
            double lon1 = point1.Longitude;

            double lat2 = point2.Latitude;
            double lon2 = point2.Longitude;

            double R = 6371;
            double dLat = (lat2 - lat1).ToRad();
            double dLon = (lon2 - lon1).ToRad();
            double firstLat = lat1.ToRad();
            double secondLat = lat2.ToRad();

            double a = Math.Sin(dLat/2)*Math.Sin(dLat/2) +
                       Math.Sin(dLon/2)*Math.Sin(dLon/2)*Math.Cos(firstLat)*Math.Cos(secondLat);
            double c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R*c;
        }
    }
}
