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
        PurchasesWindow purchasesWindow;
        AddPurchaseWindow addPurchaseWindow;
        AuthorisationWindow authorisationWindow;

        SqlConnection connectionLocalDB;
        SqlDataAdapter adapterLocalDB;
        DataRowView somerow;
        DataTable dataTableLocalDB;

        OleDbConnection oledbConnection;
        OleDbDataAdapter oledbAdapter;
        DataRowView purchaseWindowSomerow;
        DataTable oledbDataTable = new DataTable();

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
        }

        private void ConnectLocalDB(string login, string password)
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = @"(localdb)\MSSQLLocalDB",
                InitialCatalog = "localBase",
                IntegratedSecurity = false,
                Pooling = true
            };

            connectionString.UserID = login;
            connectionString.Password = password;

            connectionLocalDB = new SqlConnection(connectionString.ConnectionString);
            adapterLocalDB = new SqlDataAdapter();
            dataTableLocalDB = new DataTable();

            string selectScript = @"SELECT * FROM localDBTable Order by localDBTable.ID";
            adapterLocalDB.SelectCommand = new SqlCommand(selectScript, connectionLocalDB);

            string updateScript = @"UPDATE localDBTable SET Фамилия = @Фамилия, Имя = @Имя, Отчество = @Отчество, Номер_телефона = @Номер_телефона, Email = @Email WHERE ID = @ID";
            adapterLocalDB.UpdateCommand = new SqlCommand(updateScript, connectionLocalDB);
            adapterLocalDB.UpdateCommand.Parameters.Add("@ID", SqlDbType.Int, 4, "ID").SourceVersion = DataRowVersion.Original;
            adapterLocalDB.UpdateCommand.Parameters.Add("@Фамилия", SqlDbType.NVarChar, 255, "Фамилия");
            adapterLocalDB.UpdateCommand.Parameters.Add("@Имя", SqlDbType.NVarChar, 255, "Имя");
            adapterLocalDB.UpdateCommand.Parameters.Add("@Отчество", SqlDbType.NVarChar, 255, "Отчество");
            adapterLocalDB.UpdateCommand.Parameters.Add("@Номер_телефона", SqlDbType.NVarChar, 255, "Номер_телефона");
            adapterLocalDB.UpdateCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 255, "Email");

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

        private void ConnectMSAccess(string email)
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\LENOVO\source\repos\Skillbox Homework 17\Skillbox Homework 17\Bases\Database1.accdb";
            oledbConnection = new OleDbConnection(connectionString);

            oledbAdapter = new OleDbDataAdapter();
            oledbDataTable = new DataTable();

            OleDbCommandBuilder commandBuilder = new OleDbCommandBuilder(oledbAdapter);

            string selectScript = @"SELECT * FROM Таблица1";
            oledbAdapter.SelectCommand = new OleDbCommand(selectScript, oledbConnection);
            commandBuilder.GetUpdateCommand();

            selectScript = @"SELECT * FROM Таблица1 Where Email = @EMAIL";
            oledbAdapter.SelectCommand = new OleDbCommand(selectScript, oledbConnection);
            oledbAdapter.SelectCommand.Parameters.Add("@EMAIL", email);

            //string updateScript = @"UPDATE Таблица1 SET EMAIL = @EMAIL, CODE = @CODE, PURCHASE_NAME = @PURCHASE_NAME WHERE ID = @ID";
            //oledbAdapter.UpdateCommand = new OleDbCommand(updateScript, oledbConnection);
            //oledbAdapter.UpdateCommand.Parameters.Add("@ID", OleDbType.Integer, 10, "ID").SourceVersion = DataRowVersion.Current;
            //oledbAdapter.UpdateCommand.Parameters.Add("@EMAIL", OleDbType.WChar, 255, "EMAIL");
            //oledbAdapter.UpdateCommand.Parameters.Add("@CODE", OleDbType.WChar, 255, "CODE");
            //oledbAdapter.UpdateCommand.Parameters.Add("@PURCHASE_NAME", OleDbType.WChar, 255, "PURCHASE_NAME");

            oledbAdapter.Fill(oledbDataTable);
            purchasesWindow.gridview.DataContext = oledbDataTable.DefaultView;

            oledbConnection.StateChange += (s,e) => { Notify?.Invoke($"{nameof(oledbConnection)} has changed state to: {(s as OleDbConnection).State}"); };
         
            try
            {
                oledbConnection.Open();
            }
            catch (Exception e)
            {
                Notify?.Invoke($"{e.Message}");
            }
            finally
            {
                oledbConnection.Close();
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
            if (connectionLocalDB.ConnectionString == null)
            {
                MessageBox.Show("Нет подключения к таблице");
            }
            else
            {
                addClientWindow = new AddClientWindow();
                addClientWindow.buttonAdd.Click += new RoutedEventHandler(AddClientWindowButtonAddPressed);
                addClientWindow.Show();
            }
        }

        private void AddClientWindowButtonAddPressed(object sender, RoutedEventArgs e)
        {
            var something = (DataRowView)gridview.SelectedItem;
            ConnectMSAccess(something.Row.Field<string>("Email"));

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
            authorisationWindow = new AuthorisationWindow();
            authorisationWindow.buttonLogin.Click += new RoutedEventHandler(AuthorisationWindowButtonLoginClick);
            authorisationWindow.Show();
        }

        private void AuthorisationWindowButtonLoginClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectLocalDB(authorisationWindow.textboxLogin.Text, authorisationWindow.textboxPassword.Text);
                authorisationWindow.Close();
            }
            catch
            {
                MessageBox.Show("Неверный логин или пароль");
            }
        }

        private void ButtonLogoutClick(object sender, RoutedEventArgs e)
        {
            connectionLocalDB.ConnectionString = null;
            dataTableLocalDB.Clear();
        }

        private void MenuitemShowClick(object sender, RoutedEventArgs e)
        {
            var something = (DataRowView)gridview.SelectedItem;

            if (something == null)
            {
                MessageBox.Show("Выберите пользователя");
            }
            else
            {
                purchasesWindow = new PurchasesWindow();
                ConnectMSAccess(something.Row.Field<string>("Email"));
                purchasesWindow.deleteItem.Click += new RoutedEventHandler(PurchaseWindowDeleteItemClicked);
                purchasesWindow.gridview.CellEditEnding += (s, e) => { PurchaseWindowCellEndEditing(s, e); };
                purchasesWindow.gridview.CurrentCellChanged += (s, e) => { PurchaseWindowCurrentCellChanged(s, e); };
                purchasesWindow.Show();
            }
        }

        private void PurchaseWindowDeleteItemClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var something = (DataRowView)purchasesWindow.gridview.SelectedItem;

                string deleteScript = @"DELETE FROM Таблица1 WHERE ID = @ID";
                oledbAdapter.DeleteCommand = new OleDbCommand(deleteScript, oledbConnection);
                oledbAdapter.DeleteCommand.Parameters.AddWithValue("@ID", something.Row.Field<int>("ID"));

                oledbConnection.Open();
                oledbAdapter.DeleteCommand.ExecuteNonQuery();
                PurchaseWindowUpdateTable(something.Row.Field<string>("Email"));
                oledbConnection.Close();
            }
            catch(Exception error)
            {
                MessageBox.Show($"Выберите клиента {error.Message}");
            }
        }

        private void MenuitemAddClick(object sender, RoutedEventArgs e)
        {
            var something = (DataRowView)gridview.SelectedItem;

            if (something == null)
            {
                MessageBox.Show("Выберите пользователя");
            }
            else
            {
                string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\LENOVO\source\repos\Skillbox Homework 17\Skillbox Homework 17\Bases\Database1.accdb";
                oledbConnection = new OleDbConnection(connectionString);
                oledbAdapter = new OleDbDataAdapter();

                addPurchaseWindow = new AddPurchaseWindow();
                addPurchaseWindow.buttonAddPurchase.Click += new RoutedEventHandler(ButtonAddPurchaseClick);
                addPurchaseWindow.Show();
            }
        }

        private void ButtonAddPurchaseClick(object sender, RoutedEventArgs e)
        {
            var something = (DataRowView)gridview.SelectedItem;
            string insertScript = "INSERT INTO Таблица1(EMAIL,CODE,PURCHASE_NAME) VALUES (@EMAIL, @CODE, @PURCHASE_NAME)";
            oledbAdapter.InsertCommand = new OleDbCommand(insertScript, oledbConnection);
            oledbAdapter.InsertCommand.Parameters.AddWithValue("@EMAIL", something.Row.Field<string>("Email"));
            oledbAdapter.InsertCommand.Parameters.AddWithValue("@CODE", addPurchaseWindow.textboxPurchaseCode.Text);
            oledbAdapter.InsertCommand.Parameters.AddWithValue("@PURCHASE_NAME", addPurchaseWindow.textboxPurchaseName.Text);

            oledbConnection.Open();
            oledbAdapter.InsertCommand.ExecuteNonQuery();
            PurchaseWindowUpdateTable(something.Row.Field<string>("Email"));
            oledbConnection.Close();

            addPurchaseWindow.Close();
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

        private void PurchaseWindowUpdateTable(string email)
        {
            string selectScript = @"SELECT * FROM Таблица1 Where Email = @Email";
            oledbAdapter.SelectCommand = new OleDbCommand(selectScript, oledbConnection);
            oledbAdapter.SelectCommand.Parameters.Add("@Email", email);
            oledbDataTable.Clear();
            oledbAdapter.Fill(oledbDataTable);
        }

        private void PurchaseWindowCellEndEditing(object sender, DataGridCellEditEndingEventArgs e)
        {
            purchaseWindowSomerow = (DataRowView)purchasesWindow.gridview.SelectedItem;

            purchaseWindowSomerow.BeginEdit();            
        }

        private void PurchaseWindowCurrentCellChanged(object sender, EventArgs e)
        {
            if (purchaseWindowSomerow == null)
            {
                return;
            }
            else
            {
                var something = (DataRowView)gridview.SelectedItem;

                purchaseWindowSomerow.EndEdit();
                
                oledbAdapter.Update(oledbDataTable);
            }
        }

       
    }
}
