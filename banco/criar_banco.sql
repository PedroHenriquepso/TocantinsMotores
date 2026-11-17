/* =====================================================================
   Tocantins Motores - Controle de Estoque
   Script de criação do banco de dados (somente estrutura, sem dados)

   Como usar (SQL Server Management Studio):
     1. Abra este arquivo (Arquivo > Abrir > Arquivo...).
     2. Clique em "Executar" (F5).
     3. Depois execute o arquivo dados_iniciais.sql.

   Testado em SQL Server com Windows. Execute em um servidor onde o banco
   TocantinsMotoresDB ainda NÃO exista.
   ===================================================================== */

CREATE DATABASE [TocantinsMotoresDB]
GO
USE [TocantinsMotoresDB]
GO
/****** Objeto:  Table [dbo].[Cotacoes]    Data do Script: 17/11/2026 20:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cotacoes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NumeroCotacao] [varchar](20) NOT NULL,
	[LojaId] [int] NOT NULL,
	[UsuarioId] [int] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[Status] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Cotacoes_Itens]    Data do Script: 17/11/2026 20:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cotacoes_Itens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CotacaoId] [int] NOT NULL,
	[ProdutoId] [int] NOT NULL,
	[QuantidadeSolicitada] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Cotacoes_Precos]    Data do Script: 17/11/2026 20:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cotacoes_Precos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CotacaoItemId] [int] NOT NULL,
	[FornecedorId] [int] NOT NULL,
	[PrecoUnitario] [decimal](18, 2) NOT NULL,
	[MarcaOferecida] [varchar](50) NULL,
	[PrazoEntrega] [int] NULL,
	[Vencedor] [bit] NOT NULL,
	[FornecedorNome] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Estoques]    Data do Script: 17/11/2026 20:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Estoques](
	[ProdutoId] [int] NOT NULL,
	[LojaId] [int] NOT NULL,
	[Quantidade] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProdutoId] ASC,
	[LojaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Fornecedores]    Data do Script: 17/11/2026 20:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fornecedores](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](100) NOT NULL,
	[Cidade] [varchar](100) NOT NULL,
	[Estado] [varchar](2) NOT NULL,
	[LojaVinculadaId] [int] NOT NULL,
	[MarcasQueTrabalha] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Historico_Compras]    Data do Script: 17/11/2026 20:45:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Historico_Compras](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProdutoId] [int] NOT NULL,
	[NotaFiscalId] [int] NOT NULL,
	[QuantidadeComprada] [int] NOT NULL,
	[ValorUnitario] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Logs_Sistema]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs_Sistema](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DataHora] [datetime] NOT NULL,
	[UsuarioId] [int] NULL,
	[LojaId] [int] NULL,
	[Acao] [varchar](50) NOT NULL,
	[Detalhes] [varchar](max) NULL,
	[IP_Maquina] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Lojas]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lojas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](50) NOT NULL,
	[Ativa] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Movimentacoes]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Movimentacoes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tipo] [varchar](20) NOT NULL,
	[NumeroDocumento] [varchar](20) NOT NULL,
	[LojaOrigemId] [int] NOT NULL,
	[LojaDestinoId] [int] NULL,
	[UsuarioId] [int] NOT NULL,
	[SolicitanteNome] [varchar](100) NULL,
	[DataHora] [datetime] NOT NULL,
	[Status] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Movimentacoes_Itens]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Movimentacoes_Itens](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MovimentacaoId] [int] NOT NULL,
	[ProdutoId] [int] NOT NULL,
	[Quantidade] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[NotasFiscais]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotasFiscais](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NumeroNF] [varchar](20) NOT NULL,
	[Serie] [varchar](5) NOT NULL,
	[FornecedorId] [int] NOT NULL,
	[LojaId] [int] NOT NULL,
	[DataEmissao] [date] NOT NULL,
	[DataEntrada] [datetime] NOT NULL,
	[ValorTotal] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Pedidos_Compra]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pedidos_Compra](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CotacaoId] [int] NOT NULL,
	[FornecedorId] [int] NOT NULL,
	[NumeroPedido] [varchar](20) NOT NULL,
	[DataFechamento] [datetime] NOT NULL,
	[UsuarioId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Perfis]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Perfis](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nome] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Permissoes_Perfil]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissoes_Perfil](
	[PerfilId] [int] NOT NULL,
	[ChavePermissao] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PerfilId] ASC,
	[ChavePermissao] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Produtos]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Produtos](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CodigoInterno] [varchar](10) NOT NULL,
	[CodigoFabricante] [varchar](50) NOT NULL,
	[Descricao] [varchar](150) NOT NULL,
	[Marca] [varchar](50) NOT NULL,
	[DataCadastro] [datetime] NOT NULL,
	[Aplicacao] [varchar](150) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[CodigoInterno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Usuarios]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Login] [varchar](20) NOT NULL,
	[Nome] [varchar](100) NOT NULL,
	[SenhaHash] [varchar](255) NOT NULL,
	[PerfilId] [int] NOT NULL,
	[Ativo] [bit] NOT NULL,
	[PodeEditarEstoqueManual] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto:  Table [dbo].[Usuarios_Lojas]    Data do Script: 17/11/2026 20:45:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios_Lojas](
	[UsuarioId] [int] NOT NULL,
	[LojaId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UsuarioId] ASC,
	[LojaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Cotacoes] ADD  DEFAULT (getdate()) FOR [DataCriacao]
GO
ALTER TABLE [dbo].[Cotacoes] ADD  DEFAULT ('ABERTA') FOR [Status]
GO
ALTER TABLE [dbo].[Cotacoes_Precos] ADD  DEFAULT ((0)) FOR [Vencedor]
GO
ALTER TABLE [dbo].[Estoques] ADD  DEFAULT ((0)) FOR [Quantidade]
GO
ALTER TABLE [dbo].[Logs_Sistema] ADD  DEFAULT (getdate()) FOR [DataHora]
GO
ALTER TABLE [dbo].[Lojas] ADD  DEFAULT ((1)) FOR [Ativa]
GO
ALTER TABLE [dbo].[Movimentacoes] ADD  DEFAULT (getdate()) FOR [DataHora]
GO
ALTER TABLE [dbo].[Movimentacoes] ADD  DEFAULT ('CONCLUIDO') FOR [Status]
GO
ALTER TABLE [dbo].[NotasFiscais] ADD  DEFAULT (getdate()) FOR [DataEntrada]
GO
ALTER TABLE [dbo].[Pedidos_Compra] ADD  DEFAULT (getdate()) FOR [DataFechamento]
GO
ALTER TABLE [dbo].[Produtos] ADD  DEFAULT (getdate()) FOR [DataCadastro]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((1)) FOR [Ativo]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((0)) FOR [PodeEditarEstoqueManual]
GO
ALTER TABLE [dbo].[Cotacoes]  WITH NOCHECK ADD FOREIGN KEY([LojaId])
REFERENCES [dbo].[Lojas] ([Id])
GO
ALTER TABLE [dbo].[Cotacoes]  WITH NOCHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Cotacoes_Itens]  WITH NOCHECK ADD FOREIGN KEY([CotacaoId])
REFERENCES [dbo].[Cotacoes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cotacoes_Itens]  WITH NOCHECK ADD FOREIGN KEY([ProdutoId])
REFERENCES [dbo].[Produtos] ([Id])
GO
ALTER TABLE [dbo].[Cotacoes_Precos]  WITH NOCHECK ADD FOREIGN KEY([CotacaoItemId])
REFERENCES [dbo].[Cotacoes_Itens] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Cotacoes_Precos]  WITH NOCHECK ADD FOREIGN KEY([FornecedorId])
REFERENCES [dbo].[Fornecedores] ([Id])
GO
ALTER TABLE [dbo].[Estoques]  WITH NOCHECK ADD FOREIGN KEY([LojaId])
REFERENCES [dbo].[Lojas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Estoques]  WITH NOCHECK ADD FOREIGN KEY([ProdutoId])
REFERENCES [dbo].[Produtos] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Fornecedores]  WITH NOCHECK ADD FOREIGN KEY([LojaVinculadaId])
REFERENCES [dbo].[Lojas] ([Id])
GO
ALTER TABLE [dbo].[Historico_Compras]  WITH NOCHECK ADD FOREIGN KEY([NotaFiscalId])
REFERENCES [dbo].[NotasFiscais] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Historico_Compras]  WITH NOCHECK ADD FOREIGN KEY([ProdutoId])
REFERENCES [dbo].[Produtos] ([Id])
GO
ALTER TABLE [dbo].[Logs_Sistema]  WITH NOCHECK ADD FOREIGN KEY([LojaId])
REFERENCES [dbo].[Lojas] ([Id])
GO
ALTER TABLE [dbo].[Logs_Sistema]  WITH NOCHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Movimentacoes]  WITH NOCHECK ADD FOREIGN KEY([LojaDestinoId])
REFERENCES [dbo].[Lojas] ([Id])
GO
ALTER TABLE [dbo].[Movimentacoes]  WITH NOCHECK ADD FOREIGN KEY([LojaOrigemId])
REFERENCES [dbo].[Lojas] ([Id])
GO
ALTER TABLE [dbo].[Movimentacoes]  WITH NOCHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Movimentacoes_Itens]  WITH NOCHECK ADD FOREIGN KEY([MovimentacaoId])
REFERENCES [dbo].[Movimentacoes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Movimentacoes_Itens]  WITH NOCHECK ADD FOREIGN KEY([ProdutoId])
REFERENCES [dbo].[Produtos] ([Id])
GO
ALTER TABLE [dbo].[NotasFiscais]  WITH NOCHECK ADD FOREIGN KEY([FornecedorId])
REFERENCES [dbo].[Fornecedores] ([Id])
GO
ALTER TABLE [dbo].[NotasFiscais]  WITH NOCHECK ADD FOREIGN KEY([LojaId])
REFERENCES [dbo].[Lojas] ([Id])
GO
ALTER TABLE [dbo].[Pedidos_Compra]  WITH NOCHECK ADD FOREIGN KEY([CotacaoId])
REFERENCES [dbo].[Cotacoes] ([Id])
GO
ALTER TABLE [dbo].[Pedidos_Compra]  WITH NOCHECK ADD FOREIGN KEY([FornecedorId])
REFERENCES [dbo].[Fornecedores] ([Id])
GO
ALTER TABLE [dbo].[Pedidos_Compra]  WITH NOCHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
GO
ALTER TABLE [dbo].[Permissoes_Perfil]  WITH NOCHECK ADD FOREIGN KEY([PerfilId])
REFERENCES [dbo].[Perfis] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Usuarios]  WITH NOCHECK ADD FOREIGN KEY([PerfilId])
REFERENCES [dbo].[Perfis] ([Id])
GO
ALTER TABLE [dbo].[Usuarios_Lojas]  WITH NOCHECK ADD FOREIGN KEY([LojaId])
REFERENCES [dbo].[Lojas] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Usuarios_Lojas]  WITH NOCHECK ADD FOREIGN KEY([UsuarioId])
REFERENCES [dbo].[Usuarios] ([Id])
ON DELETE CASCADE
GO
