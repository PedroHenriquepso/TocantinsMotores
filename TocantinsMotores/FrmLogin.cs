using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using BCrypt.Net;

namespace TocantinsMotores
{
    public class FrmLogin : Form
    {
        private TextBox txtUsuario;
        private TextBox txtSenha;
        private Button btnEntrar;
        private Label lblTitulo, lblSubtitulo, lblUsuario, lblSenha, lblErro;
        private Panel painelLogin;

        string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public FrmLogin()
        {
            ConfigurarJanela();
            CriarPainelCentral();
            this.Resize += (s, e) => CentralizarPainel();
        }

        private void ConfigurarJanela()
        {
            this.Text = "Login - Tocantins Motores";
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            this.Paint += (s, e) =>
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                    Color.FromArgb(20, 30, 50), Color.FromArgb(10, 15, 25), 90F))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            };

            this.KeyPreview = true;
        }

        private void CriarPainelCentral()
        {
            painelLogin = new Panel();
            painelLogin.Size = new Size(400, 480);
            painelLogin.BackColor = Color.White;
            painelLogin.Paint += (s, e) => { ControlPaint.DrawBorder(e.Graphics, painelLogin.ClientRectangle, Color.Gray, ButtonBorderStyle.Solid); };
            this.Controls.Add(painelLogin);

            lblTitulo = new Label() { Text = "TM ESTOQUE", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.FromArgb(30, 60, 120), AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Size = new Size(400, 50), Location = new Point(0, 30) };
            painelLogin.Controls.Add(lblTitulo);

            lblSubtitulo = new Label() { Text = "Tocantins Motores - Gestão de Estoque", Font = new Font("Segoe UI", 10, FontStyle.Regular), ForeColor = Color.Gray, AutoSize = false, TextAlign = ContentAlignment.MiddleCenter, Size = new Size(400, 30), Location = new Point(0, 75) };
            painelLogin.Controls.Add(lblSubtitulo);

            lblUsuario = new Label() { Text = "Usuário", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(50, 140), AutoSize = true };
            painelLogin.Controls.Add(lblUsuario);
            txtUsuario = new TextBox() { Location = new Point(50, 165), Size = new Size(300, 28), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };
            painelLogin.Controls.Add(txtUsuario);

            lblSenha = new Label() { Text = "Senha", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(50, 215), AutoSize = true };
            painelLogin.Controls.Add(lblSenha);
            txtSenha = new TextBox() { Location = new Point(50, 240), Size = new Size(300, 28), Font = new Font("Segoe UI", 12), PasswordChar = '●' };
            painelLogin.Controls.Add(txtSenha);

            txtUsuario.KeyDown += Txt_KeyDown;
            txtSenha.KeyDown += Txt_KeyDown;

            lblErro = new Label() { Text = "", ForeColor = Color.Red, Font = new Font("Segoe UI", 9), AutoSize = true, Location = new Point(50, 280) };
            painelLogin.Controls.Add(lblErro);

            btnEntrar = new Button() { Text = "ENTRAR NO SISTEMA", Location = new Point(50, 320), Size = new Size(300, 45), BackColor = Color.FromArgb(30, 60, 120), ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.MouseEnter += (s, e) => btnEntrar.BackColor = Color.FromArgb(20, 45, 100);
            btnEntrar.MouseLeave += (s, e) => btnEntrar.BackColor = Color.FromArgb(30, 60, 120);
            btnEntrar.Click += new EventHandler(btnEntrar_Click);
            painelLogin.Controls.Add(btnEntrar);

            Button btnFecharSistema = new Button() { Text = "X FECHAR SISTEMA", Location = new Point(150, 400), Size = new Size(120, 30), BackColor = Color.Transparent, ForeColor = Color.Gray, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9), Cursor = Cursors.Hand };
            btnFecharSistema.FlatAppearance.BorderSize = 0;
            btnFecharSistema.Click += (s, e) => Application.Exit();
            painelLogin.Controls.Add(btnFecharSistema);
        }

        private void CentralizarPainel()
        {
            if (painelLogin != null)
            {
                int x = (this.ClientSize.Width - painelLogin.Width) / 2;
                int y = (this.ClientSize.Height - painelLogin.Height) / 2;
                painelLogin.Location = new Point(x, y);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            CentralizarPainel();
        }

        private void Txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnEntrar.PerformClick();
            }
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            string login = txtUsuario.Text.Trim();
            string senha = txtSenha.Text.Trim();
            lblErro.Text = "";

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(senha))
            {
                lblErro.Text = "Preencha todos os campos.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    string query = "SELECT Id, Nome, SenhaHash FROM Usuarios WHERE Login = @login AND Ativo = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int idUsuario = Convert.ToInt32(reader["Id"]);
                                string nomeUsuario = reader["Nome"].ToString();
                                string senhaHashBanco = reader["SenhaHash"].ToString();

                                if (BCrypt.Net.BCrypt.Verify(senha, senhaHashBanco))
                                {
                                    SessaoSistema.UsuarioId = idUsuario;
                                    SessaoSistema.NomeUsuario = nomeUsuario;
                                    SessaoSistema.LojaIdAtiva = 1;
                                    SessaoSistema.NomeLojaAtiva = "MATRIZ";

                                    SessaoSistema.Permissoes.Clear();
                                    using (SqlConnection conn2 = new SqlConnection(conexaoString))
                                    {
                                        conn2.Open();
                                        string sqlPerm = @"
                                            SELECT pp.ChavePermissao 
                                            FROM Permissoes_Perfil pp
                                            INNER JOIN Usuarios u ON u.PerfilId = pp.PerfilId
                                            WHERE u.Id = @userId";

                                        using (SqlCommand cmdPerm = new SqlCommand(sqlPerm, conn2))
                                        {
                                            cmdPerm.Parameters.AddWithValue("@userId", idUsuario);
                                            using (SqlDataReader readerPerm = cmdPerm.ExecuteReader())
                                            {
                                                while (readerPerm.Read())
                                                {
                                                    SessaoSistema.Permissoes.Add(readerPerm["ChavePermissao"].ToString());
                                                }
                                            }
                                        }
                                    }

                                    // GRAVA O LOG DE ENTRADA
                                    SessaoSistema.RegistrarLog("LOGIN", "Usuário entrou no sistema.");

                                    FrmPrincipal telaPrincipal = new FrmPrincipal();
                                    telaPrincipal.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    lblErro.Text = "Senha incorreta.";
                                }
                            }
                            else
                            {
                                lblErro.Text = "Usuário não encontrado ou inativo.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar ao banco: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}