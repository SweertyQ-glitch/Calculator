using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics.Eventing.Reader;

namespace Glassculator
{
    public partial class MainForm : Form
    {
        private Label displayLabel;
        private bool isNewNumber = true;
        private double firstNumber = 0;
        private string currentOperation = "";
        public MainForm()
        {
            InitializeComponent();
            Panel  displayPanel = new Panel();       //понель дисплея

            displayPanel.Name = "displayPanel"; 
            displayPanel.Size = new Size(this.ClientSize.Width - 50, 100);
            displayPanel.Location = new Point(25, 30);
            displayPanel.BackColor = Color.FromArgb (230, 255, 255, 255);
            this.Controls.Add(displayPanel);

            displayLabel = new Label();        //текст дисплея

            displayLabel.Name = "displayLabel";
            displayLabel.Text = "0";
            displayLabel.Dock = DockStyle.Fill;
            displayLabel.Font = new Font(
                "Segoe UI",
                32,
                FontStyle.Regular
            );

            displayLabel.ForeColor = Color.FromArgb(55, 55, 65);

            displayLabel.TextAlign = ContentAlignment.MiddleRight;
            displayLabel.Padding = new Padding(15,0,15,0);

            displayPanel.Controls.Add(displayLabel);

            // таблица для кнопок
            TableLayoutPanel buttonTable = new TableLayoutPanel();

            buttonTable.Name = "buttonTable";
            buttonTable.Size = new Size(this.ClientSize.Width - 50,420);
            buttonTable.Location = new Point(25, 160);

            buttonTable.ColumnCount = 4;
            buttonTable.RowCount = 6;

            buttonTable.BackColor = this.BackColor;

            // распределение столбцов
            for (int i = 0; i < 4; i++)
            {
                buttonTable.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 25)
                );
            }

            //распределение рядов
            for (int i = 0; i < 6; i++)
            {
                buttonTable.RowStyles.Add(
                    new RowStyle(SizeType.Percent, 100f / 6)
                );
            }

            this.Controls.Add(buttonTable);

            // цвета кнопок
            Color numberColor = Color.FromArgb(250, 250, 250);
            Color operationColor = Color.FromArgb(235, 229, 243);
            Color specialColor = Color.FromArgb(240, 237, 245);
            Color equalColor = Color.FromArgb(215, 205, 230);

            // первый ряд
            buttonTable.Controls.Add(CreateButton("%", specialColor), 0, 0);
            buttonTable.Controls.Add(CreateButton("CE", specialColor), 1, 0);
            buttonTable.Controls.Add(CreateButton("C", specialColor), 2, 0);
            buttonTable.Controls.Add(CreateButton("⌫", specialColor), 3, 0);

            // второй ряд
            buttonTable.Controls.Add(CreateButton("7", numberColor), 0, 1);
            buttonTable.Controls.Add(CreateButton("8", numberColor), 1, 1);
            buttonTable.Controls.Add(CreateButton("9", numberColor), 2, 1);
            buttonTable.Controls.Add(CreateButton("÷", operationColor), 3, 1);

            // третий ряд
            buttonTable.Controls.Add(CreateButton("4", numberColor), 0, 2);
            buttonTable.Controls.Add(CreateButton("5", numberColor), 1, 2);
            buttonTable.Controls.Add(CreateButton("6", numberColor), 2, 2);
            buttonTable.Controls.Add(CreateButton("×", operationColor), 3, 2);

            // четвёртый ряд
            buttonTable.Controls.Add(CreateButton("1", numberColor), 0, 3);
            buttonTable.Controls.Add(CreateButton("2", numberColor), 1, 3);
            buttonTable.Controls.Add(CreateButton("3", numberColor), 2, 3);
            buttonTable.Controls.Add(CreateButton("−", operationColor), 3, 3);

            // пятый ряд
            buttonTable.Controls.Add(CreateButton("±", specialColor), 0, 4);
            buttonTable.Controls.Add(CreateButton("0", numberColor), 1, 4);
            buttonTable.Controls.Add(CreateButton(".", numberColor), 2, 4);
            buttonTable.Controls.Add(CreateButton("+", operationColor), 3, 4);

