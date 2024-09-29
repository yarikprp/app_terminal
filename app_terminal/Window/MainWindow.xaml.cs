using app_terminal.Model;
using app_terminal.Windows;
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
                string ticketTypeName = null;

                if (ComboBoxTicketType.SelectedItem is ComboBoxItem selectedItem)
                {
                    ticketTypeName = selectedItem.Content.ToString();
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

                ShowTicketWindow(selectedTour, ticketTypeName);

                ClearInputFields();
            }
            else
            {
                MessageBox.Show("Введите все данные!");
            }
        }

        private void ShowTicketWindow(Tour selectedTour, string ticketTypeName)
        {
            string ticketInfo = $"Тур: {selectedTour}, " +
                                $"Тип: {ticketTypeName}, " +
                                $"Имя: {TextBoxName.Text}, " +
                                $"Фамилия: {TextBoxSurname.Text}, " +
                                $"Отчество: {TextBoxPatronymic.Text}, " +
                                $"Страна: {TextBoxCountry.Text}, " +
                                $"Почта: {TextBoxEmail.Text}";

            TicketWindow ticketWindow = new TicketWindow(ticketInfo);
            ticketWindow.Show();
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

        private void ClearInputFields()
        {
            TextBoxName.Text = string.Empty;
            TextBoxSurname.Text = string.Empty;
            TextBoxPatronymic.Text = string.Empty;
            TextBoxCountry.Text = string.Empty;
            TextBoxEmail.Text = string.Empty;
            ComboBoxTicketType.SelectedItem = null;
        }
    }
}