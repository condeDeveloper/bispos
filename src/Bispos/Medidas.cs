namespace Conde.Bispos;

/// <summary>
/// As tabelas do README. Todo numero que aparece la sai daqui, e esta preso
/// em <c>MedidasTestes</c>.
/// </summary>
public static class Medidas
{
    private static readonly Dictionary<int, long[]> CacheBusca = [];

    /// <summary>A contagem da busca, guardada: varias tabelas pedem a mesma.</summary>
    public static long[] PelaBusca(int n)
    {
        lock (CacheBusca)
        {
            if (!CacheBusca.TryGetValue(n, out var c))
            {
                c = Busca.Conta(n);
                CacheBusca[n] = c;
            }
            return c;
        }
    }

    /// <summary>
    /// A tabela principal, pela BUSCA: para cada n, o total de conjuntos
    /// independentes, o maximo, quantos conjuntos maximos existem e 2^n.
    /// Para em 8 porque a busca visita um no por conjunto, e em 9 por 9 isso
    /// ja sao 2,3 bilhoes de nos.
    /// </summary>
    public static IReadOnlyList<Linha> Tabela(int ate = 8)
    {
        var linhas = new List<Linha>();
        for (int n = 1; n <= ate; n++)
        {
            var c = PelaBusca(n);
            int maximo = 0;
            long total = 0;
            for (int k = 0; k < c.Length; k++)
            {
                total += c[k];
                if (c[k] > 0) maximo = k;
            }
            linhas.Add(new Linha(n, total, maximo, c[maximo], 1L << n, c));
        }
        return linhas;
    }

    /// <summary>
    /// As duas rotas, conferidas celula por celula: a busca com poda contra o
    /// produto dos dois tabuleiros de torres. Devolve quantas celulas foram
    /// conferidas e quantas divergiram.
    /// </summary>
    public static (int Conferidas, int Divergem) BuscaContraTorres(int ate = 8)
    {
        int conferidas = 0, divergem = 0;
        for (int n = 1; n <= ate; n++)
        {
            var a = PelaBusca(n);
            var b = Torres.Conta(n);
            for (int k = 0; k < a.Length; k++)
            {
                conferidas++;
                if (a[k] != b[k]) divergem++;
            }
        }
        return (conferidas, divergem);
    }

    /// <summary>
    /// O juiz que enumera todo subconjunto, contra a busca. So ate n = 5:
    /// em 6 por 6 seriam 2^36 subconjuntos.
    /// </summary>
    public static (int Conferidas, int Divergem) JuizContraBusca(int ate = 5)
    {
        int conferidas = 0, divergem = 0;
        for (int n = 1; n <= ate; n++)
        {
            var c = PelaBusca(n);
            int maximo = 2 * n - 1;
            for (int k = 0; k <= Math.Min(maximo, n * n); k++)
            {
                conferidas++;
                if (Juiz.ContaIndependentes(n, k) != c[k]) divergem++;
            }
        }
        return (conferidas, divergem);
    }

    /// <summary>
    /// Quantos conjuntos de tamanho maximo existem, contra a aposta 2^n.
    /// Vai ate 11 porque usa a rota das torres, que nao enumera nada.
    /// </summary>
    public static IReadOnlyList<(int N, int Maximo, long Quantos, long DoisElevadoN, bool Bate)> Maximos(int ate = 11)
    {
        var saida = new List<(int, int, long, long, bool)>();
        for (int n = 1; n <= ate; n++)
        {
            var c = Torres.Conta(n);
            int maximo = 0;
            for (int k = 0; k < c.Length; k++)
                if (c[k] > 0) maximo = k;
            saida.Add((n, maximo, c[maximo], 1L << n, c[maximo] == 1L << n));
        }
        return saida;
    }

    /// <summary>
    /// A rota das torres sozinha, que chega onde a busca nao chega: o total
    /// de conjuntos independentes de 1 a 11.
    /// </summary>
    public static IReadOnlyList<(int N, long Total, int Maximo)> PelasTorres(int ate = 11)
    {
        var saida = new List<(int, long, int)>();
        for (int n = 1; n <= ate; n++)
        {
            var c = Torres.Conta(n);
            long total = 0;
            int maximo = 0;
            for (int k = 0; k < c.Length; k++)
            {
                total += c[k];
                if (c[k] > 0) maximo = k;
            }
            saida.Add((n, total, maximo));
        }
        return saida;
    }

    /// <summary>
    /// Os conjuntos maximos LISTADOS pela busca, nao so contados: a terceira
    /// conferida do 2^n. Para em 7 porque a listagem anda o tabuleiro inteiro.
    /// </summary>
    public static IReadOnlyList<(int N, int Quantos)> MaximosListados(int ate = 7)
    {
        var saida = new List<(int, int)>();
        for (int n = 1; n <= ate; n++)
            saida.Add((n, Busca.Maximos(n).Count));
        return saida;
    }

