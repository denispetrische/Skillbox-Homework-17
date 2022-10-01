using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Skillbox_Homework_17
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Preset();
        }

        private void Preset()
        {
            ConnectLocalDB();
            ConnectMSAccess();
        }

        private void ConnectLocalDB()
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "localBase",
                IntegratedSecurity = true,
                UserID = "admin",
                Password = "admin",
                Pooling = true
            };

            SqlConnection connectionLocalDB = new SqlConnection(connectionString.ConnectionString);

            connectionLocalDB.StateChange += (s, e) => { MessageBox.Show($"{nameof(connectionLocalDB)} has changed state to: {(s as SqlConnection).State}"); };

            try
            {
                connectionLocalDB.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}");
            }
            finally
            {
                connectionLocalDB.Close();
            }
        }

        private void ConnectMSAccess()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\LENOVO\source\repos\Skillbox Homework 17\Skillbox Homework 17\DatabaseAccess.mdb";

            OleDbConnection connectionMSAccess = new OleDbConnection(connectionString);

            connectionMSAccess.StateChange += (s,e) => { MessageBox.Show($"{nameof(connectionMSAccess)} has changed state to: {(s as OleDbConnection).State}"); };
         
            try
            {
                connectionMSAccess.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}");
            }
            finally
            {
                connectionMSAccess.Close();
            }
        }
    }
}
