using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TocantinsMotores
{
    public class FrmFornecedores : Form
    {
        private Button btnVoltar, btnNovo;
        private DataGridView gridFornecedores;
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmFornecedores()
        {
            this.Text = "Cadastro de Fornecedores";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            Panel panelTopo = new Panel() { Dock = DockStyle.Top, Height = 80, BackColor = Color.White };
            this.Controls.Add(panelTopo);

            btnVoltar = new Button() { Text = "← VOLTAR (ESC)", Location = new Point(20, 20), Size = new Size(130, 40), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnVoltar.Click += (s, e) => this.Close();
            panelTopo.Controls.Add(btnVoltar);

            Label lblTitulo = new Label() { Text = "FORNECEDORES", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(30, 60, 120), AutoSize = true, Location = new Point(170, 25) };
            panelTopo.Controls.Add(lblTitulo);

            btnNovo = new Button() { Text = "+ NOVO FORNECEDOR", Location = new Point(800, 20), Size = new Size(170, 40), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnNovo.Click += (s, e) => AbrirCadastro(0);
            panelTopo.Controls.Add(btnNovo);

            gridFornecedores = new DataGridView() { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, Font = new Font("Segoe UI", 11) };
            gridFornecedores.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridFornecedores.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridFornecedores.EnableHeadersVisualStyles = false;

            // Duplo clique para editar
            gridFornecedores.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                {
                    int id = Convert.ToInt32(gridFornecedores.Rows[e.RowIndex].Cells["Id"].Value);
                    AbrirCadastro(id);
                }
            };
            this.Controls.Add(gridFornecedores);
            gridFornecedores.BringToFront();

            CarregarFornecedores();
        }

        private void CarregarFornecedores()
        {
            string query = @"SELECT f.Id, f.Nome AS [Nome], f.Cidade AS [Cidade], f.Estado AS [UF], 
                             l.Nome AS [Loja Vinculada], f.MarcasQueTrabalha AS [Marcas]
                             FROM Fornecedores f
                             INNER JOIN Lojas l ON f.LojaVinculadaId = l.Id
                             ORDER BY f.Nome";
            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridFornecedores.DataSource = dt;
                if (gridFornecedores.Columns["Id"] != null) gridFornecedores.Columns["Id"].Visible = false;
            }
        }

        private void AbrirCadastro(int idFornecedor)
        {
            FrmCadastroFornecedor tela = new FrmCadastroFornecedor(idFornecedor);
            tela.ShowDialog();
            CarregarFornecedores();
        }
    }

    // =========================================================
    // TELA DE CADASTRO / EDIÇÃO DE FORNECEDOR
    // =========================================================
    public class FrmCadastroFornecedor : Form
    {
        private int idFornecedorEdicao = 0;
        private TextBox txtNome, txtCidade, txtEstado, txtMarcas;
        private ComboBox cbLoja;
        private Button btnSalvar, btnCancelar, btnExcluir;
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmCadastroFornecedor(int idFornecedor = 0)
        {
            idFornecedorEdicao = idFornecedor;
            this.Text = idFornecedor == 0 ? "Novo Fornecedor" : "Editar Fornecedor";
            this.Size = new Size(500, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.White;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            Label lbl1 = new Label() { Text = "Nome do Fornecedor:*", Location = new Point(30, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtNome = new TextBox() { Location = new Point(30, 45), Size = new Size(420, 28), CharacterCasing = CharacterCasing.Upper, Font = new Font("Segoe UI", 12) };

            Label lbl2 = new Label() { Text = "Cidade:*", Location = new Point(30, 85), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtCidade = new TextBox() { Location = new Point(30, 110), Size = new Size(280, 28), CharacterCasing = CharacterCasing.Upper, Font = new Font("Segoe UI", 12) };

            Label lbl3 = new Label() { Text = "Estado (UF):*", Location = new Point(330, 85), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtEstado = new TextBox() { Location = new Point(330, 110), Size = new Size(120, 28), CharacterCasing = CharacterCasing.Upper, MaxLength = 2, Font = new Font("Segoe UI", 12) };

            Label lbl4 = new Label() { Text = "Loja Vinculada (Onde a compra é faturada):*", Location = new Point(30, 150), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cbLoja = new ComboBox() { Location = new Point(30, 175), Size = new Size(420, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };
            CarregarLojas();

            Label lbl5 = new Label() { Text = "Marcas que trabalha (Ex: CUMMINS, MWM, SCANIA):", Location = new Point(30, 215), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtMarcas = new TextBox() { Location = new Point(30, 240), Size = new Size(420, 28), CharacterCasing = CharacterCasing.Upper, Font = new Font("Segoe UI", 12) };

            btnSalvar = new Button() { Text = "SALVAR", Location = new Point(30, 300), Size = new Size(150, 45), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnSalvar.Click += (s, e) => Salvar();

            btnCancelar = new Button() { Text = "CANCELAR", Location = new Point(200, 300), Size = new Size(120, 45), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();

            btnExcluir = new Button() { Text = "EXCLUIR", Location = new Point(340, 300), Size = new Size(110, 45), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Visible = false };
            btnExcluir.Click += (s, e) => Excluir();

            this.Controls.Add(lbl1); this.Controls.Add(txtNome);
            this.Controls.Add(lbl2); this.Controls.Add(txtCidade);
            this.Controls.Add(lbl3); this.Controls.Add(txtEstado);
            this.Controls.Add(lbl4); this.Controls.Add(cbLoja);
            this.Controls.Add(lbl5); this.Controls.Add(txtMarcas);
            this.Controls.Add(btnSalvar); this.Controls.Add(btnCancelar); this.Controls.Add(btnExcluir);

            if (idFornecedorEdicao > 0)
            {
                btnExcluir.Visible = true;
                CarregarDados();
            }
        }

        private void CarregarLojas()
        {
            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                // Puxa apenas lojas ativas (Matriz e Distribuidora). Santana ficará oculta.
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Nome FROM Lojas WHERE Ativa = 1", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cbLoja.DataSource = dt;
                cbLoja.DisplayMember = "Nome";
                cbLoja.ValueMember = "Id";
            }
        }

        private void CarregarDados()
        {
            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                conn.Open();
                string query = "SELECT Nome, Cidade, Estado, LojaVinculadaId, MarcasQueTrabalha FROM Fornecedores WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idFornecedorEdicao);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtNome.Text = reader["Nome"].ToString();
                            txtCidade.Text = reader["Cidade"].ToString();
                            txtEstado.Text = reader["Estado"].ToString();
                            cbLoja.SelectedValue = reader["LojaVinculadaId"];
                            txtMarcas.Text = reader["MarcasQueTrabalha"].ToString();
                        }
                    }
                }
            }
        }

        private void Salvar()
        {
            if (string.IsNullOrEmpty(txtNome.Text) || string.IsNullOrEmpty(txtCidade.Text) || string.IsNullOrEmpty(txtEstado.Text))
            {
                MessageBox.Show("Preencha os campos obrigatórios (*).", "Aviso");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    if (idFornecedorEdicao == 0)
                    {
                        string query = "INSERT INTO Fornecedores (Nome, Cidade, Estado, LojaVinculadaId, MarcasQueTrabalha) VALUES (@nome, @cidade, @estado, @loja, @marcas)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
                            cmd.Parameters.AddWithValue("@cidade", txtCidade.Text.Trim());
                            cmd.Parameters.AddWithValue("@estado", txtEstado.Text.Trim());
                            cmd.Parameters.AddWithValue("@loja", cbLoja.SelectedValue);
                            cmd.Parameters.AddWithValue("@marcas", txtMarcas.Text.Trim());
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = "UPDATE Fornecedores SET Nome = @nome, Cidade = @cidade, Estado = @estado, LojaVinculadaId = @loja, MarcasQueTrabalha = @marcas WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
                            cmd.Parameters.AddWithValue("@cidade", txtCidade.Text.Trim());
                            cmd.Parameters.AddWithValue("@estado", txtEstado.Text.Trim());
                            cmd.Parameters.AddWithValue("@loja", cbLoja.SelectedValue);
                            cmd.Parameters.AddWithValue("@marcas", txtMarcas.Text.Trim());
                            cmd.Parameters.AddWithValue("@id", idFornecedorEdicao);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Fornecedor salvo!", "Sucesso");
                    this.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao salvar: " + ex.Message); }
        }

        private void Excluir()
        {
            if (MessageBox.Show("Deseja excluir este fornecedor?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(conexaoString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Fornecedores WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", idFornecedorEdicao);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Fornecedor excluído!", "Sucesso");
                        this.Close();
                    }
                }
                catch { MessageBox.Show("Não é possível excluir, pois ele já está vinculado a notas ou cotações.", "Erro"); }
            }
        }
    }
}