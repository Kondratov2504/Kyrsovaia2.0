using FileTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Курсовая2._0
{
    public partial class Form1 : Form
    {
       // private const char SchaeferSymbol = '|';

        // Создать пустой стек для операторов
        private Stack<char> operatorStack = new Stack<char>();

        // Создать пустой стек для операндов
        private Stack<bool> operandStack = new Stack<bool>();

        private FileOperator fileOperator = new FileOperator();

        private string logPath = Application.StartupPath + "\\log.txt";

       // в данном коде присутствуют правки 
       

        private char[] operatorOrder = new char[]
        {
            '~',// i = 0 ,эквивалентность или подобие. Когда х = у. Обозначается х ~ у
            '→',// i = 1 , импликация (следование). Обозначается ->
            '+',// i = 2 , дизъюнкция (функция или). Обозначается V
            '&',// i = 3 , конъюнкция (функция И)
            '↓',// i = 4, стрелка Пирса
            '|',// i = 5 , штрих Шеффера
            '!'// i = 6, отрицание,  инверсия 
        };
        string input;
        bool result;
        bool operand;

        private int minWidth = 300;
        private int maxWidth = 550;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Width = minWidth;
        }

        public string GetLastSymbol()
        {
             return textBox1.Text.Substring(textBox1.Text.Length - 1); 
            //set { _symbol = value; }
        }

        /*public string getSymbol()
        {
            return _symbol;
        }

        private void setSymbol(string value)
        {
            _symbol = value;
        }*/

        private void button0_Click(object sender, EventArgs e)
        {
            //Symbol = textBox1.Text.Substring(textBox1.Text.Length - 1);
            //string symbol = textBox1.Text.Substring(textBox1.Text.Length - 1);
            if (textBox1.Text.Length == 0
                || GetLastSymbol() == "(" || -1 != GetOperatorIndex(GetLastSymbol()[0]))
            {
                textBox1.Text += "0";
                AddLog("0");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || GetLastSymbol() == "(" || -1 != GetOperatorIndex(GetLastSymbol()[0]))
            {
                textBox1.Text += "1";
                AddLog("1");
            }
        }

        private void denialButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0 || (GetLastSymbol() != "!" && -1 != GetOperatorIndex(GetLastSymbol()[0])))
            {
                textBox1.Text += "!";
            }
        }


        private void conjunctionButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;

            if (GetLastSymbol() == ")" 
                || GetLastSymbol() == "1" 
                || GetLastSymbol() == "0")
            {
                textBox1.Text += "&";
            }
        }

        private void disjunctionButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;

            if ((GetLastSymbol() == ")" || GetLastSymbol() == "1" || GetLastSymbol() == "0"))
            {
                textBox1.Text += "+";
            }
            // TODO : неоптимизированная часть кода
            /*string length = Symbol();
            if (length == "1" || length == "0")
            {
                textBox1.Text += "+";
            }
            else(textBox1.Text.Length == 0)
            {
                label1.Text = " ";
            }
            */
        }

        private void implicationButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;

            if (GetLastSymbol() == ")" || GetLastSymbol() == "1" || GetLastSymbol() == "0")
            {
                textBox1.Text += "→";
            }
        }

        private void equivalenceButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;

            if (GetLastSymbol() == ")" || GetLastSymbol() == "1" || GetLastSymbol() == "0")
            {
                textBox1.Text += "~";
            }
        }

        private void buttonOfTheFirstBracket_Click(object sender, EventArgs e)
        {

            if (textBox1.Text.Length == 0  || -1 != GetOperatorIndex(GetLastSymbol()[0]))
            {
                textBox1.Text += "(";
            }
        }

        private void buttonOfTheLastBracket_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;

            if (GetLastSymbol() == "1" || GetLastSymbol() == "0")
            {
                textBox1.Text += ")";
            }
        }
        private void schaefersStrokeButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;

            if (GetLastSymbol() == ")" || GetLastSymbol() == "1" || GetLastSymbol() == "0")
            {
                textBox1.Text += "|";
            }
        }

        private void pierceArrowButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
                return;

            if (GetLastSymbol() == ")" || GetLastSymbol() == "1" || GetLastSymbol() == "0")
            {
                textBox1.Text += "↓";
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
        }

        private void solutionButton_Click(object sender, EventArgs e)
        {
            
            result = EvaluateExpression(textBox1.Text);
            textBox1.Text = result ? "1" : "0";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            input = Convert.ToString(textBox1.Text);
            

        }

        private bool EvaluateExpression(string expression)
        {
            // Преобразовать выражение в массив символов
            char[] chars = expression.ToCharArray();


            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];

                int currentOperatorIndex = GetOperatorIndex(c);
                if (currentOperatorIndex != -1)
                {
                    // Если текущий символ по индексу меньше чем индекс последнего оператора внутри стека 
                    while (operatorStack.Count > 0
                        && (currentOperatorIndex < GetOperatorIndex(operatorStack.Peek())))
                    {
                        NextCalculation();
                    }
                    // Помещаем текущий символ в стек операторов
                    operatorStack.Push(c);
                }

                else if (c == '(')
                {
                    // Если текущий символ - открывающая скобка, помещаем его в стек операторов
                    operatorStack.Push(c);
                }
                else if (c == ')')
                {
                    // Если текущий символ - закрывающая скобка, обрабатываем операторы до открывающей скобки
                    while (operatorStack.Peek() != '(')
                    {
                        NextCalculation();
                    }

                    // Удаляем открывающую скобку из стека операторов
                    operatorStack.Pop();
                }
                else
                {
                    int number = int.Parse(c.ToString());
                    // Если текущий символ - операнд, помещаем его в стек операндов
                    operand = Convert.ToBoolean(number);
                    operandStack.Push(operand);
                }
            }

            // Обрабатываем оставшиеся операторы
            while (operatorStack.Count > 0)
            {
                NextCalculation();
            }

            // Результат находится в верхнем элементе стека операндов
            return operandStack.Pop();
        }
        // находим индекс внутри массива operatorOrder, если -1,
        // то символ не найден в массиве,а значит это не оператор
        private int GetOperatorIndex(char c)
        {
            return Array.IndexOf(operatorOrder, c);
        }

        private void NextCalculation()
        {
            char op = operatorStack.Pop();
            bool rightOperand = operandStack.Pop();
            bool result = false;
            if (op == '!')
            {
                result = PerformOperation(op, rightOperand);
                AddLog($"{op} {rightOperand} = {result}");
            }
            else
            {
                bool leftOperand = operandStack.Pop();
                // Выполняем операцию и помещаем результат в стек операндов
                result = PerformOperation(op, leftOperand, rightOperand);
                AddLog($" {leftOperand} {op} {rightOperand} = {result}");
            }
            operandStack.Push(result);
        }

        private void AddLog(string line)
        {
            line += "\r\n";
            textBox2.Text += line;
            fileOperator.AppendLine(logPath, line);
        }

       

        private bool PerformOperation(char op, bool operand)
        {
            if (op == '!')
            {
                return !operand;
            }
            else
            {
                throw new ArgumentException("Недопустимый оператор: " + op);
            }
        }

        private bool PerformOperation(char op, bool operand1, bool operand2)
        {
            if (op == '&')
            {
                return operand1 && operand2;
            }
            else if (op == '+')
            {
                return operand1 || operand2;
            }
            else if (op == '→')
            {
                return !operand1 || operand2;
            }
            else if (op == '~')
            {
                return operand1 == operand2;
            }
            else if (op == '|')
            {
                //return !operand1 || !operand2;
                return !(operand1 && operand2);
            }
            else if (op == '↓')
            {
                return !operand1 && !operand2;
            }
            else
            {
                throw new ArgumentException("Недопустимый оператор: " + op);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void sizeButton_Click(object sender, EventArgs e)
        {
            ChangeSize();
        }


        bool isMinWidth = true;

        private void ChangeSizeByFlag()
        {
            if (isMinWidth)
            {
                this.Width = maxWidth;
                isMinWidth = false;
            }
            else
            {
                this.Width = minWidth;
                isMinWidth = true;
            }
        }

        private void ChangeSize()
        {
            if (this.Width == minWidth)
            {
                this.Width = maxWidth;
            }
            else
            {
                this.Width = minWidth;
            }
        }
    }
}
