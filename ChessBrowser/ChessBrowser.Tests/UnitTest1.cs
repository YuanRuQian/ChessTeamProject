namespace ChessBrowser.Tests;

[TestFixture]
public class PgnReaderTests
{

    [Test]
    public void ReadGames_ShouldPrintAllGames()
    {
        // Get the current directory
        string currentDirectory = Directory.GetCurrentDirectory();
        TestContext.WriteLine("Current Directory: " + currentDirectory);
        TestContext.WriteLine();

        // Get the target directory
        string targetDirectory = Path.GetFullPath(Path.Combine(currentDirectory, "../../../../.."));
        TestContext.WriteLine("Target Directory: " + targetDirectory);
        TestContext.WriteLine();


        // Act
        string[] pgnFiles = Directory.GetFiles(targetDirectory, "*.pgn", SearchOption.AllDirectories);

        foreach (string pgnFile in pgnFiles)
        {
            // Read games from the PGN file
            List<ChessGame> games = PgnReader.ReadGames(pgnFile);

            // Print out the name of the PGN file
            TestContext.WriteLine("PGN File: " + Path.GetFileName(pgnFile));
            TestContext.WriteLine();

            // Print out the information for each ChessGame
            foreach (ChessGame game in games)
            {
                TestContext.WriteLine(game.ToString());
                TestContext.WriteLine();
            }
        }
    }
}
