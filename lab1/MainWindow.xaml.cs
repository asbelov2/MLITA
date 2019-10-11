using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace lab1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        uint maxBracketLevel = 1;   // Максимальная вложеннность скобок(нужна чтобы знать откуда начать вычислять)
        uint bracketLevel = 1;      // Переменная для определения уровня вложенности скобок
        string realFormula = "";    // Формула в удобном для парсинга виде
        string[] vars = { };        // Множество использованных переменных
        string varsTmp = "";        // Временная строка для переменных, необходимая для исправления бага с добавлением item из ListBox
        public MainWindow()
        {
            InitializeComponent();
        }

        private int CalcString(bool[] varsCase, string expression)
        {
            int iofo = 0;           // Index of first operator
            int ioso = 0;           // Index of second operator
            if (expression == "0" || expression == "1") return int.Parse(expression);
            string formula = expression;

            // NOT /////////////////////////////////////////////////////////////////
            while (formula.IndexOf('!')>=0)
            {
                iofo = formula.IndexOf('!');
                switch (formula[iofo + 1])
                {
                    case '!':
                        formula = formula.Remove(iofo, 2);
                        break;
                    case '0':
                        formula = formula.Remove(iofo, 2);
                        formula = formula.Insert(iofo, "1");
                        break;
                    case '1':
                        formula = formula.Remove(iofo, 2);
                        formula = formula.Insert(iofo, "0");
                        break;
                    default:
                        for(int i = 0; i< vars.Length;i++)
                        {
                            if(vars[i][0]==formula[iofo+1])
                            {
                                formula = formula.Remove(iofo, 2);
                                formula = formula.Insert(iofo, varsCase[i]?"0":"1");
                                break;
                            }
                        }
                        break;
                }
            }

            // AND /////////////////////////////////////////////////////////////////
            while (formula.IndexOf('&') >= 0)
            {
                iofo = formula.IndexOf('&')-1;
                ioso = iofo + 2;
                if((formula[iofo]=='1')|| (formula[iofo] == '0'))
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        formula = formula.Insert(iofo, ((formula[iofo] == '1' ? true : false) && (formula[ioso] == '1' ? true : false))?"1":"0");
                        formula = formula.Remove(iofo + 1, 3);
                    }
                    else
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, ((formula[iofo] == '1' ? true : false) && (varsCase[i] ? true : false))?"1":"0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                formula = formula.Insert(iofo, ((varsCase[i] ? true : false) && (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                    else
                    {
                        int tmp = 0;
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                tmp = i;
                                break;
                            }
                        }
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, ((varsCase[tmp] ? true : false) && (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
            }

            // OR //////////////////////////////////////////////////////////////////
            while (formula.IndexOf('|') >= 0)
            {
                iofo = formula.IndexOf('|') - 1;
                ioso = iofo + 2;
                if ((formula[iofo] == '1') || (formula[iofo] == '0'))
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        formula = formula.Insert(iofo, ((formula[iofo] == '1' ? true : false) || (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                        formula = formula.Remove(iofo + 1, 3);
                    }
                    else
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, ((formula[iofo] == '1' ? true : false) || (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                formula = formula.Insert(iofo, ((varsCase[i] ? true : false) || (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                    else
                    {
                        int tmp = 0;
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                tmp = i;
                                break;
                            }
                        }
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, ((varsCase[tmp] ? true : false) || (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
            }

            // XOR /////////////////////////////////////////////////////////////////
            while (formula.IndexOf('/') >= 0)
            {
                iofo = formula.IndexOf('/') - 1;
                ioso = iofo + 2;
                if ((formula[iofo] == '1') || (formula[iofo] == '0'))
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        formula = formula.Insert(iofo, ((formula[iofo] == '1' ? true : false) ^ (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                        formula = formula.Remove(iofo + 1, 3);
                    }
                    else
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, ((formula[iofo] == '1' ? true : false) ^ (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                formula = formula.Insert(iofo, ((varsCase[i] ? true : false) ^ (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                    else
                    {
                        int tmp = 0;
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                tmp = i;
                                break;
                            }
                        }
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, ((varsCase[tmp] ? true : false) ^ (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
            }

            // IMP /////////////////////////////////////////////////////////////////
            while (formula.IndexOf('>') >= 0)
            {
                iofo = formula.IndexOf('>') - 1;
                ioso = iofo + 2;
                if ((formula[iofo] == '1') || (formula[iofo] == '0'))
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        formula = formula.Insert(iofo, (!(formula[iofo] == '1' ? true : false) || (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                        formula = formula.Remove(iofo + 1, 3);
                    }
                    else
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, (!(formula[iofo] == '1' ? true : false) || (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                formula = formula.Insert(iofo, (!(varsCase[i] ? true : false) || (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                    else
                    {
                        int tmp = 0;
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                tmp = i;
                                break;
                            }
                        }
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, (!(varsCase[tmp] ? true : false) || (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
            }

            // EQ //////////////////////////////////////////////////////////////////
            while (formula.IndexOf('=') >= 0)
            {
                iofo = formula.IndexOf('=') - 1;
                ioso = iofo + 2;
                if ((formula[iofo] == '1') || (formula[iofo] == '0'))
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        formula = formula.Insert(iofo, ((formula[iofo] == '1' ? true : false) == (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                        formula = formula.Remove(iofo + 1, 3);
                    }
                    else
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, ((formula[iofo] == '1' ? true : false) == (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if ((formula[ioso] == '1') || (formula[ioso] == '0'))
                    {
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                formula = formula.Insert(iofo, ((varsCase[i] ? true : false) == (formula[ioso] == '1' ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                    else
                    {
                        int tmp = 0;
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[iofo])
                            {
                                tmp = i;
                                break;
                            }
                        }
                        for (int i = 0; i < vars.Length; i++)
                        {
                            if (vars[i][0] == formula[ioso])
                            {
                                formula = formula.Insert(iofo, ((varsCase[tmp] ? true : false) == (varsCase[i] ? true : false)) ? "1" : "0");
                                formula = formula.Remove(iofo + 1, 3);
                                break;
                            }
                        }
                    }
                }
            }

            if (formula.Length == 1)
            {
                if (formula[0] == '0' || formula[0] == '1')
                {
                    return int.Parse(formula);
                }
                else
                {
                    for (int i = 0; i < vars.Length; i++)
                    {
                        if (vars[i][0] == formula[0])
                        {
                            return varsCase[i] ? 1 : 0;
                        }
                    }
                }
            }
            return -1;
        }

        private int LogicFunc(bool[] varsCase, string expression)
        {
            int ib = 0;             // Index of begin
            int ie = 0;             // Index of end
            uint maxBrLvl = maxBracketLevel;
            string formula = expression.ToUpper();

            while(maxBrLvl >= 2)
            {
                ib = formula.IndexOf(maxBrLvl.ToString());
                ie = formula.IndexOf(')');
                formula = formula.Insert(ib, CalcString(varsCase, formula.Substring(ib+2, ie-ib-2)).ToString());
                ib = formula.IndexOf(maxBrLvl.ToString());
                ie = formula.IndexOf(')');
                formula = formula.Remove(ib,ie-ib+1);
                if(formula.IndexOf(maxBrLvl.ToString())<0)
                    maxBrLvl--;
            }

            formula = CalcString(varsCase,formula).ToString();

            return int.Parse(formula);
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            realFormula = "";
            Formula.Text = realFormula;
            TruthTable.Text = "";
            Left_Bracket.IsEnabled = true;
            Right_Bracket.IsEnabled = false;
            Paste.IsEnabled = true;
            And.IsEnabled = false;
            Or.IsEnabled = false;
            Xor.IsEnabled = false;
            Imp.IsEnabled = false;
            Eq.IsEnabled = false;
        }

        private void NewVar_Click(object sender, RoutedEventArgs e)
        {
            if(VarName.Text!="")
            if (!Vars.Items.Contains(VarName.Text.ToUpper()))
            {
                Vars.Items.Add(VarName.Text.ToUpper());
                string tmp = "";
                varsTmp = (string)Vars.Items[2];
                for (int i = 3; i < Vars.Items.Count; i++)
                {
                    tmp = tmp + " " + Vars.Items[i];
                }
                varsTmp += tmp;
                FormulaName.Content = (string)Vars.Items[2];
                tmp = tmp.Replace(" ", ", ");
                FormulaName.Content += tmp;
                FormulaName.Content = "F( " + FormulaName.Content + " )";
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!Vars.SelectedItem.ToString().EndsWith("0") && !Vars.SelectedItem.ToString().EndsWith("1"))
            {
                Vars.Items.Remove(Vars.SelectedItem);
                string tmp = "";
                if (Vars.Items.Count > 2)
                {
                    varsTmp = (string)Vars.Items[2];
                }
                else
                {
                    varsTmp = "";
                }
                for (int i = 3; i < Vars.Items.Count; i++)
                {
                    tmp = tmp + " " + Vars.Items[i];
                }
                varsTmp += tmp;
                if (Vars.Items.Count > 2)
                {
                    FormulaName.Content = (string)Vars.Items[2];
                }
                else
                {
                    FormulaName.Content = "";
                }
                tmp = tmp.Replace(" ", ", ");
                FormulaName.Content += tmp;
                FormulaName.Content = "F( " + FormulaName.Content + " )";
            }
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            if (Vars.SelectedItem != null)
            {
                foreach (string item in Vars.Items)
                {
                    if ((item == "") || (item == " ")) continue;
                    if (Vars.SelectedItem.ToString().EndsWith(item))
                    {
                        realFormula += item;
                        Formula.Text += item;
                        Paste.IsEnabled = false;
                    }
                }
                if (bracketLevel > 1) Right_Bracket.IsEnabled = true;
                Or.IsEnabled = true;
                And.IsEnabled = true;
                Imp.IsEnabled = true;
                Eq.IsEnabled = true;
                Xor.IsEnabled = true;
                Not.IsEnabled = false;
                Left_Bracket.IsEnabled = false;
            }
        }

        private void Left_Bracket_Click(object sender, RoutedEventArgs e)
        {
            bracketLevel++;
            Formula.Text += "(";
            realFormula += bracketLevel.ToString() + "(";
            maxBracketLevel = (maxBracketLevel < bracketLevel) ? bracketLevel : maxBracketLevel;
            Right_Bracket.IsEnabled = false;
        }

        private void Right_Bracket_Click(object sender, RoutedEventArgs e)
        {
            Formula.Text += ")";
            realFormula += ")";
            bracketLevel--;
            if (bracketLevel <= 1) Right_Bracket.IsEnabled = false;
        }

        private void Or_Click(object sender, RoutedEventArgs e)
        {
            Formula.Text += "v";
            realFormula += "|";
            Or.IsEnabled = false;
            And.IsEnabled = false;
            Imp.IsEnabled = false;
            Eq.IsEnabled = false;
            Xor.IsEnabled = false;
            Right_Bracket.IsEnabled = false;
            Left_Bracket.IsEnabled = true;
            Not.IsEnabled = true;
            Paste.IsEnabled = true;
            if (bracketLevel > 1) Right_Bracket.IsEnabled = true;
        }

        private void And_Click(object sender, RoutedEventArgs e)
        {
            Formula.Text += "^";
            realFormula += "&";
            Or.IsEnabled = false;
            And.IsEnabled = false;
            Imp.IsEnabled = false;
            Eq.IsEnabled = false;
            Xor.IsEnabled = false;
            Right_Bracket.IsEnabled = false;
            Left_Bracket.IsEnabled = true;
            Not.IsEnabled = true;
            Paste.IsEnabled = true;
            if (bracketLevel > 1) Right_Bracket.IsEnabled = true;
        }

        private void Not_Click(object sender, RoutedEventArgs e)
        {
            Formula.Text += "¬";
            realFormula += "!";
            Right_Bracket.IsEnabled = false;
            Left_Bracket.IsEnabled = true;
            if (bracketLevel > 1) Right_Bracket.IsEnabled = true;
        }

        private void Imp_Click(object sender, RoutedEventArgs e)
        {
            Formula.Text += "→";
            realFormula += ">";
            Or.IsEnabled = false;
            And.IsEnabled = false;
            Imp.IsEnabled = false;
            Eq.IsEnabled = false;
            Xor.IsEnabled = false;
            Right_Bracket.IsEnabled = false;
            Left_Bracket.IsEnabled = true;
            Not.IsEnabled = true;
            Paste.IsEnabled = true;
            if (bracketLevel > 1) Right_Bracket.IsEnabled = true;
        }

        private void Eq_Click(object sender, RoutedEventArgs e)
        {
            Formula.Text += "↔";
            realFormula += "=";
            Or.IsEnabled = false;
            And.IsEnabled = false;
            Imp.IsEnabled = false;
            Eq.IsEnabled = false;
            Xor.IsEnabled = false;
            Right_Bracket.IsEnabled = false;
            Left_Bracket.IsEnabled = true;
            Not.IsEnabled = true;
            Paste.IsEnabled = true;
            if (bracketLevel > 1) Right_Bracket.IsEnabled = true;
        }

        private void Xor_Click(object sender, RoutedEventArgs e)
        {
            Formula.Text += "⊕";
            realFormula += "/";
            Or.IsEnabled = false;
            And.IsEnabled = false;
            Imp.IsEnabled = false;
            Eq.IsEnabled = false;
            Xor.IsEnabled = false;
            Right_Bracket.IsEnabled = false;
            Left_Bracket.IsEnabled = true;
            Not.IsEnabled = true;
            Paste.IsEnabled = true;
            if (bracketLevel > 1) Right_Bracket.IsEnabled = true;
        }

        private void VarName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(VarName.Text.Length>1)
            {
                VarName.Text = VarName.Text.Substring(0, 1);
            }
        }

        private void VarName_KeyDown(object sender, KeyEventArgs e)
        {
            if (((int)e.Key > 43) && ((int)e.Key < 70))
            {

            }
            else
            {
                e.Handled = true;
            }
        }

        private void Truth_Table_Click(object sender, RoutedEventArgs e)
        {
            if ((realFormula.Length > 0) && (bracketLevel==1))
            {
                vars = varsTmp.Split(' ');
                TruthTable.Text = "";
                bool[] varsValue = new bool[vars.Length];
                int[][] table = new int[(int)Math.Pow(2, vars.Length)][];

                for (int j = 0; j < vars.Length; j++)
                {
                    TruthTable.Text += vars[j] + "\t";
                }
                TruthTable.Text += "Function\n";

                for (int i = 0; i < table.Length; i++)
                {
                    table[i] = new int[vars.Length];
                    for (int j = 0; j < vars.Length; j++)
                    {
                        varsValue[j] = ((i & (1 << (vars.Length - j - 1))) == 0) ? false : true;
                        TruthTable.Text += (((i & (1 << (vars.Length - j - 1))) == 0) ? "0" : "1") + "\t";
                    }
                    TruthTable.Text += LogicFunc(varsValue, realFormula).ToString() + "\n";
                }
            }
        }
    }
}
