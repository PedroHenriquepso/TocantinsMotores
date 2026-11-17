using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TocantinsMotores
{
    public class FrmReceberTransferencia : Form
    {
        private Button btnVoltar;
        private TextBox txtNumeroTrf;
        private Button btnBuscar;
        private DataGridView gridItens;
        private Button btnConfirmar;
        private Label lblInfo;

        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";
        private int movimentacaoIdEncontrada = 0;

        public FrmReceberTransferencia()
        {
            this.Text = "Receber Transferência";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            Panel panelTopo = new Panel() { Dock = DockStyle.Top, Height = 70, BackColor = Color.White };
            this.Controls.Add(panelTopo);

            btnVoltar = new Button() { Text = "← VOLTAR (ESC)", Location = new Point(20, 15), Size = new Size(130, 40), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnVoltar.Click += (s, e) => this.Close();
            panelTopo.Controls.Add(btnVoltar);

            Label lblTitulo = new Label() { Text = "RECEBER TRANSFERÊNCIA", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(30, 60, 120), AutoSize = true, Location = new Point(170, 20) };
            panelTopo.Controls.Add(lblTitulo);

            Panel panelBusca = new Panel() { Dock = DockStyle.Top, Height = 100, BackColor = Color.White };
            this.Controls.Add(panelBusca);

            Label lbl = new Label() { Text = "Digite o Número da Transferência (Ex: TRF01):", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtNumeroTrf = new TextBox() { Location = new Point(20, 45), Size = new Size(200, 28), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };
            txtNumeroTrf.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; Buscar(); } };

            btnBuscar = new Button() { Text = "🔍 BUSCAR", Location = new Point(230, 45), Size = new Size(100, 28), BackColor = Color.FromArgb(30, 60, 120), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnBuscar.Click += (s, e) => Buscar();

            lblInfo = new Label() { Text = "", Location = new Point(350, 45), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.DarkGreen };

            panelBusca.Controls.Add(lbl); panelBusca.Controls.Add(txtNumeroTrf); panelBusca.Controls.Add(btnBuscar); panelBusca.Controls.Add(lblInfo);

            gridItens = new DataGridView() { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, Font = new Font("Segoe UI", 11) };
            gridItens.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridItens.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridItens.EnableHeadersVisualStyles = false;
            this.Controls.Add(gridItens);
            gridItens.BringToFront();

            Panel panelBaixo = new Panel() { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.White };
            this.Controls.Add(panelBaixo);

            btnConfirmar = new Button() { Text = "CONFIRMAR RECEBIMENTO", Location = new Point(20, 15), Size = new Size(220, 40), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold), Enabled = false };
            btnConfirmar.Click += (s, e) => ConfirmarRecebimento();
            panelBaixo.Controls.Add(btnConfirmar);
        }

        private void Buscar()
        {
            string trf = txtNumeroTrf.Text.Trim();
            if (string.IsNullOrEmpty(trf)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();

                    // Verifica se a TRF existe, está EM_TRANSITO e é destinada para a loja ativa
                    string sqlMov = "SELECT Id FROM Movimentacoes WHERE NumeroDocumento = @num AND Tipo = 'TRANSFERENCIA' AND Status = 'CONCLUIDO' AND LojaDestinoId = @lojaId";

                    using (SqlCommand cmd = new SqlCommand(sqlMov, conn))
                    {
                        cmd.Parameters.AddWithValue("@num", trf);
                        cmd.Parameters.AddWithValue("@lojaId", SessaoSistema.LojaIdAtiva);
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            movimentacaoIdEncontrada = Convert.ToInt32(result);
                            lblInfo.Text = "Transferência encontrada! Confira os itens abaixo.";
                            btnConfirmar.Enabled = true;

                            string sqlItens = @"SELECT p.CodigoFabricante AS [Cód. Fab.], p.Descricao AS [Descrição], mi.Quantidade AS [Qtd Enviada]
                                                FROM Movimentacoes_Itens mi 
                                                INNER JOIN Produtos p ON mi.ProdutoId = p.Id 
                                                WHERE mi.MovimentacaoId = @movId";

                            using (SqlCommand cmdItens = new SqlCommand(sqlItens, conn))
                            {
                                cmdItens.Parameters.AddWithValue("@movId", movimentacaoIdEncontrada);
                                SqlDataAdapter da = new SqlDataAdapter(cmdItens);
                                DataTable dt = new DataTable();
                                da.Fill(dt);
                                gridItens.DataSource = dt;
                            }
                        }
                        else
                        {
                            lblInfo.Text = "TRF não encontrada ou já recebida.";
                            lblInfo.ForeColor = Color.Red;
                            btnConfirmar.Enabled = false;
                            gridItens.DataSource = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);
            }
        }

        private void ConfirmarRecebimento()
        {
            if (movimentacaoIdEncontrada == 0) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();

                    // Pega os itens para dar entrada no estoque
                    string sqlItens = "SELECT ProdutoId, Quantidade FROM Movimentacoes_Itens WHERE MovimentacaoId = @movId";
                    DataTable dtItens = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(sqlItens, conn))
                    {
                        cmd.Parameters.AddWithValue("@movId", movimentacaoIdEncontrada);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dtItens);
                    }

                    // Atualiza o estoque da loja de destino
                    foreach (DataRow row in dtItens.Rows)
                    {
                        int prodId = Convert.ToInt32(row["ProdutoId"]);
                        int qtd = Convert.ToInt32(row["Quantidade"]);

                        string sqlUpdate = @"
                            IF EXISTS (SELECT 1 FROM Estoques WHERE ProdutoId = @prodId AND LojaId = @lojaId)
                                UPDATE Estoques SET Quantidade = Quantidade + @qtd WHERE ProdutoId = @prodId AND LojaId = @lojaId
                            ELSE
                                INSERT INTO Estoques (ProdutoId, LojaId, Quantidade) VALUES (@prodId, @lojaId, @qtd)";

                        using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn))
                        {
                            cmd.Parameters.AddWithValue("@prodId", prodId);
                            cmd.Parameters.AddWithValue("@lojaId", SessaoSistema.LojaIdAtiva);
                            cmd.Parameters.AddWithValue("@qtd", qtd);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Muda o status para RECEBIDO (ou seja, a TRF terminou)
                    string sqlStatus = "UPDATE Movimentacoes SET Status = 'CONCLUIDO' WHERE Id = @movId";
                    using (SqlCommand cmd = new SqlCommand(sqlStatus, conn))
                    {
                        cmd.Parameters.AddWithValue("@movId", movimentacaoIdEncontrada);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Transferência recebida com sucesso! Estoque atualizado.", "Sucesso");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao confirmar: " + ex.Message);
            }
        }
    }
}