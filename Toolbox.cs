using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox
{
    internal class Toolbox
    {
        /// <summary>
        /// Intended to use with MySql databases
        /// Used for WinForms by default
        /// </summary>

        // Fill combobox from a stored procedure
        // Stored procedure can include parameters
        public static void FillCombobox(ComboBox comboBox, string connectionString, string query, string column, string key, params string[] parameter)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    List<string> lstParams = new List<string>();

                    // Check if the procedure has parameters
                    string parameterChecker = $"SELECT PARAMETER_NAME FROM INFORMATION_SCHEMA.PARAMETERS WHERE SPECIFIC_NAME = '{query}'";
                    using (MySqlCommand cmdParam = new MySqlCommand(parameterChecker, connection))
                    {
                        using (MySqlDataReader reader = cmdParam.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string paramName = reader.GetString(0);
                                lstParams.Add(paramName);
                            }
                        }
                    }

                    // Add all the parameters specified in the argument array
                    if (lstParams.Count > 0)
                    {
                        foreach (string item in lstParams)
                        {
                            for (int i = 0; i <= parameter.Length - 1; i++)
                            {
                                cmd.Parameters.AddWithValue($"@{item}", parameter[i]);
                            }
                        }
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        comboBox.DisplayMember = column;
                        comboBox.ValueMember = key;
                        comboBox.DataSource = dt;
                    }
                }
            }
        }

        // Fill datagridviews and doublebuffer for speed
        public static void FillDatagridview(DataGridView datagridview, string connectionString, string query)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        datagridview.DataSource = dt;
                    }
                }
            }

            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, datagridview, new object[] { true });
        }

        // Formats datetimepickers
        public static void DateFormatter(params Control[] ctr)
        {
            foreach (Control c in ctr)
            {
                var dtp = c as DateTimePicker;
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = "yyyy-MM-dd"; // Your desired format
                dtp.Value = DateTime.Now;
            }
        }

        // Clear all controls
        // Controls can be inside a groupbox
        public static void ClearControls(Control.ControlCollection ctrCol)
        {
            foreach (Control c in ctrCol)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    c.Text = string.Empty;
                }
                else if (c.GetType() == typeof(ComboBox))
                {
                    c.Text = string.Empty;
                }
                else if (c.GetType() == typeof(DateTimePicker))
                {
                    DateFormatter(c);
                }
                else if (c.GetType() == typeof(CheckBox))
                {
                    (c as CheckBox).Checked = false;
                }
            }
        }
    }
}
