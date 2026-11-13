namespace Conde.Bispos;

/// <summary>
/// Bispos num tabuleiro n por n. Dois bispos se atacam quando estao na mesma
/// diagonal, e a definicao usada aqui e a crua: |linha1 - linha2| igual a
/// |coluna1 - coluna2|. Nada de diagonal pre-calculada neste arquivo.
/// </summary>
public static class Nucleo
{
    /// <summary>A casa de indice i num tabuleiro n por n.</summary>
    public static (int Linha, int Coluna) Casa(int i, int n) => (i / n, i % n);

    /// <summary>O indice da casa.</summary>
    public static int Indice(int linha, int coluna, int n) => linha * n + coluna;

    /// <summary>Dois bispos se atacam? Pela definicao, sem atalho.</summary>
    public static bool Ataca((int Linha, int Coluna) a, (int Linha, int Coluna) b)
        => Math.Abs(a.Linha - b.Linha) == Math.Abs(a.Coluna - b.Coluna)
           && (a.Linha != b.Linha || a.Coluna != b.Coluna);

    /// <summary>Nenhum par se ataca?</summary>
    public static bool EhIndependente(IReadOnlyList<(int Linha, int Coluna)> bispos)
    {
        for (int i = 0; i < bispos.Count; i++)
            for (int j = i + 1; j < bispos.Count; j++)
                if (Ataca(bispos[i], bispos[j])) return false;
        return true;
    }

    /// <summary>
    /// Toda casa do tabuleiro esta ocupada ou e atacada por algum bispo?
    /// Tambem pela definicao, casa por casa.
    /// </summary>
    public static bool Domina(IReadOnlyList<(int Linha, int Coluna)> bispos, int n)
    {
        for (int l = 0; l < n; l++)
            for (int c = 0; c < n; c++)
            {
                bool coberta = false;
                foreach (var b in bispos)
                    if ((b.Linha == l && b.Coluna == c) || Ataca(b, (l, c)))
                    {
                        coberta = true;
                        break;
                    }
                if (!coberta) return false;
            }
        return true;
    }

    /// <summary>A diagonal que sobe: linha + coluna, de 0 a 2n-2.</summary>
    public static int Sobe(int linha, int coluna) => linha + coluna;

    /// <summary>A diagonal que desce: linha - coluna + n - 1, de 0 a 2n-2.</summary>
    public static int Desce(int linha, int coluna, int n) => linha - coluna + n - 1;

    /// <summary>
    /// A cor da casa, que e a paridade da diagonal que sobe. As duas cores sao
    /// dois problemas separados: um bispo numa cor nunca ve a outra.
    /// </summary>
    public static int Cor(int linha, int coluna) => (linha + coluna) % 2;
}
