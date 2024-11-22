using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;


namespace L2_1
{
    public partial class Form1 : Form
    {
        MySqlConnection cnt;

        public Form1()
        {
            InitializeComponent();
            combobox_fill();


            string DB = "server=192.168.232.132; port=3306; database=POWERPLANTS; user=root; password=12345678; charset=utf8mb4;";
            
                try
            {
                cnt = new MySqlConnection(DB);
                cnt.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }


            string req = "SELECT * FROM Fuel WHERE FuelName = N'Природній газ';";
            MySqlCommand command = new MySqlCommand(req, cnt);
            MySqlDataReader reader = command.ExecuteReader();

            // Перевірка наявності першого запису
            if (!reader.Read())
            {
                reader.Close();
                req = "INSERT INTO Fuel (FuelName, FuelDescription) " +
                      "VALUES (N'Природній газ', N'Природній газ, який використовується для роботи газових турбін на ТЕС');";
                MySqlCommand cmd = new MySqlCommand(req, cnt);

                // Використовуємо ExecuteNonQuery для вставки даних
                cmd.ExecuteNonQuery();
            }
            else
            {
                reader.Close();
            }

            // Перевірка наявності другого запису
            req = "SELECT * FROM Fuel WHERE FuelName = N'Паливо';";
            MySqlCommand command2 = new MySqlCommand(req, cnt);
            MySqlDataReader reader2 = command2.ExecuteReader();

            if (!reader2.Read())
            {
                reader2.Close();
                req = "INSERT INTO Fuel (FuelName, FuelDescription) " +
                      "VALUES (N'Паливо', N'викопне паливо, яке використовується для виробництва електроенергії на ТЕС');";
                MySqlCommand cmd3 = new MySqlCommand(req, cnt);

                // Використовуємо ExecuteNonQuery для вставки даних
                cmd3.ExecuteNonQuery();
            }
            else
            {
                reader2.Close();
            }

            // Закриваємо з'єднання
            cnt.Close();
            
        }
        private void combobox_fill()
        {
            string DB = "server=192.168.232.132; port=3306; database=POWERPLANTS; user=root; password=12345678; charset=utf8mb4;";
            MySqlConnection cnt = null;
            try
            {
                cnt = new MySqlConnection(DB);
                cnt.Open();

                // SQL запит для отримання даних
                string req = "select ID from Fuel;";
                MySqlCommand command = new MySqlCommand(req, cnt);

                MySqlDataReader reader = command.ExecuteReader();

                // Чистимо елементи перед додаванням
                comboBox1.Items.Clear();

                // Заповнюємо ComboBox
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["ID"].ToString());
                }

