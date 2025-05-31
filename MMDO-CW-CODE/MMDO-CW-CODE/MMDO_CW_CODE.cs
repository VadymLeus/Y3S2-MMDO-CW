using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;

namespace MMDO_CW_CODE
{
    public partial class MMDO_CW_CODE : Form
    {
        private AreaBuilder inputAreaBuilder;
        private const int MAX_HISTORY_ITEMS = 10;
        private List<SolutionHistoryEntry> _solutionHistory = new List<SolutionHistoryEntry>();

        private static readonly string HistoryFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MMDO_SimplexSolver",
            "history.json");


        public MMDO_CW_CODE()
        {
            InitializeComponent();
            inputAreaBuilder = new AreaBuilder();

            LoadHistoryFromFile();

            numVarCount.ValueChanged += Counts_ValueChanged;
            numConstraintsCount.ValueChanged += Counts_ValueChanged;
            btnLoadHistory.Click += btnLoadHistory_Click;
            btnDeleteHistory.Click += btnDeleteHistory_Click;
            listBoxHistory.SelectedIndexChanged += listBoxHistory_SelectedIndexChanged;


            if (this.btn_example != null)
            {
                this.btn_example.Click += new System.EventHandler(this.btn_example_Click);
            }

            this.btnClearClick.Click += new System.EventHandler(this.btnClearClick_Click);
            this.btnLoadFromExcel.Click += new System.EventHandler(this.btnLoadFromExcel_Click);

            RebuildInputArea();
            UpdateHistoryListBox();
        }

        private void RebuildInputArea()
        {
            int currentVarCount = (int)numVarCount.Value;
            int currentConstraintsCount = (int)numConstraintsCount.Value;
            inputAreaBuilder.BuildInputArea(panelInput, currentVarCount, currentConstraintsCount);
        }

