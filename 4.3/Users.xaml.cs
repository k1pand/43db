using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;

namespace _4._3
{
    public partial class Users : UserControl
    {
        public MainWindow main;
        public Users()
        {
            InitializeComponent();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(Connection.String))
            {
                if (MessageBox.Show("Удалить?","Удаление",MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                   
                    connection.Open();
                    SqlCommand command = new SqlCommand($"Delete from Client where ID = {id.Content}", connection);
                    command.ExecuteNonQuery();
                    main.Load_data("");
                }
                else
                {

                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {

            Edit_add edit = new Edit_add();
            string[] a = fio.Content.ToString().Split();
            edit.id.Content = id.Content;
            edit.lastname.Text = a[0];
            edit.firstname.Text = a[1];
            edit.patronymic.Text = a[2];
            edit.email.Text = email.Content.ToString();
            edit.phone.Text = phone.Content.ToString();
            edit.photo.Source = photo.Source;
            edit.gender.SelectedIndex = gender.Content.ToString() == "м" ? 0 : 1;
            edit.birthday.SelectedDate= Convert.ToDateTime(birthday.Content);
            edit.MainWindow = main;
            edit.ShowDialog();
        }
    }
}
