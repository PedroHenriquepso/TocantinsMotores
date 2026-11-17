using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace TocantinsMotores
{
    public class FrmCotacoes : Form
    {
        private Button btnVoltar, btnNovaCotacao;
        private Label lblTitulo;
        private TabControl tabControl;
        private TabPage tabAbertas, tabFinalizadas;

        // Componentes da aba Em Aberto
        private DataGridView gridAbertas;
        private Button btnPesquisarPeca, btnGerarPdf, btnExcel, btnRegistrarPrecos, btnRemover, btnCancelarCotacao;
        private TextBox txtCodFabricante, txtQtd;
        private DataGridView gridItens;
        private DataTable dtItens;
        private int idCotacaoAtiva = 0;
        private string numeroCotacaoAtiva = "";

        // Componentes da aba Finalizadas
        private DataGridView gridFinalizadas;

        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmCotacoes()
        {
            ConfigurarJanela();
            CriarTopo();
            CriarAbas();
            CarregarCotacaoAberta();
            CarregarCotacoesFinalizadas();
        }

        private void ConfigurarJanela()
        {
            this.Text = "Módulo de Cotações";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };
        }

        private void CriarTopo()
        {
            Panel panelTopo = new Panel() { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };
            this.Controls.Add(panelTopo);

            btnVoltar = new Button() { Text = "← VOLTAR (ESC)", Location = new Point(20, 15), Size = new Size(130, 40), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnVoltar.Click += (s, e) => this.Close();
            panelTopo.Controls.Add(btnVoltar);

            lblTitulo = new Label() { Text = $"COTAÇÕES - LOJA: {SessaoSistema.NomeLojaAtiva}", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(30, 60, 120), AutoSize = true, Location = new Point(170, 20) };
            panelTopo.Controls.Add(lblTitulo);
        }

        private void CriarAbas()
        {
            tabControl = new TabControl() { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11) };

            tabAbertas = new TabPage("COTAÇÕES EM ABERTO");
            tabFinalizadas = new TabPage("COTAÇÕES FINALIZADAS");

            tabControl.TabPages.Add(tabAbertas);
            tabControl.TabPages.Add(tabFinalizadas);
            this.Controls.Add(tabControl);
            tabControl.BringToFront();

            ConfigurarAbaAbertas();
            ConfigurarAbaFinalizadas();
        }

        // ==========================================
        // CONFIGURAÇÃO DA ABA EM ABERTO
        // ==========================================
        private void ConfigurarAbaAbertas()
        {
            Panel panelForm = new Panel() { Dock = DockStyle.Top, Height = 120, BackColor = Color.White };
            tabAbertas.Controls.Add(panelForm);

            Label lblAdd = new Label() { Text = "Adicionar Peça (Cód. Fab.):", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtCodFabricante = new TextBox() { Location = new Point(20, 40), Size = new Size(200, 25), CharacterCasing = CharacterCasing.Upper };
            txtCodFabricante.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; AdicionarItem(); } };

            Label lblQtd = new Label() { Text = "Qtd:", Location = new Point(230, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtQtd = new TextBox() { Location = new Point(230, 40), Size = new Size(60, 25), Text = "1" };

            Button btnAdd = new Button() { Text = "ADICIONAR", Location = new Point(310, 38), Size = new Size(100, 28), BackColor = Color.FromArgb(30, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnAdd.Click += (s, e) => AdicionarItem();

            btnPesquisarPeca = new Button() { Text = "🔍 Pesquisar Peça", Location = new Point(420, 38), Size = new Size(150, 28), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnPesquisarPeca.Click += (s, e) => AbrirPesquisa();

            btnRemover = new Button() { Text = "🗑️ REMOVER", Location = new Point(580, 38), Size = new Size(110, 28), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnRemover.Click += (s, e) => RemoverItem();

            btnGerarPdf = new Button() { Text = "📄 Gerar PDF", Location = new Point(20, 75), Size = new Size(130, 30), BackColor = Color.DarkOrange, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnGerarPdf.Click += (s, e) => GerarPdfSolicitacao();

            btnExcel = new Button() { Text = "📊 Salvar Excel", Location = new Point(160, 75), Size = new Size(130, 30), BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnExcel.Click += (s, e) => GerarExcel();

            btnRegistrarPrecos = new Button() { Text = "💰 Registrar Preços e Finalizar", Location = new Point(300, 75), Size = new Size(200, 30), BackColor = Color.MediumBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnRegistrarPrecos.Click += (s, e) => RegistrarPrecos();

            btnCancelarCotacao = new Button() { Text = "🚫 CANCELAR COTAÇÃO", Location = new Point(750, 75), Size = new Size(170, 30), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnCancelarCotacao.Click += (s, e) => CancelarCotacao();

            panelForm.Controls.Add(lblAdd); panelForm.Controls.Add(txtCodFabricante);
            panelForm.Controls.Add(lblQtd); panelForm.Controls.Add(txtQtd);
            panelForm.Controls.Add(btnAdd); panelForm.Controls.Add(btnPesquisarPeca); panelForm.Controls.Add(btnRemover);
            panelForm.Controls.Add(btnGerarPdf); panelForm.Controls.Add(btnExcel); panelForm.Controls.Add(btnRegistrarPrecos); panelForm.Controls.Add(btnCancelarCotacao);

            gridItens = new DataGridView() { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, Font = new Font("Segoe UI", 11) };
            gridItens.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridItens.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridItens.EnableHeadersVisualStyles = false;
            gridItens.CellBeginEdit += GridItens_CellBeginEdit;
            gridItens.CellEndEdit += GridItens_CellEndEdit;
            tabAbertas.Controls.Add(gridItens);
            gridItens.BringToFront();

            dtItens = new DataTable();
            dtItens.Columns.Add("ProdutoId", typeof(int));
            dtItens.Columns.Add("ItemCotacaoId", typeof(int));
            dtItens.Columns.Add("Cód. Fab.", typeof(string));
            dtItens.Columns.Add("Descrição", typeof(string));
            dtItens.Columns.Add("Marca", typeof(string));
            dtItens.Columns.Add("Qtd", typeof(int));
            dtItens.Columns.Add("Estoque", typeof(int));

            gridItens.DataSource = dtItens;
        }

        // ==========================================
        // CONFIGURAÇÃO DA ABA FINALIZADA
        // ==========================================
        private void ConfigurarAbaFinalizadas()
        {
            gridFinalizadas = new DataGridView() { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, Font = new Font("Segoe UI", 11) };
            gridFinalizadas.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridFinalizadas.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridFinalizadas.EnableHeadersVisualStyles = false;
            tabFinalizadas.Controls.Add(gridFinalizadas);
        }

        // ==========================================
        // LÓGICA DA ABA ABERTA
        // ==========================================
        private void CarregarCotacaoAberta()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    string query = "SELECT TOP 1 Id, NumeroCotacao FROM Cotacoes WHERE LojaId = @loja AND Status = 'ABERTA' ORDER BY Id DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@loja", SessaoSistema.LojaIdAtiva);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idCotacaoAtiva = Convert.ToInt32(reader["Id"]);
                                numeroCotacaoAtiva = reader["NumeroCotacao"].ToString();
                            }
                        }
                    }

                    if (idCotacaoAtiva == 0)
                    {
                        numeroCotacaoAtiva = "COT" + DateTime.Now.ToString("ddMMyyHHmmss");
                        string sqlInsert = "INSERT INTO Cotacoes (NumeroCotacao, LojaId, UsuarioId, DataCriacao, Status) OUTPUT INSERTED.Id VALUES (@num, @loja, @usu, GETDATE(), 'ABERTA')";
                        using (SqlCommand cmd = new SqlCommand(sqlInsert, conn))
                        {
                            cmd.Parameters.AddWithValue("@num", numeroCotacaoAtiva);
                            cmd.Parameters.AddWithValue("@loja", SessaoSistema.LojaIdAtiva);
                            cmd.Parameters.AddWithValue("@usu", SessaoSistema.UsuarioId);
                            idCotacaoAtiva = (int)cmd.ExecuteScalar();
                        }
                    }
                }
                CarregarItensDaCotacao();
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar cotação: " + ex.Message); }
        }

        private void CarregarItensDaCotacao()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    string query = @"
                        SELECT ci.Id AS ItemCotacaoId, p.Id AS ProdutoId, p.CodigoFabricante, p.Descricao, p.Marca, 
                               ci.QuantidadeSolicitada, ISNULL(e.Quantidade, 0) AS Estoque
                        FROM Cotacoes_Itens ci
                        INNER JOIN Produtos p ON ci.ProdutoId = p.Id
                        LEFT JOIN Estoques e ON p.Id = e.ProdutoId AND e.LojaId = @loja
                        WHERE ci.CotacaoId = @cotId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@loja", SessaoSistema.LojaIdAtiva);
                        cmd.Parameters.AddWithValue("@cotId", idCotacaoAtiva);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dtItens.Rows.Clear();
                        foreach (DataRow row in dt.Rows)
                        {
                            dtItens.Rows.Add(
                                row["ProdutoId"], row["ItemCotacaoId"], row["CodigoFabricante"],
                                row["Descricao"], row["Marca"], row["QuantidadeSolicitada"], row["Estoque"]
                            );
                        }
                    }
                }

                if (gridItens.Columns["ProdutoId"] != null) gridItens.Columns["ProdutoId"].Visible = false;
                if (gridItens.Columns["ItemCotacaoId"] != null) gridItens.Columns["ItemCotacaoId"].Visible = false;
                if (gridItens.Columns["Cód. Fab."] != null) gridItens.Columns["Cód. Fab."].ReadOnly = true;
                if (gridItens.Columns["Descrição"] != null) gridItens.Columns["Descrição"].ReadOnly = true;
                if (gridItens.Columns["Marca"] != null) gridItens.Columns["Marca"].ReadOnly = true;
                if (gridItens.Columns["Estoque"] != null) gridItens.Columns["Estoque"].ReadOnly = true;
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar itens: " + ex.Message); }
        }

        private void AdicionarItem()
        {
            string termo = txtCodFabricante.Text.Trim();
            if (string.IsNullOrEmpty(termo)) return;
            if (!int.TryParse(txtQtd.Text, out int qtd) || qtd <= 0) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    string queryProd = $@"SELECT p.Id, p.CodigoFabricante, p.Descricao, p.Marca, ISNULL(e.Quantidade, 0) AS Estoque 
                                      FROM Produtos p LEFT JOIN Estoques e ON p.Id = e.ProdutoId AND e.LojaId = {SessaoSistema.LojaIdAtiva}
                                      WHERE p.CodigoFabricante LIKE '%{termo}%' OR p.Descricao LIKE '%{termo}%'";

                    int prodId = 0, estoque = 0;
                    string codFab = "", desc = "", marca = "";

                    using (SqlCommand cmd = new SqlCommand(queryProd, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            prodId = Convert.ToInt32(reader["Id"]);
                            codFab = reader["CodigoFabricante"].ToString();
                            desc = reader["Descricao"].ToString();
                            marca = reader["Marca"].ToString();
                            estoque = Convert.ToInt32(reader["Estoque"]);
                        }
                        else { MessageBox.Show("Produto não encontrado.", "Aviso"); return; }
                    }

                    foreach (DataRow row in dtItens.Rows)
                    {
                        if (Convert.ToInt32(row["ProdutoId"]) == prodId)
                        {
                            int itemCotacaoId = Convert.ToInt32(row["ItemCotacaoId"]);
                            int novaQtd = Convert.ToInt32(row["Qtd"]) + qtd;

                            string sqlUpdate = "UPDATE Cotacoes_Itens SET QuantidadeSolicitada = @qtd WHERE Id = @id";
                            using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn))
                            {
                                cmd.Parameters.AddWithValue("@qtd", novaQtd);
                                cmd.Parameters.AddWithValue("@id", itemCotacaoId);
                                cmd.ExecuteNonQuery();
                            }

                            row["Qtd"] = novaQtd;
                            txtCodFabricante.Clear(); txtQtd.Text = "1"; txtCodFabricante.Focus();
                            return;
                        }
                    }

                    string sqlInsertItem = "INSERT INTO Cotacoes_Itens (CotacaoId, ProdutoId, QuantidadeSolicitada) OUTPUT INSERTED.Id VALUES (@cotId, @prodId, @qtd)";
                    int novoItemId = 0;
                    using (SqlCommand cmd = new SqlCommand(sqlInsertItem, conn))
                    {
                        cmd.Parameters.AddWithValue("@cotId", idCotacaoAtiva);
                        cmd.Parameters.AddWithValue("@prodId", prodId);
                        cmd.Parameters.AddWithValue("@qtd", qtd);
                        novoItemId = (int)cmd.ExecuteScalar();
                    }

                    dtItens.Rows.Add(prodId, novoItemId, codFab, desc, marca, qtd, estoque);
                    txtCodFabricante.Clear(); txtQtd.Text = "1"; txtCodFabricante.Focus();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void RemoverItem()
        {
            if (gridItens.CurrentRow != null)
            {
                int itemCotacaoId = Convert.ToInt32(gridItens.CurrentRow.Cells["ItemCotacaoId"].Value);
                try
                {
                    using (SqlConnection conn = new SqlConnection(conexaoString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM Cotacoes_Itens WHERE Id = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", itemCotacaoId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    gridItens.Rows.RemoveAt(gridItens.CurrentRow.Index);
                }
                catch (Exception ex) { MessageBox.Show("Erro ao remover: " + ex.Message); }
            }
        }

        private void GridItens_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (gridItens.Columns[e.ColumnIndex].Name != "Qtd") e.Cancel = true;
        }

        private void GridItens_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (gridItens.Columns[e.ColumnIndex].Name == "Qtd")
            {
                DataGridViewRow row = gridItens.Rows[e.RowIndex];
                if (!int.TryParse(row.Cells["Qtd"].Value?.ToString(), out int novaQtd) || novaQtd <= 0)
                {
                    MessageBox.Show("Quantidade inválida. Voltando para 1.", "Aviso");
                    novaQtd = 1;
                    row.Cells["Qtd"].Value = 1;
                }

                int itemCotacaoId = Convert.ToInt32(row.Cells["ItemCotacaoId"].Value);
                try
                {
                    using (SqlConnection conn = new SqlConnection(conexaoString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE Cotacoes_Itens SET QuantidadeSolicitada = @qtd WHERE Id = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@qtd", novaQtd);
                            cmd.Parameters.AddWithValue("@id", itemCotacaoId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Erro ao salvar quantidade: " + ex.Message); }
            }
        }

        private void CancelarCotacao()
        {
            if (MessageBox.Show("Deseja realmente cancelar esta cotação?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(conexaoString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand("UPDATE Cotacoes SET Status = 'CANCELADA' WHERE Id = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", idCotacaoAtiva);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Cotação cancelada. Abrindo uma nova...", "Sucesso");
                    this.Close();
                }
                catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
            }
        }

        // ==========================================
        // LÓGICA DA ABA FINALIZADA
        // ==========================================
        private void CarregarCotacoesFinalizadas()
        {
            try
            {
                string query = @"
                    SELECT c.Id, c.NumeroCotacao AS [Número], CONVERT(VARCHAR, c.DataCriacao, 103) AS [Data], 
                           l.Nome AS [Loja], u.Nome AS [Usuário], c.Status
                    FROM Cotacoes c
                    INNER JOIN Lojas l ON c.LojaId = l.Id
                    INNER JOIN Usuarios u ON c.UsuarioId = u.Id
                    WHERE c.Status != 'ABERTA' AND c.LojaId = @loja
                    ORDER BY c.Id DESC";

                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@loja", SessaoSistema.LojaIdAtiva);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gridFinalizadas.DataSource = dt;
                    if (gridFinalizadas.Columns["Id"] != null) gridFinalizadas.Columns["Id"].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao carregar finalizadas: " + ex.Message); }
        }

        // ==========================================
        // AÇÕES (PDF, EXCEL, REGISTRAR PREÇOS)
        // ==========================================
        private void AbrirPesquisa()
        {
            FrmPesquisaCotacao pesquisa = new FrmPesquisaCotacao(txtCodFabricante.Text.Trim());
            if (pesquisa.ShowDialog() == DialogResult.OK)
            {
                txtCodFabricante.Text = pesquisa.CodFabricanteSelecionado;
                txtQtd.Text = pesquisa.QuantidadeSelecionada.ToString();
                AdicionarItem();
            }
        }

        private void GerarPdfSolicitacao()
        {
            if (dtItens.Rows.Count == 0) { MessageBox.Show("Adicione peças antes de gerar o PDF.", "Aviso"); return; }

            SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF (*.pdf)|*.pdf", FileName = $"Cotacao_{numeroCotacaoAtiva}.pdf" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 30, 30, 30, 30);
                    iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(sfd.FileName, FileMode.Create));
                    doc.Open();

                    iTextSharp.text.Paragraph titulo = new iTextSharp.text.Paragraph("TOCANTINS MOTORES - SOLICITAÇÃO DE COTAÇÃO", iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16));
                    titulo.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                    doc.Add(titulo);
                    doc.Add(new iTextSharp.text.Paragraph($"Cotação Nº: {numeroCotacaoAtiva} | Data: {DateTime.Now:dd/MM/yyyy} | Loja: {SessaoSistema.NomeLojaAtiva}"));
                    doc.Add(new iTextSharp.text.Paragraph("--------------------------------------------------------------------------------------"));

                    iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(5);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 15f, 40f, 10f, 15f, 20f });
                    table.AddCell("Cód. Fab."); table.AddCell("Descrição"); table.AddCell("Qtd"); table.AddCell("Preço Unit."); table.AddCell("Marca");

                    foreach (DataRow row in dtItens.Rows)
                    {
                        table.AddCell(row["Cód. Fab."].ToString());
                        table.AddCell(row["Descrição"].ToString());
                        table.AddCell(row["Qtd"].ToString());
                        table.AddCell(" ");
                        table.AddCell(" ");
                    }
                    doc.Add(table);
                    doc.Close();
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex) { MessageBox.Show("Erro ao gerar PDF: " + ex.Message); }
            }
        }

        private void GerarExcel()
        {
            if (dtItens.Rows.Count == 0) { MessageBox.Show("Adicione peças antes de gerar o Excel.", "Aviso"); return; }

            SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel (*.xlsx)|*.xlsx", FileName = $"Cotacao_{numeroCotacaoAtiva}.xlsx" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Cotação");

                        worksheet.Cell(1, 1).Value = "CÓD. FAB.";
                        worksheet.Cell(1, 2).Value = "DESCRIÇÃO";
                        worksheet.Cell(1, 3).Value = "QTD";
                        worksheet.Cell(1, 4).Value = "PREÇO UNIT. (PREENCHER)";
                        worksheet.Cell(1, 5).Value = "MARCA (PREENCHER)";

                        var headerRange = worksheet.Range("A1:E1");
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3C78");
                        headerRange.Style.Font.FontColor = XLColor.White;
                        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        int linha = 2;
                        foreach (DataRow row in dtItens.Rows)
                        {
                            worksheet.Cell(linha, 1).Value = row["Cód. Fab."].ToString();
                            worksheet.Cell(linha, 2).Value = row["Descrição"].ToString();
                            worksheet.Cell(linha, 3).Value = Convert.ToInt32(row["Qtd"]);
                            worksheet.Cell(linha, 4).Value = "";
                            worksheet.Cell(linha, 5).Value = "";

                            worksheet.Cell(linha, 4).Style.Fill.BackgroundColor = XLColor.LightYellow;
                            worksheet.Cell(linha, 5).Style.Fill.BackgroundColor = XLColor.LightYellow;

                            linha++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("Excel gerado com sucesso!", "Sucesso");
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch (Exception ex) { MessageBox.Show("Erro ao gerar Excel: " + ex.Message); }
            }
        }

        private void RegistrarPrecos()
        {
            if (dtItens.Rows.Count == 0) { MessageBox.Show("Adicione peças na lista primeiro.", "Aviso"); return; }

            FrmRegistrarPrecos telaPrecos = new FrmRegistrarPrecos(dtItens, idCotacaoAtiva, numeroCotacaoAtiva);
            if (telaPrecos.ShowDialog() == DialogResult.OK)
            {
                this.Close(); // Se finalizou, fecha e abre de novo para gerar uma nova cotação em aberto
            }
        }
    }

    // =========================================================
    // JANELA DE PESQUISA (Com Qtd)
    // =========================================================
    public class FrmPesquisaCotacao : Form
    {
        private TextBox txtBusca;
        private DataGridView grid;
        private TextBox txtQtdDesejada;
        public string CodFabricanteSelecionado { get; set; }
        public int QuantidadeSelecionada { get; set; }
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmPesquisaCotacao(string buscaInicial)
        {
            this.Text = "Pesquisar Peça para Cotação";
            this.Size = new Size(850, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label lbl = new Label() { Text = "Pesquisar:", Location = new Point(20, 20), AutoSize = true };
            txtBusca = new TextBox() { Location = new Point(20, 40), Size = new Size(400, 25), Text = buscaInicial, CharacterCasing = CharacterCasing.Upper };
            txtBusca.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Carregar(); } };

            Button btnBuscar = new Button() { Text = "🔍 BUSCAR", Location = new Point(430, 38), Size = new Size(100, 28), BackColor = Color.FromArgb(30, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnBuscar.Click += (s, e) => Carregar();

            grid = new DataGridView() { Location = new Point(20, 80), Size = new Size(790, 350), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.EnableHeadersVisualStyles = false;
            grid.CellDoubleClick += (s, e) => Selecionar();

            Label lblQtd = new Label() { Text = "Qtd a Cotar:", Location = new Point(20, 450), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtQtdDesejada = new TextBox() { Location = new Point(100, 447), Size = new Size(80, 28), Text = "1", Font = new Font("Segoe UI", 11) };

            Button btnAdd = new Button() { Text = "SELECIONAR", Location = new Point(200, 445), Size = new Size(150, 35), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnAdd.Click += (s, e) => Selecionar();

            Button btnCancelar = new Button() { Text = "CANCELAR", Location = new Point(360, 445), Size = new Size(100, 35), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.Add(lbl); this.Controls.Add(txtBusca); this.Controls.Add(btnBuscar);
            this.Controls.Add(grid); this.Controls.Add(lblQtd); this.Controls.Add(txtQtdDesejada);
            this.Controls.Add(btnAdd); this.Controls.Add(btnCancelar);

            Carregar();
        }

        private void Carregar()
        {
            string termo = txtBusca.Text.Trim();
            string filtro = string.IsNullOrEmpty(termo) ? "" : $"AND (p.Descricao LIKE '%{termo}%' OR p.CodigoFabricante LIKE '%{termo}%')";

            string query = $@"SELECT p.Id, p.CodigoFabricante AS [Cód. Fab.], p.Descricao AS [Descrição], p.Marca, ISNULL(e.Quantidade, 0) AS [Estoque]
                              FROM Produtos p LEFT JOIN Estoques e ON p.Id = e.ProdutoId AND e.LojaId = {SessaoSistema.LojaIdAtiva}
                              WHERE 1=1 {filtro} ORDER BY p.Descricao";
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
                if (!int.TryParse(txtQtdDesejada.Text, out int qtd) || qtd <= 0)
                {
                    MessageBox.Show("Digite uma quantidade válida.", "Aviso");
                    return;
                }

                CodFabricanteSelecionado = grid.CurrentRow.Cells["Cód. Fab."].Value.ToString();
                QuantidadeSelecionada = qtd;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }

    // =========================================================
    // JANELA DE REGISTRAR PREÇOS (COM SUGESTÃO INTELIGENTE)
    // =========================================================
    public class FrmRegistrarPrecos : Form
    {
        private DataGridView gridPrecos;
        private DataTable dtPrecos;
        private Button btnSalvar, btnCancelar;
        private int idCotacao;
        private string numCotacao;
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmRegistrarPrecos(DataTable itensCotacao, int idCot, string numCot)
        {
            idCotacao = idCot;
            numCotacao = numCot;

            this.Text = $"Registrar Preços - Cotação {numCotacao}";
            this.Size = new Size(950, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label lblInfo = new Label() { Text = "Preencha os preços dos fornecedores. O sistema mostra o último preço pago como dica.", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            this.Controls.Add(lblInfo);

            dtPrecos = new DataTable();
            dtPrecos.Columns.Add("ProdutoId", typeof(int));
            dtPrecos.Columns.Add("Cód. Fab.", typeof(string));
            dtPrecos.Columns.Add("Descrição", typeof(string));
            dtPrecos.Columns.Add("Qtd", typeof(int));
            dtPrecos.Columns.Add("Fornecedor", typeof(string));
            dtPrecos.Columns.Add("Preço R$", typeof(decimal));
            dtPrecos.Columns.Add("Sugestão (Último Preço)", typeof(string));

            foreach (DataRow row in itensCotacao.Rows)
            {
                int prodId = Convert.ToInt32(row["ProdutoId"]);

                // BUSCA A SUGESTÃO DE PREÇO NO HISTÓRICO
                string sugestao = BuscarUltimoPreco(prodId);

                dtPrecos.Rows.Add(prodId, row["Cód. Fab."], row["Descrição"], row["Qtd"], "", 0.00m, sugestao);
            }

            gridPrecos = new DataGridView() { Location = new Point(20, 50), Size = new Size(890, 450), DataSource = dtPrecos, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false };
            this.Controls.Add(gridPrecos);

            if (gridPrecos.Columns["ProdutoId"] != null) gridPrecos.Columns["ProdutoId"].Visible = false;
            if (gridPrecos.Columns["Cód. Fab."] != null) gridPrecos.Columns["Cód. Fab."].ReadOnly = true;
            if (gridPrecos.Columns["Descrição"] != null) gridPrecos.Columns["Descrição"].ReadOnly = true;
            if (gridPrecos.Columns["Qtd"] != null) gridPrecos.Columns["Qtd"].ReadOnly = true;
            if (gridPrecos.Columns["Sugestão (Último Preço)"] != null)
            {
                gridPrecos.Columns["Sugestão (Último Preço)"].ReadOnly = true;
                gridPrecos.Columns["Sugestão (Último Preço)"].DefaultCellStyle.ForeColor = Color.DarkGreen;
                gridPrecos.Columns["Sugestão (Último Preço)"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            }

            btnSalvar = new Button() { Text = "SALVAR E FINALIZAR COTAÇÃO", Location = new Point(20, 510), Size = new Size(250, 40), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnSalvar.Click += (s, e) => SalvarCotacao();
            this.Controls.Add(btnSalvar);

            btnCancelar = new Button() { Text = "VOLTAR", Location = new Point(280, 510), Size = new Size(100, 40), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancelar);
        }

        private string BuscarUltimoPreco(int produtoId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    // Puxa o preço mais recente salvo para esse produto
                    string query = @"
                        SELECT TOP 1 cp.PrecoUnitario, cp.FornecedorNome, cp.MarcaOferecida, CONVERT(VARCHAR, c.DataCriacao, 103) AS Data
                        FROM Cotacoes_Precos cp
                        INNER JOIN Cotacoes_Itens ci ON cp.CotacaoItemId = ci.Id
                        INNER JOIN Cotacoes c ON ci.CotacaoId = c.Id
                        WHERE ci.ProdutoId = @prodId AND cp.FornecedorNome IS NOT NULL AND c.Status = 'FINALIZADA'
                        ORDER BY c.DataCriacao DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@prodId", produtoId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string fornecedor = reader["FornecedorNome"].ToString();
                                string preco = Convert.ToDecimal(reader["PrecoUnitario"]).ToString("C2");
                                string data = reader["Data"].ToString();
                                return $"{fornecedor}: {preco} (em {data})";
                            }
                        }
                    }
                }
            }
            catch { }
            return "Sem histórico";
        }

        private void SalvarCotacao()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();

                    // Atualiza o status da cotação para FINALIZADA
                    using (SqlCommand cmd = new SqlCommand("UPDATE Cotacoes SET Status = 'FINALIZADA' WHERE Id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idCotacao);
                        cmd.ExecuteNonQuery();
                    }

                    // Busca os itens da cotação para salvar os preços
                    string queryItens = "SELECT Id, ProdutoId FROM Cotacoes_Itens WHERE CotacaoId = @cotId";
                    DataTable dtItens = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(queryItens, conn))
                    {
                        cmd.Parameters.AddWithValue("@cotId", idCotacao);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dtItens);
                    }

                    foreach (DataRow row in dtPrecos.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted) continue;

                        int prodId = Convert.ToInt32(row["ProdutoId"]);
                        int itemId = 0;

                        // Encontra o ID do item
                        foreach (DataRow itemRow in dtItens.Rows)
                        {
                            if (Convert.ToInt32(itemRow["ProdutoId"]) == prodId)
                            {
                                itemId = Convert.ToInt32(itemRow["Id"]);
                                break;
                            }
                        }

                        if (itemId > 0 && !string.IsNullOrEmpty(row["Fornecedor"].ToString()) && Convert.ToDecimal(row["Preço R$"]) > 0)
                        {
                            string sqlPreco = @"INSERT INTO Cotacoes_Precos (CotacaoItemId, FornecedorId, FornecedorNome, PrecoUnitario, MarcaOferecida, Vencedor) 
                                                VALUES (@itemId, 1, @fornecedor, @preco, '', 1)";
                            using (SqlCommand cmd = new SqlCommand(sqlPreco, conn))
                            {
                                cmd.Parameters.AddWithValue("@itemId", itemId);
                                cmd.Parameters.AddWithValue("@fornecedor", row["Fornecedor"].ToString());
                                cmd.Parameters.AddWithValue("@preco", row["Preço R$"]);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    SessaoSistema.RegistrarLog("COTACAO", $"Finalizou a cotação {numCotacao}.");
                    MessageBox.Show("Cotação finalizada e salva com sucesso!", "Sucesso");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro ao salvar: " + ex.Message); }
        }
    }
}