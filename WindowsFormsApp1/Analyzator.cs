using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace WindowsFormsApp1
{
    internal class Analyze
    {
        
        enum State
        {
            S, //начало
            C_1, //введено ключевого слово CONST
            C_2, // введен пробел после CONST
            ID, //идентификатор
            C_4,//значение
            T_1,// двоеточие
            T_2,//выбор типа
            TXT,//текст
            C_5,//выбор между окончанием и продолжением строки
            CH_1, //число без знака
            CH_2, //число со знаком
            CH_3, //число с фиксированной запятой
            CH_4, //целое число со знаком
            CH_6, //ноль
            F, //конец строки
            E //ошибка
        }
       
        public static void Analysis(string userInput, Label errorMessage, TextBox userText, out bool key_flag, out StringBuilder identificators, out StringBuilder constants)
        {
            key_flag = false;
            char curCh;
            int i = 0;
            int j = 0;
            State state = State.S;
            int len = userInput.Length;
            string[] keyword = { "const", "char", "integer", "word", "real", "string" };
            string firstKey = keyword[0];
            const int MIN_INT = -32768, MAX_INT = 32768;

            StringBuilder txt = new StringBuilder();
            StringBuilder type = new StringBuilder();
            identificators = new StringBuilder();
            StringBuilder identificator = new StringBuilder();
            constants = new StringBuilder();
            StringBuilder constant = new StringBuilder();
            string[] idArray;
            userInput = userInput.ToLower();
            
            while (state != State.F && state != State.E && i < len)
            {
                
                curCh = userInput[i];
                j++;
                i++;

               
                switch (state)
                {
                    case State.S:
                        if ((curCh == ' ')&& (j == 1))
                        { 
                            j--; 
                            continue; }
                        if ((curCh == firstKey[0] && j - 1 == 0) || (curCh == firstKey[1] && j - 1 == 1) || (curCh == firstKey[2] && j - 1 == 2)
                            || (curCh == firstKey[3] && j - 1 == 3))
                        {
                            errorMessage.Text = string.Empty;                          
                        }
                        else if (curCh == firstKey[4] && j - 1 == 4)
                        {
                            errorMessage.Text = string.Empty;
                            state = State.C_1;
                        }
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидалось {1} в слове CONST. Символ {2}", curCh, firstKey[j - 1], i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i-1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.C_1:
                        if (curCh == ' ')
                        {
                           state = State.C_2;
                        }                       
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидался пробел после CONST. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.C_2:
                        type.Clear();
                        if (curCh == ' ')
                        {
                            continue;
                        }
                        if ((curCh >= 'a') && (curCh <= 'z'))
                        {                            
                            identificator.Append(curCh);
                            state = State.ID;
                        }
                        else if((curCh < 'a') || (curCh > 'z'))
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Идентификатор должен начинаться с латинской буквы. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидался пробел после CONST. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.ID:
                        if (((curCh >= 'a') && (curCh <= 'z')) || ((curCh >= '0') && (curCh <= '9')))
                        {
                            identificator.Append(curCh);
                            continue;
                        }
                        //else if ((curCh == ' ') && ((userInput[i] == '=') || (userInput[i] == ':')))
                        //{
                        //    continue;
                        //}
                        else if (curCh == '=')
                        {
                            bool doppler = false;
                            type.Append("NONE");
                            idArray = identificators.ToString().Split(',');
                            if (idArray.Length > 1)
                            {                            
                                    if (idArray.Contains(identificator.ToString()))
                                    {
                                        
                                        errorMessage.Text = String.Format("Семантическая ошибка. Идектификатор {0} уже был использован ранее. " +
                                            "Нельзя использовать одинаковые идентификаторы", identificator);
                                        
                                        if (userInput[i] == ' ')
                                        {
                                        userText.HideSelection = false;
                                        userText.SelectionStart = i - identificator.Length - 1;
                                         userText.SelectionLength = identificator.Length;
                                            identificator.Clear();
                                            doppler = true;
                                            state = State.E;
                                            
                                         }
                                        else if ((userInput[i] != ' '))
                                        {
                                        userText.HideSelection = false;
                                        userText.SelectionStart = i - identificator.Length ;
                                            userText.SelectionLength = identificator.Length;
                                            identificator.Clear();
                                            doppler = true;
                                            state = State.E;
                                            
                                        }
                                        break;
                                }
                            }
                             if (identificator.Length > 8)
                            {
                               state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Длина идентификатора больше " + 8 + ".";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear(); 
                                

                            }
                            else if (identificator.ToString() == "const")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом CONST.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length-1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else if (identificator.ToString() == "string")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом STRING.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();
                            }
                            else if (identificator.ToString() == "real")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом REAL.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else if (identificator.ToString() == "word")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом WORD.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else if (identificator.ToString() == "integer")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом INTEGER.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();
                            }
                            else
                            {
                                if (doppler == false)
                                {
                                    identificators.Append(identificator + ",");
                                    identificator = identificator.Clear();
                                    state = State.C_4;
                                }
                            }                          
                        }
                        else if (curCh == ':')
                        {
                            idArray = identificators.ToString().Split(',');
                            bool doppler = false;
                            if (idArray.Length > 1)
                            {                              
                                    if (idArray.Contains(identificator.ToString()))
                                    {

                                    errorMessage.Text = String.Format("Семантическая ошибка. Идектификатор {0} уже был использован ранее. " +
                                       "Нельзя использовать одинаковые идентификаторы", identificator);
                                    userText.HideSelection = false;
                                   
                                        userText.SelectionStart = i - identificator.Length - 1;
                                        userText.SelectionLength = identificator.Length;
                                        identificator.Clear();
                                        doppler = true;
                                        state = State.E;
                                        
                                    
                                    break;
                                }
                            }
                            if (identificator.Length > 8)
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Длина идентификатора больше " + 8 + ".";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else if (identificator.ToString() == "const")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом CONST.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else if (identificator.ToString() == "char")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом CHAR.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else if (identificator.ToString() == "string")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом STRING.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();
                            }
                            else if (identificator.ToString() == "real")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом REAL.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else if (identificator.ToString() == "word")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом WORD.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else if (identificator.ToString() == "integer")
                            {
                                state = State.E;
                                errorMessage.Text = "Семантическая ошибка. Идентификатор совпадает с ключевым словом INTEGER.";
                                userText.HideSelection = false;
                                userText.SelectionStart = i - identificator.Length - 1;
                                userText.SelectionLength = identificator.Length;
                                identificator.Clear();

                            }
                            else 
                            {
                                if (doppler == false)
                                {
                                    identificators.Append(identificator + ",");
                                    identificator = identificator.Clear();
                                    state = State.T_1;
                                }
                            }
                        }
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидаются: латинские буквы, или цифры, или \'=\', или \':\'.  Символ {0}", i); //или  знак '=', или знак ':'
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.C_4:
                        if (curCh == ' ')
                        {
                            continue;
                        }
                        else if (curCh == '\'')
                            state = State.TXT;
                        else if ((curCh >= '0') && (curCh <= '9'))
                        {
                            constant.Append(curCh);
                            state = State.CH_1;
                        }

                        else if ((curCh == '+') || (curCh == '-'))
                        {
                            constant.Append(curCh);
                            state = State.CH_2;
                        }
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Необходимо ввести: цифру, или знак '+', или знак '-', или '(одинарную кавычку). \nСимвол {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.TXT:
                        if (curCh == '\'')
                        {
                            if (type.ToString() == "CHAR")
                            {
                                if (txt.Length == 1)
                                {
                                    constants.Append(txt + "\n");
                                    txt.Clear();
                                    state = State.C_5;
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Значение типа CHAR должно содержать один символ.");
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - txt.Length - 2;
                                    userText.SelectionLength = txt.Length + 2;
                                }
                            }
                            else if (type.ToString() == "STRING")
                            {
                                if (txt.Length <= 256)
                                {
                                    constants.Append(txt + "\n");
                                    state = State.C_5;
                                    txt.Clear();
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Значение типа STRING может содержать 0-256 символов.");
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - txt.Length - 2;
                                    userText.SelectionLength = txt.Length + 2;
                                }
                            }
                            else if (type.ToString() == "NONE")
                            {
                                constants.Append(txt + "\n");
                                state = State.C_5;
                                txt.Clear();
                            }
                            else
                            {
                                state = State.E;
                                errorMessage.Text = string.Format("Семантическая ошибка: Тип данных не соответствует константе. Ожидается константа типа {0}", type);
                                userText.HideSelection = false;
                                userText.SelectionStart = i - txt.Length - 2;
                                userText.SelectionLength = txt.Length +2;
                            }
                            type.Clear();
                        }

                        else if ((curCh == '\n') || ((i == len) && (curCh == ';')))
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Текст должен закрываться одинарной кавычкой. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        else
                        {
                            txt.Append(curCh);
                            continue;
                        }                       
                        break;
                    case State.CH_1:

                        if ((curCh >= '0') && (curCh <= '9'))
                        {
                            constant.Append(curCh);
                            continue;
                        }
                            
                        else if ((curCh == '.'))
                        {
                            constant.Append(curCh);
                            state = State.CH_3;
                        }    
                           

                        else if ((curCh == ','))
                        {
                            if (type.ToString() == "WORD")
                            {
                                int constValue;
                                if (int.TryParse(constant.ToString(), out constValue))
                                {
                                    if ((constValue < 256)&&(constValue >= 0))
                                    {
                                        constants.Append(constant + "\n");
                                        constant.Clear();
                                        state = State.C_2;
                                    }
                                    else
                                    {
                                        state = State.E;
                                        errorMessage.Text = string.Format("Семантическая ошибка: Целое число без знака вышло за допустимые границы: от 0 до 256. Символ {0}", i);
                                        userText.HideSelection = false;
                                        userText.SelectionStart = i - constant.Length - 1;
                                        userText.SelectionLength = constant.Length;
                                    }
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается целое число без знака. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "REAL")
                            {
                                int constValue;
                                if (int.TryParse(constant.ToString(), out constValue))
                                {
                                    constants.Append(constant + "\n");
                                    constant.Clear();
                                    state = State.C_2;
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается число с фиксированной запятой. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "INTEGER")
                            {
                                int constValue;
                                if (int.TryParse(constant.ToString(), out constValue))
                                {
                                    if ((constValue <= MAX_INT) && (constValue >= MIN_INT))
                                    {
                                        constants.Append(constant + "\n");
                                        constant.Clear();
                                        state = State.C_2;
                                    }
                                    else
                                    {
                                        state = State.E;
                                        errorMessage.Text = string.Format("Семантическая ошибка: Число вышло за допустимые границы: от -32678 до +32678. Символ {0}", i);
                                        userText.HideSelection = false;
                                        userText.SelectionStart = i - constant.Length - 1;
                                        userText.SelectionLength = constant.Length;
                                    }
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается целое число. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "NONE")
                            {
                                constants.Append(constant + "\n");
                                constant.Clear();
                                state = State.C_2;
                            }
                            else
                            {
                                state = State.E;
                                errorMessage.Text = string.Format("Семантическая ошибка: Ожидается константа типа {0}.", type);
                                userText.HideSelection = false;
                                userText.SelectionStart = i - constant.Length - 1;
                                userText.SelectionLength = constant.Length;
                            }
                        }
                            
                        else if (curCh == ';')
                        {
                            if (type.ToString() == "WORD")
                            {
                                int constValue;
                                if (int.TryParse(constant.ToString(), out constValue))
                                {
                                    if ((constValue < 256) && (constValue >= 0))
                                    {
                                        constants.Append(constant + "\n");
                                        constant.Clear();
                                        state = State.F;
                                    }
                                    else
                                    {
                                        state = State.E;
                                        errorMessage.Text = string.Format("Семантическая ошибка: Целое число без знака вышло за допустимые границы: от 0 до 256. Символ {0}", i);
                                        userText.HideSelection = false;
                                        userText.SelectionStart = i - constant.Length - 1;
                                        userText.SelectionLength = constant.Length;
                                    }
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается целое число без знака. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "REAL")
                            {
                                int constValue;
                                if (int.TryParse(constant.ToString(), out constValue))
                                {
                                    constants.Append(constant + "\n");
                                    constant.Clear();
                                    state = State.F;
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается число с фиксированной запятой. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "INTEGER")
                            {
                                int constValue;
                                if (int.TryParse(constant.ToString(), out constValue))
                                {
                                    if ((constValue <= MAX_INT) && (constValue >= MIN_INT))
                                    {
                                        constants.Append(constant + "\n");
                                        constant.Clear();
                                        state = State.F;
                                    }
                                    else
                                    {
                                        state = State.E;
                                        errorMessage.Text = string.Format("Семантическая ошибка: Число вышло за допустимые границы: от -32678 до +32678. Символ {0}", i);
                                        userText.HideSelection = false;
                                        userText.SelectionStart = i - constant.Length - 1;
                                        userText.SelectionLength = constant.Length;
                                    }
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается целое число. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "NONE")
                            {
                                constants.Append(constant + "\n");
                                constant.Clear();
                                state = State.F;
                            }
                            else
                            {
                                state = State.E;
                                errorMessage.Text = string.Format("Семантическая ошибка: Ожидается константа типа {0}.", type);
                                userText.HideSelection = false;
                                userText.SelectionStart = i - constant.Length - 1;
                                userText.SelectionLength = constant.Length;
                            }
                        }
                            
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Необходимо ввести: цифру, или ';', или ',', или '.'. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.CH_2:
                        if (curCh == '0')
                        {
                            constant.Append(curCh);
                            state = State.CH_6;
                        }
                        else if ((curCh >= '1') && (curCh <= '9'))
                        {
                            constant.Append(curCh);
                            state = State.CH_4;
                        }
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Необходимо ввести: цифру. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;

                    case State.CH_3:
                        if ((curCh >= '0') && (curCh <= '9'))
                        {
                            constant.Append(curCh);
                            continue;
                        }
                            
                        else if ((curCh == ','))
                        {
                            if (type.ToString() == "REAL")
                            {
                                float constValue;
                                if (float.TryParse(constant.ToString().Replace('.', ','), out constValue))
                                {
                                    constants.Append(constant + "\n");
                                    constant.Clear();
                                    state = State.C_2;
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается число с фиксированной запятой. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "NONE")
                            {
                                constants.Append(constant + "\n");
                                constant.Clear();
                                state = State.C_2;
                            }
                            else
                            {
                                state = State.E;
                                errorMessage.Text = string.Format("Семантическая ошибка: Ожидается константа типа {0}.", type);
                                userText.HideSelection = false;
                                userText.SelectionStart = i - constant.Length - 1;
                                userText.SelectionLength = constant.Length;
                            }
                        }
                        else if (curCh == ';')
                            if (type.ToString() == "REAL")
                            {
                                float constValue;
                                if (float.TryParse(constant.ToString().Replace('.', ','), out constValue))
                                {
                                    constants.Append(constant + "\n");
                                    constant.Clear();
                                    state = State.F;
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается число с фиксированной запятой. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "NONE")
                            {
                                constants.Append(constant + "\n");
                                constant.Clear();
                                state = State.F;
                            }
                            else
                            {
                                state = State.E;
                                errorMessage.Text = string.Format("Семантическая ошибка: Ожидается константа типа {0}.", type);
                                userText.HideSelection = false;
                                userText.SelectionStart = i - constant.Length - 1;
                                userText.SelectionLength = constant.Length;
                            }
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Необходимо ввести: цифру, или ';', или ','. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.CH_4:
                        if ((curCh >= '0') && (curCh <= '9'))
                        {
                            constant.Append(curCh);
                            continue;
                        }   
                        else if ((curCh == '.'))
                        {
                            constant.Append(curCh);
                            state = State.CH_3;
                        }
                        else if (curCh == ',')
                        {
                            if (type.ToString() == "INTEGER")
                            {
                                int constValue;
                                if (int.TryParse(constant.ToString(), out constValue))
                                {
                                    if ((constValue <= MAX_INT) && (constValue >= MIN_INT))
                                    {
                                        constants.Append(constant + "\n");
                                        constant.Clear();
                                        state = State.C_2;
                                    }
                                    else
                                    {
                                        state = State.E;
                                        errorMessage.Text = string.Format("Семантическая ошибка: Число вышло за допустимые границы: от -32678 до +32678. Символ {0}", i);
                                        userText.HideSelection = false;
                                        userText.SelectionStart = i - constant.Length - 1;
                                        userText.SelectionLength = constant.Length;
                                    }
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается целое число. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "NONE")
                            {
                                constants.Append(constant + "\n");
                                constant.Clear();
                                state = State.C_2;
                            }
                            else
                            {
                                state = State.E;
                                errorMessage.Text = string.Format("Семантическая ошибка: Ожидается константа типа {0}.", type);
                                userText.HideSelection = false;
                                userText.SelectionStart = i - constant.Length - 1;
                                userText.SelectionLength = constant.Length;
                            }
                        }
                        else if (curCh == ';')
                        {
                            if (type.ToString() == "INTEGER")
                            {
                                int constValue;
                                if (int.TryParse(constant.ToString(), out constValue))
                                {
                                    if ((constValue <= MAX_INT) && (constValue >= MIN_INT))
                                    {
                                        constants.Append(constant + "\n");
                                        constant.Clear();
                                        state = State.F;
                                    }
                                    else
                                    {
                                        state = State.E;
                                        errorMessage.Text = string.Format(" Семантическая ошибка: Число вышло за допустимые границы: от -32678 до +32678. Символ {0}", i);
                                        userText.HideSelection = false;
                                        userText.SelectionStart = i - constant.Length - 1;
                                        userText.SelectionLength = constant.Length;
                                    }
                                }
                                else
                                {
                                    state = State.E;
                                    errorMessage.Text = string.Format("Семантическая ошибка: Ожидается целое число. Символ {0}", i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - constant.Length - 1;
                                    userText.SelectionLength = constant.Length;
                                }
                            }
                            else if (type.ToString() == "NONE")
                            {
                                constants.Append(constant + "\n");
                                constant.Clear();
                                state = State.F;
                            }
                            else
                            {
                                state = State.E;
                                errorMessage.Text = string.Format("Семантическая ошибка: Ожидается константа типа {0}.", type);
                                userText.HideSelection = false;
                                userText.SelectionStart = i - constant.Length - 1;
                                userText.SelectionLength = constant.Length;
                            }
                        }

                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Необходимо ввести: цифру, или ';', или '.'. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.CH_6:
                        if (curCh == '.')
                            state = State.CH_3;
                        else if (curCh == ',')
                            state = State.C_2;
                        else if (curCh == ';')
                            state = State.F;
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Необходимо ввести: ';', или ',',или '.'. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.C_5:
                        if (curCh == ',')
                            state = State.C_2;
                        else if (curCh == ';')
                            state = State.F;
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Необходимо ввести: ';' или ','. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.T_1:
                        
                        bool flag = true;
                        string reference = "";
                        if (curCh == ' ')
                            continue;
                        else if (curCh == 'i')
                        {
                            reference = "integer";
                            for (int l = 0; l < reference.Length; l++)
                            {
                                if ((curCh == reference[l]) && (i + 1 < len) && (l != reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                    i++;
                                }
                                else if ((curCh == reference[l]) && (i + 1 < len) && (l == reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                }
                                else
                                {
                                    
                                    errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидалось {1} в слове INTEGER. Символ {2}", curCh, reference[l], i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - 1;
                                    userText.SelectionLength = 1;
                                    flag = false;
                                    break;
                                }
                                
                            }
                            if (flag)
                            {
                                type.Append("INTEGER");
                                state = State.T_2; 
                                
                            }
                            else state = State.E;
                        }
                        else if (curCh == 'w')
                        {
                            reference = "word";
                            for (int l = 0; l < reference.Length; l++)
                            {
                                if ((curCh == reference[l]) && (i + 1 < len) && (l != reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                    i++;
                                }
                                else if ((curCh == reference[l]) && (i + 1 < len) && (l == reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                }
                                else
                                {
                                    
                                    errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидалось {1} в слове WORD. Символ {2}", curCh, reference[l], i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - 1;
                                    userText.SelectionLength = 1;
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                type.Append("WORD");
                                state = State.T_2;

                            }
                            else state = State.E;
                        }
                        else if (curCh== 'r')
                        {
                            reference = "real";
                            for (int l = 0; l < reference.Length; l++)
                            {
                                if ((curCh == reference[l]) && (i + 1 < len) && (l != reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                    i++;
                                }
                                else if ((curCh == reference[l]) && (i + 1 < len) && (l == reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                }
                                else
                                {
                                   
                                    errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидалось {1} в слове REAL. Символ {2}", curCh, reference[l], i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - 1;
                                    userText.SelectionLength = 1;
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                type.Append("REAL");
                                state = State.T_2;

                            }
                            else state = State.E;
                        }
                        else if (curCh == 'c')
                        {
                            reference = "char";
                            for (int l = 0; l < reference.Length; l++)
                            {
                                if ((curCh == reference[l]) && (i + 1 < len) && (l != reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                    i++;
                                }
                                else if ((curCh == reference[l]) && (i + 1 < len) && (l == reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                }
                                else
                                {
                                    
                                    errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидалось {1} в слове CHAR. Символ {2}", curCh, reference[l], i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - 1;
                                    userText.SelectionLength = 1;
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                type.Append("CHAR");
                                state = State.T_2;

                            }
                            else state = State.E;
                        }
                        else if (curCh == 's')
                        {
                            reference = "string";
                            for (int l = 0; l < reference.Length; l++)
                            {
                                if ((curCh == reference[l]) && (i + 1 < len) && (l != reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                    i++;
                                }
                                else if ((curCh == reference[l]) && (i + 1 < len) && (l == reference.Length - 2))
                                {
                                    curCh = userInput[i];
                                }
                                else
                                {                                  
                                    errorMessage.Text = string.Format("Синтаксическая ошибка: Ожидалось {1} в слове STRING. Символ {2}", curCh, reference[l], i);
                                    userText.HideSelection = false;
                                    userText.SelectionStart = i - 1;
                                    userText.SelectionLength = 1;
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                type.Append("STRING");
                                state = State.T_2;

                            }
                            else state = State.E;
                        }
                        else
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: После ':' ожидается ввод типа INTEGER|REAL|CHAR|STRING|WORD. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }

                        break;
                    case State.T_2:
                        if (curCh == ' ')
                            continue;
                        else if (curCh == '=')
                            state = State.C_4;
                        else                           
                        {
                            state = State.E;
                            errorMessage.Text = string.Format("Синтаксическая ошибка: Допустимы пробелы и знак '='. Символ {0}", i);
                            userText.HideSelection = false;
                            userText.SelectionStart = i - 1;
                            userText.SelectionLength = 1;
                        }
                        break;
                    case State.F:
                        
                        
                        break;
                    case State.E:
                        break;
                }
            }
            if (state == State.F)
            {
                errorMessage.ForeColor = System.Drawing.Color.Black;
                errorMessage.Text = "Цепочка принадлежит языку";              
                key_flag = true;
            }
        }
    }
}
