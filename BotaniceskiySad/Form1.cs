using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.OleDb;

namespace BotaniceskiySad
{
    public partial class Form1 : Form
    {
        private static string ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\BotaniceskiySad.mdb";
        public static OleDbConnection Connection = new OleDbConnection(ConnectionString);

        public Form1()
        {
            InitializeComponent();

            try
            {
                Connection.Open();
            }
            catch (Exception ConnectionException)
            {
                MessageBox.Show(ConnectionException.Message, "Не удалось подключиться к базе данных");
                Application.Exit();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!comboBox1.Items.Contains(comboBox1.Text))
            {
                Rastenie Plant = new Rastenie();
                Plant.RastenieName = comboBox1.Text;
                Plant.Tsarstvo = label7.Text;
                Plant.Klass = label8.Text;
                Plant.SemeistvoName = label15.Text;
                Plant.RodName = label16.Text;
                Plant.VidName = textBox6.Text;

                Plant.NaznachenieRst = new Naznachenie(richTextBox1.Text);
                Plant.TerritoriaRst = new TerritoriaProizrostania(textBox9.Text, textBox10.Text, textBox11.Text);

                Plant.TerritoriaRst.InsertIntoDb(Connection);
                Plant.InsertIntoDb(Connection, Plant.TerritoriaRst.GetRowId(Connection), Plant.NaznachenieRst.GetNaznachenie());

                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(Rastenie.GetAllFromDb(Connection).ToArray());
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (!listBox1.Items.Contains(textBox12.Text) && comboBox1.Text != null && comboBox1.Text != "")
            {
                Sovmestimost.InsertIntoDb(Connection, comboBox1.Text, textBox12.Text);

                listBox1.Items.Add(textBox12.Text);
            }
                
            textBox12.Text = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Rastenie.GetAllFromDb(Connection).ToArray());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Rastenie Plant = new Rastenie();
            Plant.RastenieName = comboBox1.Text;

            Plant.DeleteFromDb(Connection);

            comboBox1.Items.Clear();
            comboBox1.Text = null;

            comboBox1.Items.AddRange(Rastenie.GetAllFromDb(Connection).ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            if (comboBox1.Text != null && comboBox1.Text != "")
                listBox1.Items.AddRange(Sovmestimost.GetAllFromDb(Connection, comboBox1.Text).ToArray());
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            try
            {
                string CommandString = "SELECT [Вид], [Территория произрастания], [Назначение] FROM Растение WHERE [Наименование] = @Name";
                OleDbCommand Command = new OleDbCommand(CommandString, Connection);

                Command.Parameters.Add("@Name", OleDbType.VarChar).Value = comboBox1.Text;

                OleDbDataReader Reader = Command.ExecuteReader();

                try
                {
                    Reader.Read();
                }
                catch (Exception ReadException)
                {
                    MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
                }

                int Vid = Convert.ToInt32(Reader["Вид"].ToString());
                int Terr = Convert.ToInt32(Reader["Территория произрастания"].ToString());
                richTextBox1.Text = Reader["Назначение"].ToString();
                Reader.Close();

                Command.Parameters.Clear();

                CommandString = "SELECT [Материк], [Природная зона], [Страна] FROM [Территория произрастания] WHERE [ID территории произрастания] = @Terr";
                Command.CommandText = CommandString;

                Command.Parameters.Add("@Terr", OleDbType.Integer).Value = Terr;

                Reader = Command.ExecuteReader();

                try
                {
                    Reader.Read();
                }
                catch (Exception ReadException)
                {
                    MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
                }

                textBox9.Text = Reader["Материк"].ToString();
                textBox10.Text = Reader["Природная зона"].ToString();
                textBox11.Text = Reader["Страна"].ToString();
                Reader.Close();

                Command.Parameters.Clear();

                CommandString = "SELECT [Род], [Название] FROM Вид WHERE [ID вида] = @Vid";
                Command.CommandText = CommandString;

                Command.Parameters.Add("@Vid", OleDbType.Integer).Value = Vid;

                Reader = Command.ExecuteReader();

                try
                {
                    Reader.Read();
                }
                catch (Exception ReadException)
                {
                    MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
                }

                int Id = Convert.ToInt32(Reader["Род"].ToString());
                textBox6.Text = Reader["Название"].ToString();
                Reader.Close();

                Command.Parameters.Clear();

                CommandString = "SELECT [Семейство], [Название] FROM Род WHERE [ID рода] = @Rod ";
                Command.CommandText = CommandString;

                Command.Parameters.Add("@Rod", OleDbType.Integer).Value = Id;

                Reader = Command.ExecuteReader();

                try
                {
                    Reader.Read();
                }
                catch (Exception ReadException)
                {
                    MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
                }

                Id = Convert.ToInt32(Reader["Семейство"].ToString());
                label16.Text = Reader["Название"].ToString();
                Reader.Close();

                Command.Parameters.Clear();

                CommandString = "SELECT [Царство], [Класс], [Название] FROM Семейство WHERE [ID семейства] = @Semeistvo";
                Command.CommandText = CommandString;

                Command.Parameters.Add("@Semeistvo", OleDbType.Integer).Value = Id;

                Reader = Command.ExecuteReader();

                try
                {
                    Reader.Read();
                }
                catch (Exception ReadException)
                {
                    MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
                }

                label7.Text = Reader["Царство"].ToString();
                label8.Text = Reader["Класс"].ToString();
                label15.Text = Reader["Название"].ToString();
                Reader.Close();

                Command.Parameters.Clear();
            }
            catch { }
        }
    }

