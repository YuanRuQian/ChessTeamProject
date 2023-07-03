using System.Globalization;
using System.Text.RegularExpressions;


namespace ChessBrowser
{
    public static class PgnReader
    {
        public static List<ChessGame> ReadGames(string filePath)
        {
            string pgnText = File.ReadAllText(filePath);
            List<ChessGame> games = new List<ChessGame>();

            // Split PGN text into individual games
            string[] gameTexts = Regex.Split(pgnText, @"\n\n");

            foreach (string gameText in gameTexts)
            {
                ChessGame game = ParseGame(gameText);
                if (game != null)
                {
                    games.Add(game);
                }
            }

            return games;
        }



        private static char ParseResult(string resultString)
        {
            char result = ' ';
            if (resultString == "1-0")
            {
                result = 'W';
            }
            else if (resultString == "0-1")
            {
                result = 'B';
            }
            else if (resultString == "1/2-1/2")
            {
                result = 'D';
            }
            return result;
        }


        private static ChessGame ParseGame(string gameText)
        {
            ChessGame game = null;

            // Extract tags from the game text
            MatchCollection tagMatches = Regex.Matches(gameText, @"\[(\w+)\s""([^""]+)""\]");

            if (tagMatches.Count >= 7)
            {
                // Extract tag values
                string eventName = GetTagValue(tagMatches, "Event");
                string site = GetTagValue(tagMatches, "Site");
                DateTime eventDate = DateTime.ParseExact(GetTagValue(tagMatches, "EventDate"), "yyyy.MM.dd", CultureInfo.InvariantCulture);
                string round = GetTagValue(tagMatches, "Round");
                string whitePlayer = GetTagValue(tagMatches, "White");
                string blackPlayer = GetTagValue(tagMatches, "Black");
                string result = GetTagValue(tagMatches, "Result");

                // Parse Elo ratings
                int whiteElo = int.Parse(GetTagValue(tagMatches, "WhiteElo"));
                int blackElo = int.Parse(GetTagValue(tagMatches, "BlackElo"));

                // Parse result
                char resultChar = ParseResult(result);

                // Extract moves
                string moveText = Regex.Replace(gameText, @"\[[^\]]+\]", "");
                string[] moves = moveText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                // Create a new chess game object
                game = new ChessGame(eventName, site, eventDate, round, whitePlayer, blackPlayer, whiteElo, blackElo, resultChar, moves);

         
            }

            return game;
        }



        private static string GetTagValue(MatchCollection matches, string tagName)
        {
            foreach (Match match in matches)
            {
                if (match.Groups[1].Value == tagName)
                {
                    return match.Groups[2].Value;
                }
            }
            return null;
        }
    }
}
