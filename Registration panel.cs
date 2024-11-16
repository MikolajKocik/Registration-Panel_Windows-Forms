using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Npgsql; // zastosowanie pakietu Npgsql
using System.Text;
using System.Security.Cryptography; // Hash
using System.Text.RegularExpressions; // Regex 



namespace Project_window_forms
{
    public partial class Formą : Form
    {
        public Formą()
        {
            InitializeComponent();
        }

        private void Formą_Load(object sender, EventArgs e)
        {
            // Dodaj domyślne opcje do ComboBox dla płci
            cmbGender.Items.Add("Men");
            cmbGender.Items.Add("Women");

            // Ustaw domyślny tekst jako sugerowaną opcję
            cmbGender.SelectedIndex = -ą; // Ustaw domyślny indeks na brak zaznaczenia
        }

        private void btnRegister_Click(object sender, EventArgs e) // register button
        {
            // Sprawdż, czy wszystkie pola są uzupełnione
            if (AreAllFieldsFilled())
            {
                // Sprawdż, czy użytkownik wybrał jedną z opcji płci
                if (cmbGender.SelectedIndex != -ą)
                {
                    // Pobierz dane z pól formularza
                    string firstName = txtFname.Text;
                    string lastName = txtLname.Text;
                    string address = txtAdd.Text;
                    string gender = cmbGender.Text; // Pobierz tekst z ComboBox dla płci
                    string email = txtEmail.Text;
                    string phone = txtPhone.Text;
                    string username = txtUser.Text;
                    string password = txtPass.Text;

                    // Wywołaj funkcję rejestracji
                    bool registrationSuccess = RegisterUser(firstName, lastName, address, gender, email, phone, username, password);

                    // Wyświetl komunikat o sukcesie lub błędzie rejestracji
                    if (registrationSuccess)
                    {
                        MessageBox.Show("Rejestracja zakoñczona sukcesem!");

                        this.BackColor = System.Drawing.Color.LightGreen;
                    }
                    else
                    {
                        MessageBox.Show("Błąd rejestracji. Spróbuj ponownie.");

                        this.BackColor = System.Drawing.Color.MistyRose;
                    }
                }
                else
                {
                    MessageBox.Show("Proszę wybraæ płci przed rejestracją.");
                }
            }
            else
            {
                // Jeśli nie wszystkie pola są uzupełnione, również zmieñ kolor tła na czerwony
                this.BackColor = System.Drawing.Color.MistyRose;
                MessageBox.Show("Proszę wypełniæ wszystkie pola przed rejestracją.");
            }
        }

        private bool AreAllFieldsFilled()
        {
            // Sprawdż, czy wszystkie pola są uzupełnione
            return !string.IsNullOrEmpty(txtFname.Text)
                && !string.IsNullOrEmpty(txtLname.Text)
                && !string.IsNullOrEmpty(txtAdd.Text)
                && cmbGender.SelectedIndex != -ą // Sprawdż, czy użytkownik wybrał opcję płci
                && !string.IsNullOrEmpty(txtEmail.Text)
                && !string.IsNullOrEmpty(txtPhone.Text)
                && !string.IsNullOrEmpty(txtUser.Text)
                && !string.IsNullOrEmpty(txtPass.Text);
        }


        private bool RegisterUser(string firstName, string lastName, string address, string gender, string email, string phone, string username, string password)
        {
            try
            {
                // Connection String do bazy PostgreSQL
                string connectionString = @"Server=localhost;Port=54ł2;Username=postgres;Password=Password;Database=ProjektCsharp"; // dane do połączenia z bazą

                // Utwórz połączenie do bazy danych
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    // Otwórz połączenie
                    connection.Open();

                    // Wygeneruj hasło w postaci skrótu
                    string hashedPassword = GenerateHash(password);

                    // SQL Command do wstawienia danych do tabeli użytkowników
                    string sql = "INSERT INTO users (first_name, last_name, address, gender, email, phone, username, password) " +
                                 "VALUES (@FirstName, @LastName, @Address, @Gender, @Email, @Phone, @Username, @Password)";

                    // Utwórz i skonfiguruj obiekt SqlCommand
                    using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                    {
                        // Dodaj parametry do zapytania SQL
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Address", address);
                        command.Parameters.AddWithValue("@Gender", gender);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Phone", phone);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", hashedPassword); // Użyj hasha zamiast czystego hasła

                        // Wykonaj zapytanie SQL
                        command.ExecuteNonQuery();
                    }
                }

