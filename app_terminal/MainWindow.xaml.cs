﻿using app_terminal.Model;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace app_terminal
{
    public partial class MainWindow : Window
    {
        private ViewpointContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new ViewpointContext();
            LoadTours();
            LoadTicketTypes();
        }

        private void LoadTours()
        {
            var tours = _context.Tours.ToList();
            ComboBoxTours.ItemsSource = tours;
        }

        private void LoadTicketTypes()
        {
            var ticketTypes = _context.TicketTypes.ToList();
            ComboBoxTicketType.ItemsSource = ticketTypes;
        }

        private void ComboBoxTours_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ComboBoxTours.SelectedItem is Tour selectedTour)
            {
                int availableSeats = (int)(selectedTour.MaxCapacity - selectedTour.BookedTickets);
                TextBlockAvailableSeats.Text = $"Свободные места: {availableSeats}";
            }
        }

        private void ButtonPurchase_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBoxTours.SelectedItem is Tour selectedTour &&
                !string.IsNullOrEmpty(TextBoxName.Text) &&
                !string.IsNullOrEmpty(TextBoxSurname.Text) &&
                !string.IsNullOrEmpty(TextBoxPatronymic.Text) &&
                !string.IsNullOrEmpty(TextBoxCountry.Text) &&
                !string.IsNullOrEmpty(TextBoxEmail.Text))
            {
                if (!IsValidEmail(TextBoxEmail.Text))
                {
                    MessageBox.Show("Введите корректный адрес электронной почты!");
                    return;
                }

                int? selectedTicketTypeId = null;

                if (ComboBoxTicketType.SelectedItem is ComboBoxItem selectedItem)
                {
                    string ticketTypeName = selectedItem.Content.ToString();
                    selectedTicketTypeId = _context.TicketTypes
                                                    .FirstOrDefault(tt => tt.TicketType1 == ticketTypeName)?
                                                    .IdTicketType;
                }

                var ticket = new Ticket
                {
                    IdTours = selectedTour.IdTours,
                    FirstName = TextBoxName.Text,
                    LastName = TextBoxSurname.Text,
                    Patronymic = TextBoxPatronymic.Text,
                    Country = TextBoxCountry.Text,
                    Email = TextBoxEmail.Text,
                    IdTicketType = selectedTicketTypeId,
                    PurchaseDate = DateTime.Now,
                };

                _context.Tickets.Add(ticket);
                selectedTour.BookedTickets++;
                _context.SaveChanges();

                MessageBox.Show("Билет успешно приобретен!");
            }
            else
            {
                MessageBox.Show("Введите все данные!");
            }
        }

        private void TextBoxEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string email = TextBoxEmail.Text;
            if (IsValidEmail(email))
            {
                TextBoxEmail.Background = Brushes.White;  
            }
            else
            {
                TextBoxEmail.Background = Brushes.Pink; 
            }
        }

        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private string GenerateQRCode(string ticketNumber)
        {
            return $"QRCode_{ticketNumber}";
        }
    }
}