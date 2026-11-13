namespace Conde.Bispos;

/// <summary>
/// A segunda rota, que nao olha o tabuleiro uma vez sequer.
///
/// Girando o tabuleiro 45 graus, a diagonal que sobe vira linha e a que desce
/// vira coluna, e um bispo vira uma TORRE. Casas de cor diferente nunca se
/// encontram, entao o tabuleiro se parte em DOIS tabuleiros de torres.
///
/// Em cada um deles as linhas sao intervalos ENCAIXADOS: a diagonal do meio
/// contem a vizinha, que contem a vizinha dela. Para linhas encaixadas, pondo
/// as torres nas linhas menores primeiro, a j-esima tem (tamanho - (j-1))
/// colunas livres. Dai sai a contagem inteira sem busca nenhuma.
/// </summary>
public static class Torres
{
    /// <summary>
    /// Os tamanhos das diagonais que sobem, de 0 a 2n-2:
    /// 1, 2, 3, ..., n, ..., 3, 2, 1.
    /// </summary>
    public static int[] TamanhosDasDiagonais(int n)
    {
        var t = new int[2 * n - 1];
        for (int d = 0; d < 2 * n - 1; d++)
            t[d] = n - Math.Abs(d - (n - 1));
        return t;
    }

    /// <summary>
    /// Quantas torres de cada tamanho cabem num tabuleiro de linhas
    /// encaixadas com os tamanhos dados. saida[k] = numero de colocacoes de k.
    /// </summary>
    public static long[] ContaEncaixado(IReadOnlyList<int> tamanhos)
    {
        var ordenados = tamanhos.OrderBy(x => x).ToArray();
        var dp = new long[ordenados.Length + 1];
        dp[0] = 1;
        foreach (int s in ordenados)
            for (int k = Math.Min(ordenados.Length, s); k >= 1; k--)
            {
                long livres = s - (k - 1);
                if (livres > 0) dp[k] += dp[k - 1] * livres;
            }
        return dp;
    }

    /// <summary>
    /// A contagem completa: as duas cores multiplicadas uma pela outra.
    /// saida[k] = quantos conjuntos de k bispos independentes.
    /// </summary>
    public static long[] Conta(int n)
    {
        var (a, b) = PorCor(n);
        var saida = new long[n * n + 1];
        for (int i = 0; i < a.Length; i++)
            for (int j = 0; j < b.Length; j++)
                if (a[i] != 0 && b[j] != 0 && i + j < saida.Length)
                    saida[i + j] += a[i] * b[j];
        return saida;
    }

    /// <summary>A contagem de cada cor, separada.</summary>
    public static (long[] Par, long[] Impar) PorCor(int n)
    {
        var t = TamanhosDasDiagonais(n);
        var par = new List<int>();
        var impar = new List<int>();
        for (int d = 0; d < t.Length; d++)
            (d % 2 == 0 ? par : impar).Add(t[d]);
        return (ContaEncaixado(par), ContaEncaixado(impar));
    }
}
