/* =====================================================================
   Tocantins Motores - Dados iniciais
   Execute DEPOIS do criar_banco.sql.

   Cria o mínimo para o sistema funcionar:
     - 2 lojas (Matriz e Distribuidora)
     - perfil ADMINISTRADOR com todas as permissões
     - usuário de demonstração para o primeiro acesso

   Primeiro acesso:   usuário: ADMIN    senha: admin123
   TROQUE ESSA SENHA depois de entrar (Usuários > editar usuário).
   ===================================================================== */

USE [TocantinsMotoresDB]
GO

IF NOT EXISTS (SELECT 1 FROM Lojas)
BEGIN
    INSERT INTO Lojas (Nome, Ativa) VALUES ('MATRIZ (Paraíso)', 1);
    INSERT INTO Lojas (Nome, Ativa) VALUES ('DISTRIBUIDORA', 1);
END
GO

IF NOT EXISTS (SELECT 1 FROM Perfis WHERE Nome = 'ADMINISTRADOR')
    INSERT INTO Perfis (Nome) VALUES ('ADMINISTRADOR');
GO

DECLARE @perfil INT = (SELECT Id FROM Perfis WHERE Nome = 'ADMINISTRADOR');

INSERT INTO Permissoes_Perfil (PerfilId, ChavePermissao)
SELECT @perfil, v.Chave
FROM (VALUES
        ('PRODUTO_VISUALIZAR'),
        ('PRODUTO_EDITAR'),
        ('PRODUTO_EXCLUIR'),
        ('PRODUTO_ESTOQUE_MANUAL'),
        ('SAIDA_REGISTRAR'),
        ('TRF_REGISTRAR'),
        ('TRF_RECEBER'),
        ('COTACAO_CRIAR'),
        ('USUARIO_GERENCIAR'),
        ('LOG_VISUALIZAR')
     ) AS v(Chave)
WHERE NOT EXISTS (
    SELECT 1 FROM Permissoes_Perfil pp
    WHERE pp.PerfilId = @perfil AND pp.ChavePermissao = v.Chave
);

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Login = 'ADMIN')
    INSERT INTO Usuarios (Login, Nome, SenhaHash, PerfilId, Ativo, PodeEditarEstoqueManual)
    VALUES ('ADMIN', 'Administrador', '$2b$12$DLAixJMyb3KKVRzvDs8gtubwZE2kRNklLTlWuWsk0ORCsuKtyBW8C', @perfil, 1, 1);

INSERT INTO Usuarios_Lojas (UsuarioId, LojaId)
SELECT u.Id, l.Id
FROM Usuarios u CROSS JOIN Lojas l
WHERE u.Login = 'ADMIN'
  AND NOT EXISTS (
      SELECT 1 FROM Usuarios_Lojas ul WHERE ul.UsuarioId = u.Id AND ul.LojaId = l.Id
  );
GO
