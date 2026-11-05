namespace BattleshipsApp.Model
{
    public class Player
    {
        public string Login { get; set; } = "";
        public string NickName { get; set; } = "";
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
    }
}