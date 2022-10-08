using System;
using System.Collections.Generic;
using System.Data;
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
        AddClientWindow addClientWindow;

        SqlConnection connectionLocalDB;
        SqlDataAdapter adapterLocalDB;
        DataRowView somerow;
        DataTable dataTableLocalDB;

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

            ConnectLocalDB();
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

            connectionLocalDB = new SqlConnection(connectionString.ConnectionString);
            adapterLocalDB = new SqlDataAdapter();
            dataTableLocalDB = new DataTable();

            string selectScript = @"SELECT * FROM localDBTable Order by localDBTable.ID";
            adapterLocalDB.SelectCommand = new SqlCommand(selectScript, connectionLocalDB);                      

            adapterLocalDB.Fill(dataTableLocalDB);
            this.gridview.DataContext = dataTableLocalDB.DefaultView;
            
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

        private void ConnectMSAccess()
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\LENOVO\source\repos\Skillbox Homework 17\Skillbox Homework 17\Bases\Database1.accdb";
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
            addClientWindow = new AddClientWindow();
            addClientWindow.buttonAdd.Click += new RoutedEventHandler(AddClientWindowButtonAddPressed);
            addClientWindow.Show();  
        }

        private void AddClientWindowButtonAddPressed(object sender, RoutedEventArgs e)
        {
            string insertScript = $@"INSERT INTO localDBTable (Фамилия, Имя, Отчество, Номер_телефона, Email) VALUES (@Фамилия,@Имя,@Отчество,@Номер_телефона,@Email); Set @ID = @@IDENTITY";
            adapterLocalDB.InsertCommand = new SqlCommand(insertScript, connectionLocalDB);
            adapterLocalDB.InsertCommand.Parameters.Add("@ID", SqlDbType.Int, 4, "ID").Direction = ParameterDirection.Output;

            adapterLocalDB.InsertCommand.Parameters.AddWithValue("@Фамилия", $"{addClientWindow.textboxSecondName.Text}");
            adapterLocalDB.InsertCommand.Parameters.AddWithValue("@Имя", $"{addClientWindow.textboxFirstName.Text}");
            adapterLocalDB.InsertCommand.Parameters.AddWithValue("@Отчество", $"{addClientWindow.textboxPatronymic.Text}");
            adapterLocalDB.InsertCommand.Parameters.AddWithValue("@Номер_телефона", $"{addClientWindow.textboxNumber.Text}");
            adapterLocalDB.InsertCommand.Parameters.AddWithValue("@Email", $"{addClientWindow.textboxEmail.Text}");

            connectionLocalDB.Open();

            adapterLocalDB.InsertCommand.ExecuteNonQuery();
            UpdateGridView();

            connectionLocalDB.Close();

            addClientWindow.Close();
        }

        private void ButtonDeleteClientClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var something = (DataRowView)gridview.SelectedItem;

                string deleteScript = @"DELETE FROM localDBTable WHERE ID = @ID";
                adapterLocalDB.DeleteCommand = new SqlCommand(deleteScript, connectionLocalDB);
                adapterLocalDB.DeleteCommand.Parameters.AddWithValue("@ID", something.Row.Field<int>("ID"));

                connectionLocalDB.Open();
                adapterLocalDB.DeleteCommand.ExecuteNonQuery();
                UpdateGridView();
                connectionLocalDB.Close();
            }
            catch(Exception error)
            {
                MessageBox.Show($"Выберите клиента {error.Message}");
            }
        }

        private void ButtonLoginClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonLogoutClick(object sender, RoutedEventArgs e)
        {

        }

        private void MenuitemShowClick(object sender, RoutedEventArgs e)
        {

        }

        private void MenuitemAddClick(object sender, RoutedEventArgs e)
        {

        }

        private void MenuitemDeleteClick(object sender, RoutedEventArgs e)
        {

        }

        private void MenuitemUpdateClick(object sender, RoutedEventArgs e)
        {

        }

        private void CellEndEditing(object sender, DataGridCellEditEndingEventArgs e)
        {
            somerow = (DataRowView)gridview.SelectedItem;

            somerow.BeginEdit();
        }

        private void CurrentCellChanged(object sender, EventArgs e)
        {
            if (somerow == null)
            {
                return;
            }
            else
            {
                somerow.EndEdit();
                adapterLocalDB.Update(dataTableLocalDB);
            }
        }

        private void UpdateGridView()
        {
            dataTableLocalDB.Clear();
            adapterLocalDB.Fill(dataTableLocalDB);
        }
    }
}
