using System;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Npgsql; // zastosowanie pakietu Npgsql
using System.Text;
using System.Security.Cryptography; // Hash
using System.Text.RegularExpressions; // Regex 



namespace Project_window_forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Dodaj domyœlne opcje do ComboBox dla p³ci
            cmbGender.Items.Add("Men");
            cmbGender.Items.Add("Women");

            // Ustaw domyœlny tekst jako sugerowan¹ opcjê
            cmbGender.SelectedIndex = -1; // Ustaw domyœlny indeks na brak zaznaczenia
        }

        private void btnRegister_Click(object sender, EventArgs e) // register button
        {
            // SprawdŸ, czy wszystkie pola s¹ uzupe³nione
            if (AreAllFieldsFilled())
            {
                // SprawdŸ, czy u¿ytkownik wybra³ jedn¹ z opcji p³ci
                if (cmbGender.SelectedIndex != -1)
                {
                    // Pobierz dane z pól formularza
                    string firstName = txtFname.Text;
                    string lastName = txtLname.Text;
                    string address = txtAdd.Text;
                    string gender = cmbGender.Text; // Pobierz tekst z ComboBox dla p³ci
                    string email = txtEmail.Text;
                    string phone = txtPhone.Text;
                    string username = txtUser.Text;
                    string password = txtPass.Text;

                    // Wywo³aj funkcjê rejestracji
                    bool registrationSuccess = RegisterUser(firstName, lastName, address, gender, email, phone, username, password);

                    // Wyœwietl komunikat o sukcesie lub b³êdzie rejestracji
                    if (registrationSuccess)
                    {
                        MessageBox.Show("Rejestracja zakoñczona sukcesem!");

                        this.BackColor = System.Drawing.Color.LightGreen;
                    }
                    else
                    {
                        MessageBox.Show("B³¹d rejestracji. Spróbuj ponownie.");

                        this.BackColor = System.Drawing.Color.MistyRose;
                    }
                }
                else
                {
                    MessageBox.Show("Proszê wybraæ p³ci przed rejestracj¹.");
                }
            }
            else
            {
                // Jeœli nie wszystkie pola s¹ uzupe³nione, równie¿ zmieñ kolor t³a na czerwony
                this.BackColor = System.Drawing.Color.MistyRose;
                MessageBox.Show("Proszê wype³niæ wszystkie pola przed rejestracj¹.");
            }
        }

        private bool AreAllFieldsFilled()
        {
            // SprawdŸ, czy wszystkie pola s¹ uzupe³nione
            return !string.IsNullOrEmpty(txtFname.Text)
                && !string.IsNullOrEmpty(txtLname.Text)
                && !string.IsNullOrEmpty(txtAdd.Text)
                && cmbGender.SelectedIndex != -1 // SprawdŸ, czy u¿ytkownik wybra³ opcjê p³ci
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
                string connectionString = @"Server=localhost;Port=5432;Username=postgres;Password=Password;Database=ProjektCsharp"; // dane do po³¹czenia z baz¹

                // Utwórz po³¹czenie do bazy danych
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    // Otwórz po³¹czenie
                    connection.Open();

                    // Wygeneruj has³o w postaci skrótu
                    string hashedPassword = GenerateHash(password);

                    // SQL Command do wstawienia danych do tabeli u¿ytkowników
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
                        command.Parameters.AddWithValue("@Password", hashedPassword); // U¿yj hasha zamiast czystego has³a

                        // Wykonaj zapytanie SQL
                        command.ExecuteNonQuery();
                    }
                }

                // Rejestracja zakoñczona sukcesem
                return true;
            }
            catch (Exception ex)
            {
                // Obs³uga b³êdu, np. logowanie do pliku lub wyœwietlenie u¿ytkownikowi komunikatu o b³êdzie
                MessageBox.Show($"B³¹d podczas zapisywania danych: {ex.Message}");
                return false;
            }
        }

        // Funkcja do generowania hasha z has³a
        private string GenerateHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Konwertuj has³o na bajty
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

        private void textBox2_TextChanged(object sender, EventArgs e) // last name, powinna sie nazywac zmienna txtLname ale wkrad³ siê b³ad,
                                                                      // a po usuniêciu zmiennej na inn¹ visual usuwa mi ca³y design aplikacji do ustawieñ pocz¹tkowych

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
            // sprawdza, czy "name" zawiera tylko litery (równie¿ polskie)
            return System.Text.RegularExpressions.Regex.IsMatch(name, "^[a-zA-Z¹æê³ñóœŸ¿¥ÆÊ£ÑÓŒ¯]+$");
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
            return !string.IsNullOrEmpty(address); // sprwadzenie czy adres nie jest pusty (wartoœci¹ null)
        }


        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e) // gender
        {
            // pusta metoda
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
            // W tym przyk³adzie u¿y³em doœæ prostego wyra¿enia regularnego
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }


        private void txtPhone_TextChanged(object sender, EventArgs e) // phone
        {


            // SprawdŸ, czy numer telefonu ma dok³adnie 9 cyfr
            if (txtPhone.Text.Length != 9)
            {
                // Jeœli numer telefonu nie ma 9 cyfr to kolor tekstu na czerwony
                txtPhone.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                // Jeœli numer telefonu ma 9 cyfr, przywróæ domyœlny kolor tekstu
                txtPhone.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        private void txtUser_TextChanged(object sender, EventArgs e) // username
        {
            // Obs³uga zmian w polu "username"
            string username = txtUser.Text;

            // SprawdŸ, czy nazwa u¿ytkownika spe³nia kryteria
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
            // W tym przyk³adzie wymagamy, aby nazwa mia³a co najmniej 3 litery i co najmniej 3 cyfry
            string pattern = "^(?=.*[a-zA-Z].*[a-zA-Z].*[a-zA-Z])(?=.*\\d.*\\d.*\\d).*$";
            return Regex.IsMatch(username, pattern);
        }

        private void txtPass_TextChanged(object sender, EventArgs e) // password
        {
            string password = txtPass.Text;

            if (password.Length < 8)
            {
                return; // Przerwij sprawdzanie dalszych warunków, jeœli d³ugoœæ jest niewystarczaj¹ca
            }

            bool isValid = true;

            // co najmniej jedn¹ du¿¹ literê 
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                isValid = false;
            }

            // co najmniej jedn¹ ma³¹ literê
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                isValid = false;
            }

            // co najmniej jedn¹ cyfrê
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
                MessageBox.Show("Has³o nie spe³nia wszystkich wymagañ.");
            }
        }

        private void wyjscieApp_Click(object sender, EventArgs e)
        {
            // Wyœwietl okno dialogowe z zapytaniem o potwierdzenie zamkniêcia
            DialogResult result = MessageBox.Show("Czy na pewno chcesz zamkn¹æ aplikacjê?", "Potwierdzenie zamkniêcia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // SprawdŸ, czy u¿ytkownik potwierdzi³ zamkniêcie
            if (result == DialogResult.Yes)
            {
                // Zamknij aplikacjê
                this.Close();
            }
            // Jeœli u¿ytkownik wybra³ "Nie", pozostaw aplikacjê otwart¹
        }

    }
}
