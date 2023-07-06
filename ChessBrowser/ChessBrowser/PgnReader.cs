using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using GameController;


namespace ChessBrowser
{
    public static class PgnReader
    {
        public static List<ChessGame> ReadGames(string filePath)
        {
            string pgnText = File.ReadAllText(filePath);
            List<ChessGame> games = new List<ChessGame>();

            // Split PGN text into individual games
            // there might be additional carriage return characters (\r) present before the newline characters
            string[] gameTexts = Regex.Split(pgnText, @"\r?\n\r?\n");

            for (int i = 0; i < gameTexts.Length; i += 2)
            {
                string gameText = gameTexts[i];
                string movesText = i+1 < gameTexts.Length ? gameTexts[i + 1] : "";

                ChessGame game = ParseGame(gameText);
                if (game != null)
                {
                    game.Moves = movesText;
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

                // filter out illegal characters
                eventName = Regex.Replace(eventName, @"[^\p{L}\p{N}\p{P}\p{Z}]", "");

                string site = GetTagValue(tagMatches, "Site");
                string eventDateStr = GetTagValue(tagMatches, "EventDate");
                DateTime eventDate;

                if (!DateTime.TryParseExact(eventDateStr, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out eventDate)) { 
                    eventDate = DateTime.MinValue;
                }

                string round = GetTagValue(tagMatches, "Round");
                string whitePlayer = GetTagValue(tagMatches, "White");
                string blackPlayer = GetTagValue(tagMatches, "Black");
                string result = GetTagValue(tagMatches, "Result");

                // Parse Elo ratings
                int whiteElo = int.Parse(GetTagValue(tagMatches, "WhiteElo"));
                int blackElo = int.Parse(GetTagValue(tagMatches, "BlackElo"));

                // Parse result
                char resultChar = ParseResult(result);

                // Create a new chess game object
                game = new ChessGame(eventName, site, eventDate, round, whitePlayer, blackPlayer, whiteElo, blackElo, resultChar, "");


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
