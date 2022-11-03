using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Scaffolding;
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
using System.Reflection;
using System.Diagnostics;
using Skillbox_Homework_17.Displays;

namespace Skillbox_Homework_17
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Journal journalWindow;
        AddClientWindow addClientWindow;
        PurchasesWindow purchasesWindow;
        AddPurchaseWindow addPurchaseWindow;
        AuthorisationWindow authorisationWindow;

        DataRowView somerow;
        DataTable dataTableLocalDB;

        DataRowView purchaseWindowSomerow;
        DataTable oledbDataTable = new DataTable();

        public delegate void InformationHandler(string message);
        public static event InformationHandler? Notify;

        ObservableCollection<string> notifications = new ObservableCollection<string>();

        
        public MainWindow()
        {
            InitializeComponent();

            var localBase = new localBaseContext();

            localBase.LocalDbtables.Load();

            var myCollection = localBase.ClassLocalDbs.Local.ToObservableCollection();

            foreach (var item in myCollection)
            {
                 Debug.WriteLine($"{item.Id}");
            }

            Preset();
        }

        private void Preset()
        {
            var stringCon = @"Data Source=C:\Users\LENOVO\source\repos\Skillbox Homework 17\Skillbox Homework 17\Bases\Database1.accdb;";
            var qwert = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=localBase;";
        }

        private void ConnectLocalDB(string login, string password)
        {
            
        }

        private void ConnectMSAccess(string email)
        {
            
        }

        private void AddJournalString(string message)
        {
            
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

        private void AddClientWindowButtonAddPressed(object sender, RoutedEventArgs e)
        {
            
        }

        private void ButtonDeleteClientClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void ButtonLoginClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void AuthorisationWindowButtonLoginClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void ButtonLogoutClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuitemShowClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void PurchaseWindowDeleteItemClicked(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuitemAddClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void ButtonAddPurchaseClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void CellEndEditing(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }

        private void CurrentCellChanged(object sender, EventArgs e)
        {
            
        }

        private void UpdateGridView()
        {
            
        }

        private void PurchaseWindowUpdateTable(string email)
        {
            
        }

        private void PurchaseWindowCellEndEditing(object sender, DataGridCellEditEndingEventArgs e)
        {
                        
        }

        private void PurchaseWindowCurrentCellChanged(object sender, EventArgs e)
        {
            
        }

        public static DataTable ListToDataTable<T>(List<T> list, string _tableName)
        {
            DataTable dt = new DataTable(_tableName);

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, Nullable.GetUnderlyingType(info.PropertyType) ?? info.PropertyType));
            }
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(T).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
