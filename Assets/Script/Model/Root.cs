using System;
using System.Collections.Generic;

namespace Script.Model
{
    [Serializable]
    public class Root
    {
        public string type;
        public List<Feature> features;
    }

    [Serializable]
    public class Feature
    {
        public string type;
        public Geometry geometry;
        public Properties properties;
    }

    [Serializable]
    public class Geometry
    {
        public string type;
        public List<List<List<double>>> coordinates;
    }

    [Serializable]
    public class Properties
    {
        public string location_type;
        public string localizability_quality;
        public List<string> location_target_identifiers;
    }
}