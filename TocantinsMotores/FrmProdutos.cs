using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TocantinsMotores
{
    public class FrmProdutos : Form
    {
        private TextBox txtPesquisa;
        private Button btnBuscar;
        private Button btnNovo;
        private Button btnVoltar;
        private Button btnEntradaManual;
        private DataGridView gridProdutos;
        private Label lblTitulo;

        // Novos componentes para o painel de estoque
        private Panel panelEstoqueLojas;
        private DataGridView gridEstoqueLojas;
        private Label lblEstoqueTitulo;
        private int idProdutoSelecionado = 0;

        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmProdutos()
        {
            ConfigurarJanela();
            CriarTopo();
            CriarGrid();
            CriarPainelEstoque();
            CarregarProdutos();
        }

        private void ConfigurarJanela()
        {
            this.Text = "Cadastro de Produtos";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };
        }

        private void CriarTopo()
        {
            Panel panelTopo = new Panel();
            panelTopo.Dock = DockStyle.Top;
            panelTopo.Height = 80;
            panelTopo.BackColor = Color.White;
            this.Controls.Add(panelTopo);

            btnVoltar = new Button();
            btnVoltar.Text = "← VOLTAR (ESC)";
            btnVoltar.Location = new Point(20, 20);
            btnVoltar.Size = new Size(130, 40);
            btnVoltar.BackColor = Color.LightGray;
            btnVoltar.FlatStyle = FlatStyle.Flat;
            btnVoltar.FlatAppearance.BorderSize = 0;
            btnVoltar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnVoltar.Click += (s, e) => this.Close();
            panelTopo.Controls.Add(btnVoltar);

            lblTitulo = new Label();
            lblTitulo.Text = "PRODUTOS";
            lblTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(30, 60, 120);
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(170, 25);
            panelTopo.Controls.Add(lblTitulo);

            txtPesquisa = new TextBox();
            txtPesquisa.Location = new Point(400, 28);
            txtPesquisa.Size = new Size(300, 28);
            txtPesquisa.Font = new Font("Segoe UI", 12);
            txtPesquisa.CharacterCasing = CharacterCasing.Upper;
            txtPesquisa.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; CarregarProdutos(); } };
            panelTopo.Controls.Add(txtPesquisa);

            btnBuscar = new Button();
            btnBuscar.Text = "🔍 BUSCAR";
            btnBuscar.Location = new Point(710, 25);
            btnBuscar.Size = new Size(100, 32);
            btnBuscar.BackColor = Color.FromArgb(30, 60, 120);
            btnBuscar.ForeColor = Color.White;
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBuscar.Click += (s, e) => CarregarProdutos();
            panelTopo.Controls.Add(btnBuscar);

            btnEntradaManual = new Button();
            btnEntradaManual.Text = "📥 ENTRADA MANUAL";
            btnEntradaManual.Location = new Point(830, 20);
            btnEntradaManual.Size = new Size(170, 40);
            btnEntradaManual.BackColor = Color.DarkOrange;
            btnEntradaManual.ForeColor = Color.White;
            btnEntradaManual.FlatStyle = FlatStyle.Flat;
            btnEntradaManual.FlatAppearance.BorderSize = 0;
            btnEntradaManual.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEntradaManual.Click += (s, e) => AbrirEntradaManual();
            panelTopo.Controls.Add(btnEntradaManual);

            btnNovo = new Button();
            btnNovo.Text = "+ NOVO PRODUTO";
            btnNovo.Location = new Point(1020, 20);
            btnNovo.Size = new Size(150, 40);
            btnNovo.BackColor = Color.MediumSeaGreen;
            btnNovo.ForeColor = Color.White;
            btnNovo.FlatStyle = FlatStyle.Flat;
            btnNovo.FlatAppearance.BorderSize = 0;
            btnNovo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnNovo.Click += (s, e) => AbrirCadastro(0);
            panelTopo.Controls.Add(btnNovo);
        }

        private void CriarGrid()
        {
            gridProdutos = new DataGridView();
            gridProdutos.Dock = DockStyle.Fill;
            gridProdutos.BackgroundColor = Color.White;
            gridProdutos.BorderStyle = BorderStyle.None;
            gridProdutos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridProdutos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridProdutos.AllowUserToAddRows = false;
            gridProdutos.ReadOnly = true;
            gridProdutos.RowHeadersVisible = false;
            gridProdutos.Font = new Font("Segoe UI", 11);
            gridProdutos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            gridProdutos.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridProdutos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridProdutos.EnableHeadersVisualStyles = false;

            // Eventos do Grid
            gridProdutos.CellDoubleClick += GridProdutos_CellDoubleClick;
            gridProdutos.SelectionChanged += GridProdutos_SelectionChanged; // Evento de clique para mudar o painel de baixo

            this.Controls.Add(gridProdutos);
            gridProdutos.BringToFront();
        }

        // NOVO PAINEL DE ESTOQUE POR LOJA
        private void CriarPainelEstoque()
        {
            panelEstoqueLojas = new Panel();
            panelEstoqueLojas.Dock = DockStyle.Bottom;
            panelEstoqueLojas.Height = 150;
            panelEstoqueLojas.BackColor = Color.FromArgb(235, 235, 235);
            panelEstoqueLojas.BorderStyle = BorderStyle.FixedSingle;
            panelEstoqueLojas.Visible = false; // Só aparece quando selecionar um produto
            this.Controls.Add(panelEstoqueLojas);

            lblEstoqueTitulo = new Label();
            lblEstoqueTitulo.Text = "Estoque por Loja";
            lblEstoqueTitulo.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblEstoqueTitulo.ForeColor = Color.FromArgb(30, 60, 120);
            lblEstoqueTitulo.AutoSize = true;
            lblEstoqueTitulo.Location = new Point(20, 10);
            panelEstoqueLojas.Controls.Add(lblEstoqueTitulo);

            gridEstoqueLojas = new DataGridView();
            gridEstoqueLojas.Location = new Point(20, 35);
            gridEstoqueLojas.Size = new Size(400, 100);
            gridEstoqueLojas.BackgroundColor = Color.White;
            gridEstoqueLojas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridEstoqueLojas.AllowUserToAddRows = false;
            gridEstoqueLojas.ReadOnly = true;
            gridEstoqueLojas.RowHeadersVisible = false;
            gridEstoqueLojas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridEstoqueLojas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridEstoqueLojas.EnableHeadersVisualStyles = false;
            panelEstoqueLojas.Controls.Add(gridEstoqueLojas);
        }

        private void GridProdutos_SelectionChanged(object sender, EventArgs e)
        {
            if (gridProdutos.CurrentRow != null && gridProdutos.CurrentRow.Cells["Id"].Value != null)
            {
                idProdutoSelecionado = Convert.ToInt32(gridProdutos.CurrentRow.Cells["Id"].Value);
                CarregarEstoquePorLoja(idProdutoSelecionado);
            }
        }

        private void CarregarEstoquePorLoja(int idProduto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    // Puxa o nome da loja e a quantidade. Se a loja não tiver registro, mostra 0.
                    string query = @"
                        SELECT l.Nome AS [Loja], ISNULL(e.Quantidade, 0) AS [Qtd]
                        FROM Lojas l
                        LEFT JOIN Estoques e ON l.Id = e.LojaId AND e.ProdutoId = @prodId
                        WHERE l.Ativa = 1
                        ORDER BY l.Id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@prodId", idProduto);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        gridEstoqueLojas.DataSource = dt;

                        // Ajusta a largura das colunas
                        if (gridEstoqueLojas.Columns["Loja"] != null)
                            gridEstoqueLojas.Columns["Loja"].FillWeight = 70;
                        if (gridEstoqueLojas.Columns["Qtd"] != null)
                            gridEstoqueLojas.Columns["Qtd"].FillWeight = 30;

                        panelEstoqueLojas.Visible = true; // Mostra o painel
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar estoque por loja: " + ex.Message);
            }
        }

        private void GridProdutos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int idProduto = Convert.ToInt32(gridProdutos.Rows[e.RowIndex].Cells["Id"].Value);
                AbrirCadastro(idProduto);
            }
        }

        private void CarregarProdutos()
        {
            string termo = txtPesquisa.Text.Trim();
            string filtro = string.IsNullOrEmpty(termo) ? "" : $"AND (p.Descricao LIKE '%{termo}%' OR p.CodigoFabricante LIKE '%{termo}%' OR p.Marca LIKE '%{termo}%' OR p.Aplicacao LIKE '%{termo}%')";

            string query = $@"
                SELECT 
                    p.Id,
                    p.CodigoInterno AS [Cód. Int],
                    p.CodigoFabricante AS [Cód. Fab.],
                    p.Descricao AS [Descrição],
                    p.Marca AS [Marca],
                    p.Aplicacao AS [Aplicação],
                    ISNULL(e.Quantidade, 0) AS [Estoque Atual]
                FROM Produtos p
                LEFT JOIN Estoques e ON p.Id = e.ProdutoId AND e.LojaId = {SessaoSistema.LojaIdAtiva}
                WHERE 1=1 {filtro}
                ORDER BY p.Descricao";

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    gridProdutos.DataSource = dt;
                    if (gridProdutos.Columns["Id"] != null) gridProdutos.Columns["Id"].Visible = false;

                    panelEstoqueLojas.Visible = false; // Esconde o painel ao recarregar
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar produtos: " + ex.Message);
            }
        }

        private void AbrirCadastro(int idProduto)
        {
            FrmCadastroProduto telaCadastro = new FrmCadastroProduto(idProduto);
            telaCadastro.ShowDialog();
            CarregarProdutos();
        }

        private void AbrirEntradaManual()
        {
            FrmEntradaManual telaEntrada = new FrmEntradaManual();
            telaEntrada.ShowDialog();
            CarregarProdutos();
        }
    }

    // =========================================================
    // TELA DE CADASTRO / EDIÇÃO DE PRODUTO (Mantida igual)
    // =========================================================
    public class FrmCadastroProduto : Form
    {
        private int idProdutoEdicao = 0;
        private TextBox txtCodFabricante, txtDescricao, txtMarca, txtAplicacao, txtQuantidade;
        private Button btnSalvar, btnCancelar, btnExcluir;
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmCadastroProduto(int idProduto = 0)
        {
            idProdutoEdicao = idProduto;
            this.Text = idProduto == 0 ? "Novo Produto" : "Editar Produto";
            this.Size = new Size(500, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            Label lbl1 = new Label() { Text = "Código Fabricante:*", Location = new Point(30, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtCodFabricante = new TextBox() { Location = new Point(30, 45), Size = new Size(420, 28), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };

            Label lbl2 = new Label() { Text = "Descrição:*", Location = new Point(30, 85), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtDescricao = new TextBox() { Location = new Point(30, 110), Size = new Size(420, 28), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };

            Label lbl3 = new Label() { Text = "Marca:*", Location = new Point(30, 150), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtMarca = new TextBox() { Location = new Point(30, 175), Size = new Size(420, 28), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };

            Label lblAplicacao = new Label() { Text = "Aplicação (Ex: CUMMINS, SCANIA):", Location = new Point(30, 215), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtAplicacao = new TextBox() { Location = new Point(30, 240), Size = new Size(420, 28), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };

            Label lbl4 = new Label() { Text = $"Estoque Inicial (Loja: {SessaoSistema.NomeLojaAtiva}):", Location = new Point(30, 280), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtQuantidade = new TextBox() { Location = new Point(30, 305), Size = new Size(150, 28), Font = new Font("Segoe UI", 12), Text = "0" };

            btnSalvar = new Button() { Text = "SALVAR", Location = new Point(30, 370), Size = new Size(150, 45), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnSalvar.Click += (s, e) => SalvarProduto();

            btnCancelar = new Button() { Text = "CANCELAR (ESC)", Location = new Point(200, 370), Size = new Size(150, 45), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();

            btnExcluir = new Button() { Text = "🗑️ EXCLUIR", Location = new Point(370, 370), Size = new Size(80, 45), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Visible = false };
            btnExcluir.Click += (s, e) => ExcluirProduto();

            this.Controls.Add(lbl1); this.Controls.Add(txtCodFabricante);
            this.Controls.Add(lbl2); this.Controls.Add(txtDescricao);
            this.Controls.Add(lbl3); this.Controls.Add(txtMarca);
            this.Controls.Add(lblAplicacao); this.Controls.Add(txtAplicacao);
            this.Controls.Add(lbl4); this.Controls.Add(txtQuantidade);
            this.Controls.Add(btnSalvar); this.Controls.Add(btnCancelar);
            this.Controls.Add(btnExcluir);

            if (idProdutoEdicao > 0)
            {
                CarregarDadosProduto();
                btnExcluir.Visible = true;
                txtQuantidade.Enabled = false;
                txtQuantidade.Text = "Use 'Entrada Manual' ou 'Saídas'";
            }
        }

        private void CarregarDadosProduto()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    string query = "SELECT CodigoFabricante, Descricao, Marca, Aplicacao FROM Produtos WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idProdutoEdicao);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtCodFabricante.Text = reader["CodigoFabricante"].ToString();
                                txtDescricao.Text = reader["Descricao"].ToString();
                                txtMarca.Text = reader["Marca"].ToString();
                                txtAplicacao.Text = reader["Aplicacao"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar: " + ex.Message); }
        }

        private void SalvarProduto()
        {
            if (string.IsNullOrEmpty(txtCodFabricante.Text) || string.IsNullOrEmpty(txtDescricao.Text) || string.IsNullOrEmpty(txtMarca.Text))
            {
                MessageBox.Show("Preencha todos os campos obrigatórios (*).", "Aviso");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM Produtos WHERE CodigoFabricante = @cod AND Id != @id";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@cod", txtCodFabricante.Text.Trim());
                        cmdCheck.Parameters.AddWithValue("@id", idProdutoEdicao);
                        int existe = (int)cmdCheck.ExecuteScalar();
                        if (existe > 0)
                        {
                            MessageBox.Show("Este Código de Fabricante já está cadastrado em outro produto!", "Aviso");
                            return;
                        }
                    }

                    if (idProdutoEdicao == 0)
                    {
                        if (!int.TryParse(txtQuantidade.Text, out int qtd))
                        {
                            MessageBox.Show("Quantidade inválida.", "Aviso");
                            return;
                        }

                        string queryMax = "SELECT ISNULL(MAX(CAST(CodigoInterno AS INT)), 0) + 1 FROM Produtos";
                        int proximoCodigo = 1;
                        using (SqlCommand cmdMax = new SqlCommand(queryMax, conn))
                        {
                            proximoCodigo = (int)cmdMax.ExecuteScalar();
                        }
                        string codigoInternoGerado = proximoCodigo.ToString("D3");

                        string queryProduto = "INSERT INTO Produtos (CodigoInterno, CodigoFabricante, Descricao, Marca, Aplicacao) OUTPUT INSERTED.Id VALUES (@codInt, @cod, @desc, @marca, @aplic)";

                        int novoProdutoId = 0;
                        using (SqlCommand cmd = new SqlCommand(queryProduto, conn))
                        {
                            cmd.Parameters.AddWithValue("@codInt", codigoInternoGerado);
                            cmd.Parameters.AddWithValue("@cod", txtCodFabricante.Text.Trim());
                            cmd.Parameters.AddWithValue("@desc", txtDescricao.Text.Trim());
                            cmd.Parameters.AddWithValue("@marca", txtMarca.Text.Trim());
                            cmd.Parameters.AddWithValue("@aplic", txtAplicacao.Text.Trim());
                            novoProdutoId = (int)cmd.ExecuteScalar();
                        }

                        string queryEstoque = "INSERT INTO Estoques (ProdutoId, LojaId, Quantidade) VALUES (@prodId, @lojaId, @qtd)";
                        using (SqlCommand cmd = new SqlCommand(queryEstoque, conn))
                        {
                            cmd.Parameters.AddWithValue("@prodId", novoProdutoId);
                            cmd.Parameters.AddWithValue("@lojaId", SessaoSistema.LojaIdAtiva);
                            cmd.Parameters.AddWithValue("@qtd", qtd);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show($"Produto cadastrado com sucesso! Código Interno: {codigoInternoGerado}", "Sucesso");
                    }
                    else
                    {
                        string queryUpdate = "UPDATE Produtos SET CodigoFabricante = @cod, Descricao = @desc, Marca = @marca, Aplicacao = @aplic WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(queryUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@cod", txtCodFabricante.Text.Trim());
                            cmd.Parameters.AddWithValue("@desc", txtDescricao.Text.Trim());
                            cmd.Parameters.AddWithValue("@marca", txtMarca.Text.Trim());
                            cmd.Parameters.AddWithValue("@aplic", txtAplicacao.Text.Trim());
                            cmd.Parameters.AddWithValue("@id", idProdutoEdicao);
                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Produto atualizado com sucesso!", "Sucesso");
                    }
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar produto: " + ex.Message, "Erro");
            }
        }

        private void ExcluirProduto()
        {
            if (MessageBox.Show("Tem certeza que deseja excluir este produto?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(conexaoString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Produtos WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", idProdutoEdicao);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Produto excluído!", "Sucesso");
                        this.Close();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Não é possível excluir este produto pois ele possui movimentações registradas.", "Erro");
                }
            }
        }
    }

    // A Classe FrmEntradaManual continua igual...
    public class FrmEntradaManual : Form
    {
        private TextBox txtCodFabricante, txtQuantidade;
        private Button btnSalvar, btnCancelar;
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmEntradaManual()
        {
            this.Text = "Entrada Manual de Estoque";
            this.Size = new Size(450, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            Label lbl1 = new Label() { Text = "Código Fabricante (ou parte dele):", Location = new Point(30, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtCodFabricante = new TextBox() { Location = new Point(30, 45), Size = new Size(370, 28), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };

            Label lbl2 = new Label() { Text = $"Quantidade a Adicionar (Loja: {SessaoSistema.NomeLojaAtiva}):", Location = new Point(30, 85), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtQuantidade = new TextBox() { Location = new Point(30, 110), Size = new Size(100, 28), Font = new Font("Segoe UI", 12), Text = "1" };

            btnSalvar = new Button() { Text = "ADICIONAR ESTOQUE", Location = new Point(30, 160), Size = new Size(170, 40), BackColor = Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnSalvar.Click += (s, e) => SalvarEntrada();

            btnCancelar = new Button() { Text = "CANCELAR (ESC)", Location = new Point(210, 160), Size = new Size(140, 40), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.Add(lbl1); this.Controls.Add(txtCodFabricante);
            this.Controls.Add(lbl2); this.Controls.Add(txtQuantidade);
            this.Controls.Add(btnSalvar); this.Controls.Add(btnCancelar);
        }

        private void SalvarEntrada()
        {
            if (!int.TryParse(txtQuantidade.Text, out int qtd) || qtd <= 0)
            {
                MessageBox.Show("Quantidade inválida.", "Aviso");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();

                    string queryProd = "SELECT Id FROM Produtos WHERE CodigoFabricante LIKE @cod";
                    int prodId = 0;
                    using (SqlCommand cmd = new SqlCommand(queryProd, conn))
                    {
                        cmd.Parameters.AddWithValue("@cod", $"%{txtCodFabricante.Text.Trim()}%");
                        object result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            MessageBox.Show("Produto não encontrado. Verifique o Código de Fabricante.", "Erro");
                            return;
                        }
                        prodId = (int)result;
                    }

                    string checkEstoque = "SELECT COUNT(*) FROM Estoques WHERE ProdutoId = @prodId AND LojaId = @lojaId";
                    bool temEstoque = false;
                    using (SqlCommand cmdCheck = new SqlCommand(checkEstoque, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@prodId", prodId);
                        cmdCheck.Parameters.AddWithValue("@lojaId", SessaoSistema.LojaIdAtiva);
                        temEstoque = (int)cmdCheck.ExecuteScalar() > 0;
                    }

                    if (temEstoque)
                    {
                        string sqlUpdate = "UPDATE Estoques SET Quantidade = Quantidade + @qtd WHERE ProdutoId = @prodId AND LojaId = @lojaId";
                        using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@qtd", qtd);
                            cmd.Parameters.AddWithValue("@prodId", prodId);
                            cmd.Parameters.AddWithValue("@lojaId", SessaoSistema.LojaIdAtiva);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string sqlInsert = "INSERT INTO Estoques (ProdutoId, LojaId, Quantidade) VALUES (@prodId, @lojaId, @qtd)";
                        using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                        {
                            cmd.Parameters.AddWithValue("@prodId", prodId);
                            cmd.Parameters.AddWithValue("@lojaId", SessaoSistema.LojaIdAtiva);
                            cmd.Parameters.AddWithValue("@qtd", qtd);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Estoque atualizado com sucesso!", "Sucesso");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }
    }
}