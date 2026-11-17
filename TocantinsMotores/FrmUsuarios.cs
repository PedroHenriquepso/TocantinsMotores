using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using BCrypt.Net;

namespace TocantinsMotores
{
    public class FrmUsuarios : Form
    {
        private Button btnVoltar, btnNovo, btnPerfis;
        private DataGridView gridUsuarios;

        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmUsuarios()
        {
            ConfigurarJanela();
            CriarTopo();
            CriarGrid();
            CarregarUsuarios();
        }

        private void ConfigurarJanela()
        {
            this.Text = "Gestão de Usuários";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };
        }

        private void CriarTopo()
        {
            Panel panelTopo = new Panel() { Dock = DockStyle.Top, Height = 80, BackColor = Color.White };
            this.Controls.Add(panelTopo);

            btnVoltar = new Button() { Text = "← VOLTAR (ESC)", Location = new Point(20, 20), Size = new Size(130, 40), BackColor = Color.LightGray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnVoltar.Click += (s, e) => this.Close();
            panelTopo.Controls.Add(btnVoltar);

            Label lblTitulo = new Label() { Text = $"USUÁRIOS - LOJA: {SessaoSistema.NomeLojaAtiva}", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.FromArgb(30, 60, 120), AutoSize = true, Location = new Point(170, 25) };
            panelTopo.Controls.Add(lblTitulo);

            btnPerfis = new Button() { Text = "⚙️ GERENCIAR PERFIS", Location = new Point(650, 20), Size = new Size(180, 40), BackColor = Color.DarkGray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnPerfis.Click += (s, e) => { new FrmPerfis().ShowDialog(); };
            panelTopo.Controls.Add(btnPerfis);

            btnNovo = new Button() { Text = "+ NOVO USUÁRIO", Location = new Point(840, 20), Size = new Size(160, 40), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnNovo.Click += (s, e) => AbrirCadastro(0);
            panelTopo.Controls.Add(btnNovo);
        }

        private void CriarGrid()
        {
            gridUsuarios = new DataGridView() { Dock = DockStyle.Fill, BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, RowHeadersVisible = false, Font = new Font("Segoe UI", 11) };
            gridUsuarios.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 60, 120);
            gridUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridUsuarios.EnableHeadersVisualStyles = false;

            gridUsuarios.CellDoubleClick += (s, e) => {
                if (e.RowIndex >= 0)
                {
                    int id = Convert.ToInt32(gridUsuarios.Rows[e.RowIndex].Cells["Id"].Value);
                    AbrirCadastro(id);
                }
            };
            this.Controls.Add(gridUsuarios);
            gridUsuarios.BringToFront();
        }

        private void CarregarUsuarios()
        {
            string query = @"SELECT u.Id, u.Login AS [Login], u.Nome AS [Nome Completo], p.Nome AS [Perfil], 
                             CASE WHEN u.Ativo = 1 THEN 'ATIVO' ELSE 'INATIVO' END AS [Status]
                             FROM Usuarios u 
                             INNER JOIN Perfis p ON u.PerfilId = p.Id
                             ORDER BY u.Nome";
            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridUsuarios.DataSource = dt;
                if (gridUsuarios.Columns["Id"] != null) gridUsuarios.Columns["Id"].Visible = false;
            }
        }

        private void AbrirCadastro(int idUsuario)
        {
            FrmCadastroUsuario tela = new FrmCadastroUsuario(idUsuario);
            tela.ShowDialog();
            CarregarUsuarios();
        }
    }

