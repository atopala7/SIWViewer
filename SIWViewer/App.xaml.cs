using System;

using SIWViewer.Services;

using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using System.Xml;
using Windows.Storage;
using System.IO;
using Windows.ApplicationModel;
using System.Xml.Linq;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using SIWViewer.Views;
using Windows.UI.Xaml.Controls;

namespace SIWViewer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;
        private ActivationService ActivationService { get { return _activationService.Value; } }

        public XDocument dom;
        public String title = "SIW Viewer";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("Starting SIW Viewer");

            //getFileStartup();

            //String[] arguments = Environment.GetCommandLineArgs();
            //if (arguments.GetLength(0) == 2)
            //{
            //processFile("");//arguments[1]);
            //}

            //Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }

        //private async void getFileStartup()
        //{
        //    var picker = new Windows.Storage.Pickers.FileOpenPicker();
        //    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
        //    picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
        //    picker.FileTypeFilter.Add(".xml");

        //    Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
        //    if (file != null)
        //    {
        //        // Application now has read/write access to the picked file
        //        System.Diagnostics.Debug.Write("File opened.");
        //    }
        //    else
        //    {
        //        Exit();
        //    }
        //}

        private async void processFile(StorageFile file)
        {
            //fileName = "C:/Users/andre/Documents/Visual Studio 2017/Projects/SIWViewer/SIWViewer/x.xml";

            //FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);

            // implement the open...
            try
            {
                // SECTION 1. Create a DOM Document and load the XML data into it.
                dom = XDocument.Load(readStream.AsStreamForRead());

                System.Diagnostics.Debug.Write(dom.Root.Name.ToString());


                //SECTION 1.5. Verify the version and set the title
                if (!dom.Root.Name.ToString().Equals("report"))
                {
                    //MessageBox.Show(this, "The file '" + fileName + "' is not a SIW 'report' file !", "SIW Viewer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    var dialogNotReport = new MessageDialog("The file '" + file.Name + "' is not a SIW 'report' file!");
                    await dialogNotReport.ShowAsync();
                    Exit();
                }
                System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();

                provider.NumberDecimalSeparator = ".";
                provider.NumberGroupSeparator = ",";
                provider.NumberGroupSizes = new int[] { 3 };

                float ver = System.Convert.ToSingle(dom.Root.Attribute("xml_version").Value, provider);
                if (ver < 1.2f)
                {
                    var dialogInvalidVersion = new MessageDialog("Only the versions >= 1.2 are supported, not this one: " + ver + " !");
                    Exit();
                }
                title = "SIW Viewer [" + file.Name + "] - \\\\" + dom.Root.Attribute("computer_name");
                //var dialogTitle = new MessageDialog(title);
                //await dialogTitle.ShowAsync();
                //System.Diagnostics.Debug.WriteLine(title);

                //var frame = (Frame)Window.Current.Content;
                //var page = (MainPage)frame.Content;

                //var currentFrame = Window.Current.Content as Frame;
                //var page = currentFrame.Content as MainPage;

                
                //page.ChangeTitle(title);
            }
            catch (XmlException xmlEx)
            {
                System.Diagnostics.Debug.WriteLine(xmlEx.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public static String getAttribute(XmlNode xmlNode, String attrName)
        {
            for (int i = xmlNode.Attributes.Count; --i >= 0;)
            {
                if (xmlNode.Attributes.Item(i).Name.Equals(attrName))
                {
                    return xmlNode.Attributes.Item(i).Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!e.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(e); 
            }

            //var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            //picker.FileTypeFilter.Add(".xml");

            //Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            //if (file != null)
            //{
            //    // Application now has read/write access to the picked file
            //    System.Diagnostics.Debug.Write("File opened.");
            //    processFile(file);
            //}
            //else
            //{
            //    Exit();
            //}
        }

        /// <summary>
        /// Invoked when the application is activated by some means other than normal launching.
        /// </summary>
        /// <param name="args">Event data for the event.</param>
        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);

            //var picker = new Windows.Storage.Pickers.FileOpenPicker();
            //picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            //picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            //picker.FileTypeFilter.Add(".xml");

            //Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            //if (file != null)
            //{
            //    // Application now has read/write access to the picked file
            //    System.Diagnostics.Debug.Write("File opened.");
            //}
            //else
            //{
            //    Exit();
            //}
        }
            
        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Views.ShellPage());
        }


    }
}
