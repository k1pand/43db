using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media;

namespace _4._3
{
    public partial class Edit_add : Window
    {
        public Edit_add()
        {
            InitializeComponent();
        }
        public MainWindow MainWindow;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (lastname.Text.Length > 5 && firstname.Text.Length > 3 && email.Text.Length > 5 && email.Text.IndexOf('@') > 3 && phone.Text.Length > 7)
            {
                if (id.Content.ToString() != "id")
                {
                    using (SqlConnection connection = new SqlConnection(Connection.String))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand($@"UPDATE [dbo].[Client]
                                                   SET [FirstName] = '{firstname.Text}'
                                                      ,[LastName] = '{lastname.Text}'
                                                      ,[Patronymic] = '{patronymic.Text}'
                                                      ,[Birthday] = '{birthday.SelectedDate}'
                                                      ,[Email] = '{email.Text}'
                                                      ,[Phone] = '{phone.Text}'
                                                      ,[GenderCode] = '{((ComboBoxItem)gender.SelectedItem).Content}'
                                                      {(file_Name != "" ? @",[PhotoPath] = 'Клиенты\" + file_Name + "'" : "")}
                                                   WHERE ID = {id.Content}", connection);

                        try
                        {
                            command.ExecuteNonQuery();
                            MessageBox.Show("Сохранено");
                            MainWindow.Load_data("");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            MessageBox.Show("Заполните все поля!");
                        }
                    }
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(Connection.String))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand($@"INSERT INTO [dbo].[Client]
                                       ([FirstName]
                                       ,[LastName]
                                       ,[Patronymic]
                                       ,[Birthday]
                                       ,[RegistrationDate]
                                       ,[Email]
                                       ,[Phone]
                                       ,[GenderCode]
                                       ,[PhotoPath])
                                 VALUES
                                       ('{firstname.Text}'
                                       ,'{lastname.Text}'
                                       ,'{patronymic.Text}'
                                       ,'{birthday.SelectedDate}'
                                       ,'{DateTime.Now}'
                                       ,'{email.Text}'
                                       ,'{phone.Text}'
                                       , '{((ComboBoxItem)gender.SelectedItem).Content}'
                                        {(file_Name != "" ? @",'Клиенты\" + file_Name + "'" : ",'Клиенты\\ava.png'")})", connection);

                        try
                        {
                            command.ExecuteNonQuery();
                            MessageBox.Show("Добавлен");
                            MainWindow.Load_data("");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            MessageBox.Show("Заполните все поля!");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Проверьте все поля");
            }
        }
        private string file_Name ="";
        private void photo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog saveFile = new OpenFileDialog();
            saveFile.ShowDialog();
            if (saveFile.FileName != "")
            {
                photo.Source = new BitmapImage(new Uri(saveFile.FileName));
                File.Copy(saveFile.FileName, @".\Клиенты\" + saveFile.SafeFileName, true);
                file_Name = saveFile.SafeFileName;
            }
        }

        private void lastname_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = (Char.IsDigit(e.Text, 0));
        }

        private void phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(Char.IsDigit(e.Text, 0)) && e.Text != "(" && e.Text != ")" && e.Text != "+" && e.Text != "-")
                e.Handled = true;
            Console.WriteLine(e.Text);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Tag_panel_Loaded(object sender, RoutedEventArgs e)
        {
            Tag_panel.Children.Clear();
            if (id.Content.ToString() != "ID")
                using (SqlConnection connection = new SqlConnection(Connection.String))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($@"SELECT Tag.Title, Tag.Color
                                                           FROM Tag INNER JOIN TagOfClient ON Tag.ID = TagOfClient.TagID 
                                                           WHERE TagOfClient.ClientID = {id.Content}", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Label label = new Label();
                            label.FontSize = 14;
                            label.FontWeight = FontWeights.Bold;
                            label.Content = reader[0].ToString();
                            switch (reader[1].ToString().Trim())
                            {
                                case "Green":
                                    label.Foreground = Brushes.Green;
                                    break;
                                case "Blue":
                                    label.Foreground = Brushes.Blue;
                                    break;
                                case "Red":
                                    label.Foreground = Brushes.Red;
                                    break;
                            }
                            label.MouseLeftButtonDown += Tag_panel_MouseLeftButtonDown;
                            Tag_panel.Children.Add(label);
                        }
                    }
                }
        }

        private void Tag_panel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(Connection.String))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($@"DELETE FROM [dbo].[TagOfClient]
                                                       WHERE ClientID = {id.Content} and TagID = (SELECT [ID] FROM [4.3.DB].[dbo].[Tag] 
                                                       WHERE Title = '{(sender as Label).Content}')", connection);
                command.ExecuteNonQuery();
                Tag_panel_Loaded(sender, e);
            }
        }

        private void Add_tag_Click(object sender, RoutedEventArgs e)
        {
            if (id.Content.ToString() != "ID")
            {
                using (SqlConnection connection = new SqlConnection(Connection.String))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($@"INSERT INTO [dbo].[TagOfClient] ([ClientID],[TagID]) 
                                                           VALUES ('{id.Content}', '{tagof.SelectedIndex+1}')", connection);
                    try
                    {
                        command.ExecuteNonQuery();
                        Tag_panel_Loaded(sender, e);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Тег существует/не выбран или пользователя нет в БД");
                    }
                }
            }
        }
    }
}
