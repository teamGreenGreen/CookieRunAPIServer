namespace API_Game_Server.Resources
{
    public class AttendanceDate
    {
        private static AttendanceDate _instance;
        public static AttendanceDate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AttendanceDate();
                }
                return _instance;
            } 
        }
        public DateTime? date = null;
    }
}