    public class Semeistvo
    {
        public string Tsarstvo { get; set; }
        public string Klass { get; set; }
        public string SemeistvoName { get; set; }
    }

    public class Rod : Semeistvo
    {
        public string RodName { get; set; }
    }

    public class Vid : Rod
    {
        public string VidName { get; set; }
    }

    public class Rastenie : Vid
    {
        public string RastenieName { get; set; }
        public Naznachenie NaznachenieRst { get; set; }
        public TerritoriaProizrostania TerritoriaRst { get; set; }
        public Sovmestimost SovmestimostRst { get; set; }

        public void InsertIntoDb(OleDbConnection Connection, int IdTerr, string Nazn)
        {
            string CommandString = "SELECT [ID вида] FROM [Вид] WHERE ([Название] = @Name)";
            OleDbCommand Command = new OleDbCommand(CommandString, Connection);

            Command.Parameters.Add("@Name", OleDbType.VarChar).Value = this.VidName;

            OleDbDataReader Reader = Command.ExecuteReader();

            try
            {
                Reader.Read();
            }
            catch (Exception ReadException)
            {
                MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
            }

            int Id = Convert.ToInt32(Reader["ID вида"].ToString());
            Reader.Close();

            Command.Parameters.Clear();

            CommandString = "INSERT INTO [Растение] ([Наименование], [Вид], [Территория произрастания], [Назначение]) VALUES (@Name, @Vid, @Terr, @Nazn)";
            Command.CommandText = CommandString;
            Command.Parameters.Add("@Name", OleDbType.VarChar).Value = this.RastenieName;
            Command.Parameters.Add("@Vid", OleDbType.Integer).Value = Id;
            Command.Parameters.Add("@Terr", OleDbType.Integer).Value = IdTerr;
            Command.Parameters.Add("@Nazn", OleDbType.VarChar).Value = Nazn;

            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception InsertException)
            {
                MessageBox.Show(InsertException.Message, "Ошибка добавления данных");
            }
        }

        public static List<string> GetAllFromDb(OleDbConnection Connection)
        {
            List<string> Plants = new List<string>();

            string CommandString = "SELECT [Наименование] FROM [Растение]";
            OleDbCommand Command = new OleDbCommand(CommandString, Connection);
            OleDbDataReader Reader = Command.ExecuteReader();

            while (Reader.Read())
                Plants.Add(Reader["Наименование"].ToString());

            return Plants;
        }

