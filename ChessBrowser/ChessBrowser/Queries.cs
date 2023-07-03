using Microsoft.Maui.Controls;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChessBrowser
{
  internal class Queries
  {

        /// <summary>
        /// This function runs when the upload button is pressed.
        /// Given a filename, parses the PGN file, and uploads
        /// each chess game to the user's database.
        /// </summary>
        /// <param name="PGNfilename">The path to the PGN file</param>
        internal static async Task InsertGameData(string PGNfilename, MainPage mainPage)
        {
            string connection = mainPage.GetConnectionString();

            List<ChessGame> games = PgnReader.ReadGames(PGNfilename);
            mainPage.SetNumWorkItems(games.Count);

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    foreach (ChessGame game in games)
                    {
                        // Insert into the Events table
                        string insertEventQuery = "INSERT IGNORE INTO Events (Name, Site, Date) VALUES (@Name, @Site, @Date)";
                        using (MySqlCommand eventCommand = new MySqlCommand(insertEventQuery, conn))
                        {
                            eventCommand.Parameters.AddWithValue("@Name", game.EventName);
                            eventCommand.Parameters.AddWithValue("@Site", game.Site);
                            eventCommand.Parameters.AddWithValue("@Date", game.EventDate);
                            eventCommand.ExecuteNonQuery();
                        }

                        // Retrieve the generated eID
                        ulong eID;
                        using (MySqlCommand lastIdCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", conn))
                        {
                            eID = Convert.ToUInt64(lastIdCommand.ExecuteScalar());
                        }

                        // Insert into the Players table
                        string insertWhitePlayerQuery = "INSERT IGNORE INTO Players (Name, Elo) VALUES (@BlackPlayerName, @BlackPlayerElo)";
                        string insertBlackPlayerQuery = "INSERT IGNORE INTO Players (Name, Elo) VALUES (@WhitePlayerName, @WhitePlayerElo)";

                        using (MySqlCommand whitePlayerCommand = new MySqlCommand(insertWhitePlayerQuery, conn))
                        {
                            whitePlayerCommand.Parameters.AddWithValue("@WhitePlayerName", game.WhitePlayer);
                            whitePlayerCommand.Parameters.AddWithValue("@WhitePlayerElo", game.WhiteElo);
                            whitePlayerCommand.ExecuteNonQuery();
                        }

                        ulong whitePlayerID;
                        using (MySqlCommand lastIdCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", conn))
                        {
                            whitePlayerID = Convert.ToUInt64(lastIdCommand.ExecuteScalar());
                        }

                        using (MySqlCommand blackPlayerCommand = new MySqlCommand(insertBlackPlayerQuery, conn))
                        {
                            blackPlayerCommand.Parameters.AddWithValue("@BlackPlayerName", game.BlackPlayer);
                            blackPlayerCommand.Parameters.AddWithValue("@BlackPlayerElo", game.BlackElo);
                            blackPlayerCommand.ExecuteNonQuery();
                        }

                        // Retrieve the generated pIDs
                        ulong blackPlayerID;
                        using (MySqlCommand lastIdCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", conn))
                        {
                            blackPlayerID = Convert.ToUInt64(lastIdCommand.ExecuteScalar());
                        }

                        // Insert into the Games table
                        string insertGameQuery = "INSERT IGNORE INTO Games (Round, Result, Moves, BlackPlayer, WhitePlayer, eID) " +
                            "VALUES (@Round, @Result, @Moves, @BlackPlayer, @WhitePlayer, @eID)";
                        using (MySqlCommand gameCommand = new MySqlCommand(insertGameQuery, conn))
                        {
                            gameCommand.Parameters.AddWithValue("@Round", game.Round);
                            gameCommand.Parameters.AddWithValue("@Result", game.Result.ToString());
                            gameCommand.Parameters.AddWithValue("@Moves", string.Join(" ", game.Moves));
                            gameCommand.Parameters.AddWithValue("@BlackPlayer", blackPlayerID);
                            gameCommand.Parameters.AddWithValue("@WhitePlayer", whitePlayerID);
                            gameCommand.Parameters.AddWithValue("@eID", eID);
                            gameCommand.ExecuteNonQuery();
                        }
                    }


                    await mainPage.NotifyWorkItemCompleted();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }
        }



        /// <summary>
        /// Queries the database for games that match all the given filters.
        /// The filters are taken from the various controls in the GUI.
        /// </summary>
        /// <param name="white">The white player, or null if none</param>
        /// <param name="black">The black player, or null if none</param>
        /// <param name="opening">The first move, e.g. "1.e4", or null if none</param>
        /// <param name="winner">The winner as "W", "B", "D", or null if none</param>
        /// <param name="useDate">True if the filter includes a date range, False otherwise</param>
        /// <param name="start">The start of the date range</param>
        /// <param name="end">The end of the date range</param>
        /// <param name="showMoves">True if the returned data should include the PGN moves</param>
        /// <returns>A string separated by newlines containing the filtered games</returns>
        internal static string PerformQuery( string white, string black, string opening,
      string winner, bool useDate, DateTime start, DateTime end, bool showMoves,
      MainPage mainPage )
    {
      // This will build a connection string to your user's database on atr,
      // assuimg you've typed a user and password in the GUI
      string connection = mainPage.GetConnectionString();

      // Build up this string containing the results from your query
      string parsedResult = "";

      // Use this to count the number of rows returned by your query
      // (see below return statement)
      int numRows = 0;

      using ( MySqlConnection conn = new MySqlConnection( connection ) )
      {
        try
        {
          // Open a connection
          conn.Open();

          // TODO:
          //       Generate and execute an SQL command,
          //       then parse the results into an appropriate string and return it.
        }
        catch ( Exception e )
        {
          System.Diagnostics.Debug.WriteLine( e.Message );
        }
      }

      return numRows + " results\n" + parsedResult;
    }

  }
}
