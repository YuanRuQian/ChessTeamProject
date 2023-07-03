using System;

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
        public string[] Moves { get; set; }

        public ChessGame(string eventName, string site, DateTime eventDate, string round, string whitePlayer, string blackPlayer, int whiteElo, int blackElo, char result, string[] moves)
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
            return $"Event: {EventName}\n" +
                   $"Site: {Site}\n" +
                   $"Date: {EventDate}\n" +
                   $"Round: {Round}\n" +
                   $"White: {WhitePlayer} ({WhiteElo})\n" +
                   $"Black: {BlackPlayer} ({BlackElo})\n" +
                   $"Result: {Result}\n" +
                   $"Moves: {string.Join(" ", Moves)}";
        }
    }
}
