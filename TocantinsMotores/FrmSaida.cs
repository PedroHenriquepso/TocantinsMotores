using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = System.Drawing.Font;

namespace TocantinsMotores
{
    public class FrmSaida : Form
    {
        private Button btnVoltar;
        private Label lblTitulo;
        private RadioButton rbSaidaCliente, rbTransferencia;
        private TextBox txtSolicitante, txtDestino;
        private TextBox txtCodFabricante, txtQtd;
        private Button btnAddItem, btnBuscarPeca, btnFinalizar, btnRemover;
        private DataGridView gridItens;
        private DataTable dtItens;

        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmSaida()
        {
            ConfigurarJanela();
            CriarTopo();
            CriarFormulario();
            CriarGrid();
            IniciarCarrinho();
            AtualizarCampos();
        }

        private void ConfigurarJanela()
        {
            this.Text = "Registrar Saída / Transferência";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };
        }

        private void CriarTopo()
        {
            Panel panelTopo = new Panel();
            panelTopo.Dock = DockStyle.Top;
            panelTopo.Height = 70;
            panelTopo.BackColor = Color.White;
            this.Controls.Add(panelTopo);

            btnVoltar = new Button();
            btnVoltar.Text = "← VOLTAR (ESC)";
            btnVoltar.Location = new Point(20, 15);
            btnVoltar.Size = new Size(130, 40);
            btnVoltar.BackColor = Color.LightGray;
            btnVoltar.FlatStyle = FlatStyle.Flat;
            btnVoltar.FlatAppearance.BorderSize = 0;
            btnVoltar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnVoltar.Click += (s, e) => this.Close();
            panelTopo.Controls.Add(btnVoltar);

            lblTitulo = new Label();
            lblTitulo.Text = $"SAÍDA / TRANSFERÊNCIA (Loja: {SessaoSistema.NomeLojaAtiva})";
            lblTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(30, 60, 120);
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point(170, 20);
            panelTopo.Controls.Add(lblTitulo);
        }

