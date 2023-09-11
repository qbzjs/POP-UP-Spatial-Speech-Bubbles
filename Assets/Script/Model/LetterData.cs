using System;

namespace Script.Model
{
    [Serializable]
    public struct LetterData
    {
        public string payLoad;
        public string wayspotId;
        public string message;
        public float positionX;
        public float positionY;
        public float positionZ;

        public LetterData(
            string message, 
            string wayspotId, 
            string payload, 
            float positionX, 
            float positionY, 
            float positionZ)
        {
            this.message = message;
            this.wayspotId = wayspotId;
            this.payLoad = payload;
            this.positionX = positionX;
            this.positionY = positionY;
            this.positionZ = positionZ;
        }
    }
}


