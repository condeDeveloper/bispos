using Conde.Bispos;
using Xunit;

namespace Conde.Bispos.Testes;

/// <summary>
/// Tres caminhos para o mesmo numero, e nenhum deles compartilha codigo com o
/// outro: o juiz que olha subconjunto por subconjunto, a busca que poda por
/// diagonal, e a conta das torres, que nao olha o tabuleiro uma vez sequer.
/// </summary>
public class JuizTestes
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    // o n = 5 nao esta aqui de proposito: ele custa 3.850.756 subconjuntos e
    // ja esta preso uma vez em MedidasTestes. Rodar duas vezes dobraria o
    // tempo da bateria sem conferir nada a mais.
    public void O_juiz_por_subconjunto_concorda_com_a_busca(int n)
    {
        var busca = Busca.Conta(n);
        for (int k = 0; k <= Math.Min(2 * n - 1, n * n); k++)
            Assert.Equal(Juiz.ContaIndependentes(n, k), busca[k]);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void As_torres_concordam_com_a_busca_em_todo_k(int n)
    {
        var busca = Busca.Conta(n);
        var torres = Torres.Conta(n);
        Assert.Equal(busca.Length, torres.Length);
        for (int k = 0; k < busca.Length; k++)
            Assert.Equal(busca[k], torres[k]);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void A_cobertura_por_diagonal_e_a_definicao_casa_a_casa_dao_no_mesmo(int n)
    {
        int casas = n * n;
        for (int mascara = 0; mascara < (1 << casas); mascara++)
        {
            var bispos = new List<(int, int)>();
            for (int i = 0; i < casas; i++)
                if ((mascara & (1 << i)) != 0) bispos.Add(Nucleo.Casa(i, n));
            Assert.Equal(Nucleo.Domina(bispos, n), Busca.Cobre(bispos, n));
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void O_minimo_para_dominar_sai_igual_pelo_juiz_e_pela_busca(int n)
    {
        Assert.Equal(Juiz.MenorDominante(n), Busca.Dominacao(n).Minimo);
    }

    [Fact]
    public void Os_tamanhos_das_diagonais_sobem_e_descem()
    {
        Assert.Equal(new[] { 1, 2, 3, 2, 1 }, Torres.TamanhosDasDiagonais(3));
        Assert.Equal(new[] { 1, 2, 3, 4, 3, 2, 1 }, Torres.TamanhosDasDiagonais(4));
        Assert.Equal(16, Torres.TamanhosDasDiagonais(4).Sum());
        Assert.Equal(64, Torres.TamanhosDasDiagonais(8).Sum());
    }

    [Fact]
    public void Num_tabuleiro_encaixado_a_conta_das_torres_bate_na_mao()
    {
        // linhas de tamanho 1 e 2, encaixadas: uma torre cabe de 3 jeitos,
        // duas cabem de 1 (a de tamanho 1 primeiro, e sobra 1 coluna).
        Assert.Equal(new long[] { 1, 3, 1 }, Torres.ContaEncaixado(new[] { 1, 2 }));
        Assert.Equal(new long[] { 1, 1 }, Torres.ContaEncaixado(new[] { 1 }));
    }

    [Fact]
    public void Os_conjuntos_maximos_listados_sao_mesmo_independentes()
    {
        for (int n = 1; n <= 6; n++)
            foreach (var conjunto in Busca.Maximos(n))
            {
                var casas = conjunto.Select(i => Nucleo.Casa(i, n)).ToList();
                Assert.True(Nucleo.EhIndependente(casas));
                Assert.Equal(Busca.Maximo(n), casas.Count);
            }
    }
}
