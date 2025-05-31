using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDO_CW_CODE
{
    public class SolutionHistoryEntry
    {
        public DateTime Timestamp { get; set; }
        public int VarCount { get; set; }
        public int ConstraintCount { get; set; }
        public string ObjectiveType { get; set; }
        public List<string> ObjectiveCoefficients { get; set; }
        public List<List<string>> ConstraintCoefficients { get; set; }
        public List<string> ConstraintSigns { get; set; }
        public string ResultSummary { get; set; }

        public override string ToString()
        {
            return $"{Timestamp:HH:mm:ss} | {VarCount}x{ConstraintCount} {ObjectiveType} | {ResultSummary}";
        }

        public string GetDetailedString()
        {
            var sb = new StringBuilder();

            sb.Append("F = ");
            for (int i = 0; i < ObjectiveCoefficients.Count; i++)
            {
                string coeff = string.IsNullOrWhiteSpace(ObjectiveCoefficients[i]) ? "0" : ObjectiveCoefficients[i];
                if (i > 0)
                {
                    if (!coeff.Trim().StartsWith("-"))
                    {
                        sb.Append(" + ");
                    }
                    else
                    {
                        sb.Append(" ");
                    }
                }
                sb.Append($"{coeff}·x{i + 1}");
            }
            sb.AppendLine($" → {ObjectiveType}");
            sb.AppendLine("".PadRight(30, '-'));

            for (int i = 0; i < ConstraintCount; i++)
            {
                var row = ConstraintCoefficients[i];
                for (int j = 0; j < VarCount; j++)
                {
                    string coeff = string.IsNullOrWhiteSpace(row[j]) ? "0" : row[j];
                    if (j > 0)
                    {
                        if (!coeff.Trim().StartsWith("-"))
                        {
                            sb.Append(" + ");
                        }
                        else
                        {
                            sb.Append(" ");
                        }
                    }
                    sb.Append($"{coeff}·x{j + 1}");
                }
                string sign = ConstraintSigns[i];
                string rhs = string.IsNullOrWhiteSpace(row[VarCount]) ? "0" : row[VarCount];
                sb.AppendLine($" {sign} {rhs}");
            }

            sb.AppendLine("".PadRight(30, '='));
            sb.AppendLine($"Результат: {ResultSummary}");

            return sb.ToString();
        }
    }
}