        public void DeleteFromDb(OleDbConnection Connection)
        {
            string CommandString = "DELETE FROM [Растение] WHERE [Наименование] = @Name";
            OleDbCommand Command = new OleDbCommand(CommandString, Connection);

            Command.Parameters.Add("@Name", OleDbType.VarChar).Value = this.RastenieName;

            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception DeleteException)
            {
                MessageBox.Show(DeleteException.Message, "Ошибка удаления данных");
            }
        }
    }

    public class Naznachenie
    {
        public Naznachenie(string Nz)
        {
            this.NaznachenieRastenia = Nz;
        }

        public string NaznachenieRastenia { get; set; }

        public string GetNaznachenie()
        {
            return this.NaznachenieRastenia;
        }
    }

    public class TerritoriaProizrostania
    {
        public TerritoriaProizrostania(string Mat, string Zone, string Country)
        {
            this.Materik = Mat;
            this.PrirodnayaZona = Zone;
            this.Strana = Country;
        }

        public string Materik { get; set; }
        public string PrirodnayaZona { get; set; }
        public string Strana { get; set; }

        public void InsertIntoDb(OleDbConnection Connection)
        {
            string CommandString = "INSERT INTO [Территория произрастания] ([Материк], [Природная зона], [Страна]) VALUES (@Mat, @Zone, @Country)";
            OleDbCommand Command = new OleDbCommand(CommandString, Connection);

            Command.Parameters.Add("@Mat", OleDbType.VarChar).Value = this.Materik;
            Command.Parameters.Add("@Zone", OleDbType.VarChar).Value = this.PrirodnayaZona;
            Command.Parameters.Add("@Country", OleDbType.VarChar).Value = this.Strana;

            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception InsertException)
            {
                MessageBox.Show(InsertException.Message, "Ошибка добавления данных");
            }
        }

        public int GetRowId(OleDbConnection Connection)
        {
            string CommandString = "SELECT [ID территории произрастания] FROM [Территория произрастания] WHERE ([Материк] = @Mat AND [Природная зона] = @Zone AND [Страна] = @Country)";
            OleDbCommand Command = new OleDbCommand(CommandString, Connection);

            Command.Parameters.Add("@Mat", OleDbType.VarChar).Value = this.Materik;
            Command.Parameters.Add("@Zone", OleDbType.VarChar).Value = this.PrirodnayaZona;
            Command.Parameters.Add("@Country", OleDbType.VarChar).Value = this.Strana;

            OleDbDataReader Reader = Command.ExecuteReader();

            try
            {
                Reader.Read();
            }
            catch (Exception ReadException)
            {
                MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
            }

            return Convert.ToInt32(Reader["ID территории произрастания"].ToString());
        }
    }

    public class Sovmestimost
    {
        public static void InsertIntoDb(OleDbConnection Connection, string Name, string SovmestimoeName)
        {
            string CommandString = "SELECT [ID растения] FROM [Растение] WHERE ([Наименование] = @Name)";
            OleDbCommand Command = new OleDbCommand(CommandString, Connection);

            Command.Parameters.Add("@Name", OleDbType.VarChar).Value = Name;

            OleDbDataReader Reader = Command.ExecuteReader();

            try
            {
                Reader.Read();
            }
            catch (Exception ReadException)
            {
                MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
            }

            int Id = Convert.ToInt32(Reader["ID растения"].ToString());
            Reader.Close();

            Command.Parameters.Clear();

            CommandString = "INSERT INTO [Совместимость] ([Растение], [Совместимое растение]) VALUES (@Plant, @Sovmest)";
            Command.CommandText = CommandString;
            Command.Parameters.Add("@Plant", OleDbType.Integer).Value = Id;
            Command.Parameters.Add("@Sovmest", OleDbType.VarChar).Value = SovmestimoeName;


            try
            {
                Command.ExecuteNonQuery();
            }
            catch (Exception InsertException)
            {
                MessageBox.Show(InsertException.Message, "Ошибка добавления данных");
            }
        }

        public static List<string> GetAllFromDb(OleDbConnection Connection, string Name)
        {
            List<string> Sovmestimost = new List<string>();

            string CommandString = "SELECT [ID растения] FROM [Растение] WHERE ([Наименование] = @Name)";
            OleDbCommand Command = new OleDbCommand(CommandString, Connection);

            Command.Parameters.Add("@Name", OleDbType.VarChar).Value = Name;

            OleDbDataReader Reader = Command.ExecuteReader();

            try
            {
                Reader.Read();
            }
            catch (Exception ReadException)
            {
                MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
            }

            int Id = Convert.ToInt32(Reader["ID растения"].ToString());
            Reader.Close();

            Command.Parameters.Clear();

            CommandString = "SELECT [Совместимое растение] FROM [Совместимость] WHERE [Растение] = @Plant";
            Command.CommandText = CommandString;
            Command.Parameters.Add("@Plant", OleDbType.Integer).Value = Id;
            Reader = Command.ExecuteReader();

            try
            {
                while (Reader.Read())
                    Sovmestimost.Add(Reader["Совместимое растение"].ToString());
            }
            catch (Exception ReadException)
            {
                MessageBox.Show(ReadException.Message, "Ошибка чтения данных");
            }

            return Sovmestimost;
        }
    }
}
