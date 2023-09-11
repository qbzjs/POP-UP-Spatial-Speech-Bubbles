using System;
using System.Collections.Generic;

namespace Script.Model
{
    [Serializable]
    public class AreaList
    {

        public List<LanLon> features;
        [Serializable]
        public class LanLon
        {
            public double lat;
            public double lon;
            public string title;
        
            public LanLon(double lat, double lon, string title = "")
            {
                this.lat = lat;
                this.lon = lon;
                this.title = title;
            }

            public void SetTitle(string titlestr)
            {
                this.title = titlestr;
            }
        }
    
        public AreaList(List<LanLon> list)
        {
            this.features = list;
        }
    }
}