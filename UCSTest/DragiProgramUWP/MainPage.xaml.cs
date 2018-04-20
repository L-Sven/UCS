using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DragiProgramUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private string _nameToReplace;


        #region ===== Delegates =====


        private readonly SendDataToDatabaseDelegate _sendDataToDatabase = SqlMetoder.SendDataToDatabase;
        private readonly GetDataFromDatabaseDelegate _getDataFromDatabase = SqlMetoder.GetDataFromDatabase;
        private readonly ReplaceNameAndReferenceDelegate _replaceNameAndReference = Functionality.ReplaceNameAndReference;


        #endregion

        #region ===== Buttons =====


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        //TODO:
        private void btnLäggTillNamn_Click(object sender, RoutedEventArgs e)
        {
            _nameToReplace = listViewNamnPåAvtal.SelectedItem.ToString();
            listViewNamnSomErsätts.Items.Add(_nameToReplace);
            listViewNamnPåAvtal.SelectedItems.Clear();
        }

        //TODO:
        private void btnGenomförÄndring_Click(object sender, RoutedEventArgs e)
        {
            //Gets the firstName from the list of names to replace. 
            //TODO: Add function to replace multiple names!
            _replaceNameAndReference(_nameToReplace, listViewNamnSomErsätts.Items[0].ToString());
        }


        #endregion


    }
}
