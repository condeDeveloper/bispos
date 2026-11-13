# Bispos no tabuleiro

Quantos bispos cabem num tabuleiro n por n sem que nenhum ataque o outro? E de
quantos jeitos? E quantos bastam para que **toda** casa fique atacada?

Este repositório responde as três por varredura completa, e confere cada
resposta por três caminhos que não compartilham uma linha de código.

## Como rodar

```bash
dotnet test testes/Bispos.Testes/Bispos.Testes.csproj -c Release
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- tabela
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- torres
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- juiz
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- maximos
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- dominacao
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- cobertura
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- cores
dotnet run --project ferramentas/Medidor/Medidor.csproj -c Release -- preco
```

## Os três caminhos

1. **O juiz** (`Juiz.cs`) enumera *todos* os subconjuntos de casas e testa cada
   um pela definição: `|Δlinha| == |Δcoluna|`. Não sabe o que é diagonal, não
   poda nada, e só chega ao tabuleiro 5 por 5 — e é exatamente por isso que
   serve.
2. **A busca** (`Busca.cs`) anda pelas casas em ordem, com as duas diagonais
   ocupadas guardadas em máscaras de bits. Visita um nó por conjunto
   independente, nem um a mais.
3. **As torres** (`Torres.cs`) não olham o tabuleiro uma vez sequer. Girando
   tudo 45 graus, a diagonal que sobe vira linha, a que desce vira coluna, e o
   bispo vira uma **torre**. Como bispo nunca muda de cor, o tabuleiro se parte
   em dois tabuleiros de torres com linhas encaixadas, e a contagem sai de uma
   recorrência de três linhas.

## A varredura

```
pela busca, um no por conjunto:
n   conjuntos independentes   maximo   conjuntos maximos   2^n
1                         2        1                   1       2
2                         9        2                   4       4
3                        70        4                   8       8
4                       729        6                  16      16
5                     9.918        8                  32      32
6                   167.281       10                  64      64
7                 3.423.362       12                 128     128
8                82.609.921       14                 256     256

pelas torres, que nao enumeram nada:
9             2.319.730.026       16
10           74.500.064.809       18
11        2.711.723.081.550       20
```

## O que isto mediu

**A contagem fatora exatamente no produto de dois problemas de torres.** As
duas cores do tabuleiro são dois problemas separados — um bispo numa cor nunca
vê a outra — e multiplicar as duas contagens dá o número certo em **212
células conferidas** (n de 1 a 8, todo k), com **zero divergências**. É por
isso que a rota das torres chega onde a busca não chega: os
2.711.723.081.550 conjuntos de um tabuleiro 11 por 11 saem sem enumerar nem um.

**O número de conjuntos de tamanho máximo é exatamente 2^n — e o único n que
escapa é o 1.** Para n de 2 a 11, com máximo em `2n - 2`, a contagem é
`2^n` cravado: 4, 8, 16, 32, 64, 128, 256, 512, 1.024, 2.048. Em n = 1 o máximo
é 1 bispo e existe **1** conjunto, não 2. Dez de onze.

**As duas cores dão a mesma contagem exatamente quando n é par.** Em 8 por 8,
cada cor tem 9.089 conjuntos independentes e as duas listas coincidem em todo
k. Em 9 por 9 são 52.922 contra 43.833 — e a cor par ganha em todos os ímpares
medidos. O motivo está nos tamanhos das diagonais: em n ímpar a diagonal maior
cai na cor par, e a outra cor nunca alcança.

**Para dominar o tabuleiro inteiro bastam n bispos, e os conjuntos mínimos são
poucos.**

```
n   minimo para dominar   conjuntos minimos
1                     1                   1
2                     2                   4
3                     3                   6
4                     4                  25
5                     5                 104
6                     6                 484
```

A varredura é por subconjunto, então ela para em 6: em 7 por 7 seriam 85
milhões de conjuntos de sete casas. O mínimo foi conferido contra o juiz, que
testa casa por casa, até n = 4.

**O preço de não saber podar.** A busca visita um nó por conjunto; o juiz teria
de olhar todo subconjunto de tamanho até `2n - 1`:

```
n   nos da busca   subconjuntos do juiz        razao
5          9.918             3.850.756          388,3
6        167.281           990.134.948        5.919,0
7      3.423.362       394.821.713.910      115.331,6
8     82.609.921   224.723.513.577.529    2.720.296,9
```

## O que as medidas me corrigiram

**Comparei as duas cores com uma igualdade de vetor e deu "nunca iguais" —
em todos os onze.** Os dois vetores têm comprimentos diferentes: a cor par tem
uma diagonal a mais que a ímpar, sempre. Comparados termo a termo, com zero no
fim, a lei aparece inteira: iguais exatamente quando n é par. Um teste escrito
em cima da primeira medida ("as cores nunca batem") teria passado, ficado
verde para sempre e enterrado o achado.

**Apostei que os conjuntos máximos fossem 2^n para todo n.** Escrevi a aposta
antes de rodar. Em n = 1 são 1 contra 2. O teste guarda as duas coisas: a lei
de n = 2 a 11, e o fato de que ela tem exatamente uma exceção, com os números
dela.

**Pedi uma tabela que não terminava.** A primeira versão ia até 10 por 10 pela
busca. Como a busca visita um nó por conjunto, isso são 74,5 bilhões de nós —
o comando ficou dois minutos sem imprimir a primeira linha. A tabela da busca
foi cortada em 8, com o motivo escrito, e quem leva as linhas até 11 é a rota
das torres, que não enumera nada.

**A bateria levava 1 minuto e 47 e passou a levar 18 segundos.** Dois
desperdícios: o caso n = 5 do juiz (3.850.756 subconjuntos) rodava uma vez no
teste por teoria e outra na tabela de medidas, e a tabela de preço refazia os
82 milhões de nós do n = 8 a cada uma das três chamadas. Teste pesado não é só
tempo: é exposição a cancelamento de runner.

## Os arquivos

```
src/Bispos/Nucleo.cs     a definicao crua de ataque e de dominacao
src/Bispos/Juiz.cs       todo subconjunto, testado pela definicao
src/Bispos/Busca.cs      a varredura com as duas diagonais em mascara
src/Bispos/Torres.cs     o tabuleiro girado 45 graus, sem enumerar nada
src/Bispos/Medidas.cs    as tabelas deste README
ferramentas/Medidor/     o executavel que imprime as tabelas
testes/                  67 testes, com os numeros daqui presos um a um
```

67 testes.
