using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.IO;

namespace _4._3
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        internal void Load_data(string a)
        {
            list.Children.Clear();
            using (SqlConnection connection = new SqlConnection(Connection.String))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($@"Select * FROM Client where (LastName like '{Search.Text}%' or 
                                                                                  Email like '{Search.Text}%' or 
                                                                                  Phone like '{Search.Text}%') and 
                                                                                  GenderCode like '{(Gender.SelectedIndex == 0 ? "" : ((ComboBoxItem)Gender.SelectedItem).Content)}%' " + a, connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Users clients = new Users();
                        clients.id.Content = reader[0];
                        clients.fio.Content = $"{reader[2]} {reader[1]} {reader[3]}";
                        clients.birthday.Content = reader[4];
                        clients.reg_date.Content = reader[5];
                        clients.email.Content = reader[6];
                        clients.phone.Content = reader[7];
                        clients.gender.Content = reader[8];
                        clients.photo.Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\" + reader[9]));
                        clients.main = this;
                        list.Children.Add(clients);
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Load_data("");
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Load_data("");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Load_data("");
        }

        private void Birth_Click(object sender, RoutedEventArgs e)
        {
            Load_data($"and Birthday like '%-{(DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month : DateTime.Now.Month.ToString())}-%'");
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            Edit_add open = new Edit_add();
            open.MainWindow = this;
            open.Show();
        }
    }
}
