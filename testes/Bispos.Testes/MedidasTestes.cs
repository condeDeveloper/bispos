using Conde.Bispos;
using Xunit;

namespace Conde.Bispos.Testes;

/// <summary>
/// Todo numero que aparece no README esta preso aqui.
/// </summary>
public class MedidasTestes
{
    // n, total de conjuntos independentes, maximo, conjuntos maximos
    public static TheoryData<int, long, int, long> PelaBusca() => new()
    {
        { 1, 2, 1, 1 },
        { 2, 9, 2, 4 },
        { 3, 70, 4, 8 },
        { 4, 729, 6, 16 },
        { 5, 9_918, 8, 32 },
        { 6, 167_281, 10, 64 },
        { 7, 3_423_362, 12, 128 },
        { 8, 82_609_921, 14, 256 },
    };

    [Theory]
    [MemberData(nameof(PelaBusca))]
    public void A_tabela_da_busca(int n, long total, int maximo, long maximos)
    {
        var l = Medidas.Tabela(8).Single(x => x.N == n);
        Assert.Equal(total, l.Total);
        Assert.Equal(maximo, l.Maximo);
        Assert.Equal(maximos, l.QuantosMaximos);
    }

    // n, total, maximo - onde so a rota das torres chega
    public static TheoryData<int, long, int> PelasTorres() => new()
    {
        { 9, 2_319_730_026, 16 },
        { 10, 74_500_064_809, 18 },
        { 11, 2_711_723_081_550, 20 },
    };

    [Theory]
    [MemberData(nameof(PelasTorres))]
    public void A_tabela_das_torres_vai_alem_da_busca(int n, long total, int maximo)
    {
        var (_, t, m) = Medidas.PelasTorres(11).Single(x => x.N == n);
        Assert.Equal(total, t);
        Assert.Equal(maximo, m);
    }

    [Fact]
    public void As_duas_rotas_nao_divergem_em_212_celulas()
    {
        Assert.Equal((212, 0), Medidas.BuscaContraTorres(8));
    }

    [Fact]
    public void O_juiz_por_subconjunto_nao_diverge_em_30_celulas()
    {
        Assert.Equal((30, 0), Medidas.JuizContraBusca(5));
    }

    [Fact]
    public void O_maximo_e_sempre_2n_menos_2_a_partir_de_dois()
    {
        foreach (var (n, maximo, _, _, _) in Medidas.Maximos(11))
            if (n >= 2) Assert.Equal(2 * n - 2, maximo);
    }

    [Fact]
    public void Os_conjuntos_maximos_sao_2_elevado_a_n_de_2_ate_11_e_so_o_n_igual_a_1_escapa()
    {
        var m = Medidas.Maximos(11);
        Assert.Equal(11, m.Count);
        Assert.Equal(10, m.Count(x => x.Bate));
        var fora = m.Single(x => !x.Bate);
        Assert.Equal(1, fora.N);
        Assert.Equal(1, fora.Quantos);
        Assert.Equal(2, fora.DoisElevadoN);
    }

    [Fact]
    public void Os_conjuntos_maximos_contados_e_listados_dao_o_mesmo()
    {
        foreach (var (n, listados) in Medidas.MaximosListados(7))
            Assert.Equal(Medidas.Maximos(11).Single(x => x.N == n).Quantos, listados);
    }

    // n, minimo para dominar, quantos conjuntos minimos
    public static TheoryData<int, int, long> ADominacao() => new()
    {
        { 1, 1, 1 },
        { 2, 2, 4 },
        { 3, 3, 6 },
        { 4, 4, 25 },
        { 5, 5, 104 },
        { 6, 6, 484 },
    };

    [Theory]
    [MemberData(nameof(ADominacao))]
    public void A_dominacao_minima_e_n_e_os_conjuntos_minimos_sao_estes(int n, int minimo, long quantos)
    {
        var (_, m, q) = Medidas.Dominacao(6).Single(x => x.N == n);
        Assert.Equal(minimo, m);
        Assert.Equal(n, m);
        Assert.Equal(quantos, q);
    }

    [Fact]
    public void A_cobertura_por_diagonal_nao_diverge_em_66066_subconjuntos()
    {
        Assert.Equal((66_066L, 0L), Medidas.CobreContraDomina(4));
    }

    [Fact]
    public void As_duas_cores_sao_iguais_exatamente_quando_n_e_par()
    {
        foreach (var (n, iguais, _, _) in Medidas.Cores(11))
            Assert.Equal(n % 2 == 0, iguais);
    }

    [Fact]
    public void Em_n_par_as_cores_tem_o_mesmo_total_e_em_n_impar_a_par_ganha()
    {
        foreach (var (n, _, totalPar, totalImpar) in Medidas.Cores(11))
            if (n % 2 == 0) Assert.Equal(totalPar, totalImpar);
            else Assert.True(totalPar > totalImpar);
    }

    [Fact]
    public void Os_totais_por_cor_em_oito_e_em_nove()
    {
        var oito = Medidas.Cores(11).Single(x => x.N == 8);
        Assert.Equal(9_089, oito.TotalPar);
        Assert.Equal(9_089, oito.TotalImpar);
        var nove = Medidas.Cores(11).Single(x => x.N == 9);
        Assert.Equal(52_922, nove.TotalPar);
        Assert.Equal(43_833, nove.TotalImpar);
    }

    // n, nos da busca, subconjuntos que o juiz teria de olhar
    public static TheoryData<int, long, long> OPreco() => new()
    {
        { 1, 2, 2 },
        { 2, 9, 15 },
        { 3, 70, 382 },
        { 4, 729, 26_333 },
        { 5, 9_918, 3_850_756 },
        { 6, 167_281, 990_134_948 },
        { 7, 3_423_362, 394_821_713_910 },
        { 8, 82_609_921, 224_723_513_577_529 },
    };

    [Theory]
    [MemberData(nameof(OPreco))]
    public void O_preco_de_nao_podar(int n, long nos, long subconjuntos)
    {
        var (_, medidoNos, medidoSub, _) = Medidas.Preco(8).Single(x => x.N == n);
        Assert.Equal(nos, medidoNos);
        Assert.Equal(subconjuntos, medidoSub);
    }

    [Fact]
    public void Em_oito_por_oito_o_juiz_pagaria_mais_de_dois_milhoes_de_vezes_mais()
    {
        var oito = Medidas.Preco(8).Single(x => x.N == 8);
        Assert.True(oito.Razao > 2_700_000);
        Assert.True(oito.Razao < 2_800_000);
    }

    [Fact]
    public void O_numero_de_nos_da_busca_e_o_proprio_total_de_conjuntos()
    {
        // a busca visita um no por conjunto independente, nem um a mais:
        // e o que torna a razao contra o juiz uma medida honesta.
        foreach (var (n, nos, _, _) in Medidas.Preco(8))
            Assert.Equal(Medidas.Tabela(8).Single(x => x.N == n).Total, nos);
    }

    [Fact]
    public void O_binomial_bate_nos_casos_sabidos()
    {
        Assert.Equal(1, Medidas.Binomial(10, 0));
        Assert.Equal(10, Medidas.Binomial(10, 1));
        Assert.Equal(252, Medidas.Binomial(10, 5));
        Assert.Equal(0, Medidas.Binomial(10, 11));
        Assert.Equal(4_426_165_368, Medidas.Binomial(64, 8));
    }
}
