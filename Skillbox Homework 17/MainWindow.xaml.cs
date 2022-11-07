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
using Skillbox_Homework_17.Access;
using Skillbox_Homework_17.SQL_Server;

namespace Skillbox_Homework_17
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AddClientWindow addClientWindow;
        PurchasesWindow purchasesWindow;
        AddPurchaseWindow addPurchaseWindow;
        AuthorisationWindow authorisationWindow;

        DataRowView selectedRowMainTable;
        DataTable dataTableLocalDB;

        DataRowView selectedRowAccessTable;
        DataTable accessDataTable = new DataTable();

        localBaseContext sqlContext;
        ModelContext accessContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectLocalDB(string login, string password)
        {
            if (login == "")
            {
                login = "q";
            }

            if (password == "")
            {
                password = "1";
            }

            var options = new DbContextOptionsBuilder<localBaseContext>();
            options.UseSqlServer($"Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = localBase;User ID={login};Password={password};Integrated security=false;");
            sqlContext = new localBaseContext(options.Options);

            var selectedCollection = sqlContext.LocalDbtable.OrderBy(e => e.Id);

            dataTableLocalDB = ListToDataTable<LocalDbtable>(selectedCollection.ToList<LocalDbtable>(), "");

            gridview.ItemsSource = dataTableLocalDB.DefaultView;
        }

        private void ConnectMSAccess()
        {
            accessContext = new ModelContext();
        }

        private void ButtonAddClientClick(object sender, RoutedEventArgs e)
        {
            addClientWindow = new AddClientWindow();
            addClientWindow.buttonAdd.Click += new RoutedEventHandler(AddClientWindowButtonAddPressed);
            addClientWindow.Show();
        }

        private void AddClientWindowButtonAddPressed(object sender, RoutedEventArgs e)
        {
            try
            {
                sqlContext.Add(new LocalDbtable() { Фамилия = $"{addClientWindow.textboxSecondName.Text}", Имя = $"{addClientWindow.textboxFirstName.Text}", Отчество = $"{addClientWindow.textboxPatronymic.Text}", НомерТелефона = $"{addClientWindow.textboxNumber.Text}", Email = $"{addClientWindow.textboxEmail.Text}" });
                sqlContext.SaveChanges();

                UpdateGridView();
            }
            catch (Exception)
            {
                MessageBox.Show("Нет подключения к базе данных");
            }

            addClientWindow.Close();
        }

        private void ButtonDeleteClientClick(object sender, RoutedEventArgs e)
        {
            if (gridview.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента");
            }
            else
            {
                selectedRowMainTable = gridview.SelectedItem as DataRowView;
                var item = sqlContext.LocalDbtable.Where(e => e.Id == selectedRowMainTable.Row.Field<int>("ID")).First();
                sqlContext.LocalDbtable.Remove(item);
                sqlContext.SaveChanges();

                UpdateGridView();
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
                ConnectMSAccess();
            }
            catch (Exception)
            {
                MessageBox.Show("Неправильный логин или пароль");
            }
 
            authorisationWindow.Close();
        }

        private void ButtonLogoutClick(object sender, RoutedEventArgs e)
        {
            dataTableLocalDB.Clear();
        }

        private void MenuitemShowClick(object sender, RoutedEventArgs e)
        {
            if (gridview.SelectedItem != null)
            {
                purchasesWindow = new PurchasesWindow();
                purchasesWindow.deleteItem.Click += new RoutedEventHandler(PurchaseWindowDeleteItemClicked);

                var selectedItem = (DataRowView)gridview.SelectedItem;
                string selectedEmail = selectedItem.Row.Field<string>("Email");
                accessDataTable = ListToDataTable<Таблица1>(accessContext.Таблица1.Where(e => e.Email == selectedEmail).ToList(), "");

                purchasesWindow.gridview.ItemsSource = accessDataTable.DefaultView;

                purchasesWindow.gridview.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(PurchaseWindowCellEndEditing);
                purchasesWindow.gridview.CurrentCellChanged += new EventHandler<EventArgs>(PurchaseWindowCurrentCellChanged);

                purchasesWindow.Show();
            }
            else
            {
                MessageBox.Show("Выберите клиента");
            }
        }

        private void PurchaseWindowDeleteItemClicked(object sender, RoutedEventArgs e)
        {
            if (purchasesWindow.gridview.SelectedItem != null)
            {
                selectedRowAccessTable = (DataRowView)purchasesWindow.gridview.SelectedItem;
                var item = accessContext.Таблица1.Where(e => e.Id == selectedRowAccessTable.Row.Field<int>("ID")).FirstOrDefault();
                accessContext.Remove(item);
                accessContext.SaveChanges();

                PurchaseWindowUpdateTable((gridview.SelectedItem as DataRowView).Row.Field<string>("Email"));
            }
            else
            {
                MessageBox.Show("Выберите товар");
            }
        }

        private void MenuitemAddClick(object sender, RoutedEventArgs e)
        {
            if (gridview.SelectedItem != null)
            {
                addPurchaseWindow = new AddPurchaseWindow();
                addPurchaseWindow.buttonAddPurchase.Click += new RoutedEventHandler(ButtonAddPurchaseClick);
                addPurchaseWindow.Show();
            }
            else
            {
                MessageBox.Show("Выберите клиента");
            }
        }

        private void ButtonAddPurchaseClick(object sender, RoutedEventArgs e)
        {
            var something = (DataRowView)gridview.SelectedItem;
            accessContext.Add(new Таблица1() { Email = something.Row.Field<string>("Email"), Code = addPurchaseWindow.textboxPurchaseCode.Text, PurchaseName = addPurchaseWindow.textboxPurchaseName.Text});
            accessContext.SaveChanges();

            addPurchaseWindow.Close();
        }

        private void CellEndEditing(object sender, DataGridCellEditEndingEventArgs e)
        {
            selectedRowMainTable = (DataRowView)gridview.SelectedItem;
            selectedRowMainTable.BeginEdit();
        }

        private void CurrentCellChanged(object sender, EventArgs e)
        {
            if (selectedRowMainTable == null)
            {
                return;
            }
            else
            {
                selectedRowMainTable.EndEdit();

                var something = selectedRowMainTable;

                var entity = sqlContext.LocalDbtable.Where(e => e.Id == something.Row.Field<int>("ID")).FirstOrDefault();

                entity.Имя = something.Row.Field<string>("Имя");
                entity.Фамилия = something.Row.Field<string>("Фамилия");
                entity.Отчество = something.Row.Field<string>("Отчество");
                entity.НомерТелефона = something.Row.Field<string>("НомерТелефона");
                entity.Email = something.Row.Field<string>("Email");

                sqlContext.SaveChanges();
            }
        }

        private void UpdateGridView()
        {
            dataTableLocalDB.Clear();
            var selectedCollection = sqlContext.LocalDbtable.OrderBy(e => e.Id);
            dataTableLocalDB = ListToDataTable<LocalDbtable>(selectedCollection.ToList<LocalDbtable>(), "");
            gridview.ItemsSource = dataTableLocalDB.DefaultView;
        }

        private void PurchaseWindowUpdateTable(string email)
        {
            accessDataTable.Clear();
            accessDataTable = ListToDataTable<Таблица1>(accessContext.Таблица1.Where(e => e.Email == email).ToList(), "");

            purchasesWindow.gridview.ItemsSource = accessDataTable.DefaultView;
        }

        private void PurchaseWindowCellEndEditing(object sender, DataGridCellEditEndingEventArgs e)
        {
            selectedRowAccessTable = (DataRowView)purchasesWindow.gridview.SelectedItem;
            selectedRowAccessTable.BeginEdit();
        }

        private void PurchaseWindowCurrentCellChanged(object sender, EventArgs e)
        {
            if (selectedRowAccessTable == null)
            {
                return;
            }
            else
            {
                selectedRowAccessTable.EndEdit();

                var something = selectedRowAccessTable;

                var entity = accessContext.Таблица1.Where(e => e.Id == something.Row.Field<int>("ID")).FirstOrDefault();

                entity.Code = something.Row.Field<string>("Code");
                entity.PurchaseName = something.Row.Field<string>("PurchaseName");

                accessContext.SaveChanges();
            }
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
