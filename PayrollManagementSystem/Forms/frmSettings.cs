using System.Data;
using PayrollManagementSystem.Helpers;

namespace PayrollManagementSystem.Forms;

public partial class frmSettings : Form
{
    public frmSettings()
    {
        InitializeComponent();
    }

    private void frmSettings_Load(object sender, EventArgs e)
    {
        txtCompanyName.Text = PayrollSettings.CompanyName;
        txtAddress.Text     = PayrollSettings.CompanyAddress;
        txtPhone.Text       = PayrollSettings.CompanyPhone;
        txtEmail.Text       = PayrollSettings.CompanyEmail;
        nudEpfRate.Value    = PayrollSettings.EpfRate * 100m;
        nudEtfRate.Value    = PayrollSettings.EtfRate * 100m;
        nudWorkHours.Value  = PayrollSettings.WorkingHoursPerMonth;
        UpdatePreview();
        LoadTaxBrackets();
    }

    // ── Tax Brackets tab ──────────────────────────────────────────────────

    private void LoadTaxBrackets()
    {
        var dt = new DataTable();
        dt.Columns.Add("MinIncome",       typeof(decimal));
        dt.Columns.Add("MaxIncomeDisplay",typeof(string));
        dt.Columns.Add("TaxRatePct",      typeof(decimal));

        foreach (var b in PayrollSettings.TaxBrackets)
        {
            dt.Rows.Add(
                b.Min,
                b.Max == decimal.MaxValue ? "" : b.Max.ToString("0"),
                Math.Round(b.Rate * 100, 4));
        }
        dgvTaxBrackets.DataSource = dt;
    }

    private void btnAddBracket_Click(object sender, EventArgs e)
    {
        var dt = (DataTable)dgvTaxBrackets.DataSource!;
        dt.Rows.Add(0m, "", 0m);
    }

    private void btnDeleteBracket_Click(object sender, EventArgs e)
    {
        if (dgvTaxBrackets.CurrentRow == null) return;
        var dt = (DataTable)dgvTaxBrackets.DataSource!;
        dt.Rows.RemoveAt(dgvTaxBrackets.CurrentRow.Index);
    }

    private void btnSaveBrackets_Click(object sender, EventArgs e)
    {
        dgvTaxBrackets.EndEdit();
        var dt = (DataTable)dgvTaxBrackets.DataSource!;
        var brackets = new List<(decimal Min, decimal? Max, decimal Rate, int Order)>();

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            var row = dt.Rows[i];
            if (!decimal.TryParse(row["MinIncome"]?.ToString(), out decimal min) || min < 0)
            {
                MessageBox.Show($"Row {i + 1}: invalid Min Income.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string maxStr = row["MaxIncomeDisplay"]?.ToString()?.Trim() ?? "";
            decimal? max = null;
            if (!string.IsNullOrEmpty(maxStr))
            {
                if (!decimal.TryParse(maxStr, out decimal maxVal) || maxVal <= min)
                {
                    MessageBox.Show($"Row {i + 1}: Max Income must be blank or greater than Min Income.",
                        "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                max = maxVal;
            }
            if (!decimal.TryParse(row["TaxRatePct"]?.ToString(), out decimal pct) || pct < 0 || pct > 100)
            {
                MessageBox.Show($"Row {i + 1}: Tax Rate % must be 0–100.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            brackets.Add((min, max, pct / 100m, i + 1));
        }

        try
        {
            PayrollSettings.SaveTaxBrackets(brackets);
            MessageBox.Show("Tax brackets saved.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadTaxBrackets();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving brackets:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnResetBrackets_Click(object sender, EventArgs e)
    {
        if (MessageBox.Show("Reset to Sri Lanka PAYE defaults?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        var defaults = new List<(decimal Min, decimal? Max, decimal Rate, int Order)>
        {
            (0,       150000,  0.0000m, 1),
            (150000,  250000,  0.0600m, 2),
            (250000,  375000,  0.1200m, 3),
            (375000,  500000,  0.1800m, 4),
            (500000,  625000,  0.2400m, 5),
            (625000,  null,    0.3000m, 6)
        };
        try
        {
            PayrollSettings.SaveTaxBrackets(defaults);
            MessageBox.Show("Brackets reset to defaults.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadTaxBrackets();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error resetting brackets:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
        {
            MessageBox.Show("Company name is required.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtCompanyName.Focus();
            return;
        }

        try
        {
            PayrollSettings.Save(
                txtCompanyName.Text.Trim(),
                txtAddress.Text.Trim(),
                txtPhone.Text.Trim(),
                txtEmail.Text.Trim(),
                nudEpfRate.Value,
                nudEtfRate.Value,
                (int)nudWorkHours.Value);

            MessageBox.Show("Settings saved successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdatePreview()
    {
        decimal epfPct = nudEpfRate.Value;
        decimal etfPct = nudEtfRate.Value;
        decimal gross  = 50000m;
        decimal epf    = Math.Round(gross * epfPct / 100m, 2);
        decimal etf    = Math.Round(gross * etfPct / 100m, 2);
        decimal net    = gross - epf;

        lblExampleText.Text =
            $"Example — Gross Salary: LKR 50,000.00\r\n" +
            $"  EPF ({epfPct:0.##}% employee deduction):  LKR {epf:N2}   →  Net: LKR {net:N2}\r\n" +
            $"  ETF ({etfPct:0.##}% employer contribution): LKR {etf:N2}   →  Paid by employer, not deducted";
    }
}
