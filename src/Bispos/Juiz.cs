namespace Conde.Bispos;

/// <summary>
/// O juiz enumera TODOS os subconjuntos de casas e testa cada um pela
/// definicao do <see cref="Nucleo"/>. Ele nao sabe o que e diagonal, nao sabe
/// que as cores se separam e nao poda nada. So serve para tabuleiro pequeno,
/// e e exatamente por isso que serve: e o unico caminho que nao usa nenhuma
/// das ideias que a busca usa.
/// </summary>
public static class Juiz
{
    /// <summary>
    /// Quantos conjuntos de k bispos independentes existem num n por n,
    /// contados subconjunto por subconjunto.
    /// </summary>
    public static long ContaIndependentes(int n, int k)
    {
        int casas = n * n;
        if (k > casas) return 0;
        long achados = 0;
        var escolha = new (int, int)[k];

        void Anda(int inicio, int quantos)
        {
            if (quantos == k)
            {
                if (Nucleo.EhIndependente(escolha)) achados++;
                return;
            }
            for (int i = inicio; i < casas; i++)
            {
                escolha[quantos] = Nucleo.Casa(i, n);
                Anda(i + 1, quantos + 1);
            }
        }

        Anda(0, 0);
        return achados;
    }

    /// <summary>
    /// Quantos conjuntos de k bispos dominam o tabuleiro inteiro, contados
    /// subconjunto por subconjunto, cada um testado casa por casa.
    /// </summary>
    public static long ContaDominantes(int n, int k)
    {
        int casas = n * n;
        if (k > casas) return 0;
        long achados = 0;
        var escolha = new (int, int)[k];

        void Anda(int inicio, int quantos)
        {
            if (quantos == k)
            {
                if (Nucleo.Domina(escolha, n)) achados++;
                return;
            }
            for (int i = inicio; i < casas; i++)
            {
                escolha[quantos] = Nucleo.Casa(i, n);
                Anda(i + 1, quantos + 1);
            }
        }

        Anda(0, 0);
        return achados;
    }

    /// <summary>O menor k com algum conjunto dominante, pela forca bruta.</summary>
    public static int MenorDominante(int n)
    {
        for (int k = 0; k <= n * n; k++)
            if (ContaDominantes(n, k) > 0) return k;
        throw new InvalidOperationException("o tabuleiro inteiro sempre domina");
    }
}
