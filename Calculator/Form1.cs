using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            // Add TextChanged event handler for amountPurchase
            amountPurchase.TextChanged += AmountPurchase_TextChanged;
        }

        private void AmountPurchase_TextChanged(object sender, EventArgs e)
        {
            UpdateCalculatedSum();
        }

        private void SetArgs(Object s, EventArgs e)
        {
            if (s is Button button)
            {
                switch (button.Name)
                {
                    case nameof(btnCompute):
                        ComputeButton();
                        break;
                    // Add more cases for additional Buttons
                    default:
                        break;
                }
            }
            else if (s is Form form)
            {
                string[] list = { "Solo Parent / 10%", "Athlete / 20%" };
                foreach (string item in list)
                {
                    listOption.Items.Add(item);
                }
                listOption.SelectedIndex = 0;
            }
            else if (s is TextBox textBox)
            {
                if (textBox == amountPurchase)
                {
                    // No need to call UpdateCalculatedSum here anymore
                }
                if (textBox.Text.Equals("Enter number"))
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            }
            else if (s is ComboBox comboBox)
            {
                comboBox.DroppedDown = true;
            }
        }

        private void FocusLeave(object s, EventArgs e)
        {
            if (s is TextBox textBox)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = "Enter number";
                    textBox.ForeColor = Color.Gray;
                }
            }
        }

        private void UpdateCalculatedSum()
        {
            try
            {
                // Split the input text by '+' and sum the values
                string[] values = amountPurchase.Text.Split('+');
                double total = 0;
                foreach (string value in values)
                {
                    if (double.TryParse(value.Trim(), out double number))
                    {
                        total += number;
                    }
                }
                CalculatedSum.Text = total.ToString("F2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error calculating sum: {ex.Message}");
            }
        }

        private void ComputeButton()
        {
            try
            {
                if (double.TryParse(CalculatedSum.Text, out double originalValue))
                {
                    double discountedValue = originalValue;           
                    double LessVAT = 0;
                    double finalAmount = 0;
                    double AddLessVat = 0;
                    double LessWHoldingTAX = 0;
                    double TotalAmount = 0;
                    if (listOption.SelectedItem != null)
                    {
                        string selectedItem = listOption.SelectedItem.ToString().Trim();

                        if (selectedItem == "Solo Parent / 10%")
                        {
                            LessVAT = originalValue - (originalValue / 1.12);
                            discountedValue = originalValue / 1.12 * 0.1;
                            AddLessVat = 0;
                            LessWHoldingTAX = 0;
                            TotalAmount = originalValue / 1.12 - discountedValue;
                        }
                        else if (selectedItem == "Athlete / 20%")
                        {
                            LessVAT = originalValue - (originalValue / 1.12);
                            discountedValue = originalValue / 1.12 * 0.2;
                            AddLessVat = originalValue - (originalValue / 1.12);
                            LessWHoldingTAX = 0;
                            TotalAmount = (originalValue / 1.12 + AddLessVat) - discountedValue;
                        }
                    }

                    if (double.TryParse(amountTendered.Text, out double tenderedAmount))
                    {
                        double change = tenderedAmount - finalAmount;

                        if (listOption.SelectedIndex >= 0)
                        {
                            // Update computeBox
                            computeBox.Text = FormatForComputeBox(
                                originalValue,
                                LessVAT,
                                discountedValue,
                                AddLessVat,
                                LessWHoldingTAX,
                                TotalAmount,
                                change);

                            // Update computedBox2
                            computedBox2.Text = FormatForComputedBox2();
                        }
                        else
                        {
                            SetBox(Color.Black, "Error; Select discount from list");
                        }
                    }
                    else
                    {
                        SetBox(Color.Black, "Error; Enter tendered amount");
                    }
                }
                else
                {
                    SetBox(Color.Black, "Error; Enter total amount purchase");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void SetBox(Color color, string text)
        {
            computeBox.Clear();
            computeBox.ForeColor = color;
            computeBox.Text = text;
        }

        private string FormatForComputeBox(double originalValue, double LessVAT, double discountedValue, double AddLessVat, double LessWHoldingTAX, double TotalAmount, double change)
        {
            computeBox.ForeColor = Color.Black;
           
            const int width = 10; // Adjust the width as needed for alignment

            // Create the receipt lines with right alignment
            return new StringBuilder()
                .AppendLine($"Total Sales: {originalValue:F2}".PadLeft(width))
                .AppendLine($"Less VAT: {LessVAT:F2}".PadLeft(width))
                .AppendLine($"Amount Net of VAT: {originalValue / 1.12:F2}".PadLeft(width))
                .AppendLine($"Less Discount: {discountedValue:F2}".PadLeft(width))
                .AppendLine($"Add: VAT: {AddLessVat:F2}".PadLeft(width))
                .AppendLine($"Less withholding Tax: {LessWHoldingTAX:F2}".PadLeft(width))
                .AppendLine($"Total Amount: {TotalAmount:F2}".PadLeft(width))
                .AppendLine($"Change: {change - TotalAmount:F2}".PadLeft(width))
                .ToString();
        }

        private string FormatForComputedBox2()
        {
            computedBox2.ForeColor = Color.Black;
            const int valueWidth = 10; // Adjust this width based on your needs

            return new StringBuilder()
                .AppendLine($"Vatable Sales: {0.00:F2}".PadLeft(valueWidth))
                .AppendLine($"Vat:   {0.00:F2}".PadLeft(valueWidth))
                .AppendLine($"Zero-Rated Sales: {0.00:F2}".PadLeft(valueWidth))
                .AppendLine($"VAT-Exempt Sales: {0.00:F2}".PadLeft(valueWidth))
                .ToString();
        }

       
    }
}