        private void Counts_ValueChanged(object sender, EventArgs e)
        {
            RebuildInputArea();
            panelOutput.Controls.Clear();
            panelOutput.AutoScrollMinSize = new Size(0, 0);
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            panelOutput.Controls.Clear();
            panelOutput.AutoScrollMinSize = new Size(0, 0);

            if (inputAreaBuilder.ObjectiveTypeComboBox == null || inputAreaBuilder.ObjectiveTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Помилка: Не обрано тип цільової функції (min/max). Можливо, потрібно змінити кількість змінних/обмежень, щоб область вводу перегенерувалась.",
                    "Помилка вибору функції", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isMin = inputAreaBuilder.ObjectiveTypeComboBox.SelectedItem.ToString() == "min";
            var solver = new ConditionType(inputAreaBuilder, isMin);
            solver.Solve(panelOutput);

            var entry = new SolutionHistoryEntry
            {
                Timestamp = DateTime.Now,
                VarCount = inputAreaBuilder.ObjectiveFunctionBoxes.Count,
                ConstraintCount = inputAreaBuilder.ConstraintBoxes.Count,
                ObjectiveType = inputAreaBuilder.ObjectiveTypeComboBox.SelectedItem.ToString(),
                ObjectiveCoefficients = inputAreaBuilder.ObjectiveFunctionBoxes.Select(tb => tb.Text).ToList(),
                ConstraintCoefficients = inputAreaBuilder.ConstraintBoxes.Select(row => row.Select(tb => tb.Text).ToList()).ToList(),
                ConstraintSigns = inputAreaBuilder.ConstraintSigns.Select(cb => cb.SelectedItem.ToString()).ToList(),
                ResultSummary = solver.LastResultSummary ?? "N/A"
            };
            AddEntryToHistory(entry);
        }

        private void btn_example_Click(object sender, EventArgs e)
        {
            inputAreaBuilder.LoadDefaultValues(numVarCount, numConstraintsCount);
            panelOutput.Controls.Clear();
            panelOutput.AutoScrollMinSize = new Size(0, 0);
        }

        private void btnLoadFromExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Файли Excel|*.xlsx;*.xls";
                openFileDialog.Title = "Виберіть файл Excel із задачею";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var (coefficients, constraints, signs, objectiveType) = ExcelLoader.LoadFromExcel(openFileDialog.FileName);
                    if (coefficients == null || constraints == null || signs == null || objectiveType == null)
                        return;
                    try
                    {
                        numVarCount.Value = coefficients.Length;
                        numConstraintsCount.Value = constraints.Length;
                        Application.DoEvents();

                        for (int i = 0; i < coefficients.Length; i++)
                        {
                            inputAreaBuilder.ObjectiveFunctionBoxes[i].Text = coefficients[i].ToString();
                        }

                        inputAreaBuilder.ObjectiveTypeComboBox.SelectedItem = objectiveType;
                        for (int i = 0; i < constraints.Length; i++)
                        {
                            for (int j = 0; j < coefficients.Length; j++)
                            {
                                inputAreaBuilder.ConstraintBoxes[i][j].Text =
                                    j < constraints[i].Length - 1 ?
                                    constraints[i][j].ToString() : "0";
                            }

                            inputAreaBuilder.ConstraintSigns[i].SelectedItem = signs[i];
                            inputAreaBuilder.ConstraintBoxes[i][coefficients.Length].Text =
                                constraints[i].Last().ToString();
                        }

                        MessageBox.Show("Дані успішно завантажені з файлу Excel!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnClearClick_Click(object sender, EventArgs e)
        {
            RebuildInputArea();
            panelOutput.Controls.Clear();
            panelOutput.AutoScrollMinSize = new Size(0, 0);

            foreach (var textBox in inputAreaBuilder.ObjectiveFunctionBoxes)
            {
                textBox.Text = "";
            }

            foreach (var constraintRow in inputAreaBuilder.ConstraintBoxes)
            {
                foreach (var textBox in constraintRow)
                {
                    textBox.Text = "";
                }
            }

            foreach (var comboBox in inputAreaBuilder.ConstraintSigns)
            {
                comboBox.SelectedIndex = 0;
            }

            if (inputAreaBuilder.ObjectiveTypeComboBox != null)
            {
                inputAreaBuilder.ObjectiveTypeComboBox.SelectedIndex = 1;
            }
        }

        private void listBoxHistory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxHistory.SelectedItem is SolutionHistoryEntry selectedEntry)
            {
                richTextBoxHistoryDetails.Text = selectedEntry.GetDetailedString();
            }
            else
            {
                richTextBoxHistoryDetails.Text = "Оберіть запис, щоб побачити деталі.";
            }
        }

        private void AddEntryToHistory(SolutionHistoryEntry entry)
        {
            _solutionHistory.Insert(0, entry);
            if (_solutionHistory.Count > MAX_HISTORY_ITEMS)
            {
                _solutionHistory.RemoveAt(_solutionHistory.Count - 1);
            }
            UpdateHistoryListBox();
            SaveHistoryToFile();
        }

        private void UpdateHistoryListBox()
        {
            listBoxHistory.DataSource = null;
            listBoxHistory.DataSource = _solutionHistory;

            if (_solutionHistory.Any())
            {
                listBoxHistory.SelectedIndex = 0;
            }
            else
            {
                richTextBoxHistoryDetails.Text = "Історія розв'язків порожня.";
            }
        }

        private void btnLoadHistory_Click(object sender, EventArgs e)
        {
            if (listBoxHistory.SelectedItem is SolutionHistoryEntry selectedEntry)
            {
                numVarCount.Value = selectedEntry.VarCount;
                numConstraintsCount.Value = selectedEntry.ConstraintCount;

                Application.DoEvents();

                if (inputAreaBuilder.ObjectiveTypeComboBox != null)
                {
                    inputAreaBuilder.ObjectiveTypeComboBox.SelectedItem = selectedEntry.ObjectiveType;
                }

                for (int i = 0; i < selectedEntry.ObjectiveCoefficients.Count; i++)
                {
                    if (i < inputAreaBuilder.ObjectiveFunctionBoxes.Count)
                        inputAreaBuilder.ObjectiveFunctionBoxes[i].Text = selectedEntry.ObjectiveCoefficients[i];
                }

                for (int i = 0; i < selectedEntry.ConstraintCount; i++)
                {
                    if (i < inputAreaBuilder.ConstraintBoxes.Count)
                    {
                        for (int j = 0; j < selectedEntry.VarCount + 1; j++)
                        {
                            if (j < inputAreaBuilder.ConstraintBoxes[i].Count)
                                inputAreaBuilder.ConstraintBoxes[i][j].Text = selectedEntry.ConstraintCoefficients[i][j];
                        }
                        inputAreaBuilder.ConstraintSigns[i].SelectedItem = selectedEntry.ConstraintSigns[i];
                    }
                }
                panelOutput.Controls.Clear();
                panelOutput.AutoScrollMinSize = new Size(0, 0);
            }
        }

        private void btnDeleteHistory_Click(object sender, EventArgs e)
        {
            if (listBoxHistory.SelectedItem is SolutionHistoryEntry selectedEntry)
            {
                int oldIndex = listBoxHistory.SelectedIndex;
                _solutionHistory.Remove(selectedEntry);
                UpdateHistoryListBox();
                SaveHistoryToFile();

                if (listBoxHistory.Items.Count > 0)
                {
                    listBoxHistory.SelectedIndex = Math.Min(oldIndex, listBoxHistory.Items.Count - 1);
                }
            }
        }

        private void SaveHistoryToFile()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(HistoryFilePath));
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(_solutionHistory, options);
                File.WriteAllText(HistoryFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not save history to file: {ex.Message}");
            }
        }

        private void LoadHistoryFromFile()
        {
            if (!File.Exists(HistoryFilePath))
            {
                _solutionHistory = new List<SolutionHistoryEntry>();
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(HistoryFilePath);
                _solutionHistory = JsonSerializer.Deserialize<List<SolutionHistoryEntry>>(jsonString) ?? new List<SolutionHistoryEntry>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not load history from file: {ex.Message}");
                _solutionHistory = new List<SolutionHistoryEntry>();
            }
        }
    }
}