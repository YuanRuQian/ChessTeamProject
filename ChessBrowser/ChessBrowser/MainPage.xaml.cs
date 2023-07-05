namespace ChessBrowser;

public partial class MainPage : ContentPage
{
  private int numWorkItems = 0;
  private int workItemsCompleted = 0;

  public MainPage()
	{
		InitializeComponent();
	}

    /// <summary>
    /// Handler for the upload button.
    /// Picks a file and passes it to the database controller
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnFileUpload(object sender, EventArgs e)
    {
        try
        {
            // Disable the "Go" button when uploading
            goButton.IsEnabled = false;
            FileResult fileResult = await FilePicker.Default.PickAsync();
            if (fileResult != null)
            {
                await Queries.InsertGameData(fileResult.FullPath, this);
                string fileContents = File.ReadAllText(fileResult.FullPath);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            // Once the upload is complete, enable the "Go" button if necessary
            goButton.IsEnabled = true;
        }
    }


    /// <summary>
    /// Handler for the go button.
    /// Passes the query parameters to the database controller and displays the result
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnGoClicked(object sender, EventArgs e)
    {
        // Disable the upload button when searching
        uploadButton.IsEnabled = false;

        string winner = null;
        if (whiteWon.IsChecked)
        {
            winner = "W";
        }
        else if (blackWon.IsChecked)
        {
            winner = "B";
        }
        else if (drawGame.IsChecked)
        {
            winner = "D";
        }

        string wp = string.IsNullOrEmpty(whiteplayer.Text) ? null : whiteplayer.Text;
        string bp = string.IsNullOrEmpty(blackplayer.Text) ? null : blackplayer.Text;
        string open = string.IsNullOrEmpty(openingmove.Text) ? null : openingmove.Text;

        var res = Queries.PerformQuery(wp, bp, open, winner, filterByDate.IsChecked,
            startDate.Date, endDate.Date, showMoves.IsChecked, this);

        outputText.Text = res;

        // Once the search is complete, enable the upload button if necessary
        uploadButton.IsEnabled = true;
    }

    /// <summary>
    /// Tell the progress bar how many "work items" you're going to perform
    /// </summary>
    /// <param name="numItems"></param>
    public void SetNumWorkItems( int numItems )
  {
    numWorkItems = numItems;
    workItemsCompleted = 0;
  }

  /// <summary>
  /// Tell the progress bar that you've completed one of the "work items"
  /// so it will update the bar
  /// </summary>
  public async Task NotifyWorkItemCompleted()
  {
    workItemsCompleted++;
    double newProgress = ((double)workItemsCompleted) / numWorkItems;
    await progressbar.ProgressTo( newProgress, 1, Easing.Linear ).ContinueWith( ( res ) => { } );
  }

  /// <summary>
  /// Returns a mysql connection string using the inputs entered for username and password
  /// </summary>
  /// <returns></returns>
  internal string GetConnectionString()
  {
        return "server=155.98.12.109;database=Team12ChessProject;uid=" + username.Text + ";password=" + password.Text;
  }
}

