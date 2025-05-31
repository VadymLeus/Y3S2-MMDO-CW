using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace MMDO_CW_CODE
{
    public abstract class MathCore
    {
        protected const double M_VALUE = 1000.0;
        protected const double TOLERANCE = 1e-9;
        protected const double PIVOT_TOLERANCE = 1e-12;
        protected const int MAX_DENOMINATOR = 1000;
        protected const int Y_OFFSET_STEP = 10;
        protected const int EXTRA_Y_OFFSET = 5;
        public string LastResultSummary { get; protected set; }
        protected List<VariableInfo> Variables { get; private set; }
        protected Dictionary<int, VariableInfo> _variablesCache;
        protected readonly AreaBuilder input;
        protected int currentYOffset = Y_OFFSET_STEP;
        protected int requiredWidth = 0; 

        protected MathCore(AreaBuilder input)
        {
            this.input = input;
            Variables = new List<VariableInfo>();
            _variablesCache = new Dictionary<int, VariableInfo>();
        }

        public abstract void Solve(Panel outputPanel);
        protected double[,] BuildInitialTable(double[,] A, double[] b, double[] c, string[] signs, bool isMin, out int[] basis)
        {
            InitializeVariables(c);
            ProcessNegativeRHS(ref A, ref b, ref signs);

            var counts = CountConstraintTypes(signs);
            var table = CreateInitialTable(A, b, counts, out basis);
            AddSlackAndArtificialVariables(table, signs, ref basis);
            RecalculateObjectiveRow(table, basis, isMin);

            return table;
        }

        private void InitializeVariables(double[] c)
        {
            Variables.Clear();
            _variablesCache.Clear();

            for (int j = 0; j < c.Length; j++)
            {
                var variable = new VariableInfo($"x{j + 1}", VariableType.Original, j, c[j]);
                Variables.Add(variable);
                _variablesCache[j] = variable;
            }
        }

        private void ProcessNegativeRHS(ref double[,] A, ref double[] b, ref string[] signs)
        {
            for (int i = 0; i < b.Length; i++)
            {
                if (b[i] < 0)
                {
                    b[i] *= -1;
                    for (int j = 0; j < A.GetLength(1); j++)
                        A[i, j] *= -1;
                    if (signs[i] == "<=")
                        signs[i] = ">=";
                    else if (signs[i] == ">=")
                        signs[i] = "<=";
                }
            }
        }

        private (int slack, int surplus, int artificial) CountConstraintTypes(string[] signs)
        {
            int slack = 0, surplus = 0, artificial = 0;
            foreach (var sign in signs)
            {
                switch (sign)
                {
                    case "<=": slack++; break;
                    case ">=": surplus++; artificial++; break;
                    default: artificial++; break;
                }
            }
            return (slack, surplus, artificial);
        }

        private double[,] CreateInitialTable(double[,] A, double[] b, (int slack, int surplus, int artificial) counts, out int[] basis)
        {
            int m = A.GetLength(0);
            int n = A.GetLength(1);
            int totalVars = n + counts.slack + counts.surplus + counts.artificial;

            basis = new int[m];
            var table = new double[m + 1, totalVars + 1];
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                    table[i, j] = A[i, j];
                table[i, totalVars] = b[i];
            }

            return table;
        }

        private void AddSlackAndArtificialVariables(double[,] table, string[] signs, ref int[] basis)
        {
            int n = Variables.Count(v => v.Type == VariableType.Original);
            int sIndex = 1, aIndex = 1;
            int currentVarIndex = n;
            for (int i = 0; i < signs.Length; i++)
            {
                switch (signs[i])
                {
                    case "<=":
                        AddVariable(table, i, currentVarIndex++, $"s{sIndex++}", VariableType.Slack, ref basis, true);
                        break;
                    case ">=":
                        AddVariable(table, i, currentVarIndex++, $"s{sIndex++}", VariableType.Surplus, ref basis, false);
                        AddVariable(table, i, currentVarIndex++, $"a{aIndex++}", VariableType.Artificial, ref basis, true);
                        break;
                    default:
                        AddVariable(table, i, currentVarIndex++, $"a{aIndex++}", VariableType.Artificial, ref basis, true);
                        break;
                }
            }
        }

        private void AddVariable(double[,] table, int row, int index, string name, VariableType type, ref int[] basis, bool isBasic)
        {
            table[row, index] = (type == VariableType.Surplus) ? -1 : 1;
            var variable = new VariableInfo(name, type, index);
            Variables.Add(variable);
            _variablesCache[index] = variable;
            if (isBasic) basis[row] = index;
        }

        protected void RecalculateObjectiveRow(double[,] table, int[] basis, bool isMin)
        {
            int m = basis.Length;
            int totalVars = Variables.Count;
            int lastRow = m;

            for (int j = 0; j <= totalVars; j++)
            {
                double zj = 0;
                for (int i = 0; i < m; i++)
                    zj += _variablesCache[basis[i]].GetEffectiveCost(M_VALUE, isMin) * table[i, j];
                if (j < totalVars)
                    zj -= _variablesCache[j].GetEffectiveCost(M_VALUE, isMin);
                table[lastRow, j] = zj;
            }
        }

        protected int FindPivotColumn(double[,] table, Func<double, bool> condition)
        {
            int deltaRow = table.GetLength(0) - 1;
            int totalVars = table.GetLength(1) - 1;
            int pivotCol = -1;
            double extremeDelta = 0;
            for (int j = 0; j < totalVars; j++)
            {
                if (_variablesCache[j].Type == VariableType.Artificial) continue;
                double currentDelta = table[deltaRow, j];
                if (condition(currentDelta) &&
                    (pivotCol == -1 || Math.Abs(currentDelta) > Math.Abs(extremeDelta)))
                {
                    extremeDelta = currentDelta;
                    pivotCol = j;
                }
            }
            if (pivotCol != -1) return pivotCol;
            for (int j = 0; j < totalVars; j++)
            {
                if (_variablesCache[j].Type != VariableType.Artificial) continue;
                double currentDelta = table[deltaRow, j];
                if (condition(currentDelta) &&
                   (pivotCol == -1 || Math.Abs(currentDelta) > Math.Abs(extremeDelta)))
                {
                    extremeDelta = currentDelta;
                    pivotCol = j;
                }
            }
            return pivotCol;
        }

        protected int FindPivotRow(double[,] table, int pivotCol, int[] basis)
        {
            int rows = table.GetLength(0) - 1;
            int rhsCol = table.GetLength(1) - 1;
            int pivotRow = -1;
            double minRatio = double.MaxValue;
            for (int i = 0; i < rows; i++)
            {
                double elem = table[i, pivotCol];
                if (elem <= TOLERANCE) continue;

                double rhs = table[i, rhsCol];
                double ratio = Math.Abs(rhs) < TOLERANCE ? 0 : rhs / elem;

                if (ratio < minRatio - TOLERANCE ||
                    (Math.Abs(ratio - minRatio) < TOLERANCE && basis[i] < (pivotRow != -1 ? basis[pivotRow] : int.MaxValue)))
                {
                    minRatio = ratio;
                    pivotRow = i;
                }
            }
            return pivotRow;
        }

        protected void Pivot(double[,] table, int pivotRow, int pivotCol, ref int[] basis, bool isMin)
        {
            double pivotVal = table[pivotRow, pivotCol];
            if (Math.Abs(pivotVal) < PIVOT_TOLERANCE)
            {
                MessageBox.Show($"Warning: Small pivot element [{pivotRow},{pivotCol}] = {pivotVal}. Possible precision loss.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            for (int j = 0; j < table.GetLength(1); j++)
                table[pivotRow, j] /= pivotVal;
            table[pivotRow, pivotCol] = 1.0;
            basis[pivotRow] = pivotCol;

            for (int i = 0; i < table.GetLength(0); i++)
            {
                if (i == pivotRow) continue;
                double multiplier = table[i, pivotCol];
                if (Math.Abs(multiplier) < TOLERANCE) continue;
                for (int j = 0; j < table.GetLength(1); j++)
                    table[i, j] -= multiplier * table[pivotRow, j];
                table[i, pivotCol] = 0.0;
            }
        }

        protected void ShowTable(Panel panel, double[,] table, int[] basis, string title, bool isMin,
                               int pivotRowHighlight = -1, int pivotColHighlight = -1)
        {
            AddTitleLabel(panel, title);

            var dgv = CreateDataGridView(panel, panel.Width - 40);
            ConfigureDataGridViewColumns(dgv, isMin);

            AddCjRow(dgv, isMin);
            AddMainTableRows(dgv, table, basis, isMin);
            AddDeltaRow(dgv, table, basis, isMin);
            HighlightElements(dgv, pivotRowHighlight, pivotColHighlight);
            AddControlToPanel(panel, dgv);

            AddDeltaCalculations(panel, table, basis, isMin);
            AddPivotInfo(panel, table, basis, isMin);
        }

        private Label CreateLabel(Panel panel, string text, Font font, Color? color = null)
        {
            return new Label
            {
                Text = text,
                Font = font,
                ForeColor = color ?? SystemColors.ControlText,
                AutoSize = true,
                Location = new Point(10, currentYOffset)
            };
        }

        private void AddTitleLabel(Panel panel, string title)
        {
            var label = CreateLabel(panel, title, new Font("Arial", 14, FontStyle.Bold));
            AddControlToPanel(panel, label, 15);
        }

        private DataGridView CreateDataGridView(Panel panel, int width)
        {
            var dgv = new DataGridView
            {
                Font = new Font("Arial", 10),
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                Width = width,
                Location = new Point(10, currentYOffset),
                ScrollBars = ScrollBars.Both,
                ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText,
                AllowUserToResizeColumns = true
            };

            dgv.ColumnHeadersVisible = true;
            dgv.RowHeadersVisible = false;

            return dgv;
        }

        private void ConfigureDataGridViewColumns(DataGridView dgv, bool isMin)
        {
            dgv.Columns.Clear();

            dgv.Columns.Add("BasisCol", "Базис");
            dgv.Columns.Add("CostCol", "C(б)");

            var sortedVariables = Variables
                .OrderBy(v => GetVariableTypeOrder(v.Type))
                .ThenBy(v => v.Index);

            foreach (var varInfo in sortedVariables)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    HeaderText = varInfo.Name,
                    Name = $"Col{varInfo.Index}",
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };
                dgv.Columns.Add(col);
            }

            dgv.Columns.Add("ColRHS", "RHS");

            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.Visible = true;
                column.Frozen = false;
            }
        }

        private int GetVariableTypeOrder(VariableType type)
        {
            switch (type)
            {
                case VariableType.Original: return 0;
                case VariableType.Slack:
                case VariableType.Surplus: return 1;
                case VariableType.Artificial: return 2;
                default: return 3;
            }
        }

        private void AddCjRow(DataGridView dgv, bool isMin)
        {
            var cjValues = new List<string> { "", "" };
            var sortedVariables = Variables
                .OrderBy(v => GetVariableTypeOrder(v.Type))
                .ThenBy(v => v.Index);
            cjValues.AddRange(sortedVariables.Select(v => v.GetSymbolicCost(isMin)));
            cjValues.Add("");

            AddDataGridViewRow(dgv, cjValues.ToArray(), FontStyle.Italic, Color.LightGray);
        }

        private void AddMainTableRows(DataGridView dgv, double[,] table, int[] basis, bool isMin)
        {
            int rhsColIndex = Variables.Count;
            for (int i = 0; i < basis.Length; i++)
            {
                var basisVar = _variablesCache[basis[i]];
                var rowValues = new List<string> { basisVar.Name, basisVar.GetSymbolicCost(isMin) };

                var sortedVariables = Variables
                    .OrderBy(v => GetVariableTypeOrder(v.Type))
                    .ThenBy(v => v.Index);
                foreach (var varInfo in sortedVariables)
                {
                    rowValues.Add(ToFraction(table[i, varInfo.Index]));
                }
                rowValues.Add(ToFraction(table[i, rhsColIndex]));

                AddDataGridViewRow(dgv, rowValues.ToArray());
            }
        }

        private void AddDeltaRow(DataGridView dgv, double[,] table, int[] basis, bool isMin)
        {
            var deltaValues = new List<string> { "Δ=Zj-Cj", "" };
            var sortedVariables = Variables
               .OrderBy(v => GetVariableTypeOrder(v.Type))
               .ThenBy(v => v.Index);
            deltaValues.AddRange(sortedVariables.Select(v =>
                CalculateOnlyFinalSymbolicDelta(v.Index, table, basis, isMin)));
            double z_constantPart = 0;
            double z_mCoefficient = 0;
            int z_rhs_col_index_in_table = Variables.Count;
            for (int i = 0; i < basis.Length; i++)
            {
                var basisVarInfo = _variablesCache[basis[i]];
                double basisVarRhsValue = table[i, z_rhs_col_index_in_table];

                if (basisVarInfo.Type == VariableType.Artificial)
                {
                    z_mCoefficient += (isMin ? 1.0 : -1.0) * basisVarRhsValue;
                }
                else
                {
                    z_constantPart += basisVarInfo.OriginalCost * basisVarRhsValue;
                }
            }
            deltaValues.Add(FormatDeltaToString(z_constantPart, z_mCoefficient));
            AddDataGridViewRow(dgv, deltaValues.ToArray(), FontStyle.Bold);
        }

        private void AddDataGridViewRow(DataGridView dgv, string[] values, FontStyle style = FontStyle.Regular, Color? backColor = null)
        {
            int rowIndex = dgv.Rows.Add(values);
            dgv.Rows[rowIndex].DefaultCellStyle.Font = new Font(dgv.Font, style);

            if (backColor.HasValue)
                dgv.Rows[rowIndex].DefaultCellStyle.BackColor = backColor.Value;
        }

        private void HighlightElements(DataGridView dgv, int pivotRow, int pivotCol)
        {
            if (pivotRow >= 0)
            {
                int dgvRow = pivotRow + 1;
                for (int c = 0; c < dgv.Columns.Count; c++)
                {
                    if (dgv.Rows[dgvRow].Cells[c].Style.BackColor != Color.Yellow && dgvRow != dgv.Rows.Count - 1)
                    {
                        dgv.Rows[dgvRow].Cells[c].Style.BackColor = Color.FromArgb(255, 160, 160);
                    }
                }
            }

            if (pivotCol >= 0)
            {
                string pivotColName = _variablesCache[pivotCol].Name;
                int colIndexInDgv = -1;
                for (int c = 0; c < dgv.Columns.Count; ++c)
                {
                    if (dgv.Columns[c].HeaderText == pivotColName)
                    {
                        colIndexInDgv = c;
                        break;
                    }
                }

                if (colIndexInDgv != -1)
                {
                    for (int r = 1; r < dgv.Rows.Count - 1; r++)
                    {
                        if (dgv.Rows[r].Cells[colIndexInDgv].Style.BackColor != Color.Yellow)
                        {
                            dgv.Rows[r].Cells[colIndexInDgv].Style.BackColor = Color.FromArgb(255, 160, 160);
                        }
                    }
                    if (pivotRow >= 0 && dgv.Rows.Count > pivotRow + 1 && dgv.Columns.Count > colIndexInDgv)
                    {
                        dgv.Rows[pivotRow + 1].Cells[colIndexInDgv].Style.BackColor = Color.Yellow;
                        dgv.Rows[pivotRow + 1].Cells[colIndexInDgv].Style.ForeColor = Color.Black;
                    }
                }
            }
        }

        protected void AddControlToPanel(Panel panel, Control ctrl, int extraYOffset = EXTRA_Y_OFFSET)
        {
            panel.Controls.Add(ctrl);
            currentYOffset += ctrl.Height + extraYOffset;
            if (ctrl.Right > this.requiredWidth)
            {
                this.requiredWidth = ctrl.Right;
            }
        }

        private void AddDeltaCalculations(Panel panel, double[,] table, int[] basis, bool isMin)
        {
            var deltaLabel = CreateLabel(panel, "Детальний розрахунок оцінок (Δ = Zj - Cj):",
                new Font("Arial", 10, FontStyle.Bold | FontStyle.Underline));
            AddControlToPanel(panel, deltaLabel, 5);
            double zValue_ConstantPart = 0;
            double zValue_MCoefficient = 0;
            int rhsColumnIndex = Variables.Count;
            for (int i = 0; i < basis.Length; i++)
            {
                var basisVarInfo = _variablesCache[basis[i]];
                double basisVarRhsValue = table[i, rhsColumnIndex];
                if (basisVarInfo.Type == VariableType.Artificial)
                {
                    zValue_MCoefficient += (isMin ? 1.0 : -1.0) * basisVarRhsValue;
                }
                else
                {
                    zValue_ConstantPart += basisVarInfo.OriginalCost * basisVarRhsValue;
                }
            }

            var zLabel = CreateLabel(panel, FormatLinearCombination(
                basis.Select(b_idx => _variablesCache[b_idx]).ToList(),
                Enumerable.Range(0, basis.Length).Select(row_idx => table[row_idx, Variables.Count]).ToArray(),
                isMin, "Z* = Sum(Cb[i] * b[i]) = [", "] = " +
                 FormatDeltaToString(zValue_ConstantPart, zValue_MCoefficient)
            ), new Font("Consolas", 9));
            AddControlToPanel(panel, zLabel, 3);

            var sortedVariables = Variables
                .OrderBy(v => GetVariableTypeOrder(v.Type))
                .ThenBy(v => v.Index);
            foreach (var varInfo in sortedVariables)
            {
                var deltaCalcLabel = CreateLabel(panel,
                    FormatDeltaCalculation(varInfo, table, basis, isMin),
                    new Font("Consolas", 9));
                AddControlToPanel(panel, deltaCalcLabel, 3);
            }
            currentYOffset += Y_OFFSET_STEP;
        }

        private string FormatDeltaCalculation(VariableInfo varInfo, double[,] table, int[] basis, bool isMin)
        {
            string Cj_sym = varInfo.GetSymbolicCost(isMin);
            string zjCalc = FormatLinearCombination(
                basis.Select(i => _variablesCache[i]).ToList(),
                Enumerable.Range(0, basis.Length).Select(i => table[i, varInfo.Index]).ToArray(),
                isMin, "[", "]");
            var (constantPart, mCoefficient) = CalculateDeltaComponents(varInfo.Index, table, basis, isMin);
            string finalResult = FormatDeltaToString(constantPart, mCoefficient);
            return $"Δ({varInfo.Name}) = Zj - Cj = {zjCalc} - ({Cj_sym}) = {finalResult}";
        }

        private string FormatLinearCombination(List<VariableInfo> variables, double[] coefficients,
            bool isMin, string prefix = "", string suffix = "")
        {
            var sb = new StringBuilder(prefix);
            bool first = true;

            for (int i = 0; i < variables.Count; i++)
            {
                if (Math.Abs(coefficients[i]) > TOLERANCE)
                {
                    if (!first) sb.Append(" + ");
                    sb.Append($"({variables[i].GetSymbolicCost(isMin)})*({ToFraction(coefficients[i])})");
                    first = false;
                }
            }

            if (first) sb.Append("0");
            sb.Append(suffix);

            return sb.ToString();
        }

        private void AddPivotInfo(Panel panel, double[,] table, int[] basis, bool isMin)
        {
            int pivotCol = isMin
                ? FindPivotColumn(table, delta => delta > 0)
                : FindPivotColumn(table, delta => delta < 0);
            int pivotRow = pivotCol >= 0 ? FindPivotRow(table, pivotCol, basis) : -1;
            var pivotInfoLabel = CreateLabel(panel,
                GetPivotInfoText(pivotCol, pivotRow, table, basis, isMin),
                new Font("Arial", 12, FontStyle.Bold),
                GetPivotInfoColor(pivotCol, pivotRow, basis, table));
            AddControlToPanel(panel, pivotInfoLabel, Y_OFFSET_STEP);
            panel.AutoScrollMinSize = new Size(0, currentYOffset);
        }

        private string GetPivotInfoText(int pivotCol, int pivotRow, double[,] table, int[] basis, bool isMin)
        {
            if (pivotCol >= 0 && pivotRow >= 0)
            {
                var pivotColVar = _variablesCache[pivotCol];
                var pivotRowVar = _variablesCache[basis[pivotRow]];
                return $"Напрямний елемент: Стовпець {pivotColVar.Name}, Рядок {pivotRowVar.Name}, " +
                       $"Значення {ToFraction(table[pivotRow, pivotCol])}";
            }

            if (pivotCol >= 0)
                return "Розв'язок необмежений (немає додатних елементів у напрямному стовпці).";
            return HasArtificialInBasis(basis, table)
                ? "ОПТИМАЛЬНЕ (?): Штучні змінні в базисі. Немає допустимих розв'язків."
                : "РОЗВ'ЯЗОК ОПТИМАЛЬНИЙ - критерій оптимальності виконано.";
        }

        private Color GetPivotInfoColor(int pivotCol, int pivotRow, int[] basis, double[,] table)
        {
            if (pivotCol >= 0 && pivotRow >= 0) return SystemColors.ControlText;
            if (pivotCol >= 0) return Color.Red;
            return HasArtificialInBasis(basis, table) ? Color.OrangeRed : Color.Green;
        }

        protected void DisplayResults(Panel panel, double[,] table, int[] basis, bool isMin, bool multipleOptimaPossible)
        {
            var resultText = new StringBuilder();
            bool hasArtificialInBasis;
            var values = ExtractSolutionValues(table, basis, out hasArtificialInBasis);

            if (hasArtificialInBasis)
            {
                BuildNoSolutionMessage(resultText, values);
                this.LastResultSummary = "Немає допустимих розв'язків";
            }
            else
            {
                BuildSolutionMessage(resultText, values, table, isMin);
                double fValue = table[table.GetLength(0) - 1, Variables.Count];
                this.LastResultSummary = $"F = {ToFraction(fValue)}";

                if (multipleOptimaPossible)
                {
                    resultText.AppendLine("\nЗавдання має множинні оптимальні рішення. Знайдений варіант є одним із можливих.");
                    this.LastResultSummary += " (множинні оптимальні рішення)";
                }
            }

            var resultLabel = CreateLabel(panel,
                resultText.ToString(),
                new Font("Arial", 12, FontStyle.Bold),
                hasArtificialInBasis ? Color.Red : Color.DarkGreen);
            AddControlToPanel(panel, resultLabel, 15);
            panel.AutoScrollMinSize = new Size(0, currentYOffset);
        }

        private double[] ExtractSolutionValues(double[,] table, int[] basis, out bool hasArtificialInBasis)
        {
            hasArtificialInBasis = false;
            var values = new double[Variables.Count];
            int rhsCol = Variables.Count;

            for (int i = 0; i < basis.Length; i++)
            {
                int basisVarIndex = basis[i];
                if (basisVarIndex >= 0 && basisVarIndex < values.Length)
                {
                    values[basisVarIndex] = table[i, rhsCol];
                    var basisVar = _variablesCache[basisVarIndex];
                    if (basisVar.Type == VariableType.Artificial && Math.Abs(table[i, rhsCol]) > TOLERANCE)
                        hasArtificialInBasis = true;
                }
            }
            return values;
        }

        private void BuildNoSolutionMessage(StringBuilder resultText, double[] values)
        {
            var artificialVars = Variables
                .Where(v => v.Type == VariableType.Artificial && v.Index < values.Length && Math.Abs(values[v.Index]) > TOLERANCE)
                .ToList();
            resultText.Append("Оптимальний розв'язок НЕ ЗНАЙДЕНО.");
            resultText.Append($"\nПричина: Штучн{(artificialVars.Count > 1 ? "і змінні" : "а змінна")} у базисі:");
            foreach (var av in artificialVars)
                resultText.Append($" {av.Name}={ToFraction(values[av.Index])};");
            resultText.Append("\nВихідна задача не має допустимих розв'язків.");
        }

        private void BuildSolutionMessage(StringBuilder resultText, double[] values, double[,] table, bool isMin)
        {
            var originalVars = Variables.Where(v => v.Type == VariableType.Original).ToList();
            resultText.Append("Оптимальний розв'язок: ");

            if (originalVars.Count > 0)
                resultText.Append(string.Join("; ", originalVars.Select(v => $"{v.Name} = {ToFraction(v.Index < values.Length ? values[v.Index] : 0)}")));
            else
                resultText.Append("(немає вихідних змінних)");
            double zValue = table[table.GetLength(0) - 1, Variables.Count];
            double fValue = zValue;

            resultText.Append($"\nЦільова функція F = {ToFraction(fValue)}");
        }

        protected bool HasArtificialInBasis(int[] basis, double[,] table)
        {
            int rhsCol = Variables.Count;
            return basis.Any(b =>
                b < Variables.Count &&
                _variablesCache.ContainsKey(b) &&
                _variablesCache[b].Type == VariableType.Artificial &&
                Math.Abs(table[Array.IndexOf(basis, b), rhsCol]) > TOLERANCE);
        }

        private (double constantPart, double mCoefficient) CalculateDeltaComponents(int j, double[,] table, int[] basis, bool isMin)
        {
            double constantPart = 0;
            double mCoefficient = 0;

            for (int i = 0; i < basis.Length; i++)
            {
                var basisVar = _variablesCache[basis[i]];
                double aij = table[i, j];
                if (Math.Abs(aij) < TOLERANCE) continue;
                if (basisVar.Type == VariableType.Artificial)
                    mCoefficient += (isMin ? 1.0 : -1.0) * aij;
                else
                    constantPart += basisVar.OriginalCost * aij;
            }

            var varJ = _variablesCache[j];
            if (varJ.Type == VariableType.Artificial)
                mCoefficient -= (isMin ? 1.0 : -1.0);
            else
                constantPart -= varJ.OriginalCost;
            return (constantPart, mCoefficient);
        }

        protected string CalculateOnlyFinalSymbolicDelta(int j, double[,] table, int[] basis, bool isMin)
        {
            var (constantPart, mCoefficient) = CalculateDeltaComponents(j, table, basis, isMin);
            return FormatDeltaToString(constantPart, mCoefficient);
        }

        protected string FormatDeltaToString(double constantPart, double mCoefficient, double tolerance = 1e-9)
        {
            var parts = new List<string>();
            if (Math.Abs(mCoefficient) > tolerance)
            {
                if (Math.Abs(mCoefficient - 1.0) < tolerance) parts.Add("M");
                else if (Math.Abs(mCoefficient - (-1.0)) < tolerance) parts.Add("-M");
                else parts.Add(ToFraction(mCoefficient) + "M");
            }

            if (Math.Abs(constantPart) > tolerance || parts.Count == 0)
            {
                string constStr = ToFraction(constantPart);
                if (parts.Count > 0 && constantPart > tolerance && !constStr.StartsWith("-") && !constStr.StartsWith("+"))
                {
                    parts.Add("+");
                }

                if (!(parts.Count > 0 && Math.Abs(constantPart) < tolerance))
                {
                    parts.Add(constStr);
                }
            }

            if (parts.Count == 0) return "0";
            return string.Join(" ", parts).Replace("  ", " ").Trim();
        }

        protected string ToFraction(double value, int maxDenominator = MAX_DENOMINATOR, double tolerance = TOLERANCE)
        {
            if (double.IsNaN(value)) return "NaN";
            if (double.IsPositiveInfinity(value)) return "∞";
            if (double.IsNegativeInfinity(value)) return "-∞";
            if (Math.Abs(value) < tolerance) return "0";
            if (Math.Abs(value - Math.Round(value)) < tolerance) return Math.Round(value).ToString("F0");

            int sign = Math.Sign(value);
            value = Math.Abs(value);
            double h1 = 1, h2 = 0, k1 = 0, k2 = 1;
            double y = value;
            do
            {
                double a = Math.Floor(y);
                double x = h1; h1 = a * h1 + h2; h2 = x;
                x = k1;
                k1 = a * k1 + k2; k2 = x;
                if (Math.Abs(y - a) < tolerance) break;
                y = 1 / (y - a);
                if (k1 > maxDenominator) break;
            }
            while (Math.Abs(value - h1 / k1) > value * tolerance);
            long numer, denom;
            if (k1 <= maxDenominator && k1 != 0)
            {
                numer = (long)Math.Round(h1);
                denom = (long)Math.Round(k1);
            }
            else if (k2 != 0)
            {
                numer = (long)Math.Round(h2);
                denom = (long)Math.Round(k2);
            }
            else
            {
                return (sign * value).ToString("G4");
            }

            if (denom == 0) return (sign * value).ToString("G5");
            if (Math.Abs(value - (double)numer / denom) > value * tolerance * 10 && denom != 0)
                return (sign * value).ToString("G4");
            long common = GCD(numer, denom);
            numer /= common;
            denom /= common;
            return denom == 1
                ? $"{(sign < 0 ? "-" : "")}{numer}"
                : $"{(sign < 0 ? "-" : "")}{numer}/{denom}";
        }

        protected static long GCD(long a, long b)
        {
            a = Math.Abs(a);
            b = Math.Abs(b);
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a == 0 ? 1 : a;
        }
    }
}