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
            // Dodaj domy�lne opcje do ComboBox dla p�ci
            cmbGender.Items.Add("Men");
            cmbGender.Items.Add("Women");

            // Ustaw domy�lny tekst jako sugerowan� opcj�
            cmbGender.SelectedIndex = -1; // Ustaw domy�lny indeks na brak zaznaczenia
        }

        private void btnRegister_Click(object sender, EventArgs e) // register button
        {
            // Sprawd�, czy wszystkie pola s� uzupe�nione
            if (AreAllFieldsFilled())
            {
                // Sprawd�, czy u�ytkownik wybra� jedn� z opcji p�ci
                if (cmbGender.SelectedIndex != -1)
                {
                    // Pobierz dane z p�l formularza
                    string firstName = txtFname.Text;
                    string lastName = txtLname.Text;
                    string address = txtAdd.Text;
                    string gender = cmbGender.Text; // Pobierz tekst z ComboBox dla p�ci
                    string email = txtEmail.Text;
                    string phone = txtPhone.Text;
                    string username = txtUser.Text;
                    string password = txtPass.Text;

                    // Wywo�aj funkcj� rejestracji
                    bool registrationSuccess = RegisterUser(firstName, lastName, address, gender, email, phone, username, password);

                    // Wy�wietl komunikat o sukcesie lub b��dzie rejestracji
                    if (registrationSuccess)
                    {
                        MessageBox.Show("Rejestracja zako�czona sukcesem!");

                        this.BackColor = System.Drawing.Color.LightGreen;
                    }
                    else
                    {
                        MessageBox.Show("B��d rejestracji. Spr�buj ponownie.");

                        this.BackColor = System.Drawing.Color.MistyRose;
                    }
                }
                else
                {
                    MessageBox.Show("Prosz� wybra� p�ci przed rejestracj�.");
                }
            }
            else
            {
                // Je�li nie wszystkie pola s� uzupe�nione, r�wnie� zmie� kolor t�a na czerwony
                this.BackColor = System.Drawing.Color.MistyRose;
                MessageBox.Show("Prosz� wype�ni� wszystkie pola przed rejestracj�.");
            }
        }

        private bool AreAllFieldsFilled()
        {
            // Sprawd�, czy wszystkie pola s� uzupe�nione
            return !string.IsNullOrEmpty(txtFname.Text)
                && !string.IsNullOrEmpty(txtLname.Text)
                && !string.IsNullOrEmpty(txtAdd.Text)
                && cmbGender.SelectedIndex != -1 // Sprawd�, czy u�ytkownik wybra� opcj� p�ci
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
                string connectionString = @"Server=localhost;Port=5432;Username=postgres;Password=Password;Database=ProjektCsharp"; // dane do po��czenia z baz�

                // Utw�rz po��czenie do bazy danych
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    // Otw�rz po��czenie
                    connection.Open();

                    // Wygeneruj has�o w postaci skr�tu
                    string hashedPassword = GenerateHash(password);

                    // SQL Command do wstawienia danych do tabeli u�ytkownik�w
                    string sql = "INSERT INTO users (first_name, last_name, address, gender, email, phone, username, password) " +
                                 "VALUES (@FirstName, @LastName, @Address, @Gender, @Email, @Phone, @Username, @Password)";

                    // Utw�rz i skonfiguruj obiekt SqlCommand
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
                        command.Parameters.AddWithValue("@Password", hashedPassword); // U�yj hasha zamiast czystego has�a

                        // Wykonaj zapytanie SQL
                        command.ExecuteNonQuery();
                    }
                }

                // Rejestracja zako�czona sukcesem
                return true;
            }
            catch (Exception ex)
            {
                // Obs�uga b��du, np. logowanie do pliku lub wy�wietlenie u�ytkownikowi komunikatu o b��dzie
                MessageBox.Show($"B��d podczas zapisywania danych: {ex.Message}");
                return false;
            }
        }

        // Funkcja do generowania hasha z has�a
        private string GenerateHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Konwertuj has�o na bajty
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

        private void textBox2_TextChanged(object sender, EventArgs e) // last name, powinna sie nazywac zmienna txtLname ale wkrad� si� b�ad,
                                                                      // a po usuni�ciu zmiennej na inn� visual usuwa mi ca�y design aplikacji do ustawie� pocz�tkowych

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
            // sprawdza, czy "name" zawiera tylko litery (r�wnie� polskie)
            return System.Text.RegularExpressions.Regex.IsMatch(name, "^[a-zA-Z����󜟿��ʣ�ӌ��]+$");
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
            return !string.IsNullOrEmpty(address); // sprwadzenie czy adres nie jest pusty (warto�ci� null)
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
            // W tym przyk�adzie u�y�em do�� prostego wyra�enia regularnego
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }


        private void txtPhone_TextChanged(object sender, EventArgs e) // phone
        {


            // Sprawd�, czy numer telefonu ma dok�adnie 9 cyfr
            if (txtPhone.Text.Length != 9)
            {
                // Je�li numer telefonu nie ma 9 cyfr to kolor tekstu na czerwony
                txtPhone.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                // Je�li numer telefonu ma 9 cyfr, przywr�� domy�lny kolor tekstu
                txtPhone.ForeColor = System.Drawing.SystemColors.ControlText;
            }
        }

        private void txtUser_TextChanged(object sender, EventArgs e) // username
        {
            // Obs�uga zmian w polu "username"
            string username = txtUser.Text;

            // Sprawd�, czy nazwa u�ytkownika spe�nia kryteria
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
            // W tym przyk�adzie wymagamy, aby nazwa mia�a co najmniej 3 litery i co najmniej 3 cyfry
            string pattern = "^(?=.*[a-zA-Z].*[a-zA-Z].*[a-zA-Z])(?=.*\\d.*\\d.*\\d).*$";
            return Regex.IsMatch(username, pattern);
        }

        private void txtPass_TextChanged(object sender, EventArgs e) // password
        {
            string password = txtPass.Text;

            if (password.Length < 8)
            {
                return; // Przerwij sprawdzanie dalszych warunk�w, je�li d�ugo�� jest niewystarczaj�ca
            }

            bool isValid = true;

            // co najmniej jedn� du�� liter� 
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                isValid = false;
            }

            // co najmniej jedn� ma�� liter�
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                isValid = false;
            }

            // co najmniej jedn� cyfr�
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
                MessageBox.Show("Has�o nie spe�nia wszystkich wymaga�.");
            }
        }

        private void wyjscieApp_Click(object sender, EventArgs e)
        {
            // Wy�wietl okno dialogowe z zapytaniem o potwierdzenie zamkni�cia
            DialogResult result = MessageBox.Show("Czy na pewno chcesz zamkn�� aplikacj�?", "Potwierdzenie zamkni�cia", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Sprawd�, czy u�ytkownik potwierdzi� zamkni�cie
            if (result == DialogResult.Yes)
            {
                // Zamknij aplikacj�
                this.Close();
            }
            // Je�li u�ytkownik wybra� "Nie", pozostaw aplikacj� otwart�
        }

    }
}
