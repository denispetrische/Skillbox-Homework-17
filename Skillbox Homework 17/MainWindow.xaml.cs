using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Collections.ObjectModel;
using Skillbox_Homework_17.Displays;

namespace Skillbox_Homework_17
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Journal journalWindow;

        Thread threadLocalDB = new Thread(ConnectLocalDB);
        Thread threadMSAccess = new Thread(ConnectMSAccess);

        public delegate void InformationHandler(string message);
        public static event InformationHandler? Notify;

        ObservableCollection<string> notifications = new ObservableCollection<string>();
        
        public MainWindow()
        {
            InitializeComponent();

            Preset();
        }

        private void Preset()
        {
            Notify += AddJournalString;

            threadLocalDB.Start();
            threadMSAccess.Start();
        }

        private static void ConnectLocalDB()
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

            connectionLocalDB.StateChange += (s, e) => { Notify?.Invoke($"{nameof(connectionLocalDB)} has changed state to: {(s as SqlConnection).State}"); };

            try
            {
                connectionLocalDB.Open();
            }
            catch (Exception e)
            {
                Notify?.Invoke($"{e.Message}");
            }
            finally
            {
                connectionLocalDB.Close();
            }
        }

        private static void ConnectMSAccess()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\LENOVO\source\repos\Skillbox Homework 17\Skillbox Homework 17\DatabaseAccess.mdb";

            OleDbConnection connectionMSAccess = new OleDbConnection(connectionString);

            connectionMSAccess.StateChange += (s,e) => { Notify?.Invoke($"{nameof(connectionMSAccess)} has changed state to: {(s as OleDbConnection).State}"); };
         
            try
            {
                connectionMSAccess.Open();
            }
            catch (Exception e)
            {
                Notify?.Invoke($"{e.Message}");
            }
            finally
            {
                connectionMSAccess.Close();
            }
        }

        private void AddJournalString(string message)
        {
            notifications.Add(message);
        }

        private void ButtonJournalClick(object sender, RoutedEventArgs e)
        {
            journalWindow = new Journal();
            journalWindow.listview.ItemsSource = notifications;
            journalWindow.Show();
        }

        private void ButtonAddClientClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteClientClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonUpdateClientClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonLoginClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
