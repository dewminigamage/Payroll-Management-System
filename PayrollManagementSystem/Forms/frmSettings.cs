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
        UpdatePreview();
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
                nudEtfRate.Value);

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
