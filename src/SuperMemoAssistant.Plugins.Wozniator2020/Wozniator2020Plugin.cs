#region License & Metadata

// The MIT License (MIT)
// 
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// 
// 
// Created On:   5/14/2020 4:48:16 AM
// Modified By:  SMA

#endregion




namespace SuperMemoAssistant.Plugins.Wozniator2020
{
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using System.Runtime.InteropServices;
  using System.Runtime.Remoting;
  using System.Windows;
  using System.Windows.Input;
  using Anotar.Serilog;
  using SuperMemoAssistant.Extensions;
  using SuperMemoAssistant.Interop.SuperMemo.Content.Contents;
  using SuperMemoAssistant.Interop.SuperMemo.Core;
  using SuperMemoAssistant.Interop.SuperMemo.Elements.Builders;
  using SuperMemoAssistant.Interop.SuperMemo.Elements.Models;
  using SuperMemoAssistant.Services;
  using SuperMemoAssistant.Services.IO.Keyboard;
  using SuperMemoAssistant.Services.Sentry;
  using SuperMemoAssistant.Sys.IO.Devices;
  using SuperMemoAssistant.Sys.Remoting;

  // ReSharper disable once UnusedMember.Global
  // ReSharper disable once ClassNeverInstantiated.Global
  [SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces")]
  public class Wozniator2020Plugin : SentrySMAPluginBase<Wozniator2020Plugin>
  {
    /// <summary>
    /// The url of Dr. Wozniak's picture
    /// </summary>
    private const string WozniakImageUrl = "https://www.wired.com/wp-content/uploads/archive/images/article/magazine/1605/ff_wozniak_swim_f.jpg";


    #region Constructors

    /// <inheritdoc />
    public Wozniator2020Plugin() : base("Enter your Sentry.io api key (strongly recommended)") { }

    #endregion




    #region Properties Impl - Public

    /// <inheritdoc />
    public override string Name => "Wozniator 2020";

    /// <inheritdoc />
    public override bool HasSettings => false;

    #endregion




    #region Methods Impl

    /// <inheritdoc />
    protected override void PluginInit()
    {
      Svc.HotKeyManager

         //
         // Woz Emergency
         .RegisterGlobal(
           "WozEmergency",                           // internal id
           "Woz Emergency",                          // name displayed to the user in the settings (more on that later)
           HotKeyScopes.SMBrowser,                   // this hotkey will only trigger if used inside SM element window
           new HotKey(Key.W, KeyModifiers.CtrlAlt),  // the keyboard key stroke combination
           WozEmergency                              // the callback
         )

         //
         // Pool of Insight
         .RegisterGlobal(
           "PoolOfInsight",
           "Pool of Insight",
           HotKeyScopes.SM,
           new HotKey(Key.Q, KeyModifiers.CtrlAlt),
           PoolOfInsight
         );

      // Uncomment to register an event handler which will be notified when the displayed element changes
      Svc.SM.UI.ElementWdw.OnElementChanged += new ActionProxy<SMDisplayedElementChangedEventArgs>(OnElementChanged);
    }

    // Set HasSettings to true, and uncomment this method to add your custom logic for settings
    // /// <inheritdoc />
    // public override void ShowSettings()
    // {
    // }

    #endregion




    #region Methods

    [LogToErrorOnException]
    private void OnElementChanged(SMDisplayedElementChangedEventArgs e)
    {
    }

    /// <summary>
    /// Inserts a html &lt;img&gt; tag at the top of the currently selected HTML component (if any).
    /// </summary>
    [LogToErrorOnException]
    private void WozEmergency()
    {
      try
      {
        var htmlCtrl = Svc.SM.UI.ElementWdw.ControlGroup?.FocusedControl?.AsHtml();

        if (htmlCtrl is )
          return;

        htmlCtrl.Text = $@"<img src=""{WozniakImageUrl}"" /> <br />
{htmlCtrl.Text}";
      }
      catch (RemotingException) { }
    }

    /// <summary>
    /// Creates a new Topic containing a random quote from Piotr's supermemo.guru website.
    /// </summary>
    [LogToErrorOnException]
    private void PoolOfInsight()
    {
      try
      {
        var quoteText = Utils.GetRandomQuote();

        if (quoteText == null)
        {
          MessageBox.Show("An error occured while loading a random quote from the quote database");
          return;
        }

        var elemBuilder = new ElementBuilder(
          ElementType.Topic,
          quoteText,
          false);

        bool success = Svc.SM.Registry.Element.Add(
          out var results,
          ElemCreationFlags.CreateSubfolders,
          elemBuilder);

        if (success == false)
        {
          var errMsg = results.GetErrorString();

          MessageBox.Show(errMsg);
        }
      }
      catch (RemotingException) { }
    }

    #endregion
  }
}
