using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MMDO_CW_CODE
{
    public static class ExcelLoader
    {
        public static (double[] coefficients, double[][] constraints, string[] signs, string objectiveType) LoadFromExcel(string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var objectiveRow = worksheet.Row(1);
                    var maxCol = objectiveRow.LastCellUsed().Address.ColumnNumber;

                    string objectiveType = worksheet.Cell(1, maxCol).GetString().ToLower();
                    if (objectiveType != "max" && objectiveType != "min")
                    {
                        throw new FormatException("Тип задачі має бути 'max' або 'min'");
                    }

                    var coefficients = new List<double>();
                    for (int col = 1; col < maxCol; col++)
                    {
                        var cell = worksheet.Cell(1, col);
                        coefficients.Add(cell.IsEmpty() ? 0 : cell.GetValue<double>());
                    }

                    var constraints = new List<double[]>();
                    var signs = new List<string>();

                    for (int rowNum = 2; rowNum <= worksheet.LastRowUsed().RowNumber(); rowNum++)
                    {
                        var row = worksheet.Row(rowNum);
                        var lastCol = row.LastCellUsed().Address.ColumnNumber;

                        var constraintCoeffs = new List<double>();
                        for (int col = 1; col < lastCol - 1; col++)
                        {
                            var cell = worksheet.Cell(rowNum, col);
                            constraintCoeffs.Add(cell.IsEmpty() ? 0 : cell.GetValue<double>());
                        }

                        string sign = worksheet.Cell(rowNum, lastCol - 1).GetString();
                        if (sign != "<=" && sign != ">=" && sign != "=")
                        {
                            throw new FormatException($"Недопустимий знак нерівності: '{sign}' у рядку {rowNum}");
                        }

                        var rhsCell = worksheet.Cell(rowNum, lastCol);
                        double rhs = rhsCell.IsEmpty() ? 0 : rhsCell.GetValue<double>();

                        var fullConstraint = constraintCoeffs.Concat(new[] { rhs }).ToArray();

                        constraints.Add(fullConstraint);
                        signs.Add(sign);
                    }

                    return (coefficients.ToArray(), constraints.ToArray(), signs.ToArray(), objectiveType);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при читанні Excel файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null, null, null);
            }
        }
    }
}