    /// <summary>
    /// O menor numero de bispos que domina, e quantos conjuntos minimos
    /// existem. Varredura completa dos subconjuntos, entao para em n = 6.
    /// </summary>
    public static IReadOnlyList<(int N, int Minimo, long Quantos)> Dominacao(int ate = 6)
    {
        var saida = new List<(int, int, long)>();
        for (int n = 1; n <= ate; n++)
        {
            var (m, q) = Busca.Dominacao(n);
            saida.Add((n, m, q));
        }
        return saida;
    }

    /// <summary>
    /// A cobertura por diagonais (usada pela busca) contra a definicao casa a
    /// casa (usada pelo juiz), em todo subconjunto de todo tabuleiro ate n.
    /// </summary>
    public static (long Conferidos, long Divergem) CobreContraDomina(int ate = 4)
    {
        long conferidos = 0, divergem = 0;
        for (int n = 1; n <= ate; n++)
        {
            int casas = n * n;
            for (int mascara = 0; mascara < (1 << casas); mascara++)
            {
                var bispos = new List<(int, int)>();
                for (int i = 0; i < casas; i++)
                    if ((mascara & (1 << i)) != 0) bispos.Add(Nucleo.Casa(i, n));
                conferidos++;
                if (Busca.Cobre(bispos, n) != Nucleo.Domina(bispos, n)) divergem++;
            }
        }
        return (conferidos, divergem);
    }

    private static readonly Dictionary<int, IReadOnlyList<(int N, long Nos, long Subconjuntos, double Razao)>> CachePreco = [];

    /// <summary>
    /// Quantos nos a busca visita contra quantos subconjuntos o juiz teria de
    /// olhar. Guardado: tres testes pedem a mesma tabela, e cada chamada
    /// refazia os 82 milhoes de nos do n = 8.
    /// </summary>
    public static IReadOnlyList<(int N, long Nos, long Subconjuntos, double Razao)> Preco(int ate = 8)
    {
        lock (CachePreco)
        {
            if (CachePreco.TryGetValue(ate, out var pronto)) return pronto;
            var feito = CalculaPreco(ate);
            CachePreco[ate] = feito;
            return feito;
        }
    }

    private static IReadOnlyList<(int N, long Nos, long Subconjuntos, double Razao)> CalculaPreco(int ate)
    {
        var saida = new List<(int, long, long, double)>();
        for (int n = 1; n <= ate; n++)
        {
            Busca.Conta(n);
            long nos = Busca.UltimoNumeroDeNos;
            long subconjuntos = 0;
            for (int k = 0; k <= Math.Min(2 * n - 1, n * n); k++)
                subconjuntos += Binomial(n * n, k);
            saida.Add((n, nos, subconjuntos, (double)subconjuntos / nos));
        }
        return saida;
    }

    /// <summary>
    /// As duas cores, lado a lado. A pergunta e se elas sao sempre iguais:
    /// nao sao. Em n par os dois tabuleiros de torres dao a mesma conta, em
    /// n impar nao, e e so olhar os tamanhos das diagonais para ver por que.
    /// </summary>
    public static IReadOnlyList<(int N, bool Iguais, long TotalPar, long TotalImpar)> Cores(int ate = 11)
    {
        var saida = new List<(int, bool, long, long)>();
        for (int n = 1; n <= ate; n++)
        {
            var (par, impar) = Torres.PorCor(n);
            // os dois vetores tem comprimentos diferentes (a cor par tem uma
            // diagonal a mais), entao a comparacao e por k, com zero no fim.
            int ate2 = Math.Max(par.Length, impar.Length);
            bool iguais = true;
            for (int k = 0; k < ate2; k++)
            {
                long a = k < par.Length ? par[k] : 0;
                long b = k < impar.Length ? impar[k] : 0;
                if (a != b) { iguais = false; break; }
            }
            saida.Add((n, iguais, par.Sum(), impar.Sum()));
        }
        return saida;
    }

    /// <summary>C(n, k) em inteiro longo, sem fatorial.</summary>
    public static long Binomial(int n, int k)
    {
        if (k < 0 || k > n) return 0;
        k = Math.Min(k, n - k);
        long r = 1;
        for (int i = 1; i <= k; i++)
            r = r * (n - k + i) / i;
        return r;
    }
}

/// <summary>Uma linha da tabela principal.</summary>
public sealed record Linha(int N, long Total, int Maximo, long QuantosMaximos,
                           long DoisElevadoN, long[] PorTamanho);
