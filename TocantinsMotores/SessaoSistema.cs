using System.Collections.Generic;
using System.Data.SqlClient;

namespace TocantinsMotores
{
    public static class SessaoSistema
    {
        public static int UsuarioId { get; set; }
        public static string NomeUsuario { get; set; }
        public static int LojaIdAtiva { get; set; } = 1;
        public static string NomeLojaAtiva { get; set; } = "MATRIZ";

        public static List<string> Permissoes { get; set; } = new List<string>();

        public static bool TemPermissao(string chave)
        {
            return Permissoes.Contains(chave);
        }

        // FUNÇÃO GLOBAL PARA GRAVAR LOG NO BANCO DE DADOS
        public static void RegistrarLog(string acao, string detalhes = "")
        {
            string conexaoString = @"Server=localhost;Database=TocantinsMotoresDB;Trusted_Connection=True;TrustServerCertificate=True;";
            try
            {
                using (SqlConnection conn = new SqlConnection(conexaoString))
                {
                    conn.Open();
                    string query = "INSERT INTO Logs_Sistema (UsuarioId, LojaId, Acao, Detalhes) VALUES (@usuario, @loja, @acao, @detalhes)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", UsuarioId == 0 ? (object)System.DBNull.Value : UsuarioId);
                        cmd.Parameters.AddWithValue("@loja", LojaIdAtiva);
                        cmd.Parameters.AddWithValue("@acao", acao);
                        cmd.Parameters.AddWithValue("@detalhes", detalhes);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { /* Ignora erro de log para não travar o sistema */ }
        }
    }
}