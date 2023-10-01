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
            //Убираем сортировку
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

            //Выравнивание по центру
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.AllowUserToAddRows = false;

            textBox1.ReadOnly = true;
        }

        //Класс файла с программой для станка
        public class UP
        {
            public string Code = ""; // Код исходного документа
            public string filename = ""; // Название файла исходного документа

            public string Instruments = ""; // Список инструментов в порядке следования с повторами в исходном коде
            public string UniqInstruments = ""; // Список уникальных инструментов

            public int InstrumentsCount = 0; // Количество уникальных инструментов

            public int[] Instruments_array = new int[3000]; // Массив всех инструментов с повторяющимися
            public int[] Uniq_Instruments_array = new int[25]; // Массив только уникальных инструментов
            public int[] Uniq_Instruments_array_changed = new int[25]; // Массив только уникальных инструментов с измененными номерами

            public int[] replaced_instruments = new int[200]; // Индексы инструментов, которые уже были заменены в данной УП
            public int replaced_instrument_index;// Индекс, измененного инструмента

            //Метод, анализирующий файл УП
            public void File_Analysis()
            {
                //Просто тренируюсь
                /*Array.LastIndexOf(Instruments_array, 1);
                Array.Copy(Uniq_Instruments_array, Instruments_array, 25);
                Array.Sort(Uniq_Instruments_array);*/

                //При повторном вызове необходимо занулить
                Instruments = "";
                UniqInstruments = "";
                InstrumentsCount = 0;
                int current_number = 0; // Номер текущего инструмента в цикле
                int i = 0; // Итератор цикла
                int x = 0; // Итератор массива инструментов

                int Code_Length = Code.Length;
                Code = '1' + Code + "12345678911111";// Добавил дополнительные знаки для увеличения длинны string-массива

                //Поиск инструментов
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
                //Поиск уникальных инструментов
                x--;
                bool repeat_instrument;
                for (int y = 0; y <= x; y++)
                {
                    repeat_instrument = false;
                    //Берется элемент массива и в след цикле сравнивается со всеми предыдущими позади него
                    //Если его значение (номер) уже был, значит он повторный и не учитывается как уникальный
                    for (int z = y; z >= 0; z--)
                    {
                        if ((Instruments_array[y] == Instruments_array[z]) & (y != z))
                        {
                            repeat_instrument = true;
                        }
                    }
                    //Если инструмент имеет номер, которого в массиве ещё не было позади данного элемента массива, значит он уникальный
                    if (!repeat_instrument)
                    {
                        Uniq_Instruments_array[InstrumentsCount] = Instruments_array[y];
                        InstrumentsCount++;
                    }
                }
                //Array.Sort(Uniq_Instruments_array); // Сортировка уникальных инструментов по возрастанию сказали не нужна
                for (int n = 0; n < Uniq_Instruments_array.Length; n++)
                {
                    if (Uniq_Instruments_array[n] != 0)
                        UniqInstruments += 'T' + Uniq_Instruments_array[n].ToString() + ',' + ' ';
                }
                Code = Code.Substring(1, Code.Length - 15);//Удаляем лишние знаки из кода УП
            }

            //Замена одного инструмента на другой вместе с корректорами H, D.
            public void Change_Instrument(string old_instrument, string new_instrument)
            {
                int Code_Length = Code.Length;
                Code = '1' + Code + "12345678911111";// Добавил дополнительные знаки для увеличения длинны string-массива
                int i = 0; // Итератор цикла
                int z = 0; // Итератор вложенного цикла для поиска корректоров
                int n = 0; // Итератор вложенного цикла для поиска подготовки инструмента
                string buffer1 = "";
                string buffer2 = "";
                Boolean already_replaced = false;
                string old_instrument_number = old_instrument.Substring(1);
                string new_instrument_number = new_instrument.Substring(1);
                int number_count = 0; //Количество цифр при T

                //Цикл по коду УП
                for (i = 0; i < Code_Length - 2; i++)
                {
                    //Находим любой инструмент
                    if (Code[i] == 'T' & (Convert.ToInt32(Code[i + 1]) >= 48) & (Convert.ToInt32(Code[i + 1]) <= 57) &
                       (((Convert.ToInt32(Code[i + 2]) >= 48) & (Convert.ToInt32(Code[i + 2])) <= 57) || Code[i + 2] == ' ' || Code[i + 2] == 'M') &

                       ((Code.Substring(i + 2, 7)).Contains("M6") || (Code.Substring(i + 2, 7)).Contains("M06") || (Code.Substring(i + 2, 7)).Contains("M 6")
                       || (Code.Substring(i + 2, 7)).Contains("M 06") || (Code.Substring(i + 2, 7)).Contains("M006") || (Code.Substring(i + 2, 7)).Contains("M 006")
                       || (Code.Substring(i + 2, 7)).Contains("M  6") || (Code.Substring(i + 2, 7)).Contains("M  06") || (Code.Substring(i + 2, 7)).Contains("M  006")))
                    {
                        //Определяем сколько цифр после T у данного инструмента
                        if ((Convert.ToInt32(Code[i + 2]) >= 48) & (Convert.ToInt32(Code[i + 2])) <= 57) { number_count = 2; }
                        else { number_count = 1; }

                        //Нашли старый искомый инструмент
                        if (old_instrument == Code.Substring(i, number_count + 1))
                        {
                            //Проверяем не заменялся ли он уже
                            for (int j = 0; j < replaced_instruments.Length; j++)
                            {
                                if (i == replaced_instruments[j]) { already_replaced = true; }
                            }

                            //Если не заменялся, то заменяем
                            if (!already_replaced)
                            {
                                buffer1 = Code.Substring(0, i);//всё, что до инструмента
                                buffer2 = Code.Substring(i + old_instrument.Length);//всё, что после инструмента
                                Code = buffer1 + new_instrument + buffer2;//замена инструмента
                                replaced_instruments[replaced_instrument_index] = i;
                                replaced_instrument_index++;

                                //Делаем поправку на изменение индексов тех инструментов, что стоят после измененного инструмента, если изменилось кол-во цифр в его номере и номере корректоров и проверки
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

                                //Теперь ищем корректоры H и D
                                for (z = (i + 5); z < Code_Length - 2; z++)
                                {
                                    //Если нашли новый инструмент, то прекращаем поиск и замену корректоров
                                    if (Code[z] == 'T' & (Convert.ToInt32(Code[z + 1]) >= 48) & (Convert.ToInt32(Code[z + 1]) <= 57) &
                                       (((Convert.ToInt32(Code[z + 2]) >= 48) & (Convert.ToInt32(Code[z + 2])) <= 57) || Code[z + 2] == ' ' || Code[z + 2] == 'M') &

                                       ((Code.Substring(z + 2, 7)).Contains("M6") || (Code.Substring(z + 2, 7)).Contains("M06") || (Code.Substring(z + 2, 7)).Contains("M 6")
                                       || (Code.Substring(z + 2, 7)).Contains("M 06") || (Code.Substring(z + 2, 7)).Contains("M006") || (Code.Substring(z + 2, 7)).Contains("M 006")
                                       || (Code.Substring(z + 2, 7)).Contains("M  6") || (Code.Substring(z + 2, 7)).Contains("M  06") || (Code.Substring(z + 2, 7)).Contains("M  006")))
                                    { break; }
                                    //Нашли корректор H или D
                                    if ((Code[z] == 'H' || Code[z] == 'D') & (Convert.ToInt32(Code[z + 1]) >= 48) & (Convert.ToInt32(Code[z + 1]) <= 57))
                                    {
                                        //Проверяем его номер на принадлежность к старому инструменту
                                        if (old_instrument_number == Code.Substring(z + 1, old_instrument_number.Length))
                                        {
                                            //Меняем только номер корректора, сам корректор остаётся
                                            buffer1 = Code.Substring(0, z + 1);
                                            buffer2 = Code.Substring(z + 1 + old_instrument_number.Length);
                                            Code = buffer1 + new_instrument_number + buffer2;

                                            //Делаем поправку на изменение индексов тех инструментов, что стоят после измененного инструмента, если изменилось кол-во цифр в его номере и номере корректоров и проверки
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

                                //Далее последний этап - нужно изменить подготовку данного инструмента, если она присутствует в УП
                                if (i > 10)//Имеет смысл, если это не совсем начало файла
                                {
                                    for (n = (i - 5); n > 0; n--)
                                    {
                                        //Если нашли новый инструмент, то прекращаем поиск подготовки
                                        if (Code[n] == 'T' & (Convert.ToInt32(Code[n + 1]) >= 48) & (Convert.ToInt32(Code[n + 1]) <= 57) &
                                           (((Convert.ToInt32(Code[n + 2]) >= 48) & (Convert.ToInt32(Code[n + 2])) <= 57) || Code[n + 2] == ' ' || Code[n + 2] == 'M') &

                                           ((Code.Substring(n + 2, 7)).Contains("M6") || (Code.Substring(n + 2, 7)).Contains("M06") || (Code.Substring(n + 2, 7)).Contains("M 6")
                                           || (Code.Substring(n + 2, 7)).Contains("M 06") || (Code.Substring(n + 2, 7)).Contains("M006") || (Code.Substring(n + 2, 7)).Contains("M 006")
                                           || (Code.Substring(n + 2, 7)).Contains("M  6") || (Code.Substring(n + 2, 7)).Contains("M  06") || (Code.Substring(n + 2, 7)).Contains("M  006")))
                                        { break; }
                                        //Нашли подготовку
                                        if (Code[n] == 'T' & (Convert.ToInt32(Code[n + 1]) >= 48) & (Convert.ToInt32(Code[n + 1]) <= 57) &

                                           !((Code.Substring(n + 2, 7)).Contains("M6") || (Code.Substring(n + 2, 7)).Contains("M06") || (Code.Substring(n + 2, 7)).Contains("M 6")
                                           || (Code.Substring(n + 2, 7)).Contains("M 06") || (Code.Substring(n + 2, 7)).Contains("M006") || (Code.Substring(n + 2, 7)).Contains("M 006")
                                           || (Code.Substring(n + 2, 7)).Contains("M  6") || (Code.Substring(n + 2, 7)).Contains("M  06") || (Code.Substring(n + 2, 7)).Contains("M  006")))
                                        {
                                            //Проверяем номер подготовки на принадлежность к старому инструменту
                                            if (old_instrument_number == Code.Substring(n + 1, old_instrument_number.Length))
                                            {
                                                //Меняем только номер подготовки
                                                buffer1 = Code.Substring(0, n + 1);
                                                buffer2 = Code.Substring(n + 1 + old_instrument_number.Length);
                                                Code = buffer1 + new_instrument_number + buffer2;

                                                //Делаем поправку на изменение индексов тех инструментов, что стоят после подготовки измененного инструмента (включая его самого), если изменилось кол-во цифр в его номере и номере корректоров и проверки
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
                            already_replaced = false;//Флаг уже замененного занулить не забываем
                        }
                    }
                }
                Code = Code.Substring(1, Code.Length - 15);//Удаляем лишние знаки из кода УП
            }//Конец метода замены инструмента Change_Instrument

        }//Конец класса  UP

        //Метод вывода информации на экран при первичном чтении файла
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
                textBox1.Text = "Файл: " + up.filename + Environment.NewLine + "Количество инструментов: " + up.InstrumentsCount.ToString() + Environment.NewLine + Environment.NewLine +
                    "Список инструментов:" + Environment.NewLine + up.UniqInstruments.Substring(0, up.UniqInstruments.Length - 2) + '.' + Environment.NewLine + Environment.NewLine +
                      "Порядок следования инструментов в программе с повторами:" + Environment.NewLine + up.Instruments.Substring(0, up.Instruments.Length - 2) + '.' + Environment.NewLine;

            }
            else textBox1.Text = "Файл: " + up.filename + Environment.NewLine + "Количество найденных инструментов: " + up.InstrumentsCount.ToString();
        }

        //Метод вывода информации после замены инструментов
        public void preview_information_output(UP up)
        {
            if (up.Instruments.Length >= 4)
            {
                textBox1.Text += Environment.NewLine + Environment.NewLine +
                      "Порядок следования инструментов c учетом внесённых изменений:" + Environment.NewLine + up.Instruments.Substring(0, up.Instruments.Length - 2) + '.' + Environment.NewLine;

            }
            else textBox1.Text = Environment.NewLine + "Количество найденных инструментов: " + up.InstrumentsCount.ToString();
        }

        //Открыть файл
        UP up_global;
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
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
                        up.File_Analysis(); // Анализировать файл
                        first_information_output(up); // Вывести первичную информацию о файле уп
                        up_global = up;
                    }
                }
            }
        }

        //Закрыть приложение
        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Применить изменения - заменить старые инструменты на новые с соответствующими корректорами
        private void change_up_button_Click(object sender, EventArgs e)
        {
            message_label.Text = "";
            string? cell_str;

            //Сначала заполним массив с измененными инструментами
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
                            message_label.Text = "Ошибка! Вводить можно только цифры.";
                            dataGridView1.ClearSelection();
                            dataGridView1.Rows[n].Cells[1].Selected = true;
                            change_up_button.Enabled = false;
                            return;
                        }
                        if (up_global.Uniq_Instruments_array_changed[n] > 25 || up_global.Uniq_Instruments_array_changed[n] == 0)
                        {
                            message_label.Text = "Ошибка! Номер инструмента не может быть больше 25 либо равным 0.";
                            dataGridView1.ClearSelection();
                            dataGridView1.Rows[n].Cells[1].Selected = true;
                            change_up_button.Enabled = false;
                            return;
                        }
                    }
                    else
                    {
                        message_label.Text = "Ошибка! Заполните все поля таблицы.";
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[n].Cells[1].Selected = true;
                        change_up_button.Enabled = false;
                        return;
                    }
                }
            }

            //Сделаем недоступнымм изменения в УП
            for (int x = 0; x < up_global.InstrumentsCount; x++)
            {
                dataGridView1.Rows[x].Cells[1].ReadOnly = true;
            }

            //Заменим старые инструменты на новые, если они разные
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

            //Убираем выделение ячеек мышью
            dataGridView1.ClearSelection();

            //Проанализируем изменения
            up_global.File_Analysis();
            preview_information_output(up_global);

            save_as_button.Enabled = true;
        }

        //Открыть последний сохраненный файл УП
        private void update_button_Click(object sender, EventArgs e)
        {
            save_as_button.Enabled = false;
            StreamReader reader = new StreamReader(Last_save_file_name);
            UP up = new UP();
            up.Code = reader.ReadToEnd();
            up.filename = Last_save_file_name;
            up.File_Analysis(); // Анализировать файл
            first_information_output(up); // Вывести первичную информацию о файле уп
            up_global = up;
            update_button.Enabled = false;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            change_up_button.Enabled = true;
        }

        //Сохранить файл с изменениями
        string Last_save_file_name;//Путь к последнему сохраненному файлу
        private void save_as_button_Click(object sender, EventArgs e)
        {
            //Если нужна пробная версия
            /*   var date = new DateTime(2022, 10, 17);
               if (DateTime.Today > date)
               {
                   message_label.Text = "Пробный период программы истёк.";
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