                // Закриваємо рідер і з'єднання
                reader.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Помилка");
            }
            finally
            {
                if (cnt != null)
                {
                    cnt.Close();
                }
            }
        }


        private void AutoSizeColumns(DataGridView dgv)
        {
            for (int i = 0; i < dgv.Columns.Count; i++)
                dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }
        public static DataTable GetSQLTable(string Query)
        {
            string DB = "server=192.168.232.132; port=3306; database=POWERPLANTS; user=root; password=12345678; charset=utf8mb4;";
            using (MySqlConnection Conn = new MySqlConnection(DB))
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                Conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(Query, DB);
                da.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
        }

        
        public static void MODIFY(string Query)
        {
            string DB = "server=192.168.232.132; port=3306; database=POWERPLANTS; user=root; password=12345678; charset=utf8mb4;";
            using (MySqlConnection Conn = new MySqlConnection(DB))
            {
                Conn.Open();
                MySqlCommand Comm = new MySqlCommand(Query, Conn);
                MessageBox.Show(Comm.ExecuteNonQuery().ToString());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            AutoSizeColumns(DGV1);

            if (radioButton1.Checked)
            {
                if (checkBox1.Checked && checkBox2.Checked)
                {
                    string tb = textBox5.Text;
                    string tb1 = comboBox1.Text;

                    // Перевірка на правильність ID
                    if (!PassFormatID(tb))
                    {
                        MessageBox.Show("Невірний формат ID!");
                        return;
                    }

                    DataTable T = GetSQLTable("select * FROM Plants WHERE ID = " + tb + " AND FuelName_ID = " + tb1);
                    DGV1.DataSource = T;
                    AutoSizeColumns(DGV1);
                }
                else if (checkBox1.Checked)
                {
                    string tb = comboBox1.Text;
                    if (PassFormatID(tb))
                    {
                        DataTable T = GetSQLTable($"SELECT * FROM Plants WHERE FuelName_ID = {tb}");
                        DGV1.DataSource = T;
                        AutoSizeColumns(DGV1);
                    }
                    else
                    {
                        MessageBox.Show("Невірний формат ID!");
                    }
                }
                else if (checkBox2.Checked)
                {
                    string tb = textBox5.Text;
                    if (PassFormatID(tb))
                    {
                        DataTable T = GetSQLTable($"SELECT * FROM Plants WHERE ID = {tb}");
                        DGV1.DataSource = T;
                        AutoSizeColumns(DGV1);
                    }
                    else
                    {
                        MessageBox.Show("Невірний формат ID!");
                    }
                }
                else
                {
                    DataTable T = GetSQLTable("SELECT * FROM Plants INNER JOIN Fuel ON Plants.FuelName_ID = Fuel.ID");
                    DGV1.DataSource = T;
                    AutoSizeColumns(DGV1);
                }
            }
            else if (radioButton2.Checked)
            {
                string PlantName = textBox4.Text;
                string LocationPlant = textBox3.Text;
                string FuelName_ID = comboBox1.Text;
                string Capacity = textBox1.Text;
                string YearCommissioned = textBox2.Text;

                // Перевірка форматів
                if (!IsValidName(PlantName) || !IsValidName(LocationPlant))
                {
                    MessageBox.Show("Ім'я та місце розташування повинні містити тільки букви.", "Помилка формату");
                    return;
                }

                if (!IsValidCapacity(Capacity))
                {
                    MessageBox.Show("Кількість потужності повинна бути дійсним числом.", "Помилка формату");
                    return;
                }

                if (!IsValidYear(YearCommissioned))
                {
                    MessageBox.Show("Рік введення повинен бути між 1900 і " + DateTime.Now.Year + ".", "Помилка формату");
                    return;
                }

                int affectedRows = ExecuteNonQuery($"INSERT INTO Plants (PlantName, LocationPlant, FuelName_ID, Capacity, YearCommissioned) VALUES (N'{PlantName}', N'{LocationPlant}', {FuelName_ID}, {Capacity}, {YearCommissioned})");
                MessageBox.Show($"{affectedRows} записів успішно додано.");
            }
            else if (radioButton3.Checked)
            {
                string PlantName = textBox4.Text;
                string LocationPlant = textBox3.Text;
                string FuelName_ID = comboBox1.Text;
                string Capacity = textBox1.Text;
                string YearCommissioned = textBox2.Text;
                string ID = textBox5.Text;

                // Перевірка форматів
                if (!IsValidName(PlantName) || !IsValidName(LocationPlant))
                {
                    MessageBox.Show("Ім'я та місце розташування повинні містити тільки букви.", "Помилка формату");
                    return;
                }

                if (!IsValidCapacity(Capacity))
                {
                    MessageBox.Show("Кількість потужності повинна бути дійсним числом.", "Помилка формату");
                    return;
                }

                if (!IsValidYear(YearCommissioned))
                {
                    MessageBox.Show("Рік введення повинен бути між 1900 і " + DateTime.Now.Year + ".", "Помилка формату");
                    return;
                }

                if (!PassFormatID(ID))
                {
                    MessageBox.Show("Невірний формат ID!");
                    return;
                }

                 int affectedRows = ExecuteNonQuery($"UPDATE Plants SET PlantName=N'{PlantName}', LocationPlant=N'{LocationPlant}', FuelName_ID='{FuelName_ID}', Capacity='{Capacity}', YearCommissioned={YearCommissioned} WHERE ID ={ID};");
                MessageBox.Show($"{affectedRows} записів успішно оновлено.");
            }
            else if (radioButton4.Checked)
            {
                string ID = textBox5.Text;
                if (PassFormatID(ID))
                {
                    //MODIFY("DELETE FROM Plants WHERE ID =" + ID + ";");
                    int affectedRows = ExecuteNonQuery($"DELETE FROM Plants WHERE ID ={ID};");
                    MessageBox.Show($"{affectedRows} записів успішно вилучено.");
                }
                else
                {
                    MessageBox.Show("Невірний формат ID для видалення!");
                }
            }

        }
        private int ExecuteNonQuery(string query)
        {
            using (MySqlConnection connection = new MySqlConnection("server=192.168.232.132; port=3306; database=POWERPLANTS; user=root; password=12345678; charset=utf8mb4;"))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }
        public bool IsValidName(string name)
        {
            Regex regex = new Regex("^[A-Za-zА-Яа-яЁёІіЇїЄє]{1,50}$");
            return regex.IsMatch(name);
        }

        public bool IsValidCapacity(string capacity)
        {
            Regex regex = new Regex("^[0-9]+$"); // Припустимо, Capacity - це дійсне число
            return regex.IsMatch(capacity);
        }

        public bool IsValidYear(string year)
        {
            if (int.TryParse(year, out int parsedYear))
            {
                return parsedYear >= 1900 && parsedYear <= DateTime.Now.Year;
            }
            return false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DGV1.DataSource = GetSQLTable();
            //DGV1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //DGV1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //DGV1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //DGV1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //DGV1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //DGV1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //DGV1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //DGV1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

            AutoSizeColumns(DGV1);


            this.Hide();

            Form2 childForm = new Form2();
            childForm.FormClosed += (s, args) =>
            {
                MessageBox.Show("Updating DataGridView on Form1"); // Налагодження
                DGV1.DataSource = GetSQLTable();
                this.Show();
                combobox_fill();
            };
            childForm.Show();

        }
        public static DataTable GetSQLTable()
        {
            string DB = "server=192.168.232.132; port=3306; database=POWERPLANTS; user=root; password=12345678; charset=utf8mb4;";
            string Query = "SELECT * FROM Plants AS P INNER JOIN Fuel AS F ON P.FuelName_ID = F.ID";
            using (MySqlConnection Conn = new MySqlConnection(DB))
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                Conn.Open();
                MySqlDataAdapter da = new MySqlDataAdapter(Query, DB);
                da.Fill(ds);
                dt = ds.Tables[0];
                return dt;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string DB = "server=192.168.232.132; port=3306; database=POWERPLANTS; user=root; password=12345678; charset=utf8mb4;";
            string Query = "INSERT INTO Plants (PlantName, LocationPlant, FuelName_ID, Capacity, YearCommissioned) VALUES (N'ТЕС-3', N'Одеса Україна', 2, 3000, 2020)";
            using (MySqlConnection Conn = new MySqlConnection(DB))
            {
                Conn.Open();
                MySqlCommand Comm = new MySqlCommand(Query, Conn);
                MessageBox.Show(Comm.ExecuteNonQuery().ToString());

            }
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public bool PassFormatID(string p)
        {
            Regex R = new Regex("[0-9]+");
            Match M = R.Match(p);
            return M.Success;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