    public class FrmCadastroUsuario : Form
    {
        private int idUsuarioEdicao = 0;
        private TextBox txtLogin, txtNome, txtSenha;
        private ComboBox cbPerfil, cbStatus;
        private Button btnSalvar, btnCancelar, btnExcluir;
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmCadastroUsuario(int idUsuario = 0)
        {
            idUsuarioEdicao = idUsuario;
            this.Text = idUsuario == 0 ? "Novo Usuário" : "Editar Usuário";
            this.Size = new Size(450, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.White;
            this.KeyPreview = true;
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };

            Label lbl1 = new Label() { Text = "Login (Ex: 86):", Location = new Point(30, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtLogin = new TextBox() { Location = new Point(30, 45), Size = new Size(370, 28), CharacterCasing = CharacterCasing.Upper, Font = new Font("Segoe UI", 12) };

            Label lbl2 = new Label() { Text = "Nome Completo:", Location = new Point(30, 85), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtNome = new TextBox() { Location = new Point(30, 110), Size = new Size(370, 28), CharacterCasing = CharacterCasing.Upper, Font = new Font("Segoe UI", 12) };

            Label lbl3 = new Label() { Text = "Senha (deixe em branco para não alterar):", Location = new Point(30, 150), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtSenha = new TextBox() { Location = new Point(30, 175), Size = new Size(370, 28), PasswordChar = '●', Font = new Font("Segoe UI", 12) };

            Label lbl4 = new Label() { Text = "Perfil de Acesso:", Location = new Point(30, 215), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cbPerfil = new ComboBox() { Location = new Point(30, 240), Size = new Size(180, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };
            CarregarPerfis();

            Label lbl5 = new Label() { Text = "Status:", Location = new Point(230, 215), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cbStatus = new ComboBox() { Location = new Point(230, 240), Size = new Size(170, 28), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 12) };
            cbStatus.Items.Add("ATIVO");
            cbStatus.Items.Add("INATIVO");
            cbStatus.SelectedIndex = 0;

            btnSalvar = new Button() { Text = "SALVAR", Location = new Point(30, 300), Size = new Size(150, 45), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnSalvar.Click += (s, e) => Salvar();

            btnCancelar = new Button() { Text = "CANCELAR", Location = new Point(200, 300), Size = new Size(120, 45), BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();

            btnExcluir = new Button() { Text = "EXCLUIR", Location = new Point(340, 300), Size = new Size(80, 45), BackColor = Color.DarkRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold), Visible = false };
            btnExcluir.Click += (s, e) => Excluir();

            this.Controls.Add(lbl1); this.Controls.Add(txtLogin);
            this.Controls.Add(lbl2); this.Controls.Add(txtNome);
            this.Controls.Add(lbl3); this.Controls.Add(txtSenha);
            this.Controls.Add(lbl4); this.Controls.Add(cbPerfil);
            this.Controls.Add(lbl5); this.Controls.Add(cbStatus);
            this.Controls.Add(btnSalvar); this.Controls.Add(btnCancelar); this.Controls.Add(btnExcluir);

            if (idUsuarioEdicao > 0)
            {
                btnExcluir.Visible = true;
                CarregarDados();
            }
        }

        private void CarregarPerfis()
        {
            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Nome FROM Perfis", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cbPerfil.DataSource = dt;
                cbPerfil.DisplayMember = "Nome";
                cbPerfil.ValueMember = "Id";
            }
        }

        private void CarregarDados()
        {
            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                conn.Open();
                string query = "SELECT Login, Nome, PerfilId, Ativo FROM Usuarios WHERE Id = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idUsuarioEdicao);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtLogin.Text = reader["Login"].ToString();
                            txtNome.Text = reader["Nome"].ToString();
                            cbPerfil.SelectedValue = reader["PerfilId"];
                            cbStatus.SelectedItem = Convert.ToBoolean(reader["Ativo"]) ? "ATIVO" : "INATIVO";
                        }
                    }
                }
            }
        }

        private void Salvar()
        {
            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtNome.Text)) return;
            if (idUsuarioEdicao == 0 && string.IsNullOrEmpty(txtSenha.Text))
            {
                MessageBox.Show("A senha é obrigatória para novos usuários.", "Aviso");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();

                    string checkSql = "SELECT COUNT(*) FROM Usuarios WHERE Login = @login AND Id != @id";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@login", txtLogin.Text.Trim());
                        cmdCheck.Parameters.AddWithValue("@id", idUsuarioEdicao);
                        if ((int)cmdCheck.ExecuteScalar() > 0)
                        {
                            MessageBox.Show("Este Login já existe!", "Aviso");
                            return;
                        }
                    }

                    int ativo = cbStatus.SelectedItem.ToString() == "ATIVO" ? 1 : 0;

                    if (idUsuarioEdicao == 0)
                    {
                        string query = "INSERT INTO Usuarios (Login, Nome, SenhaHash, PerfilId, Ativo) VALUES (@login, @nome, @hash, @perfil, @ativo)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@login", txtLogin.Text.Trim());
                            cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
                            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(txtSenha.Text));
                            cmd.Parameters.AddWithValue("@perfil", cbPerfil.SelectedValue);
                            cmd.Parameters.AddWithValue("@ativo", ativo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txtSenha.Text))
                        {
                            string query = "UPDATE Usuarios SET Login = @login, Nome = @nome, PerfilId = @perfil, Ativo = @ativo WHERE Id = @id";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@login", txtLogin.Text.Trim());
                                cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
                                cmd.Parameters.AddWithValue("@perfil", cbPerfil.SelectedValue);
                                cmd.Parameters.AddWithValue("@ativo", ativo);
                                cmd.Parameters.AddWithValue("@id", idUsuarioEdicao);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            string query = "UPDATE Usuarios SET Login = @login, Nome = @nome, SenhaHash = @hash, PerfilId = @perfil, Ativo = @ativo WHERE Id = @id";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@login", txtLogin.Text.Trim());
                                cmd.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
                                cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(txtSenha.Text));
                                cmd.Parameters.AddWithValue("@perfil", cbPerfil.SelectedValue);
                                cmd.Parameters.AddWithValue("@ativo", ativo);
                                cmd.Parameters.AddWithValue("@id", idUsuarioEdicao);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    MessageBox.Show("Usuário salvo com sucesso!", "Sucesso");
                    this.Close();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erro: " + ex.Message); }
        }

        private void Excluir()
        {
            if (idUsuarioEdicao == SessaoSistema.UsuarioId)
            {
                MessageBox.Show("Você não pode excluir o usuário que está logado no momento.", "Erro");
                return;
            }

            if (MessageBox.Show("Deseja realmente excluir este usuário?", "Atenção", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(conexaoString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Usuarios WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", idUsuarioEdicao);
                            cmd.ExecuteNonQuery();
                        }
                        MessageBox.Show("Usuário excluído!", "Sucesso");
                        this.Close();
                    }
                }
                catch { MessageBox.Show("Não é possível excluir pois ele possui movimentações registradas. Apenas inative-o.", "Erro"); }
            }
        }
    }

    // =========================================================
    // TELA DE GERENCIAR PERFIS (CORRIGIDA)
    // =========================================================
    public class FrmPerfis : Form
    {
        private ComboBox cbPerfis;
        private CheckedListBox chkPermissoes;
        private Button btnSalvar;
        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmPerfis()
        {
            this.Text = "Gerenciar Permissões";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            Label lbl1 = new Label() { Text = "Selecione o Perfil:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cbPerfis = new ComboBox() { Location = new Point(20, 45), Size = new Size(440, 28), DropDownStyle = ComboBoxStyle.DropDownList };
            cbPerfis.SelectedIndexChanged += (s, e) => CarregarPermissoes();

            Label lbl2 = new Label() { Text = "Marque o que este perfil pode fazer:", Location = new Point(20, 85), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            chkPermissoes = new CheckedListBox() { Location = new Point(20, 110), Size = new Size(440, 400), CheckOnClick = true, Font = new Font("Segoe UI", 10) };
            // Adicionando as permissões
            chkPermissoes.Items.Add("PRODUTO_VISUALIZAR|Visualizar Produtos e Estoque");
            chkPermissoes.Items.Add("PRODUTO_EDITAR|Cadastrar e Editar Produtos");
            chkPermissoes.Items.Add("PRODUTO_EXCLUIR|Excluir Produtos");
            chkPermissoes.Items.Add("PRODUTO_ESTOQUE_MANUAL|Fazer Entrada Manual de Estoque");
            chkPermissoes.Items.Add("SAIDA_REGISTRAR|Registrar Saídas (SV)");
            chkPermissoes.Items.Add("TRF_REGISTRAR|Registrar Transferências (TRF)");
            chkPermissoes.Items.Add("TRF_RECEBER|Receber Transferências");
            chkPermissoes.Items.Add("COTACAO_CRIAR|Criar Cotações e Pedidos");
            chkPermissoes.Items.Add("USUARIO_GERENCIAR|Gerenciar Usuários e Perfis");
            chkPermissoes.Items.Add("LOG_VISUALIZAR|Visualizar Logs do Sistema");

            btnSalvar = new Button() { Text = "SALVAR PERMISSÕES", Location = new Point(20, 520), Size = new Size(440, 40), BackColor = Color.MediumSeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
            btnSalvar.Click += (s, e) => Salvar();

            this.Controls.Add(lbl1); this.Controls.Add(cbPerfis);
            this.Controls.Add(lbl2); this.Controls.Add(chkPermissoes);
            this.Controls.Add(btnSalvar);

            CarregarComboPerfis();
        }

        private void CarregarComboPerfis()
        {
            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT Id, Nome FROM Perfis", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cbPerfis.DataSource = dt;
                cbPerfis.DisplayMember = "Nome";
                cbPerfis.ValueMember = "Id";
            }
        }

        private void CarregarPermissoes()
        {
            if (cbPerfis.SelectedValue == null || cbPerfis.SelectedIndex == -1) return;

            // CORREÇÃO AQUI: Pega o valor real convertendo o DataRowView
            int perfilId = 0;
            if (cbPerfis.SelectedValue is DataRowView)
            {
                DataRowView row = (DataRowView)cbPerfis.SelectedValue;
                perfilId = Convert.ToInt32(row["Id"]);
            }
            else
            {
                perfilId = Convert.ToInt32(cbPerfis.SelectedValue);
            }

            // Desmarca tudo primeiro
            for (int i = 0; i < chkPermissoes.Items.Count; i++)
                chkPermissoes.SetItemChecked(i, false);

            // ... (o resto do código do SELECT continua igual) ...

            // Desmarca tudo primeiro
            for (int i = 0; i < chkPermissoes.Items.Count; i++)
                chkPermissoes.SetItemChecked(i, false);

            using (SqlConnection conn = new SqlConnection(conexaoString))
            {
                conn.Open();
                string query = "SELECT ChavePermissao FROM Permissoes_Perfil WHERE PerfilId = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", perfilId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string chave = reader["ChavePermissao"].ToString().Trim();
                            for (int i = 0; i < chkPermissoes.Items.Count; i++)
                            {
                                // Compara a chave antes do pipe "|"
                                if (chkPermissoes.Items[i].ToString().Split('|')[0] == chave)
                                {
                                    chkPermissoes.SetItemChecked(i, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Salvar()
        {
            if (cbPerfis.SelectedValue == null) return;
            int perfilId = Convert.ToInt32(cbPerfis.SelectedValue);

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();

                    // 1. Deleta tudo o que existe para esse perfil
                    string deleteSql = "DELETE FROM Permissoes_Perfil WHERE PerfilId = @id";
                    using (SqlCommand cmd = new SqlCommand(deleteSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", perfilId);
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Insere apenas o que está marcado
                    foreach (var item in chkPermissoes.CheckedItems)
                    {
                        string chave = item.ToString().Split('|')[0];

                        string insertSql = "INSERT INTO Permissoes_Perfil (PerfilId, ChavePermissao) VALUES (@perfil, @chave)";
                        using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@perfil", perfilId);
                            cmd.Parameters.AddWithValue("@chave", chave);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    MessageBox.Show("Permissões salvas com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao salvar permissões: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}