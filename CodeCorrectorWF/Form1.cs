using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CodeCorrectorWF
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            change_up_button.Enabled = false;
            update_button.Enabled = false;
            save_as_button.Enabled = false;
            // Create an unbound DataGridView by declaring a column count.
            dataGridView1.ColumnCount = 2;
            dataGridView1.ColumnHeadersVisible = true;

            // Set the column header style.
            DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
            columnHeaderStyle.Font = new Font("Verdana", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

            // Set the column header names.
            dataGridView1.Columns[0].Name = "T old";
            dataGridView1.Columns[1].Name = "T new";
            //������� ����������
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            //������������ �� ������
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.AllowUserToAddRows = false;

            textBox1.ReadOnly = true;
        }

        //����� ����� � ���������� ��� ������
        public class UP
        {
            public string Code = ""; // ��� ��������� ���������
            public string filename = ""; // �������� ����� ��������� ���������

            public string Instruments = ""; // ������ ������������ � ������� ���������� � ��������� � �������� ����
            public string UniqInstruments = ""; // ������ ���������� ������������

            public int InstrumentsCount = 0; // ���������� ���������� ������������

            public int[] Instruments_array = new int[3000]; // ������ ���� ������������ � ��������������
            public int[] Uniq_Instruments_array = new int[25]; // ������ ������ ���������� ������������
            public int[] Uniq_Instruments_array_changed = new int[25]; // ������ ������ ���������� ������������ � ����������� ��������

            public int[] replaced_instruments = new int[200]; // ������� ������������, ������� ��� ���� �������� � ������ ��
            public int replaced_instrument_index;// ������, ����������� �����������

            //�����, ������������� ���� ��
            public void File_Analysis()
            {
                //������ ����������
                /*Array.LastIndexOf(Instruments_array, 1);
                Array.Copy(Uniq_Instruments_array, Instruments_array, 25);
                Array.Sort(Uniq_Instruments_array);*/

                //��� ��������� ������ ���������� ��������
                Instruments = "";
                UniqInstruments = "";
                InstrumentsCount = 0;
                int current_number = 0; // ����� �������� ����������� � �����
                int i = 0; // �������� �����
                int x = 0; // �������� ������� ������������

                int Code_Length = Code.Length;
                Code = '1' + Code + "12345678911111";// ������� �������������� ����� ��� ���������� ������ string-�������

                //����� ������������
                for (i = 0; i < Code_Length; i++)
                {
                    if (Code[i] == 'T' & (Convert.ToInt32(Code[i + 1]) >= 48) & (Convert.ToInt32(Code[i + 1]) <= 57) &
                        (((Convert.ToInt32(Code[i + 2]) >= 48) & (Convert.ToInt32(Code[i + 2])) <= 57) || Code[i + 2] == ' ' || Code[i + 2] == 'M') &

                        ((Code.Substring(i + 2, 7)).Contains("M6") || (Code.Substring(i + 2, 7)).Contains("M06") || (Code.Substring(i + 2, 7)).Contains("M 6")
                        || (Code.Substring(i + 2, 7)).Contains("M 06") || (Code.Substring(i + 2, 7)).Contains("M006") || (Code.Substring(i + 2, 7)).Contains("M 006")
                        || (Code.Substring(i + 2, 7)).Contains("M  6") || (Code.Substring(i + 2, 7)).Contains("M  06") || (Code.Substring(i + 2, 7)).Contains("M  006")))
                    {

                        if ((Convert.ToInt32(Code[i + 2]) >= 48) & (Convert.ToInt32(Code[i + 2]) <= 57))
                        {
                            Instruments += Code.Substring(i, 3) + ", ";
                            current_number = int.Parse(Code.Substring(i + 1, 2));
                        }
                        else
                        {
                            Instruments += Code.Substring(i, 2) + ", ";
                            current_number = int.Parse(Code[i + 1].ToString());
                        }

                        Instruments_array[x] = current_number;
                        x++;
                    }
                }
                //����� ���������� ������������
                x--;
                bool repeat_instrument;
                for (int y = 0; y <= x; y++)
                {
                    repeat_instrument = false;
                    //������� ������� ������� � � ���� ����� ������������ �� ����� ����������� ������ ����
                    //���� ��� �������� (�����) ��� ���, ������ �� ��������� � �� ����������� ��� ����������
                    for (int z = y; z >= 0; z--)
                    {
                        if ((Instruments_array[y] == Instruments_array[z]) & (y != z))
                        {
                            repeat_instrument = true;
                        }
                    }
                    //���� ���������� ����� �����, �������� � ������� ��� �� ���� ������ ������� �������� �������, ������ �� ����������
                    if (!repeat_instrument)
                    {
                        Uniq_Instruments_array[InstrumentsCount] = Instruments_array[y];
                        InstrumentsCount++;
                    }
                }
                //Array.Sort(Uniq_Instruments_array); // ���������� ���������� ������������ �� ����������� ������� �� �����
                for (int n = 0; n < Uniq_Instruments_array.Length; n++)
                {
                    if (Uniq_Instruments_array[n] != 0)
                        UniqInstruments += 'T' + Uniq_Instruments_array[n].ToString() + ',' + ' ';
                }
                Code = Code.Substring(1, Code.Length - 15);//������� ������ ����� �� ���� ��
            }

            //������ ������ ����������� �� ������ ������ � ������������ H, D.
            public void Change_Instrument(string old_instrument, string new_instrument)
            {
                int Code_Length = Code.Length;
                Code = '1' + Code + "12345678911111";// ������� �������������� ����� ��� ���������� ������ string-�������
                int i = 0; // �������� �����
                int z = 0; // �������� ���������� ����� ��� ������ �����������
                int n = 0; // �������� ���������� ����� ��� ������ ���������� �����������
                string buffer1 = "";
                string buffer2 = "";
                Boolean already_replaced = false;
                string old_instrument_number = old_instrument.Substring(1);
                string new_instrument_number = new_instrument.Substring(1);
                int number_count = 0; //���������� ���� ��� T

                //���� �� ���� ��
                for (i = 0; i < Code_Length - 2; i++)
                {
                    //������� ����� ����������
                    if (Code[i] == 'T' & (Convert.ToInt32(Code[i + 1]) >= 48) & (Convert.ToInt32(Code[i + 1]) <= 57) &
                       (((Convert.ToInt32(Code[i + 2]) >= 48) & (Convert.ToInt32(Code[i + 2])) <= 57) || Code[i + 2] == ' ' || Code[i + 2] == 'M') &

                       ((Code.Substring(i + 2, 7)).Contains("M6") || (Code.Substring(i + 2, 7)).Contains("M06") || (Code.Substring(i + 2, 7)).Contains("M 6")
                       || (Code.Substring(i + 2, 7)).Contains("M 06") || (Code.Substring(i + 2, 7)).Contains("M006") || (Code.Substring(i + 2, 7)).Contains("M 006")
                       || (Code.Substring(i + 2, 7)).Contains("M  6") || (Code.Substring(i + 2, 7)).Contains("M  06") || (Code.Substring(i + 2, 7)).Contains("M  006")))
                    {
                        //���������� ������� ���� ����� T � ������� �����������
                        if ((Convert.ToInt32(Code[i + 2]) >= 48) & (Convert.ToInt32(Code[i + 2])) <= 57) { number_count = 2; }
                        else { number_count = 1; }

                        //����� ������ ������� ����������
                        if (old_instrument == Code.Substring(i, number_count + 1))
                        {
                            //��������� �� ��������� �� �� ���
                            for (int j = 0; j < replaced_instruments.Length; j++)
                            {
                                if (i == replaced_instruments[j]) { already_replaced = true; }
                            }

                            //���� �� ���������, �� ��������
                            if (!already_replaced)
                            {
                                buffer1 = Code.Substring(0, i);//��, ��� �� �����������
                                buffer2 = Code.Substring(i + old_instrument.Length);//��, ��� ����� �����������
                                Code = buffer1 + new_instrument + buffer2;//������ �����������
                                replaced_instruments[replaced_instrument_index] = i;
                                replaced_instrument_index++;

                                //������ �������� �� ��������� �������� ��� ������������, ��� ����� ����� ����������� �����������, ���� ���������� ���-�� ���� � ��� ������ � ������ ����������� � ��������
                                if ((new_instrument_number.Length - old_instrument_number.Length) != 0)
                                {
                                    for (int f = 0; f < replaced_instruments.Length; f++)
                                    {
                                        if (replaced_instruments[f] > i)
                                        {
                                            replaced_instruments[f] = replaced_instruments[f] + (new_instrument_number.Length - old_instrument_number.Length);
                                        }
                                    }
                                }

                                //������ ���� ���������� H � D
                                for (z = (i + 5); z < Code_Length - 2; z++)
                                {
                                    //���� ����� ����� ����������, �� ���������� ����� � ������ �����������
                                    if (Code[z] == 'T' & (Convert.ToInt32(Code[z + 1]) >= 48) & (Convert.ToInt32(Code[z + 1]) <= 57) &
                                       (((Convert.ToInt32(Code[z + 2]) >= 48) & (Convert.ToInt32(Code[z + 2])) <= 57) || Code[z + 2] == ' ' || Code[z + 2] == 'M') &

                                       ((Code.Substring(z + 2, 7)).Contains("M6") || (Code.Substring(z + 2, 7)).Contains("M06") || (Code.Substring(z + 2, 7)).Contains("M 6")
                                       || (Code.Substring(z + 2, 7)).Contains("M 06") || (Code.Substring(z + 2, 7)).Contains("M006") || (Code.Substring(z + 2, 7)).Contains("M 006")
                                       || (Code.Substring(z + 2, 7)).Contains("M  6") || (Code.Substring(z + 2, 7)).Contains("M  06") || (Code.Substring(z + 2, 7)).Contains("M  006")))
                                    { break; }
                                    //����� ��������� H ��� D
                                    if ((Code[z] == 'H' || Code[z] == 'D') & (Convert.ToInt32(Code[z + 1]) >= 48) & (Convert.ToInt32(Code[z + 1]) <= 57))
                                    {
                                        //��������� ��� ����� �� �������������� � ������� �����������
                                        if (old_instrument_number == Code.Substring(z + 1, old_instrument_number.Length))
                                        {
                                            //������ ������ ����� ����������, ��� ��������� �������
                                            buffer1 = Code.Substring(0, z + 1);
                                            buffer2 = Code.Substring(z + 1 + old_instrument_number.Length);
                                            Code = buffer1 + new_instrument_number + buffer2;

                                            //������ �������� �� ��������� �������� ��� ������������, ��� ����� ����� ����������� �����������, ���� ���������� ���-�� ���� � ��� ������ � ������ ����������� � ��������
                                            if ((new_instrument_number.Length - old_instrument_number.Length) != 0)
                                            {
                                                for (int f = 0; f < replaced_instruments.Length; f++)
                                                {
                                                    if (replaced_instruments[f] > i)
                                                    {
                                                        replaced_instruments[f] = replaced_instruments[f] + (new_instrument_number.Length - old_instrument_number.Length);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                //����� ��������� ���� - ����� �������� ���������� ������� �����������, ���� ��� ������������ � ��
                                if (i > 10)//����� �����, ���� ��� �� ������ ������ �����
                                {
                                    for (n = (i - 5); n > 0; n--)
                                    {
                                        //���� ����� ����� ����������, �� ���������� ����� ����������
                                        if (Code[n] == 'T' & (Convert.ToInt32(Code[n + 1]) >= 48) & (Convert.ToInt32(Code[n + 1]) <= 57) &
                                           (((Convert.ToInt32(Code[n + 2]) >= 48) & (Convert.ToInt32(Code[n + 2])) <= 57) || Code[n + 2] == ' ' || Code[n + 2] == 'M') &

                                           ((Code.Substring(n + 2, 7)).Contains("M6") || (Code.Substring(n + 2, 7)).Contains("M06") || (Code.Substring(n + 2, 7)).Contains("M 6")
                                           || (Code.Substring(n + 2, 7)).Contains("M 06") || (Code.Substring(n + 2, 7)).Contains("M006") || (Code.Substring(n + 2, 7)).Contains("M 006")
                                           || (Code.Substring(n + 2, 7)).Contains("M  6") || (Code.Substring(n + 2, 7)).Contains("M  06") || (Code.Substring(n + 2, 7)).Contains("M  006")))
                                        { break; }
                                        //����� ����������
                                        if (Code[n] == 'T' & (Convert.ToInt32(Code[n + 1]) >= 48) & (Convert.ToInt32(Code[n + 1]) <= 57) &

                                           !((Code.Substring(n + 2, 7)).Contains("M6") || (Code.Substring(n + 2, 7)).Contains("M06") || (Code.Substring(n + 2, 7)).Contains("M 6")
                                           || (Code.Substring(n + 2, 7)).Contains("M 06") || (Code.Substring(n + 2, 7)).Contains("M006") || (Code.Substring(n + 2, 7)).Contains("M 006")
                                           || (Code.Substring(n + 2, 7)).Contains("M  6") || (Code.Substring(n + 2, 7)).Contains("M  06") || (Code.Substring(n + 2, 7)).Contains("M  006")))
                                        {
                                            //��������� ����� ���������� �� �������������� � ������� �����������
                                            if (old_instrument_number == Code.Substring(n + 1, old_instrument_number.Length))
                                            {
                                                //������ ������ ����� ����������
                                                buffer1 = Code.Substring(0, n + 1);
                                                buffer2 = Code.Substring(n + 1 + old_instrument_number.Length);
                                                Code = buffer1 + new_instrument_number + buffer2;

                                                //������ �������� �� ��������� �������� ��� ������������, ��� ����� ����� ���������� ����������� ����������� (������� ��� ������), ���� ���������� ���-�� ���� � ��� ������ � ������ ����������� � ��������
                                                if ((new_instrument_number.Length - old_instrument_number.Length) != 0)
                                                {
                                                    for (int f = 0; f < replaced_instruments.Length; f++)
                                                    {
                                                        if (replaced_instruments[f] > n)
                                                        {
                                                            replaced_instruments[f] = replaced_instruments[f] + (new_instrument_number.Length - old_instrument_number.Length);
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                    }

                                }
                            }
                            already_replaced = false;//���� ��� ����������� �������� �� ��������
                        }
                    }
                }
                Code = Code.Substring(1, Code.Length - 15);//������� ������ ����� �� ���� ��
            }//����� ������ ������ ����������� Change_Instrument

        }//����� ������  UP

        //����� ������ ���������� �� ����� ��� ��������� ������ �����
        public void first_information_output(UP up)
        {
            dataGridView1.Rows.Clear();

            if (up.Instruments.Length >= 2)
            {
                for (int i = 0; i < up.InstrumentsCount; i++)
                {
                    if (up.Uniq_Instruments_array[i] != 0)
                    {
                        dataGridView1.Rows.Add();
                        dataGridView1.Rows[i].Cells[0].Value = up.Uniq_Instruments_array[i];
                        dataGridView1.Rows[i].Cells[0].ReadOnly = true;
                        dataGridView1.Rows[i].Cells[1].Value = up.Uniq_Instruments_array[i];
                    }
                }
                textBox1.Text = "����: " + up.filename + Environment.NewLine + "���������� ������������: " + up.InstrumentsCount.ToString() + Environment.NewLine + Environment.NewLine +
                    "������ ������������:" + Environment.NewLine + up.UniqInstruments.Substring(0, up.UniqInstruments.Length - 2) + '.' + Environment.NewLine + Environment.NewLine +
                      "������� ���������� ������������ � ��������� � ���������:" + Environment.NewLine + up.Instruments.Substring(0, up.Instruments.Length - 2) + '.' + Environment.NewLine;

            }
            else textBox1.Text = "����: " + up.filename + Environment.NewLine + "���������� ��������� ������������: " + up.InstrumentsCount.ToString();
        }

        //����� ������ ���������� ����� ������ ������������
        public void preview_information_output(UP up)
        {
            if (up.Instruments.Length >= 4)
            {
                textBox1.Text += Environment.NewLine + Environment.NewLine +
                      "������� ���������� ������������ c ������ �������� ���������:" + Environment.NewLine + up.Instruments.Substring(0, up.Instruments.Length - 2) + '.' + Environment.NewLine;

            }
            else textBox1.Text = Environment.NewLine + "���������� ��������� ������������: " + up.InstrumentsCount.ToString();
        }

        //������� ����
        UP up_global;
        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            message_label.Text = "";
            save_as_button.Enabled = false;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                //openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    UP up = new UP();
                    //Get the path of specified file
                    up.filename = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        up.Code = reader.ReadToEnd();
                        up.File_Analysis(); // ������������� ����
                        first_information_output(up); // ������� ��������� ���������� � ����� ��
                        up_global = up;
                    }
                }
            }
        }

        //������� ����������
        private void �������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //��������� ��������� - �������� ������ ����������� �� ����� � ���������������� ������������
        private void change_up_button_Click(object sender, EventArgs e)
        {
            message_label.Text = "";
            string? cell_str;

            //������� �������� ������ � ����������� �������������
            for (int n = 0; n < up_global.InstrumentsCount; n++)
            {
                if (up_global.Uniq_Instruments_array[n] != 0)
                {
                    cell_str = Convert.ToString(dataGridView1.Rows[n].Cells[1].Value);
                    if (cell_str.Length > 0)
                    {
                        cell_str = cell_str.Trim();
                        bool success = int.TryParse(cell_str, out up_global.Uniq_Instruments_array_changed[n]);
                        if (!success)
                        {
                            message_label.Text = "������! ������� ����� ������ �����.";
                            dataGridView1.ClearSelection();
                            dataGridView1.Rows[n].Cells[1].Selected = true;
                            change_up_button.Enabled = false;
                            return;
                        }
                        if (up_global.Uniq_Instruments_array_changed[n] > 25 || up_global.Uniq_Instruments_array_changed[n] == 0)
                        {
                            message_label.Text = "������! ����� ����������� �� ����� ���� ������ 25 ���� ������ 0.";
                            dataGridView1.ClearSelection();
                            dataGridView1.Rows[n].Cells[1].Selected = true;
                            change_up_button.Enabled = false;
                            return;
                        }
                    }
                    else
                    {
                        message_label.Text = "������! ��������� ��� ���� �������.";
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[n].Cells[1].Selected = true;
                        change_up_button.Enabled = false;
                        return;
                    }
                }
            }

            //������� ������������ ��������� � ��
            for (int x = 0; x < up_global.InstrumentsCount; x++)
            {
                dataGridView1.Rows[x].Cells[1].ReadOnly = true;
            }

            //������� ������ ����������� �� �����, ���� ��� ������
            for (int i = 0; i < up_global.InstrumentsCount; i++)
            {
                if (up_global.Uniq_Instruments_array_changed[i] != up_global.Uniq_Instruments_array[i])
                {
                    dataGridView1.Rows[i].Cells[0].Style.BackColor = System.Drawing.Color.Aquamarine;
                    dataGridView1.Rows[i].Cells[1].Style.BackColor = System.Drawing.Color.Aquamarine;
                    up_global.Change_Instrument("T" + up_global.Uniq_Instruments_array[i].ToString(), "T" + up_global.Uniq_Instruments_array_changed[i].ToString());
                }
            }

            change_up_button.Enabled = false;

            //������� ��������� ����� �����
            dataGridView1.ClearSelection();

            //�������������� ���������
            up_global.File_Analysis();
            preview_information_output(up_global);

            save_as_button.Enabled = true;
        }

        //������� ��������� ����������� ���� ��
        private void update_button_Click(object sender, EventArgs e)
        {
            save_as_button.Enabled = false;
            StreamReader reader = new StreamReader(Last_save_file_name);
            UP up = new UP();
            up.Code = reader.ReadToEnd();
            up.filename = Last_save_file_name;
            up.File_Analysis(); // ������������� ����
            first_information_output(up); // ������� ��������� ���������� � ����� ��
            up_global = up;
            update_button.Enabled = false;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            change_up_button.Enabled = true;
        }

        //��������� ���� � �����������
        string Last_save_file_name;//���� � ���������� ������������ �����
        private void save_as_button_Click(object sender, EventArgs e)
        {
            //���� ����� ������� ������
            /*   var date = new DateTime(2022, 10, 17);
               if (DateTime.Today > date)
               {
                   message_label.Text = "������� ������ ��������� ����.";
               }
               else
               {
                   SaveFileDialog saveFileDialog = new SaveFileDialog();
                   saveFileDialog.Filter = "All file (*.*)|*.*";
                   saveFileDialog.FileName = up_global.filename;
                   if (saveFileDialog.ShowDialog() == DialogResult.OK)
                   {
                       File.WriteAllText(saveFileDialog.FileName, up_global.Code);
                       update_button.Enabled = true;
                       save_as_button.Enabled = false;
                   }
                   Last_save_file_name = saveFileDialog.FileName;
               }*/
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "All file (*.*)|*.*";
            saveFileDialog.FileName = up_global.filename;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, up_global.Code);
                update_button.Enabled = true;
                save_as_button.Enabled = false;
            }
            Last_save_file_name = saveFileDialog.FileName;
        }
    }
}