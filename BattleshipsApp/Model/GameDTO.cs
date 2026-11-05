using System;
using System.Collections.Generic;
using System.Text;

namespace BattleshipsApp.Model
{
    public class GameDTO
    {
        public int Id { get; set; }
        public DateTime StartTimeStamp { get; set; }
        public string DateString { get
            {
                return (StartTimeStamp + (DateTime.Now - DateTime.UtcNow)).ToString("HH:mm");
            } }
        public string CreatorNickname { get; set; } = "";
    }
}
