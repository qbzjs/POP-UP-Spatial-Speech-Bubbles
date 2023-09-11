namespace Script.Model
{
    public class PostLetterData
    {
        public string message;
        public string wayspotId;

        public void ResetData()
        {
            message = null;
            wayspotId = null;
        }

        public void ResetMessage()
        {
            message = null;
        }

        public void SetMessage(string message)
        {
            this.message = message;
        }
        
        public void SetWaySpotId(string wayspotId)
        {
            this.wayspotId = wayspotId;
        }
        
        public bool IsReady => message != null && wayspotId != null;
        
    }
}