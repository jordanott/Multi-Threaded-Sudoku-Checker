using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Sudoku_Checker
{
    public partial class Form1 : Form
    {
        public List<List<int>> grid = new List<List<int>>();
        List<List<int>> color_values;
        public Label[,] labels = new Label[9,9];
        volatile int thread_done_count = 0;
        int row_error = -1;
        int column_error = -1;
        public Form1()
        {
            InitializeComponent();
            
            labels = new Label[9, 9] {
                {this.label_0_0, this.label_0_1,this.label_0_2,this.label_0_3, this.label_0_4, this.label_0_5, this.label_0_6, this.label_0_7, this.label_0_8 },
                {this.label_1_0, this.label_1_1,this.label_1_2,this.label_1_3, this.label_1_4, this.label_1_5, this.label_1_6, this.label_1_7, this.label_1_8 },
                {this.label_2_0, this.label_2_1,this.label_2_2,this.label_2_3, this.label_2_4, this.label_2_5, this.label_2_6, this.label_2_7, this.label_2_8 },
                {this.label_3_0, this.label_3_1,this.label_3_2,this.label_3_3, this.label_3_4, this.label_3_5, this.label_3_6, this.label_3_7, this.label_3_8 },
                {this.label_4_0, this.label_4_1,this.label_4_2,this.label_4_3, this.label_4_4, this.label_4_5, this.label_4_6, this.label_4_7, this.label_4_8 },
                {this.label_5_0, this.label_5_1,this.label_5_2,this.label_5_3, this.label_5_4, this.label_5_5, this.label_5_6, this.label_5_7, this.label_5_8 },
                {this.label_6_0, this.label_6_1,this.label_6_2,this.label_6_3, this.label_6_4, this.label_6_5, this.label_6_6, this.label_6_7, this.label_6_8 },
                {this.label_7_0, this.label_7_1,this.label_7_2,this.label_7_3, this.label_7_4, this.label_7_5, this.label_7_6, this.label_7_7, this.label_7_8 },
                {this.label_8_0, this.label_8_1,this.label_8_2,this.label_8_3, this.label_8_4, this.label_8_5, this.label_8_6, this.label_8_7, this.label_8_8 },
            };          
        }
        public void flash_error(Label error_elem)
        {
            int i = 0;
            while(i < 50)
            {
                error_elem.BackColor = Color.Yellow;
                Thread.Sleep(750);
                error_elem.BackColor = Color.Red;
                Thread.Sleep(750);
                i++;
            }
        }
        public void check_columns()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            
            for (int rows = 0; rows < 9; rows++)
            {
                for (int columns = 0; columns < 9; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(columns).ElementAt(rows)))
                    {
                        column_error = rows;
                        if (row_error != -1)
                        {
                            Thread flashing = new Thread(() => flash_error(labels[row_error, column_error]));
                            flashing.Start();
                        }
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[columns, rows].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(columns).RemoveAt(rows);
                                    color_values.ElementAt(columns).Insert(rows,-1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(columns).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(columns).ElementAt(rows) > 0)
                            {
                                int old_value = color_values.ElementAt(columns).ElementAt(rows);
                                try
                                {
                                    color_values.ElementAt(columns).RemoveAt(rows);
                                    color_values.ElementAt(columns).Insert(rows, old_value - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(columns).Add(old_value - 30);
                                }
                                lock (labels)
                                {
                                    labels[columns, rows].BackColor = Color.FromArgb(0, color_values.ElementAt(columns).ElementAt(rows), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
                check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            }
            thread_done_count++;
        }
        public void check_rows()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
           
            for (int rows = 0; rows < 9; rows++)
            {
                for (int columns = 0; columns < 9; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        row_error = rows;
                        if(column_error != -1)
                        {
                            Thread flashing = new Thread(() => flash_error(labels[row_error, column_error]));
                            flashing.Start();
                        }
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, -1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
                check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            }
            thread_done_count++;
        }
        public void check_grid1()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            for (int rows = 0; rows < 3; rows++)
            {
                for (int columns = 0; columns < 3; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            try
                            {
                                color_values.ElementAt(rows).RemoveAt(columns);
                                color_values.ElementAt(rows).Insert(columns, -1);
                            }
                            catch (Exception e)
                            {
                                color_values.ElementAt(rows).Add(-1);
                            }
                        }
                        lock (labels)
                        {
                            labels[rows, columns].BackColor = Color.Red;
                        }

                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        public void check_grid2()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int rows = 0; rows < 3; rows++)
            {
                for (int columns = 3; columns < 6; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns,-1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        public void check_grid3()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int rows = 0; rows < 3; rows++)
            {
                for (int columns = 6; columns < 9; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, -1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        public void check_grid4()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int rows = 3; rows < 6; rows++)
            {
                for (int columns = 0; columns < 3; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, -1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        public void check_grid5()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int rows = 3; rows < 6; rows++)
            {
                for (int columns = 3; columns < 6; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, -1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns)> 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        public void check_grid6()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int rows = 3; rows < 6; rows++)
            {
                for (int columns = 6; columns < 9; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, -1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        public void check_grid7()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int rows = 6; rows < 9; rows++)
            {
                for (int columns = 0; columns < 3; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, -1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        public void check_grid8()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int rows = 6; rows < 9; rows++)
            {
                for (int columns = 3; columns < 6; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, -1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        public void check_grid9()
        {
            List<int> check_num = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            for (int rows = 6; rows < 9; rows++)
            {
                for (int columns = 6; columns < 9; columns++)
                {
                    if (!check_num.Remove(grid.ElementAt(rows).ElementAt(columns)))
                    {
                        lock (color_values)
                        {
                            lock (labels)
                            {
                                labels[rows, columns].BackColor = Color.Red;
                                try
                                {
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, -1);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(-1);
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (color_values)
                        {
                            if (color_values.ElementAt(rows).ElementAt(columns) > 0)
                            {
                                int old_val = 0;
                                try
                                {
                                    old_val = color_values.ElementAt(rows).ElementAt(columns);
                                    color_values.ElementAt(rows).RemoveAt(columns);
                                    color_values.ElementAt(rows).Insert(columns, old_val - 30);
                                }
                                catch (Exception e)
                                {
                                    color_values.ElementAt(rows).Add(old_val - 30);
                                }
                                lock (labels)
                                {
                                    labels[rows, columns].BackColor = Color.FromArgb(0, color_values.ElementAt(rows).ElementAt(columns), 0);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            thread_done_count++;
        }
        private void populate_table()
        {
            for(int i = 0;i < 9; i++)
            {
                for(int j = 0;j < 9; j++)
                {
                    labels[i, j].Text = grid.ElementAt(i).ElementAt(j).ToString();
                }
            }
        }

        private void start_button_Click(object sender, EventArgs e)
        {
            color_values = new List<List<int>>();
            for (int i = 0; i < 9; i++)
            {
                color_values.Add(new List<int>(new int[9] { 250, 250, 250, 250, 250, 250, 250, 250, 250 }));
            }
            Thread columns = new Thread(check_columns);
            Thread rows = new Thread(check_rows);
            Thread grid1 = new Thread(check_grid1);
            Thread grid2 = new Thread(check_grid2);
            Thread grid3 = new Thread(check_grid3);
            Thread grid4 = new Thread(check_grid4);
            Thread grid5 = new Thread(check_grid5);
            Thread grid6 = new Thread(check_grid6);
            Thread grid7 = new Thread(check_grid7);
            Thread grid8 = new Thread(check_grid8);
            Thread grid9 = new Thread(check_grid9);

            
            grid1.Start();
            grid2.Start();
            grid3.Start();
            grid4.Start();
            grid5.Start();
            grid6.Start();
            grid7.Start();
            grid8.Start();
            grid9.Start();
            columns.Start();
            rows.Start();


            if (checkBox1.Checked)
            {
                // Option to join thread but blocks UI because all threads alter UI 
                grid1.Join();
                grid2.Join();
                grid3.Join();
                grid4.Join();
                grid5.Join();
                grid6.Join();
                grid7.Join();
                grid8.Join();
                grid9.Join();
                columns.Join();
                rows.Join();
            }
            

        }
        private void load_button_Click(object sender, EventArgs e)
        {
            load_function();
        }
        private void load_function()
        {
            string[] lines = System.IO.File.ReadAllLines(@textBox1.Text.ToString());

            foreach (string line in lines)
            {
                List<string> s_line = line.Split(',').ToList<string>();
                List<int> i_line = new List<int>();
                foreach (string elem in s_line)
                {
                    int i;
                    int.TryParse(elem, out i);
                    if (i != 0)
                    {
                        i_line.Add(i);
                    }
                }
                grid.Add(i_line);
            }
            populate_table();
        }
    }
}
