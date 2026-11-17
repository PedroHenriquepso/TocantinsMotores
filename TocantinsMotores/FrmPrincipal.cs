using System;
using System.Drawing;
using System.Windows.Forms;

namespace TocantinsMotores
{
    public class FrmPrincipal : Form
    {
        private MenuStrip menuStrip;
        private Panel panelBotoes;
        private Button btnProdutos, btnSaidas, btnReceber, btnCotacoes, btnSair;
        private Panel panelLateral;
        private GroupBox groupLoja;
        private RadioButton rbMatriz, rbDistribuidora;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblUsuario, lblLojaAtiva;
        private Panel faixaCorLoja;

        public FrmPrincipal()
        {
            ConfigurarJanela();
            CriarMenuSuperior();
            CriarBarraBotoes();
            CriarPainelLateral();
            CriarRodape();
        }

        private void ConfigurarJanela()
        {
            this.Text = "Tocantins Motores - Controle de Estoque";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.KeyPreview = true;
        }

        private void CriarMenuSuperior()
        {
            menuStrip = new MenuStrip();
            menuStrip.BackColor = Color.White;
            menuStrip.Font = new Font("Segoe UI", 10);
            menuStrip.Padding = new Padding(10, 5, 10, 5);

            if (SessaoSistema.TemPermissao("PRODUTO_VISUALIZAR") || SessaoSistema.TemPermissao("USUARIO_GERENCIAR"))
            {
                ToolStripMenuItem menuCadastro = new ToolStripMenuItem("Cadastros");

                ToolStripMenuItem subItemProdutos = new ToolStripMenuItem("Produtos");
                subItemProdutos.Click += (s, e) => { FrmProdutos tela = new FrmProdutos(); tela.ShowDialog(); };
                menuCadastro.DropDownItems.Add(subItemProdutos);

                ToolStripMenuItem subItemFornecedores = new ToolStripMenuItem("Fornecedores");
                subItemFornecedores.Click += (s, e) => { FrmFornecedores tela = new FrmFornecedores(); tela.ShowDialog(); };
                menuCadastro.DropDownItems.Add(subItemFornecedores);

                ToolStripMenuItem subItemUsuarios = new ToolStripMenuItem("Usuários");
                subItemUsuarios.Click += (s, e) => { FrmUsuarios tela = new FrmUsuarios(); tela.ShowDialog(); };
                subItemUsuarios.Visible = SessaoSistema.TemPermissao("USUARIO_GERENCIAR");
                menuCadastro.DropDownItems.Add(subItemUsuarios);

                menuStrip.Items.Add(menuCadastro);
            }

            if (SessaoSistema.TemPermissao("LOG_VISUALIZAR"))
            {
                ToolStripMenuItem menuRelatorios = new ToolStripMenuItem("Relatórios");
                ToolStripMenuItem subItemLogs = new ToolStripMenuItem("Logs do Sistema");
                subItemLogs.Click += (s, e) => { FrmLogs tela = new FrmLogs(); tela.ShowDialog(); };
                menuRelatorios.DropDownItems.Add(subItemLogs);
                menuStrip.Items.Add(menuRelatorios);
            }

            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void CriarBarraBotoes()
        {
            faixaCorLoja = new Panel();
            faixaCorLoja.Dock = DockStyle.Top;
            faixaCorLoja.Height = 4;
            faixaCorLoja.BackColor = Color.DodgerBlue;
            this.Controls.Add(faixaCorLoja);

            panelBotoes = new Panel();
            panelBotoes.Dock = DockStyle.Top;
            panelBotoes.Height = 90;
            panelBotoes.BackColor = Color.White;
            this.Controls.Add(panelBotoes);

            btnProdutos = CriarBotaoAtalho("📦 Produtos", 20);
            btnProdutos.Click += (s, e) => { FrmProdutos tela = new FrmProdutos(); tela.ShowDialog(); };
            btnProdutos.Visible = SessaoSistema.TemPermissao("PRODUTO_VISUALIZAR");

            btnSaidas = CriarBotaoAtalho("📤 Saídas / TRF", 150);
            btnSaidas.Click += (s, e) => { FrmSaida tela = new FrmSaida(); tela.ShowDialog(); };
            btnSaidas.Visible = SessaoSistema.TemPermissao("SAIDA_REGISTRAR");

            btnReceber = CriarBotaoAtalho("📥 Receber TRF", 280);
            btnReceber.Click += (s, e) => { FrmReceberTransferencia tela = new FrmReceberTransferencia(); tela.ShowDialog(); };
            btnReceber.Visible = SessaoSistema.TemPermissao("TRF_RECEBER");

            btnCotacoes = CriarBotaoAtalho("🛒 Cotações", 410);
            btnCotacoes.Click += (s, e) => { FrmCotacoes tela = new FrmCotacoes(); tela.ShowDialog(); };
            btnCotacoes.Visible = SessaoSistema.TemPermissao("COTACAO_CRIAR");

            btnSair = new Button();
            btnSair.Text = "🔒 TROCAR USUÁRIO";
            btnSair.Size = new Size(140, 40);
            btnSair.Location = new Point(panelBotoes.Width - 160, 25);
            btnSair.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSair.BackColor = Color.IndianRed;
            btnSair.ForeColor = Color.White;
            btnSair.FlatStyle = FlatStyle.Flat;
            btnSair.FlatAppearance.BorderSize = 0;
            btnSair.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSair.Cursor = Cursors.Hand;

            btnSair.Click += (s, e) =>
            {
                if (MessageBox.Show("Deseja sair do sistema e trocar de usuário?", "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SessaoSistema.RegistrarLog("LOGOUT", "Usuário saiu do sistema.");
                    this.Hide();
                    FrmLogin telaLogin = new FrmLogin();
                    telaLogin.ShowDialog();
                    this.Close();
                }
            };
            panelBotoes.Controls.Add(btnSair);
        }

        private Button CriarBotaoAtalho(string texto, int posX)
        {
            Button btn = new Button();
            btn.Text = texto;
            btn.Size = new Size(120, 55);
            btn.Location = new Point(posX, 15);
            btn.BackColor = Color.FromArgb(245, 247, 250);
            btn.ForeColor = Color.FromArgb(50, 50, 50);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Color.LightGray;
            btn.FlatAppearance.BorderSize = 1;
            btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => { btn.BackColor = Color.FromArgb(230, 235, 245); btn.ForeColor = Color.FromArgb(30, 60, 120); };
            btn.MouseLeave += (s, e) => { btn.BackColor = Color.FromArgb(245, 247, 250); btn.ForeColor = Color.FromArgb(50, 50, 50); };

            panelBotoes.Controls.Add(btn);
            return btn;
        }

        private void CriarPainelLateral()
        {
            panelLateral = new Panel();
            panelLateral.Dock = DockStyle.Right;
            panelLateral.Width = 260;
            panelLateral.BackColor = Color.White;
            panelLateral.Padding = new Padding(20);
            this.Controls.Add(panelLateral);

            Label lblTituloLoja = new Label();
            lblTituloLoja.Text = "AMBIENTE DE TRABALHO";
            lblTituloLoja.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblTituloLoja.ForeColor = Color.Gray;
            lblTituloLoja.AutoSize = true;
            lblTituloLoja.Location = new Point(20, 30);
            panelLateral.Controls.Add(lblTituloLoja);

            groupLoja = new GroupBox();
            groupLoja.Text = "LOJA ATIVA";
            groupLoja.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupLoja.Size = new Size(220, 110);
            groupLoja.Location = new Point(15, 60);
            groupLoja.ForeColor = Color.FromArgb(30, 60, 120);

            rbMatriz = new RadioButton();
            rbMatriz.Text = "MATRIZ (Paraíso)";
            rbMatriz.Location = new Point(15, 30);
            rbMatriz.AutoSize = true;
            rbMatriz.Checked = true;
            rbMatriz.Font = new Font("Segoe UI", 10);
            rbMatriz.CheckedChanged += RbLoja_CheckedChanged;

            rbDistribuidora = new RadioButton();
            rbDistribuidora.Text = "DISTRIBUIDORA";
            rbDistribuidora.Location = new Point(15, 65);
            rbDistribuidora.AutoSize = true;
            rbDistribuidora.Font = new Font("Segoe UI", 10);
            rbDistribuidora.CheckedChanged += RbLoja_CheckedChanged;

            groupLoja.Controls.Add(rbMatriz);
            groupLoja.Controls.Add(rbDistribuidora);
            panelLateral.Controls.Add(groupLoja);
        }

        private void RbLoja_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMatriz.Checked)
            {
                SessaoSistema.LojaIdAtiva = 1;
                SessaoSistema.NomeLojaAtiva = "MATRIZ";
                faixaCorLoja.BackColor = Color.DodgerBlue;
            }
            else if (rbDistribuidora.Checked)
            {
                SessaoSistema.LojaIdAtiva = 2;
                SessaoSistema.NomeLojaAtiva = "DISTRIBUIDORA";
                faixaCorLoja.BackColor = Color.MediumSeaGreen;
            }
            lblLojaAtiva.Text = "Loja: " + SessaoSistema.NomeLojaAtiva;

            // GRAVA O LOG DE TROCA DE LOJA
            SessaoSistema.RegistrarLog("TROCA_LOJA", $"Alterou para a loja {SessaoSistema.NomeLojaAtiva}");
        }

        private void CriarRodape()
        {
            statusStrip = new StatusStrip();
            statusStrip.BackColor = Color.FromArgb(30, 60, 120);
            statusStrip.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            lblUsuario = new ToolStripStatusLabel();
            lblLojaAtiva = new ToolStripStatusLabel();

            lblUsuario.Text = $"👤 Usuário: {SessaoSistema.NomeUsuario} ({SessaoSistema.UsuarioId})  |  ";
            lblUsuario.ForeColor = Color.White;

            lblLojaAtiva.Text = "Loja: " + SessaoSistema.NomeLojaAtiva;
            lblLojaAtiva.ForeColor = Color.White;

            statusStrip.Items.Add(lblUsuario);
            statusStrip.Items.Add(lblLojaAtiva);
            this.Controls.Add(statusStrip);
        }
    }
}