            // шестой ряд
            Button equalButton = CreateButton("=", equalColor);

            buttonTable.Controls.Add(equalButton, 0, 5);

            // растягиваем кнопку равно на четыре столбца
            buttonTable.SetColumnSpan(equalButton, 4);
        }
        private Button CreateButton(string text, Color color)
        {
            RoundedButton button = new RoundedButton();

            button.Text = text;
            button.BackColor = color;

            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(5);

            button.CornerRadius = 15;
            button.Click += Button_Click;

            return button;
        }
        private void Button_Click(object sender, EventArgs e)    //обработчик нажатий
        {
            Button clickedButton = (Button)sender;
            string value = clickedButton.Text;

            if (value == "C")
            {
                displayLabel.Text = "0";
                firstNumber = 0;
                currentOperation = "";
                isNewNumber = true;
                return;
            }
            if (value == "CE")
            {
                displayLabel.Text = "0";
                isNewNumber= true;
                return;
            } 
            //удаление последнего символа
            if (value == "⌫")
            {
                if (displayLabel.Text.Length > 1)
                {
                    displayLabel.Text = displayLabel.Text.Substring(0, displayLabel.Text.Length - 1);
                }
                else
                {
                    displayLabel.Text = "0";
                }
                return;
            }
             

            if (char.IsDigit(value[0]))
            {
                if (isNewNumber || displayLabel.Text == "0")
                {
                    displayLabel.Text = value;
                    isNewNumber = false;
                }
                else
                {
                    displayLabel.Text += value;
                }

                return;
            }
            if (value == ".")   //обработка точки
            {
                if (isNewNumber)
                {
                    displayLabel.Text = "0.";
                    isNewNumber = false;
                }
                else if (!displayLabel.Text.Contains("."))
                {
                    displayLabel.Text += ".";
                }
                return;
            }
            // смена знака числа
            if (value == "±")
            {
                if (displayLabel.Text != "0")
                {
                    if (displayLabel.Text.StartsWith("-"))
                    {
                        displayLabel.Text = displayLabel.Text.Substring(1);
                    }
                    else
                    {
                        displayLabel.Text = "-" + displayLabel.Text;
                    }
                }

                return;
            }

            //вычисление процента
            if (value == "%")
            {
                double currentNumber;
                double.TryParse(displayLabel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out currentNumber);

                if (currentOperation != "")
                {
                    currentNumber = firstNumber * currentNumber / 100;
                }
                else
                {
                    currentNumber = currentNumber / 100;
                }
                displayLabel.Text = currentNumber.ToString(CultureInfo.InvariantCulture);

                isNewNumber = true;
                return;
             }
          
            if (value == "+" || value == "−" || value == "×" || value == "÷")   //выбор математической операции
            {
                double.TryParse(displayLabel.Text, NumberStyles.Float,CultureInfo.InvariantCulture, out firstNumber);
                currentOperation = value;
                isNewNumber = true;

                return;
            }
            //вычисление результата
            if (value == "=")
            {
                double secondNumber;

                bool isvalid = double.TryParse(displayLabel.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out secondNumber);
                
                if (!isvalid || currentOperation == "")
                {
                    return;
                }

               
                double result = 0;

                switch (currentOperation)
                {
                    case "+":
                        result = firstNumber + secondNumber; break;
                    case "-":
                    case "−":
                        result = firstNumber - secondNumber;
                        break;
                    case "×":
                        result = firstNumber * secondNumber; break;
                    case "÷":
                        if (secondNumber == 0)
                        {
                            displayLabel.Text = "Ошибка";
                            currentOperation = "";
                            isNewNumber = true;
                            return;
                        }
                        result = firstNumber / secondNumber; break;
                }
                // Проверка корректности результата
                if (double.IsNaN(result) || double.IsInfinity(result))
                {
                    displayLabel.Text = "Ошибка";
                    currentOperation = "";
                    isNewNumber = true;
                    return;
                }
                displayLabel.Text = result.ToString("G", CultureInfo.InvariantCulture);

                currentOperation = "";
                isNewNumber = true;
                return;
            }

        }   
    }
}
