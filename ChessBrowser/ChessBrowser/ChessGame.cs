using System.Text;

namespace ChessBrowser
{
    public class ChessGame
    {
        public string EventName { get; set; }
        public string Site { get; set; }
        public DateTime EventDate { get; set; }
        public string Round { get; set; }
        public string WhitePlayer { get; set; }
        public string BlackPlayer { get; set; }
        public int WhiteElo { get; set; }
        public int BlackElo { get; set; }
        public char Result { get; set; }
        public string Moves { get; set; }

        public ChessGame(string eventName, string site, DateTime eventDate, string round, string whitePlayer, string blackPlayer, int whiteElo, int blackElo, char result, string moves)
        {
            EventName = eventName;
            Site = site;
            EventDate = eventDate;
            Round = round;
            WhitePlayer = whitePlayer;
            BlackPlayer = blackPlayer;
            WhiteElo = whiteElo;
            BlackElo = blackElo;
            Result = result;
            Moves = moves;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Event: {EventName}");
            sb.AppendLine($"Site: {Site}");
            sb.AppendLine($"Event Date: {EventDate.ToString("yyyy.MM.dd")}");
            sb.AppendLine($"Round: {Round}");
            sb.AppendLine($"White Player: {WhitePlayer}");
            sb.AppendLine($"Black Player: {BlackPlayer}");
            sb.AppendLine($"White Elo: {WhiteElo}");
            sb.AppendLine($"Black Elo: {BlackElo}");
            sb.AppendLine($"Result: {Result}");
            sb.AppendLine("Moves:");
            sb.AppendLine(string.Join(" ", Moves));
            return sb.ToString();
        }

        public string GetGameInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Event: {EventName}");
            sb.AppendLine($"Site: {Site}");
            sb.AppendLine($"Date: {EventDate.ToString("MM/dd/yyyy hh:mm:ss tt")}");
            sb.AppendLine($"White: {WhitePlayer} ({WhiteElo})");
            sb.AppendLine($"Black: {BlackPlayer} ({BlackElo})");
            sb.AppendLine($"Result: {Result}");
            return sb.ToString();
        }


    }
}
