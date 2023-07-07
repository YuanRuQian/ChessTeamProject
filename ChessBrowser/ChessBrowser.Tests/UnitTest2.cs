namespace ChessBrowser.Tests;
using MySqlConnector;
using System.IO;
using Microsoft.Extensions.Configuration;

[TestFixture]
public class PerformQueryTests
{
    // input your uid and password before running the test
    static string connection;

    [SetUp]
    public void Init() {
        var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
        string password = configuration["Password"];
        string uid = configuration["UID"];
        connection = "server=atr.eng.utah.edu;database = Team12ChessProject;uid = " + uid + ";password = " + password;
    }

    [Test]
    public static void TestWithNoFilter()
    {
        string white = null;
        string black = null;
        string opening = null;
        string winner = null;
        bool useDate = false;
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        bool showMoves = false;

        string result = PerformQuery(white, black, opening, winner, useDate, start, end, showMoves);
        Assert.That(ReadFirstLine(result), Is.EqualTo("2999 results"));
    }

    [Test]
    public static void TestWithWhite()
    {
        string white = "Carlsen, Magnus";
        string black = null;
        string opening = null;
        string winner = null;
        bool useDate = false;
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        bool showMoves = false;

        string result = PerformQuery(white, black, opening, winner, useDate, start, end, showMoves);
        Assert.That(ReadFirstLine(result), Is.EqualTo("9 results"));

    }

    [Test]
    public static void TestWithBlack()
    {
        string white = null;
        string black = "Carlsen, Magnus";
        string opening = null;
        string winner = null;
        bool useDate = false;
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        bool showMoves = false;

        string result = PerformQuery(white, black, opening, winner, useDate, start, end, showMoves);
        Assert.That(ReadFirstLine(result), Is.EqualTo("3 results"));

    }

    [Test]
    public static void TestWithOpening()
    {
        string white = null;
        string black = null;
        string opening = "1.Nf3";
        string winner = null;
        bool useDate = false;
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        bool showMoves = false;

        string result = PerformQuery(white, black, opening, winner, useDate, start, end, showMoves);
        Assert.That(ReadFirstLine(result), Is.EqualTo("945 results"));
    }


    [Test]
    public static void TestWithWhiteAndWinner() {
        string white = "Carlsen, Magnus";
        string black = null;
        string opening = null;
        bool useDate = false;
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        bool showMoves = false;

        string result1 = PerformQuery(white, black, opening, "W", useDate,
            start, end, showMoves);
        Assert.That(ReadFirstLine(result1), Is.EqualTo("8 results"));

        string result2 = PerformQuery(white, black, opening, "B", useDate,
            start, end, showMoves);
        Assert.That(ReadFirstLine(result2), Is.EqualTo("0 results"));

        string result3 = PerformQuery(white, black, opening, "D", useDate,
            start, end, showMoves);
        Assert.That(ReadFirstLine(result3), Is.EqualTo("1 results"));

    }

    [Test]
    public static void TestDateFilter() {
        string white = null;
        string black = null;
        string opening = null;
        string winner = null;
        bool useDate = true;
        DateTime start = new DateTime(2018, 1, 1, 0, 0, 0);
        DateTime end = new DateTime(2018, 12, 31, 0, 0, 0);
        bool showMoves = false;

        string result = PerformQuery(white, black, opening, winner, useDate,
            start, end, showMoves);
        Assert.That(ReadFirstLine(result), Is.EqualTo("975 results"));
    }

    [Test]
    public static void TestWithShowMoves() {
        string white = "Carlsen, Magnus";
        string black = null;
        string opening = null;
        string winner = "D";
        bool useDate = false;
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now;
        bool showMoves = true;

        string result = PerformQuery(white, black, opening, winner, useDate,
            start, end, showMoves);
        string expected = "1 results\n\n" +
            "Event: 23. ECC Men\n" + "Site: Kemer TUR\n" +
            "Date: 10/3/2007 12:00:00 AM\n" + "White: Carlsen, Magnus (2835)\n" +
            "Black: Cornette, Matthieu (2533)\n" + "Result: D\n" +
            "Moves: 1.d4 Nf6 2.c4 e6 3.Nf3 c5 4.g3 cxd4 5.Nxd4 d5 6.Bg2 e5 7.Nf3 e4 8.Nd4 dxc4\r\n" +
            "9.Qa4+ Bd7 10.Qxc4 Na6 11.O-O Rc8 12.Qb3 Nc5 13.Qd1 Be7 14.Nc3 O-O 15.Be3 \r\n" +
            "Ng4 16.Bf4 e3 17.Bxe3 Nxe3 18.fxe3 Bg5 19.Nf5 Bxf5 20.Rxf5 Bxe3+ 21.Kh1 g6\r\n" +
            "22.Rd5 Qf6 23.Rd6 Qe7 24.Qd5 Rfd8 25.Rd1 Rxd6 26.Qxd6 Qxd6 27.Rxd6 Bg5 28.\r\n" +
            "Bf3 Be7 29.Rd1 Kg7 30.Nd5 Bf8 31.Rc1 Rd8 32.Kg2 Ne6 33.Nc7 Bd6 34.Nxe6+ \r\n" +
            "fxe6 35.Bxb7 Rb8 36.Ba6 Rxb2 37.Bc4 Kf6 38.Kf3 a5 39.a4 h5 40.Bb5 Rb3+ 41.\r\n" +
            "Kg2 Bb4 42.Rc7 Rc3 43.Ra7 Rc2 44.Ra6 g5 45.Ra7 h4 46.gxh4 gxh4 47.Rh7 Be1 \r\n" +
            "48.Rh6+ Ke7 49.Rg6 Rc3 50.Bd3 Ra3 51.Rg4 Bb4 52.Rxh4 Rxa4 53.Re4 Kf6 54.h4\r\n" +
            "Ra1 55.h5 Bd2 56.Rg4 Be3 57.Kf3 Rg1 58.Ra4 Bh6 59.Rxa5 Rg5 60.Rxg5 1/2-1/2";
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public static void TestFiltersCombination() {
        string white = "Carlsen, Magnus";
        string black = null;
        string opening = null;
        string winner = "W";
        bool useDate = true;
        DateTime start = new DateTime(2018, 1, 1, 0, 0, 0);
        DateTime end = new DateTime(2018, 12, 31, 0, 0, 0);
        bool showMoves = false;

        string result = PerformQuery(white, black, opening, winner, useDate,
            start, end, showMoves);
        Assert.That(ReadFirstLine(result), Is.EqualTo("7 results"));
    }

    internal static string ReadFirstLine(string s)
    {
        using (StringReader reader = new StringReader(s))
        {
            return reader.ReadLine();
        }
    }
    
    internal static string PerformQuery(string white, string black, string opening,
      string winner, bool useDate, DateTime start, DateTime end, bool showMoves)
    {
        // This will build a connection string to your user's database on atr,
        // assuimg you've typed a user and password in the GUI

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
