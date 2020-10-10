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
            using (RegistryKey keyInterface = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces"))
            {
                foreach (var interfaceKeyName in keyInterface.GetSubKeyNames())
                {
                    string UID = interfaceKeyName;

                    using (RegistryKey keyService = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\NetworkCards"))
                    {
                        foreach (var serviceKeyName in keyService.GetSubKeyNames())
                        {
                            using (RegistryKey subService = keyService.OpenSubKey(serviceKeyName))
                            {
                                string ServiceName = subService.GetValue("ServiceName").ToString();
                                if (ServiceName.ToUpper() == UID.ToUpper())
                                {
                                    string Description = subService.GetValue("Description").ToString();

                                    using (RegistryKey keyName = Registry.LocalMachine.OpenSubKey($@"SYSTEM\ControlSet001\Control\Network\{{4D36E972-E325-11CE-BFC1-08002BE10318}}\{UID}\Connection"))
                                    {
                                        if (keyName != null)
                                        {
                                            string Name = keyName.GetValue("Name").ToString();

                                            using (RegistryKey interfaceKey = keyInterface.OpenSubKey(interfaceKeyName))
                                            {
                                                var v1 = interfaceKey.GetValue("TcpAckFrequency");
                                                var v2 = interfaceKey.GetValue("TCPNoDelay");

                                                string NagleDisable = "NO";
                                                if (v1 != null)
                                                {
                                                    if (v2 != null)
                                                    {
                                                        if (v1.ToString() == "1" && v2.ToString() == "1")
                                                        {
                                                            NagleDisable = "YES";
                                                        }
                                                    }
                                                }

                                                ListViewItem item = new ListViewItem();
                                                item.Text = Name;
                                                item.Tag = $@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\{UID}";
                                                item.SubItems.Add(NagleDisable);
                                                item.SubItems.Add(Description);
                                                item.SubItems.Add(UID);
                                                listViewInterfaces.Items.Add(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //*************************************************************************************************************
        private void listViewInterfaces_DoubleClick(object sender, EventArgs e)
        {
            if (listViewInterfaces.SelectedItems.Count == 1)
            {
                var item = listViewInterfaces.SelectedItems[0];
                string active = item.SubItems[1].Text;

                using (RegistryKey interfaceKey = Registry.LocalMachine.OpenSubKey(item.Tag.ToString(), true))
                {
                    try
                    {
                        if (active == "YES")
                        {
                            interfaceKey.DeleteValue("TcpAckFrequency");
                            interfaceKey.DeleteValue("TCPNoDelay");
                            item.SubItems[1].Text = "NO";
                        }
                        else
                        {
                            interfaceKey.SetValue("TcpAckFrequency", 1);
                            interfaceKey.SetValue("TCPNoDelay", 1);
                            item.SubItems[1].Text = "YES";
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        MessageBox.Show("To make changes, run XwNoNagle as Administrator");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
    }
}
