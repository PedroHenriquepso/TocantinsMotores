using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TocantinsMotores
{
    public class FrmLogs : Form
    {
        private Button btnVoltar;
        private DataGridView gridLogs;
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmLogs()
        {
            this.Text = "Logs do Sistema";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            Panel panelTopo = new Panel() { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };
            this.Controls.Add(panelTopo);

            btnVoltar = new Button() { Text = "← VOLTAR (ESC)", Location = new Point(20, 15), Size = new Size(130, 40), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat };
            btnVoltar.Click += (s, e) => this.Close();
            panelTopo.Controls.Add(btnVoltar);

            Label lblTitulo = new Label() { Text = "AUDITORIA DE LOGS", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(30, 60, 120), AutoSize = true, Location = new Point(170, 20) };
            panelTopo.Controls.Add(lblTitulo);

            gridLogs = new DataGridView() { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, Font = new Font("Segoe UI", 10) };
            gridLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridLogs.EnableHeadersVisualStyles = false;
            this.Controls.Add(gridLogs);
            gridLogs.BringToFront();

            Carregar();
        }

        private void Carregar()
        {
            string query = @"SELECT TOP 1000 l.DataHora AS [Data/Hora], u.Nome AS [Usuário], l.Acao AS [Ação], l.Detalhes AS [Detalhes] 
                             FROM Logs_Sistema l 
                             LEFT JOIN Usuarios u ON l.UsuarioId = u.Id 
                             ORDER BY l.DataHora DESC";
            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridLogs.DataSource = dt;
            }
        }
    }
}