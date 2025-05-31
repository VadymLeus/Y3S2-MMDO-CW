using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MMDO_CW_CODE
{
    public class ConditionType : MathCore
    {
        private readonly bool _isMinimization;
        public ConditionType(AreaBuilder input, bool isMinimization) : base(input)
        {
            _isMinimization = isMinimization;
        }

        private bool TryParseInputText(string text, out double value, string errorMessageTitle, string errorMessageContentTemplate, params object[] errorMessageArgs)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            string normalizedText = text.Trim().Replace(',', '.');
            if (normalizedText.Contains("/"))
            {
                string[] parts = normalizedText.Split('/');
                if (parts.Length == 2 &&
                    !string.IsNullOrWhiteSpace(parts[0]) &&
                    !string.IsNullOrWhiteSpace(parts[1]))
                {
                    if (double.TryParse(parts[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double numerator) &&
                        double.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double denominator))
                    {
                        if (Math.Abs(denominator) < 1e-9)
                        {
                            string divByZeroError = string.Format(errorMessageContentTemplate, errorMessageArgs) +
                                                    " Помилка: знаменник у дробі не може бути нулем." +
                                                    " Будь ласка, використовуйте числа (роздільник - крапка або кома), дроби (наприклад, 4/10, де знаменник не нуль). " +
                                                    "Порожні поля автоматично вважаються нулями.";
                            MessageBox.Show(divByZeroError, errorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                        value = numerator / denominator;
                        return true;
                    }
                }
            }

            if (double.TryParse(normalizedText, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
            {
                return true;
            }

            string formattedErrorMessage = string.Format(errorMessageContentTemplate, errorMessageArgs) +
                                           " Будь ласка, використовуйте числа (роздільник - крапка або кома) або дроби (наприклад, 4/10). " +
                                           "Порожні поля автоматично вважаються нулями.";
            MessageBox.Show(formattedErrorMessage, errorMessageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        private string BuildObjectiveFunctionString(double[] c, int varCount)
        {
            StringBuilder sb = new StringBuilder(" ");
            for (int i = 0; i < c.Length; i++)
            {
                if (i > 0)
                {
                    if (c[i] >= 0) sb.Append(" + ");
                    else sb.Append(" - ");
                }
                else if (c[i] < 0)
                {
                    sb.Append("-");
                }

                double absC = Math.Abs(c[i]);
                if (absC != 1 || varCount == 1 || (i == 0 && absC == 1 && c.Length > 1 && c[0] == -1))
                {
                    if (absC != 1 || i == 0 && c.Length == 1) sb.Append($"{ToFraction(absC)}·");
                }
                sb.Append($"x{i + 1}");
            }
            sb.AppendLine(_isMinimization ? " → min" : " → max");
            return sb.ToString();
        }

        private string BuildConstraintString(double[,] A, double[] b, string[] signs, int constraintIndex, int varCount)
        {
            StringBuilder sb = new StringBuilder(" ");
            for (int j = 0; j < varCount; j++)
            {
                if (j > 0)
                {
                    if (A[constraintIndex, j] >= 0) sb.Append(" + ");
                    else sb.Append(" - ");
                }
                else if (A[constraintIndex, j] < 0)
                {
                    sb.Append("-");
                }

                double absA = Math.Abs(A[constraintIndex, j]);
                if (absA != 1 || (j == 0 && varCount == 1))
                {
                    if (absA != 1 || j == 0 && varCount == 1) sb.Append($"{ToFraction(absA)}·");
                }
                sb.Append($"x{j + 1}");
            }
            sb.Append($" {signs[constraintIndex]} {ToFraction(b[constraintIndex])}");
            sb.AppendLine();
            return sb.ToString();
        }

        public override void Solve(Panel outputPanel)
        {
            currentYOffset = 10;
            requiredWidth = 0;
            outputPanel.Controls.Clear();

            try
            {
                var varCount = input.ObjectiveFunctionBoxes.Count;
                var constrCount = input.ConstraintBoxes.Count;
                var A = new double[constrCount, varCount];
                var b = new double[constrCount];
                var signs = input.ConstraintSigns.Select(cb => cb.SelectedItem.ToString()).ToArray();
                var c = new double[varCount];
                for (int i = 0; i < varCount; i++)
                {
                    if (!TryParseInputText(input.ObjectiveFunctionBoxes[i].Text, out c[i],
                        "Помилка формату даних цільової функції",
                        $"Помилка введення даних у цільовій функції для x{i + 1}.", i + 1))
                    {
                        return;
                    }
                }

                for (int i = 0; i < constrCount; i++)
                {
                    if (!TryParseInputText(input.ConstraintBoxes[i][varCount].Text, out double rhsValue,
                        "Помилка формату даних обмежень",
                        $"Помилка введення даних у правій частині обмеження {i + 1}.", i + 1))
                    {
                        return;
                    }

                    double signMultiplier = 1.0;
                    if (rhsValue < 0)
                    {
                        signMultiplier = -1.0;
                        string currentSign = signs[i];
                        if (currentSign == "<=") signs[i] = ">=";
                        else if (currentSign == ">=") signs[i] = "<=";
                        MessageBox.Show($"Виявлено від'ємний вільний член у обмеженні {i + 1}. Рядок буде помножено на -1, знак нерівності змінено.", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    for (int j = 0; j < varCount; j++)
                    {
                        if (!TryParseInputText(input.ConstraintBoxes[i][j].Text, out double cellValue,
                            "Помилка формату даних коефіцієнтів",
                            $"Помилка введення даних у коефіцієнті для x{j + 1} в обмеженні {i + 1}.", j + 1, i + 1))
                        {
                            return;
                        }
                        A[i, j] = cellValue * signMultiplier;
                    }
                    b[i] = rhsValue * signMultiplier;
                }

                StringBuilder introText = new StringBuilder();
                introText.Append(BuildObjectiveFunctionString(c, varCount));
                for (int i = 0; i < constrCount; i++)
                {
                    introText.Append(BuildConstraintString(A, b, signs, i, varCount));
                }

                var headerLabel = new Label
                {
                    Text = "Введені дані:",
                    AutoSize = true,
                    Font = new Font("Times New Roman", 12F, FontStyle.Bold),
                    Location = new Point(10, currentYOffset)
                };
                AddControlToPanel(outputPanel, headerLabel);
                currentYOffset += 5;

                var contentLabel = new Label
                {
                    Text = introText.ToString(),
                    AutoSize = true,
                    Font = new Font("Times New Roman", 11F),
                    Location = new Point(10, currentYOffset)
                };
                AddControlToPanel(outputPanel, contentLabel);
                currentYOffset += 15;


                var table = BuildInitialTable(A, b, c, signs, _isMinimization, out int[] basis);
                if (table == null) return;

                ShowTable(outputPanel, table, basis, _isMinimization ? "Початкова симплекс-таблиця (M-задача)" : "Початкова симплекс-таблиця", _isMinimization, -1, -1);
                Application.DoEvents();
                int step = 1;
                int maxSteps = 50;
                bool optimalFound = false;
                bool unbounded = false;
                while (step <= maxSteps)
                {
                    int pivotCol = _isMinimization ?
                        FindPivotColumn(table, delta => delta > 0) : FindPivotColumn(table, delta => delta < 0);
                    if (pivotCol < 0)
                    {
                        optimalFound = true;
                        ShowTable(outputPanel, table, basis, $"Крок {step}: Оптимальний розв'язок знайдено", _isMinimization, -1, -1);
                        break;
                    }

                    int pivotRow = FindPivotRow(table, pivotCol, basis);
                    if (pivotRow < 0)
                    {
                        unbounded = true;
                        ShowTable(outputPanel, table, basis, $"Крок {step}: Цільова функція необмежена", _isMinimization, -1, pivotCol);
                        break;
                    }

                    ShowTable(outputPanel, table, basis, $"Крок {step}: Ітерація", _isMinimization, pivotRow, pivotCol);
                    Application.DoEvents();

                    Pivot(table, pivotRow, pivotCol, ref basis, _isMinimization);
                    step++;
                }

                if (unbounded)
                {
                    this.LastResultSummary = "Цільова функція необмежена";
                    var unboundedLabel = new Label
                    {
                        Text = _isMinimization
                            ? "Цільова функція необмежена на множині допустимих рішень (прямує до -∞)."
                            : "Цільова функція необмежена на множині допустимих рішень (прямує до +∞).",
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        ForeColor = Color.Red,
                        AutoSize = true,
                        Location = new Point(10, currentYOffset)
                    };
                    AddControlToPanel(outputPanel, unboundedLabel);
                    currentYOffset += 15;
                }
                else if (optimalFound)
                {
                    ShowTable(outputPanel, table, basis, $"Крок {step}: Оптимальний розв'язок знайдено", _isMinimization, -1, -1);
                    bool finalMultipleOptimaDetected = false;
                    if (!HasArtificialInBasis(basis, table))
                    {
                        int deltaRowIndex = table.GetLength(0) - 1;
                        for (int j = 0; j < Variables.Count; j++)
                        {
                            bool isBasic = false;
                            for (int k = 0; k < basis.Length; k++)
                            {
                                if (basis[k] == j)
                                {
                                    isBasic = true;
                                    break;
                                }
                            }

                            if (!isBasic)
                            {
                                VariableInfo varInfo = _variablesCache[j];
                                if (varInfo.Type != VariableType.Artificial)
                                {
                                    double deltaValue = table[deltaRowIndex, varInfo.Index];

                                    if (Math.Abs(deltaValue) < TOLERANCE) 
                                    {
                                        finalMultipleOptimaDetected = true;
                                        break; 
                                    }
                                }
                            }
                        }
                    }
                    DisplayResults(outputPanel, table, basis, _isMinimization, finalMultipleOptimaDetected);
                }
                else
                {
                    this.LastResultSummary = $"Досягнуто ліміт ітерацій ({maxSteps})";
                    MessageBox.Show($"Досягнуто максимальну кількість ітерацій ({maxSteps}). Можливо, зациклювання?", "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    bool multipleOptimaIfMaxIterations = false;
                    int currentPivotColTest = _isMinimization ?
                                              FindPivotColumn(table, delta => delta > 0) : FindPivotColumn(table, delta => delta < 0);

                    if (currentPivotColTest < 0 && !HasArtificialInBasis(basis, table))
                    {
                        int deltaRowIndex = table.GetLength(0) - 1;
                        for (int j = 0; j < Variables.Count; j++)
                        {
                            bool isBasic = false;
                            for (int k = 0; k < basis.Length; k++) { if (basis[k] == j) { isBasic = true; break; } }
                            if (!isBasic)
                            {
                                VariableInfo varInfo = _variablesCache[j];
                                if (varInfo.Type != VariableType.Artificial)
                                {
                                    double deltaValue = table[deltaRowIndex, varInfo.Index];
                                    if (Math.Abs(deltaValue) < TOLERANCE)
                                    {
                                        multipleOptimaIfMaxIterations = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    DisplayResults(outputPanel, table, basis, _isMinimization, multipleOptimaIfMaxIterations);
                }
            }
            catch (Exception ex)
            {
                this.LastResultSummary = "Помилка розрахунку";
                MessageBox.Show($"Сталася неочікувана помилка: {ex.Message}\n{ex.StackTrace}", "Загальна помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                outputPanel.AutoScroll = true;
                outputPanel.AutoScrollMinSize = new Size(this.requiredWidth + 20, currentYOffset);
            }
        }
    }
}