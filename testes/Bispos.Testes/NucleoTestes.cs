using Conde.Bispos;
using Xunit;

namespace Conde.Bispos.Testes;

/// <summary>Casos pequenos, conferidos na mao, pela definicao crua.</summary>
public class NucleoTestes
{
    [Fact]
    public void Dois_bispos_se_atacam_quando_a_distancia_e_a_mesma_nos_dois_eixos()
    {
        Assert.True(Nucleo.Ataca((0, 0), (3, 3)));
        Assert.True(Nucleo.Ataca((2, 5), (5, 2)));
        Assert.False(Nucleo.Ataca((0, 0), (3, 2)));
        Assert.False(Nucleo.Ataca((0, 0), (0, 4)));
    }

    [Fact]
    public void Um_bispo_nao_ataca_a_propria_casa()
    {
        Assert.False(Nucleo.Ataca((4, 4), (4, 4)));
    }

    [Fact]
    public void As_duas_diagonais_dao_a_casa_de_volta()
    {
        const int n = 8;
        var vistas = new HashSet<(int, int)>();
        for (int l = 0; l < n; l++)
            for (int c = 0; c < n; c++)
                Assert.True(vistas.Add((Nucleo.Sobe(l, c), Nucleo.Desce(l, c, n))));
        Assert.Equal(n * n, vistas.Count);
    }

    [Fact]
    public void Atacar_e_o_mesmo_que_dividir_uma_das_diagonais()
    {
        const int n = 7;
        for (int i = 0; i < n * n; i++)
            for (int j = 0; j < n * n; j++)
            {
                var a = Nucleo.Casa(i, n);
                var b = Nucleo.Casa(j, n);
                bool mesmaDiagonal = Nucleo.Sobe(a.Linha, a.Coluna) == Nucleo.Sobe(b.Linha, b.Coluna)
                                  || Nucleo.Desce(a.Linha, a.Coluna, n) == Nucleo.Desce(b.Linha, b.Coluna, n);
                Assert.Equal(mesmaDiagonal && i != j, Nucleo.Ataca(a, b));
            }
    }

    [Fact]
    public void Um_bispo_na_ponta_nao_domina_um_tabuleiro_tres_por_tres()
    {
        Assert.False(Nucleo.Domina(new[] { (0, 0) }, 3));
        Assert.False(Nucleo.Domina(new[] { (1, 1) }, 3));
        Assert.True(Nucleo.Domina(new[] { (0, 1), (1, 1), (2, 1) }, 3));
    }

    [Fact]
    public void Casa_e_indice_sao_o_mesmo_objeto_nos_dois_sentidos()
    {
        for (int n = 1; n <= 9; n++)
            for (int i = 0; i < n * n; i++)
            {
                var (l, c) = Nucleo.Casa(i, n);
                Assert.Equal(i, Nucleo.Indice(l, c, n));
            }
    }

    [Fact]
    public void A_cor_e_a_paridade_da_diagonal_que_sobe()
    {
        Assert.Equal(0, Nucleo.Cor(0, 0));
        Assert.Equal(1, Nucleo.Cor(0, 1));
        Assert.Equal(0, Nucleo.Cor(1, 1));
    }
}
