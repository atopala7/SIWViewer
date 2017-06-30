using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SIWViewer.Views
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        //public XDocument dom;

        public MainPage()
        {
            InitializeComponent();

            System.Diagnostics.Debug.WriteLine("Main");

            GetInitialFile();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public async void GetInitialFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            picker.FileTypeFilter.Add(".xml");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                System.Diagnostics.Debug.Write("File opened.");
                ProcessFile(file);
            }
            else
            {
                Application.Current.Exit();
            }
        }

        public void ChangeTitle(string newTitle)
        {
            TitlePage.Text = newTitle;
        }

        private async void ProcessFile(StorageFile file)
        {
            //fileName = "C:/Users/andre/Documents/Visual Studio 2017/Projects/SIWViewer/SIWViewer/x.xml";

            //FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);

            // implement the open...
            try
            {
                // SECTION 1. Create a DOM Document and load the XML data into it.
                
                ((App)Application.Current).dom = XDocument.Load(readStream.AsStreamForRead());

                var dom = ((App)Application.Current).dom;

                System.Diagnostics.Debug.Write(dom.Root.Name.ToString());


                //SECTION 1.5. Verify the version and set the title
                if (!dom.Root.Name.ToString().Equals("report"))
                {
                    //MessageBox.Show(this, "The file '" + fileName + "' is not a SIW 'report' file !", "SIW Viewer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    var dialogNotReport = new MessageDialog("The file '" + file.Name + "' is not a SIW 'report' file!");
                    await dialogNotReport.ShowAsync();
                    Application.Current.Exit();
                }
                System.Globalization.NumberFormatInfo provider = new System.Globalization.NumberFormatInfo();

                provider.NumberDecimalSeparator = ".";
                provider.NumberGroupSeparator = ",";
                provider.NumberGroupSizes = new int[] { 3 };

                float ver = System.Convert.ToSingle(dom.Root.Attribute("xml_version").Value, provider);
                if (ver < 1.2f)
                {
                    var dialogInvalidVersion = new MessageDialog("Only the versions >= 1.2 are supported, not this one: " + ver + " !");
                    Application.Current.Exit();
                }
                var title = "SIW Viewer [" + file.Name + "] - \\\\" + dom.Root.Attribute("computer_name");
                //var dialogTitle = new MessageDialog(title);
                //await dialogTitle.ShowAsync();
                //System.Diagnostics.Debug.WriteLine(title);

                //var frame = (Frame)Window.Current.Content;
                //var page = (MainPage)frame.Content;

                //var currentFrame = Window.Current.Content as Frame;
                //var page = currentFrame.Content as ShellPage;

                var page = Window.Current.Content as ShellPage;

                page.PopulateNavItemsWithXMLFile();

                ChangeTitle(title);

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
    }
}
