using MySqlConnector;

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
                        // Retrieve the eID of the event
                        string eventIdQuery = "SELECT eID FROM Events WHERE Name = @Name AND Date = @Date AND Site = @Site";
                        using (MySqlCommand eventIdCommand = new MySqlCommand(eventIdQuery, conn))
                        {
                            eventIdCommand.Parameters.AddWithValue("@Name", game.EventName);
                            eventIdCommand.Parameters.AddWithValue("@Site", game.Site);
                            eventIdCommand.Parameters.AddWithValue("@Date", game.EventDate);
                            eID = Convert.ToUInt64(eventIdCommand.ExecuteScalar());
                        }


                        string insertAndUpdateWhitePlayerQuery = "INSERT INTO Players (Name, Elo) VALUES (@WhitePlayerName, @WhitePlayerElo) " +
                            "ON DUPLICATE KEY UPDATE Elo = GREATEST(Elo, @WhitePlayerElo)";

                        using (MySqlCommand whitePlayerCommand = new MySqlCommand(insertAndUpdateWhitePlayerQuery, conn))
                        {
                            whitePlayerCommand.Parameters.AddWithValue("@WhitePlayerName", game.WhitePlayer);
                            whitePlayerCommand.Parameters.AddWithValue("@WhitePlayerElo", game.WhiteElo);
                            whitePlayerCommand.ExecuteNonQuery();
                        }

                        ulong whitePlayerID;
                        // Retrieve the pID of the white player
                        string whitePlayerIdQuery = "SELECT pID FROM Players WHERE Name = @WhitePlayerName";
                        using (MySqlCommand whitePlayerIdCommand = new MySqlCommand(whitePlayerIdQuery, conn))
                        {
                            whitePlayerIdCommand.Parameters.AddWithValue("@WhitePlayerName", game.WhitePlayer);
                            whitePlayerID = Convert.ToUInt64(whitePlayerIdCommand.ExecuteScalar());
                        }

                        string insertAndUpdateBlackPlayerQuery = "INSERT INTO Players (Name, Elo) VALUES (@BlackPlayerName, @BlackPlayerElo) " +
                            "ON DUPLICATE KEY UPDATE Elo = GREATEST(Elo, @BlackPlayerElo)";

                        using (MySqlCommand blackPlayerCommand = new MySqlCommand(insertAndUpdateBlackPlayerQuery, conn))
                        {
                            blackPlayerCommand.Parameters.AddWithValue("@BlackPlayerName", game.BlackPlayer);
                            blackPlayerCommand.Parameters.AddWithValue("@BlackPlayerElo", game.BlackElo);
                            blackPlayerCommand.ExecuteNonQuery();
                        }

                        // Retrieve the generated pIDs
                        ulong blackPlayerID;
                        string blackPlayerIdQuery = "SELECT pID FROM Players WHERE Name = @BlackPlayerName";
                        using (MySqlCommand blackPlayerIdCommand = new MySqlCommand(blackPlayerIdQuery, conn))
                        {
                            blackPlayerIdCommand.Parameters.AddWithValue("@BlackPlayerName", game.BlackPlayer);
                            blackPlayerID = Convert.ToUInt64(blackPlayerIdCommand.ExecuteScalar());
                        }

                        // Insert into the Games table
                        string insertGameQuery = "INSERT IGNORE INTO Games (Round, Result, Moves, BlackPlayer, WhitePlayer, eID) " +
                            "VALUES (@Round, @Result, @Moves, @BlackPlayer, @WhitePlayer, @eID)";
                        using (MySqlCommand gameCommand = new MySqlCommand(insertGameQuery, conn))
                        {
                            gameCommand.Parameters.AddWithValue("@Round", game.Round);
                            gameCommand.Parameters.AddWithValue("@Result", game.Result.ToString());
                            gameCommand.Parameters.AddWithValue("@Moves", game.Moves);
                            gameCommand.Parameters.AddWithValue("@BlackPlayer", blackPlayerID);
                            gameCommand.Parameters.AddWithValue("@WhitePlayer", whitePlayerID);
                            gameCommand.Parameters.AddWithValue("@eID", eID);
                            gameCommand.ExecuteNonQuery();
                        }

                        await mainPage.NotifyWorkItemCompleted();
                    }

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
        internal static string PerformQuery(string white, string black, string opening,
      string winner, bool useDate, DateTime start, DateTime end, bool showMoves,
      MainPage mainPage)
        {
            // This will build a connection string to your user's database on atr,
            // assuimg you've typed a user and password in the GUI
            string connection = mainPage.GetConnectionString();

            // Build up this string containing the results from your query
            string parsedResult = "";

            // Use this to count the number of rows returned by your query
            // (see below return statement)
            int numRows = 0;

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                try
                {
                    // Open a connection
                    conn.Open();

                    // Generate and execute an SQL command,
                    string query = "SELECT e.Name AS EventName, e.Site, e.Date,\n" +
                        "p1.Name AS WhitePlayerName, p2.Name AS BlackPlayerName,\n" +
                        "p1.Elo AS WhiteElo, p2.Elo AS BlackElo, g.Result, g.Moves\n" +
                        "FROM Events e JOIN Games g ON e.eID = g.eID\n" +
                        "JOIN Players p1 ON g.WhitePlayer = p1.pID\n" +
                        "JOIN Players p2 ON g.BlackPlayer = p2.pID WHERE TRUE\n";
                    if (white != null)
                    {
                        query += " AND p1.Name=@WhitePlayerName";
                    }
                    if (black != null)
                    {
                        query += " AND p2.Name=@BlackPlayerName";
                    }
                    if (opening != null)
                    {
                        query += " AND Moves LIKE @OpeningMove";
                    }
                    if (winner != null)
                    {
                        query += " AND Result=@Result";
                    }
                    if (useDate)
                    {
                        query += " AND Date>=@StartDate AND Date<=@EndDate";
                    }

                    MySqlCommand queryCommand = new MySqlCommand(query, conn);
                    if (white != null)
                    {
                        queryCommand.Parameters.AddWithValue("@WhitePlayerName", white);
                    }
                    if (black != null)
                    {
                        queryCommand.Parameters.AddWithValue("@BlackPlayerName", black);
                    }
                    if (opening != null)
                    {
                        queryCommand.Parameters.AddWithValue("@OpeningMove", opening + "%");
                    }
                    if (winner != null)
                    {
                        queryCommand.Parameters.AddWithValue("@Result", winner);
                    }
                    if (useDate)
                    {
                        queryCommand.Parameters.AddWithValue("@StartDate", start);
                        queryCommand.Parameters.AddWithValue("@EndDate", end);
                    }
                    Console.WriteLine(query);
                    // parse the results into an appropriate string and return it.
                    using (MySqlDataReader reader = queryCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            numRows += 1;
                            parsedResult += "\n\nEvent: " + reader["EventName"];
                            parsedResult += "\nSite: " + reader["Site"];
                            parsedResult += "\nDate: " + reader["Date"];
                            parsedResult += "\nWhite: " + reader["WhitePlayerName"] + " (" + reader["WhiteElo"] + ")";
                            parsedResult += "\nBlack: " + reader["BlackPlayerName"] + " (" + reader["BlackElo"] + ")";
                            parsedResult += "\nResult: " + reader["Result"];
                            if (showMoves)
                            {
                                parsedResult += "\nMoves: " + reader["Moves"];
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
            }

            return numRows + " results" + parsedResult;
        }
    }
}
