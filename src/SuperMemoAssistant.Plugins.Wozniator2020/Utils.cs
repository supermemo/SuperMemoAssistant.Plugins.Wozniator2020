using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Anotar.Serilog;
using Extensions.System.IO;
using SuperMemoAssistant.Interop;

namespace SuperMemoAssistant.Plugins.Wozniator2020
{
  public static class Utils
  {
    #region Constants & Statics

    private static readonly string[] SentenceEndingPunctuation = { ".", "!", "?" };
    private const string QuoteFileName = "quotes.tsv";

    #endregion



    #region Methods

    /// <summary>
    /// Loads the quote file and selects a random quote.
    /// </summary>
    /// <returns>The quote text or <see langword="null" /> if an error occured.</returns>
    public static string GetRandomQuote()
    {
      // Tab separated file with a heading
      // Quote, Author, Url, Title
      var quoteFile = new FilePath(QuoteFileName);

      if (!quoteFile.Exists())
        return null;

      try
      {
        var lines = File.ReadAllLines(quoteFile.FullPath);

        // First line is the heading.
        if (lines.Length <= 1)
          return null;

        var randInt = new Random();
        var randomLineNumber = randInt.Next(1, lines.Length - 1);
        var quoteLine = lines[randomLineNumber];
        var splitQuoteLine = quoteLine.Split('\t');

        // Check that the chosen line has 4 fields
        if (splitQuoteLine.Length == 4)
        {
          // If the quote doesn't end with sentence ending punctuation, add a full stop.
          if (!SentenceEndingPunctuation.Any(p => splitQuoteLine[0]
                                        .EndsWith(p, StringComparison.OrdinalIgnoreCase)))
            splitQuoteLine[0] += ".";

          // Build and return the text
          return $@"<p><b>{splitQuoteLine[0]}</b> -- {splitQuoteLine[1]}</p>
<br />
<p><small><i>Source</i>: <a href='{splitQuoteLine[2]}'>{splitQuoteLine[3]}</a></small></p>";
        }
      }
      catch (IOException ex)
      {
        LogTo.Warning(ex, "IOException when trying to open {FullPath}", quoteFile.FullPath);
      }
      catch (Exception ex)
      {
        LogTo.Error(ex, "Exception caught while opening file {FullPath}", quoteFile.FullPath);
      }

      return null;
    }

    #endregion
  }
}
