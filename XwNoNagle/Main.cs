using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace XwNoNagle
{
    public partial class Main : Form
    {
        //*************************************************************************************************************
        public Main()
        {
            InitializeComponent();
            string CurrentVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(
               System.Reflection.Assembly.GetAssembly(typeof(Main)).Location).FileVersion.ToString();
            Text = $"XwNoNagle {CurrentVersion}";
        }

        //*************************************************************************************************************
        private void Main_Load(object sender, EventArgs e)
        {
            listViewInterfaces.FullRowSelect = true;
            listViewInterfaces.Columns.Add("Interface");
            listViewInterfaces.Columns.Add("Nagle disabled");
            listViewInterfaces.Columns.Add("Interface Uid");
            Main_Resize(sender, e);
            LoadInterfaces();
        }

        //*************************************************************************************************************
        private void Main_Resize(object sender, EventArgs e)
        {
            if (listViewInterfaces.Columns.Count == 0)
                return;

            int colW = (listViewInterfaces.Width - 20) / listViewInterfaces.Columns.Count;
            for (int i = 0; i < listViewInterfaces.Columns.Count; i++)
            {
                listViewInterfaces.Columns[i].Width = colW;
            }
        }

        //*************************************************************************************************************
        private void LoadInterfaces()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces");
            foreach (var interf in key.GetSubKeyNames())
            {
                string UID = interf;
                /*
RegistryKey productKey = key.OpenSubKey(v);
if (productKey != null)
{
    foreach (var value in productKey.GetValueNames())
    {
        Console.WriteLine("\tValue:" + value);

        // Check for the publisher to ensure it's our product
        string keyValue = Convert.ToString(productKey.GetValue("Publisher"));
        if (!keyValue.Equals("MyPublisherCompanyName", StringComparison.OrdinalIgnoreCase))
            continue;

        string productName = Convert.ToString(productKey.GetValue("DisplayName"));
        if (!productName.Equals("MyProductName", StringComparison.OrdinalIgnoreCase))
            return;

        string uninstallPath = Convert.ToString(productKey.GetValue("InstallSource"));

        // Do something with this valuable information
    }
}
*/

                ListViewItem item = new ListViewItem();
                item.Text = "";
                item.Tag = false;
                item.SubItems.Add("");
                item.SubItems.Add(UID);
                listViewInterfaces.Items.Add(item);
            }
        }
    }
}
