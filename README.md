# 🔧 Tocantins Motores — Controle de Estoque

Sistema desktop para **gestão de estoque de uma loja de peças para motores**, com controle por loja, transferências entre filiais, cotações com fornecedores e controle de acesso por perfil.

## ✨ Funcionalidades

- 🔐 **Login seguro** com senhas protegidas por hash BCrypt
- 👥 **Usuários e perfis de permissão**: cada perfil libera só as telas que ele precisa
- 📦 **Cadastro de produtos** com código do fabricante, descrição, marca e aplicação (ex.: Cummins, Scania)
- 🏬 **Estoque separado por loja** (Matriz e Distribuidora)
- 📥 **Entrada manual de estoque**
- 📤 **Saída para cliente** e **transferência interna** entre lojas
- 🔄 **Recebimento de transferência**: a loja de destino confirma o que chegou
- 💰 **Módulo de cotações**: pesquisa peças, registra preços de fornecedores e finaliza a cotação
- 📄 **Exportação para PDF e Excel**
- 🏢 **Cadastro de fornecedores**
- 📝 **Log de auditoria**: registra as ações feitas no sistema
- ⌨️ **Atalhos de teclado** (F2 para pesquisar peça, Enter para adicionar, ESC para cancelar)

## 🛠️ Tecnologias

| | |
|---|---|
| Linguagem | C# |
| Interface | Windows Forms (.NET Framework 4.7.2) |
| Banco de dados | SQL Server (`System.Data.SqlClient`) |
| Segurança | BCrypt.Net-Next |
| Relatórios | iTextSharp (PDF) e ClosedXML (Excel) |

## 🗄️ Banco de dados

O sistema usa o banco `TocantinsMotoresDB`, com estas tabelas:

`Produtos` · `Estoques` · `Lojas` · `Movimentacoes` · `Movimentacoes_Itens` · `Usuarios` · `Perfis` · `Permissoes_Perfil` · `Fornecedores` · `Cotacoes` · `Cotacoes_Itens` · `Cotacoes_Precos` · `Logs_Sistema`

## ▶️ Como executar

**Pré-requisitos:** Windows, Visual Studio 2022 (ou superior) com o desenvolvimento para desktop .NET, e SQL Server (pode ser o Express).

1. Clone o repositório:
   ```bash
   git clone https://github.com/PedroHenriquepso/TocantinsMotores.git
   ```
2. Crie o banco `TocantinsMotoresDB` no SQL Server e as tabelas listadas acima.
3. Abra `TocantinsMotores.slnx` no Visual Studio e restaure os pacotes NuGet.
4. Se o seu SQL Server não for `localhost`, ajuste a string de conexão (`Server=...`) nos arquivos do projeto.
5. Pressione **F5** para executar.

## 🧠 O que aprendi

- Modelagem de banco relacional com várias tabelas e relacionamentos
- Controle de acesso por perfis e permissões
- Consultas SQL com parâmetros para evitar SQL Injection
- Geração de relatórios em PDF e Excel
- Organização de um sistema com várias telas e regras de negócio reais

## 🗺️ Próximos passos

- [ ] Adicionar o script `.sql` de criação do banco
- [ ] Mover a string de conexão para o `App.config`
- [ ] Adicionar capturas de tela ao README

---
Feito por [Pedro Henrique](https://github.com/PedroHenriquepso)
