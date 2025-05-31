using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MMDO_CW_CODE
{
    public class AreaBuilder
    {
        public List<TextBox> ObjectiveFunctionBoxes { get; } = new List<TextBox>();
        public List<List<TextBox>> ConstraintBoxes { get; } = new List<List<TextBox>>();
        public List<ComboBox> ConstraintSigns { get; } = new List<ComboBox>(); 
        public ComboBox ObjectiveTypeComboBox { get; private set; }

        public void BuildInputArea(Panel panel, int varCount, int constraintsCount)
        {
            panel.Controls.Clear();
            ClearCollections();
            panel.AutoScroll = true;

            int panelWidth = panel.ClientSize.Width;
            int marginTop = 10;
            int marginLeft = 10;
            int fieldWidth = 40;
            int stepX = fieldWidth + 35;
            int spacingBetweenElements = 5;

            BuildObjectiveFunction(panel, varCount, panelWidth, ref marginTop, marginLeft, fieldWidth, stepX, spacingBetweenElements);
            marginTop += 40;
            BuildConstraints(panel, varCount, constraintsCount, panelWidth, ref marginTop, marginLeft, fieldWidth, stepX, spacingBetweenElements);

            panel.AutoScrollMinSize = new Size(0, marginTop + 10);
        }

        public void LoadDefaultValues(NumericUpDown numVar, NumericUpDown numConstr)
        {
            const int defaultVarCount = 5;
            const int defaultConstraintsCount = 4;
            numVar.Value = defaultVarCount;
            numConstr.Value = defaultConstraintsCount;

            Application.DoEvents();
            if (!ValidateControlsCreation())
            {
                MessageBox.Show("Не вдалося створити елементи керування. Спробуйте ще раз.",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ObjectiveTypeComboBox.SelectedItem = "min";
            var objectiveCoefficients = new[] { "8", "17", "10", "12", "15" };
            for (int i = 0; i < objectiveCoefficients.Length; i++)
            {
                ObjectiveFunctionBoxes[i].Text = objectiveCoefficients[i];
            }

            var constraints = new[]
            {
                new { Coeffs = new[] { "1", "1", "1", "1", "1" }, Sign = "=", Rhs = "1" },
                new { Coeffs = new[] { "0.15", "0.10", "0.30", "0.40", "0.10" }, Sign = "<=", Rhs = "1" },
                new { Coeffs = new[] { "0.40", "0.80", "0.30", "0.25", "0.70" }, Sign = ">=", Rhs = "0.20" },
                new { Coeffs = new[] { "0.45", "0.10", "0.40", "0.35", "0.20" }, Sign = "<=", Rhs = "0.40" }
            };

            for (int i = 0; i < constraints.Length; i++)
            {
                for (int j = 0; j < constraints[i].Coeffs.Length; j++)
                {
                    ConstraintBoxes[i][j].Text = constraints[i].Coeffs[j];
                }
                ConstraintSigns[i].SelectedItem = constraints[i].Sign;
                ConstraintBoxes[i][defaultVarCount].Text = constraints[i].Rhs;
            }
        }

        private void ClearCollections()
        {
            ObjectiveFunctionBoxes.Clear();
            ConstraintBoxes.Clear();
            ConstraintSigns.Clear();
            ObjectiveTypeComboBox = null;
        }

        private bool ValidateControlsCreation()
        {
            return ObjectiveTypeComboBox != null &&
                   ObjectiveFunctionBoxes.Count == 5 &&
                   ConstraintBoxes.Count == 4 &&
                   ConstraintSigns.Count == 4;
        }

        private void BuildObjectiveFunction(Panel panel, int varCount, int panelWidth, ref int marginTop,
                                            int startX, int fieldWidth, int stepX, int spacingBetweenElements)
        {
            AddLabel(panel, "Цільова функція:", startX, ref marginTop, true);

            int currentXPos = startX;
            for (int i = 0; i < varCount; i++)
            {
                var tb = AddTextBox(panel, fieldWidth, currentXPos, marginTop);
                ObjectiveFunctionBoxes.Add(tb);

                var lblVar = AddLabel(panel, $"x{i + 1}", tb.Right + spacingBetweenElements, marginTop + 3);
                currentXPos = lblVar.Right + spacingBetweenElements;
                if (i < varCount - 1)
                {
                    var lblPlus = AddLabel(panel, "+", currentXPos, marginTop + 3);
                    currentXPos = lblPlus.Right + spacingBetweenElements;
                }
            }

            var lblArrow = AddLabel(panel, "→", currentXPos, marginTop + 3, bold: true);
            AddObjectiveTypeComboBox(panel, lblArrow.Right + 5, marginTop);
        }

        private void BuildConstraints(Panel panel, int varCount, int constraintsCount, int panelWidth,
                                      ref int marginTop, int startX, int fieldWidth, int stepX, int spacingBetweenElements)
        {
            AddLabel(panel, "Обмеження:", startX, ref marginTop, true);
            for (int i = 0; i < constraintsCount; i++)
            {
                var row = new List<TextBox>();
                int currentXPos = startX;

                for (int j = 0; j < varCount; j++)
                {
                    var tb = AddTextBox(panel, fieldWidth, currentXPos, marginTop);
                    row.Add(tb);

                    var lblVar = AddLabel(panel, $"x{j + 1}", tb.Right + spacingBetweenElements, marginTop + 3);
                    currentXPos = lblVar.Right + spacingBetweenElements;
                    if (j < varCount - 1)
                    {
                        var lblPlus = AddLabel(panel, "+", currentXPos, marginTop + 3);
                        currentXPos = lblPlus.Right + spacingBetweenElements;
                    }
                }

                var sign = AddSignComboBox(panel, currentXPos, marginTop);
                var rhs = AddTextBox(panel, fieldWidth, sign.Right + 5, marginTop);
                row.Add(rhs);

                ConstraintBoxes.Add(row);
                ConstraintSigns.Add(sign);
                marginTop += 35;
            }
        }

        private TextBox AddTextBox(Panel panel, int width, int x, int y)
        {
            var tb = new TextBox()
            {
                Width = width,
                Location = new Point(x, y),
                Font = new Font("Times New Roman", 10F)
            };
            panel.Controls.Add(tb);
            return tb;
        }

        private Label AddLabel(Panel panel, string text, int x, int y, bool bold = false, bool autoSize = true)
        {
            var label = new Label()
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = autoSize,
                Font = new Font("Times New Roman", 10F, bold ? FontStyle.Bold : FontStyle.Regular)
            };
            panel.Controls.Add(label);
            return label;
        }

        private void AddLabel(Panel panel, string text, int x, ref int y, bool bold = false)
        {
            var label = AddLabel(panel, text, x, y, bold);
            y += 25;
        }

        private ComboBox AddSignComboBox(Panel panel, int x, int y)
        {
            var sign = new ComboBox()
            {
                Width = 45,
                Location = new Point(x, y),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Times New Roman", 10F)
            };
            sign.Items.AddRange(new string[] { "<=", "=", ">=" });
            sign.SelectedIndex = 0;
            panel.Controls.Add(sign);
            return sign;
        }

        private void AddObjectiveTypeComboBox(Panel panel, int x, int y)
        {
            ObjectiveTypeComboBox = new ComboBox()
            {
                Location = new Point(x, y),
                Width = 60,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Times New Roman", 10F)
            };
            ObjectiveTypeComboBox.Items.AddRange(new string[] { "min", "max" });
            ObjectiveTypeComboBox.SelectedIndex = 0;
            ObjectiveTypeComboBox.Name = "cmbObjectiveType";
            panel.Controls.Add(ObjectiveTypeComboBox);
        }
    }
}