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
            listViewInterfaces.Columns.Add("Interface Card");
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
            RegistryKey keyInterface = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces");
            foreach (var interf in keyInterface.GetSubKeyNames())
            {
                string UID = interf;

                RegistryKey keyService = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkCards");
                foreach (var serv in keyService.GetSubKeyNames())
                {
                    RegistryKey subService = keyService.OpenSubKey(serv);
                    string ServiceName = subService.GetValue("ServiceName").ToString();
                    if (ServiceName.ToUpper() == UID.ToUpper())
                    { 
                        string Description = subService.GetValue("Description").ToString();

                        RegistryKey keyName = Registry.LocalMachine.OpenSubKey($@"SYSTEM\ControlSet001\Control\Network\{{4D36E972-E325-11CE-BFC1-08002BE10318}}\{UID}\Connection");
                        if (keyName != null)
                        {
                            string Name = keyName.GetValue("Name").ToString();

                            ListViewItem item = new ListViewItem();
                            item.Text = Name;
                            item.Tag = false;
                            item.SubItems.Add("");
                            item.SubItems.Add(Description);
                            item.SubItems.Add(UID);
                            listViewInterfaces.Items.Add(item);
                        }
                    }
                }
            }
        }

        //*************************************************************************************************************
        private void listViewInterfaces_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