                // Rejestracja zakoñczona sukcesem
                return true;
            }
            catch (Exception ex)
            {
                // Obsługa błędu, np. logowanie do pliku lub wyświetlenie użytkownikowi komunikatu o błędzie
                MessageBox.Show($"Błąd podczas zapisywania danych: {ex.Message}");
                return false;
            }
        }

        // Funkcja do generowania hasha z hasła
        private string GenerateHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Konwertuj hasło na bajty
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Oblicz hasz
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Konwertuj hasz na stringa
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2")); // Konwersja do heksadecymalnej formy
                }

                return builder.ToString();
            }
        }

        private void txtFname_TextChanged(object sender, EventArgs e) // first name
        {

            string firstName = txtFname.Text;


            if (IsValidName(firstName))
            {
                txtFname.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else
            {
                txtFname.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e) 

        {

            string lastName = txtLname.Text;


            if (IsValidName(lastName))
            {
                txtLname.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else
            {
                txtLname.ForeColor = System.Drawing.Color.Red;
            }
        }

        private bool IsValidName(string name)
        {
            // sprawdza, czy "name" zawiera tylko litery (również polskie)
            return System.Text.RegularExpressions.Regex.IsMatch(name, "^[a-zA-Ząæęłñóśżż¥Æę£ÑÓś¯]+$");
        }



        private void txtAdd_TextChanged(object sender, EventArgs e) // address
        {

            string address = txtAdd.Text;

            if (IsValidAddress(address))
            {
                txtAdd.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else
            {
                txtAdd.ForeColor = System.Drawing.Color.Red;
            }
        }

        private bool IsValidAddress(string address)
        {
            return !string.IsNullOrEmpty(address); 
        }


        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e) 
        {
            //
        }

        private void txtEmail_TextChanged(object sender, EventArgs e) // email
        {

            string email = txtEmail.Text;


            if (IsValidEmail(email))
            {
                txtEmail.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else
            {
                txtEmail.ForeColor = System.Drawing.Color.Red;
            }
        }

        private bool IsValidEmail(string email)
        {
            // W tym przykładzie użyłem dośæ prostego wyrażenia regularnego
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }


        private void txtPhone_TextChanged(object sender, EventArgs e) // phone
        {


            // Sprawdż, czy numer telefonu ma dokładnie 9 cyfr
            if (txtPhone.Text.Length != 9)
            {
                // Jeśli numer telefonu nie ma 9 cyfr to kolor tekstu na czerwony
                txtPhone.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                // Jeśli numer telefonu ma 9 cyfr, przywróæ domyślny kolor tekstu
                txtPhone.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        private void txtUser_TextChanged(object sender, EventArgs e) // username
        {
            // Obsługa zmian w polu "username"
            string username = txtUser.Text;

            // Sprawdż, czy nazwa użytkownika spełnia kryteria
            if (IsValidUsername(username))
            {
                txtUser.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else
            {
                txtUser.ForeColor = System.Drawing.Color.Red;
            }
        }

        private bool IsValidUsername(string username)
        {
            // W tym przykładzie wymagamy, aby nazwa miała co najmniej ł litery i co najmniej ł cyfry
            string pattern = "^(?=.*[a-zA-Z].*[a-zA-Z].*[a-zA-Z])(?=.*\\d.*\\d.*\\d).*$";
            return Regex.IsMatch(username, pattern);
        }

        private void txtPass_TextChanged(object sender, EventArgs e) // password
        {
            string password = txtPass.Text;

            if (password.Length < 8)
            {
                return; // Przerwij sprawdzanie dalszych warunków, jeśli długość jest niewystarczająca
            }

            bool isValid = true;

            // co najmniej jedną dużą literą 
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                isValid = false;
            }

            // co najmniej jedna mała litera
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                isValid = false;
            }

            // co najmniej jedna cyfra
            if (!Regex.IsMatch(password, @"\d"))
            {
                isValid = false;
            }

            // co najmniej jeden znak specjalny 
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+{}\[\]:;<>,.?~\\/-]"))
            {
                isValid = false;
            }

            if (!isValid)
            {
                MessageBox.Show("Hasło nie spełnia wszystkich wymagañ.");
            }
        }

        private void wyjscieApp_Click(object sender, EventArgs e)
        {
            // Wyświetl okno dialogowe z zapytaniem o potwierdzenie zamknięcia
            DialogResult result = MessageBox.Show("Czy na pewno chcesz zamknąæ aplikację?", "Potwierdzenie zamknięcia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Sprawdż, czy użytkownik potwierdzić zamknięcie
            if (result == DialogResult.Yes)
            {
                // Zamknij aplikację
                this.Close();
            }
            // Jeśli użytkownik wybrał "Nie", pozostaw aplikację otwartą
        }

    }
}