        private void CriarFormulario()
        {
            Panel panelForm = new Panel();
            panelForm.Dock = DockStyle.Top;
            panelForm.Height = 150;
            panelForm.BackColor = Color.White;
            this.Controls.Add(panelForm);

            rbSaidaCliente = new RadioButton() { Text = "Saída para Cliente", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), Checked = true };
            rbTransferencia = new RadioButton() { Text = "Transferência entre Lojas", Location = new Point(180, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            rbSaidaCliente.CheckedChanged += (s, e) => AtualizarCampos();
            rbTransferencia.CheckedChanged += (s, e) => AtualizarCampos();

            panelForm.Controls.Add(rbSaidaCliente); panelForm.Controls.Add(rbTransferencia);

            Label lblSolicitante = new Label() { Text = "Solicitante:", Location = new Point(20, 60), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtSolicitante = new TextBox() { Location = new Point(20, 80), Size = new Size(200, 25), CharacterCasing = CharacterCasing.Upper };

            Label lblDestino = new Label() { Text = "Cliente / Destino:", Location = new Point(240, 60), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtDestino = new TextBox() { Location = new Point(240, 80), Size = new Size(250, 25), CharacterCasing = CharacterCasing.Upper };

            panelForm.Controls.Add(lblSolicitante); panelForm.Controls.Add(txtSolicitante);
            panelForm.Controls.Add(lblDestino); panelForm.Controls.Add(txtDestino);

            Label lblAdd = new Label() { Text = "Adicionar Peça (Cód. Fab. ou Descrição):", Location = new Point(520, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtCodFabricante = new TextBox() { Location = new Point(520, 40), Size = new Size(250, 25), CharacterCasing = CharacterCasing.Upper };
            txtCodFabricante.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; AdicionarItem(); } };

            Label lblQtd = new Label() { Text = "Qtd:", Location = new Point(780, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtQtd = new TextBox() { Location = new Point(780, 40), Size = new Size(60, 25), Text = "1" };

            btnAddItem = new Button() { Text = "ADICIONAR (Enter)", Location = new Point(850, 38), Size = new Size(150, 28), BackColor = Color.FromArgb(30, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnAddItem.Click += (s, e) => AdicionarItem();

            btnBuscarPeca = new Button() { Text = "🔍 Pesquisar Peça (F2)", Location = new Point(520, 80), Size = new Size(180, 30), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnBuscarPeca.Click += (s, e) => AbrirPesquisa();

            btnRemover = new Button() { Text = "🗑️ REMOVER SELECIONADO", Location = new Point(710, 80), Size = new Size(180, 30), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnRemover.Click += (s, e) => RemoverItem();

            panelForm.Controls.Add(lblAdd); panelForm.Controls.Add(txtCodFabricante);
            panelForm.Controls.Add(lblQtd); panelForm.Controls.Add(txtQtd);
            panelForm.Controls.Add(btnAddItem); panelForm.Controls.Add(btnBuscarPeca);
            panelForm.Controls.Add(btnRemover);
        }

        private void CriarGrid()
        {
            gridItens = new DataGridView();
            gridItens.Dock = DockStyle.Fill;
            gridItens.BackgroundColor = Color.White;
            gridItens.BorderStyle = BorderStyle.None;
            gridItens.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridItens.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridItens.AllowUserToAddRows = false;
            gridItens.RowHeadersVisible = false;
            gridItens.Font = new Font("Segoe UI", 11);
            gridItens.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            gridItens.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridItens.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridItens.EnableHeadersVisualStyles = false;

            gridItens.CellEndEdit += GridItens_CellEndEdit;
            gridItens.CellBeginEdit += GridItens_CellBeginEdit;

            this.Controls.Add(gridItens);
            gridItens.BringToFront();

            Panel panelBaixo = new Panel();
            panelBaixo.Dock = DockStyle.Bottom;
            panelBaixo.Height = 70;
            panelBaixo.BackColor = Color.White;
            this.Controls.Add(panelBaixo);

            btnFinalizar = new Button() { Text = "SALVAR E GERAR PDF", Location = new Point(this.Width - 250, 15), Size = new Size(220, 40), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnFinalizar.Click += (s, e) => FinalizarMovimentacao();
            panelBaixo.Controls.Add(btnFinalizar);
        }

        private void IniciarCarrinho()
        {
            dtItens = new DataTable();
            dtItens.Columns.Add("ProdutoId", typeof(int));
            dtItens.Columns.Add("Cód. Fab.", typeof(string));
            dtItens.Columns.Add("Descrição", typeof(string));
            dtItens.Columns.Add("Aplicação", typeof(string));
            dtItens.Columns.Add("Marca", typeof(string));
            dtItens.Columns.Add("Qtd", typeof(int));
            dtItens.Columns.Add("Estoque Atual", typeof(int));

            gridItens.DataSource = dtItens;
            gridItens.Columns["ProdutoId"].Visible = false;

            gridItens.Columns["Cód. Fab."].ReadOnly = true;
            gridItens.Columns["Descrição"].ReadOnly = true;
            gridItens.Columns["Aplicação"].ReadOnly = true;
            gridItens.Columns["Marca"].ReadOnly = true;
            gridItens.Columns["Estoque Atual"].ReadOnly = true;
        }

        private void GridItens_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (gridItens.Columns[e.ColumnIndex].Name != "Qtd")
            {
                e.Cancel = true;
            }
        }

        private void GridItens_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (gridItens.Columns[e.ColumnIndex].Name == "Qtd")
            {
                DataGridViewRow row = gridItens.Rows[e.RowIndex];
                int estoqueMaximo = Convert.ToInt32(row.Cells["Estoque Atual"].Value);
                int novaQtd;

                if (!int.TryParse(row.Cells["Qtd"].Value?.ToString(), out novaQtd) || novaQtd <= 0)
                {
                    MessageBox.Show("Quantidade inválida.", "Aviso");
                    row.Cells["Qtd"].Value = 1;
                    return;
                }

                if (novaQtd > estoqueMaximo)
                {
                    MessageBox.Show($"Estoque insuficiente! Máximo disponível: {estoqueMaximo}", "Aviso");
                    row.Cells["Qtd"].Value = estoqueMaximo;
                }
            }
        }

        private void RemoverItem()
        {
            if (gridItens.CurrentRow != null)
            {
                gridItens.Rows.RemoveAt(gridItens.CurrentRow.Index);
            }
        }

        private void AtualizarCampos()
        {
            if (rbSaidaCliente.Checked)
            {
                txtDestino.Enabled = true;
            }
            else
            {
                txtDestino.Enabled = false;
                txtDestino.Text = "TRANSFERÊNCIA INTERNA";
            }
        }

        private void AbrirPesquisa()
        {
            FrmPesquisaProdutoSaida pesquisa = new FrmPesquisaProdutoSaida(txtCodFabricante.Text.Trim());
            if (pesquisa.ShowDialog() == DialogResult.OK)
            {
                txtCodFabricante.Text = pesquisa.CodFabricanteSelecionado;
                AdicionarItem();
            }
        }

        private void AdicionarItem()
        {
            string termo = txtCodFabricante.Text.Trim();
            if (string.IsNullOrEmpty(termo)) return;

            if (!int.TryParse(txtQtd.Text, out int qtd) || qtd <= 0)
            {
                MessageBox.Show("Quantidade inválida.", "Aviso");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    string query = $@"SELECT p.Id, p.CodigoFabricante, p.Descricao, p.Marca, p.Aplicacao, ISNULL(e.Quantidade, 0) AS Estoque 
                                      FROM Produtos p 
                                      LEFT JOIN Estoques e ON p.Id = e.ProdutoId AND e.LojaId = {SessaoSistema.LojaIdAtiva}
                                      WHERE p.CodigoFabricante LIKE '%{termo}%' OR p.Descricao LIKE '%{termo}%' OR p.Aplicacao LIKE '%{termo}%'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int prodId = Convert.ToInt32(reader["Id"]);
                            string codFab = reader["CodigoFabricante"].ToString();
                            string desc = reader["Descricao"].ToString();
                            string aplicacao = reader["Aplicacao"].ToString();
                            string marca = reader["Marca"].ToString();
                            int estoque = Convert.ToInt32(reader["Estoque"]);

                            if (estoque < qtd)
                            {
                                MessageBox.Show($"Estoque insuficiente na loja atual ({SessaoSistema.NomeLojaAtiva})! Disponível: {estoque}", "Aviso");
                                return;
                            }

                            foreach (DataRow row in dtItens.Rows)
                            {
                                if (Convert.ToInt32(row["ProdutoId"]) == prodId)
                                {
                                    int qtdNoCarrinho = Convert.ToInt32(row["Qtd"]);
                                    if (qtdNoCarrinho + qtd > estoque)
                                    {
                                        MessageBox.Show($"A soma com o que já está no carrinho ultrapassa o estoque (Disponível: {estoque}).", "Aviso");
                                        return;
                                    }
                                    row["Qtd"] = qtdNoCarrinho + qtd;
                                    txtCodFabricante.Clear();
                                    txtQtd.Text = "1";
                                    txtCodFabricante.Focus();
                                    return;
                                }
                            }

                            dtItens.Rows.Add(prodId, codFab, desc, aplicacao, marca, qtd, estoque);
                            txtCodFabricante.Clear();
                            txtQtd.Text = "1";
                            txtCodFabricante.Focus();
                        }
                        else
                        {
                            MessageBox.Show("Produto não encontrado.", "Aviso");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void FinalizarMovimentacao()
        {
            if (dtItens.Rows.Count == 0)
            {
                MessageBox.Show("Adicione pelo menos um item.", "Aviso");
                return;
            }

            if (string.IsNullOrEmpty(txtSolicitante.Text))
            {
                MessageBox.Show("Preencha o nome do Solicitante.", "Aviso");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();

                    string tipo = rbSaidaCliente.Checked ? "SAIDA_CLIENTE" : "TRANSFERENCIA";
                    string prefixo = rbSaidaCliente.Checked ? "SV" : "TRF";

                    string queryMax = $"SELECT COUNT(*) FROM Movimentacoes WHERE Tipo = '{tipo}'";
                    int contador = 1;
                    using (SqlCommand cmdMax = new SqlCommand(queryMax, conn))
                    {
                        contador = Convert.ToInt32(cmdMax.ExecuteScalar()) + 1;
                    }
                    string numeroDoc = $"{prefixo}{contador:D2}";

                    int lojaDestinoId = SessaoSistema.LojaIdAtiva;
                    if (rbTransferencia.Checked)
                        lojaDestinoId = SessaoSistema.LojaIdAtiva == 1 ? 2 : 1;

                    string sqlMov = @"INSERT INTO Movimentacoes (Tipo, NumeroDocumento, LojaOrigemId, LojaDestinoId, UsuarioId, SolicitanteNome, Status) 
                                      OUTPUT INSERTED.Id 
                                      VALUES (@tipo, @numero, @lojaId, @destinoId, @usuarioId, @solicitante, 'CONCLUIDO')";

                    int movId = 0;
                    using (SqlCommand cmd = new SqlCommand(sqlMov, conn))
                    {
                        cmd.Parameters.AddWithValue("@tipo", tipo);
                        cmd.Parameters.AddWithValue("@numero", numeroDoc);
                        cmd.Parameters.AddWithValue("@lojaId", SessaoSistema.LojaIdAtiva);
                        cmd.Parameters.AddWithValue("@destinoId", lojaDestinoId);
                        cmd.Parameters.AddWithValue("@usuarioId", SessaoSistema.UsuarioId);
                        cmd.Parameters.AddWithValue("@solicitante", txtSolicitante.Text.Trim());
                        movId = (int)cmd.ExecuteScalar();
                    }

                    foreach (DataRow row in dtItens.Rows)
                    {
                        int prodId = Convert.ToInt32(row["ProdutoId"]);
                        int qtd = Convert.ToInt32(row["Qtd"]);

                        string sqlItem = "INSERT INTO Movimentacoes_Itens (MovimentacaoId, ProdutoId, Quantidade) VALUES (@movId, @prodId, @qtd)";
                        using (SqlCommand cmd = new SqlCommand(sqlItem, conn))
                        {
                            cmd.Parameters.AddWithValue("@movId", movId);
                            cmd.Parameters.AddWithValue("@prodId", prodId);
                            cmd.Parameters.AddWithValue("@qtd", qtd);
                            cmd.ExecuteNonQuery();
                        }

                        string sqlEstoque = "UPDATE Estoques SET Quantidade = Quantidade - @qtd WHERE ProdutoId = @prodId AND LojaId = @lojaId";
                        using (SqlCommand cmd = new SqlCommand(sqlEstoque, conn))
                        {
                            cmd.Parameters.AddWithValue("@qtd", qtd);
                            cmd.Parameters.AddWithValue("@prodId", prodId);
                            cmd.Parameters.AddWithValue("@lojaId", SessaoSistema.LojaIdAtiva);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    SessaoSistema.RegistrarLog(tipo, $"Gerou documento {numeroDoc} | Solicitante: {txtSolicitante.Text}");

                    GerarPDF(numeroDoc, tipo);
                    MessageBox.Show($"Movimentação {numeroDoc} salva com sucesso!", "Sucesso");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao finalizar: " + ex.Message);
            }
        }

        private void GerarPDF(string numeroDoc, string tipo)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF (*.pdf)|*.pdf";
            sfd.FileName = $"Movimentacao_{numeroDoc}.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4, 30, 30, 30, 30);
                    PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    Paragraph titulo = new Paragraph("TOCANTINS MOTORES - CONTROLE DE ESTOQUE", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    doc.Add(titulo);

                    Paragraph sub = new Paragraph($"COMPROVANTE DE {(tipo == "TRANSFERENCIA" ? "TRANSFERÊNCIA" : "SAÍDA")} Nº: {numeroDoc}", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                    sub.Alignment = Element.ALIGN_CENTER;
                    doc.Add(sub);
                    doc.Add(new Paragraph($"Data: {DateTime.Now:dd/MM/yyyy HH:mm} | Loja: {SessaoSistema.NomeLojaAtiva}"));
                    doc.Add(new Paragraph($"Solicitante: {txtSolicitante.Text} | Estoquista: {SessaoSistema.NomeUsuario}"));
                    doc.Add(new Paragraph("--------------------------------------------------------------------------------------"));

                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 15f, 40f, 20f, 10f, 15f });

                    table.AddCell("Cód. Fab.");
                    table.AddCell("Descrição");
                    table.AddCell("Aplicação");
                    table.AddCell("Qtd");
                    table.AddCell("Marca");

                    foreach (DataRow row in dtItens.Rows)
                    {
                        table.AddCell(row["Cód. Fab."].ToString());
                        table.AddCell(row["Descrição"].ToString());
                        table.AddCell(row["Aplicação"].ToString());
                        table.AddCell(row["Qtd"].ToString());
                        table.AddCell(row["Marca"].ToString());
                    }
                    doc.Add(table);

                    doc.Add(new Paragraph("\n\n\n"));
                    doc.Add(new Paragraph("______________________________             ______________________________"));
                    doc.Add(new Paragraph($"Assinatura do Solicitante                 Assinatura do Estoquista"));
                    doc.Add(new Paragraph($"({txtSolicitante.Text})                              ({SessaoSistema.NomeUsuario})"));

                    doc.Close();
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao gerar PDF: " + ex.Message);
                }
            }
        }
    }

    // =========================================================
    // JANELA DE PESQUISA DE PEÇA (Com Estoques Separados)
    // =========================================================
    public class FrmPesquisaProdutoSaida : Form
    {
        private TextBox txtBusca;
        private DataGridView grid;
        public string CodFabricanteSelecionado { get; set; }
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmPesquisaProdutoSaida(string buscaInicial)
        {
            this.Text = "Pesquisar Peça";
            this.Size = new Size(900, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            Label lbl = new Label() { Text = "Pesquisar (Cód. Fab, Descrição ou Aplicação):", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtBusca = new TextBox() { Location = new Point(20, 40), Size = new Size(400, 25), Text = buscaInicial, CharacterCasing = CharacterCasing.Upper };
            txtBusca.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Carregar(); } };

            Button btnBuscar = new Button() { Text = "🔍 BUSCAR", Location = new Point(430, 38), Size = new Size(100, 28), BackColor = Color.FromArgb(30, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnBuscar.Click += (s, e) => Carregar();

            grid = new DataGridView() { Location = new Point(20, 80), Size = new Size(840, 350), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, Font = new Font("Segoe UI", 11) };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.EnableHeadersVisualStyles = false;
            grid.CellDoubleClick += (s, e) => Selecionar();
            grid.RowPrePaint += Grid_RowPrePaint;

            Button btnAdd = new Button() { Text = "SELECIONAR", Location = new Point(20, 450), Size = new Size(150, 40), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnAdd.Click += (s, e) => Selecionar();

            Button btnCancelar = new Button() { Text = "CANCELAR", Location = new Point(180, 450), Size = new Size(100, 40), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.Add(lbl); this.Controls.Add(txtBusca); this.Controls.Add(btnBuscar);
            this.Controls.Add(grid); this.Controls.Add(btnAdd); this.Controls.Add(btnCancelar);

            Carregar();
        }

        private void Carregar()
        {
            string termo = txtBusca.Text.Trim();
            string filtro = string.IsNullOrEmpty(termo) ? "" : $"AND (p.Descricao LIKE '%{termo}%' OR p.CodigoFabricante LIKE '%{termo}%' OR p.Aplicacao LIKE '%{termo}%')";

            // BUSCA O ESTOQUE DA MATRIZ E DA DISTRIBUIDORA SEPARADAMENTE
            string query = $@"
                SELECT 
                    p.Id, 
                    p.CodigoFabricante AS [Cód. Fab.], 
                    p.Descricao AS [Descrição], 
                    p.Aplicacao AS [Aplicação], 
                    p.Marca,
                    ISNULL(e1.Quantidade, 0) AS [Estoque Matriz],
                    ISNULL(e2.Quantidade, 0) AS [Estoque Distrib.]
                FROM Produtos p 
                LEFT JOIN Estoques e1 ON p.Id = e1.ProdutoId AND e1.LojaId = 1 
                LEFT JOIN Estoques e2 ON p.Id = e2.ProdutoId AND e2.LojaId = 2 
                WHERE 1=1 {filtro} 
                ORDER BY p.Descricao";

            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                grid.DataSource = dt;
                if (grid.Columns["Id"] != null) grid.Columns["Id"].Visible = false;
            }
        }

        private void Selecionar()
        {
            if (grid.CurrentRow != null)
            {
                CodFabricanteSelecionado = grid.CurrentRow.Cells["Cód. Fab."].Value.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void Grid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            // Pega a linha atual
            DataGridViewRow row = grid.Rows[e.RowIndex];

            // Verifica qual loja está ativa
            int colEstoqueIndex = SessaoSistema.LojaIdAtiva == 1 ? grid.Columns["Estoque Matriz"].Index : grid.Columns["Estoque Distrib."].Index;

            // Se a quantidade for 0, pinta a linha de vermelho claro
            int estoque = Convert.ToInt32(row.Cells[colEstoqueIndex].Value);

            if (estoque == 0)
            {
                row.DefaultCellStyle.BackColor = Color.MistyRose; // Vermelho bem claro
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
            }
            else
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;
            }
        }
    }
}