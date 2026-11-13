namespace Conde.Bispos;

/// <summary>
/// A busca de verdade: percorre as casas em ordem e guarda as duas diagonais
/// ja usadas em mascaras de bits. Conta quantos conjuntos de cada tamanho
/// existem numa passada so, sem repetir trabalho por k.
///
/// Esta e a rota com poda. A outra rota, independente desta, esta em
/// <see cref="Torres"/>, e nao compartilha uma linha de codigo com ela.
/// </summary>
public static class Busca
{
    /// <summary>
    /// contagem[k] = quantos conjuntos de k bispos independentes cabem no
    /// tabuleiro n por n. O indice vai de 0 ate n*n, e tudo acima de 2n-2
    /// sai zero.
    /// </summary>
    public static long[] Conta(int n)
    {
        var contagem = new long[n * n + 1];
        int casas = n * n;
        long nos = 0;

        void Anda(int proxima, int quantos, int sobe, int desce)
        {
            contagem[quantos]++;
            nos++;
            for (int i = proxima; i < casas; i++)
            {
                var (l, c) = Nucleo.Casa(i, n);
                int bs = 1 << Nucleo.Sobe(l, c);
                int bd = 1 << Nucleo.Desce(l, c, n);
                if ((sobe & bs) != 0 || (desce & bd) != 0) continue;
                Anda(i + 1, quantos + 1, sobe | bs, desce | bd);
            }
        }

        Anda(0, 0, 0, 0);
        UltimoNumeroDeNos = nos;
        return contagem;
    }

    /// <summary>Quantos nos a ultima chamada de <see cref="Conta"/> visitou.</summary>
    public static long UltimoNumeroDeNos { get; private set; }

    /// <summary>O maior k com contagem maior que zero.</summary>
    public static int Maximo(int n)
    {
        var c = Conta(n);
        for (int k = c.Length - 1; k >= 0; k--)
            if (c[k] > 0) return k;
        return 0;
    }

    /// <summary>
    /// Os conjuntos de tamanho maximo, listados por inteiro. Sao poucos: a
    /// pergunta e quantos, e a resposta medida vira teste.
    /// </summary>
    public static List<int[]> Maximos(int n)
    {
        int alvo = Maximo(n);
        var saida = new List<int[]>();
        int casas = n * n;
        var atual = new int[alvo];

        void Anda(int proxima, int quantos, int sobe, int desce)
        {
            if (quantos == alvo)
            {
                saida.Add((int[])atual.Clone());
                return;
            }
            for (int i = proxima; i < casas; i++)
            {
                var (l, c) = Nucleo.Casa(i, n);
                int bs = 1 << Nucleo.Sobe(l, c);
                int bd = 1 << Nucleo.Desce(l, c, n);
                if ((sobe & bs) != 0 || (desce & bd) != 0) continue;
                atual[quantos] = i;
                Anda(i + 1, quantos + 1, sobe | bs, desce | bd);
            }
        }

        Anda(0, 0, 0, 0);
        return saida;
    }

    /// <summary>
    /// O menor numero de bispos que domina um n por n, e quantos conjuntos
    /// desse tamanho existem. A busca e por tamanho crescente: quem domina com
    /// k tambem domina com k+1, entao o primeiro k com resposta e o minimo.
    /// </summary>
    public static (int Minimo, long Quantos) Dominacao(int n)
    {
        int casas = n * n;
        for (int k = 0; k <= casas; k++)
        {
            long achados = 0;
            var escolha = new (int, int)[k];

            void Anda(int inicio, int quantos)
            {
                if (quantos == k)
                {
                    if (Cobre(escolha, n)) achados++;
                    return;
                }
                if (casas - inicio < k - quantos) return;
                for (int i = inicio; i < casas; i++)
                {
                    escolha[quantos] = Nucleo.Casa(i, n);
                    Anda(i + 1, quantos + 1);
                }
            }

            Anda(0, 0);
            if (achados > 0) return (k, achados);
        }
        throw new InvalidOperationException("impossivel");
    }

    /// <summary>
    /// Cobertura por diagonais: uma casa esta coberta quando a diagonal que
    /// sobe OU a que desce tem bispo. Isso nao e a definicao, e uma
    /// consequencia dela, e o <see cref="Juiz"/> existe para conferir que da
    /// no mesmo.
    /// </summary>
    public static bool Cobre(IReadOnlyList<(int Linha, int Coluna)> bispos, int n)
    {
        int sobe = 0, desce = 0;
        foreach (var (l, c) in bispos)
        {
            sobe |= 1 << Nucleo.Sobe(l, c);
            desce |= 1 << Nucleo.Desce(l, c, n);
        }
        for (int l = 0; l < n; l++)
            for (int c = 0; c < n; c++)
                if ((sobe & (1 << Nucleo.Sobe(l, c))) == 0 &&
                    (desce & (1 << Nucleo.Desce(l, c, n))) == 0) return false;
        return true;
    }
}
