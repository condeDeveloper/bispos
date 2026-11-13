using Conde.Bispos;

if (args.Length == 0)
{
    Console.WriteLine("medidor <tabela|torres|juiz|maximos|dominacao|cobertura|cores|preco|tudo>");
    return 1;
}

switch (args[0])
{
    case "tabela": Tabela(); break;
    case "torres": TorresContraBusca(); break;
    case "juiz": JuizContraBusca(); break;
    case "maximos": Maximos(); break;
    case "dominacao": Dominacao(); break;
    case "cobertura": Cobertura(); break;
    case "cores": Cores(); break;
    case "preco": Preco(); break;
    case "tudo":
        Tabela(); TorresContraBusca(); JuizContraBusca(); Maximos();
        Dominacao(); Cobertura(); Cores(); Preco();
        break;
    default:
        Console.WriteLine($"nao conheco '{args[0]}'");
        return 1;
}
return 0;

void Tabela()
{
    Console.WriteLine("pela busca, um no por conjunto:");
    Console.WriteLine("n   conjuntos independentes   maximo   conjuntos maximos   2^n");
    foreach (var l in Medidas.Tabela(8))
        Console.WriteLine($"{l.N,-3} {l.Total,23:n0}   {l.Maximo,6}   {l.QuantosMaximos,17:n0}   {l.DoisElevadoN,5:n0}");
    Console.WriteLine();
    Console.WriteLine("pelas torres, que nao enumeram nada:");
    Console.WriteLine("n   conjuntos independentes   maximo");
    foreach (var (n, total, maximo) in Medidas.PelasTorres(11))
        Console.WriteLine($"{n,-3} {total,23:n0}   {maximo,6}");
}

void TorresContraBusca()
{
    var (conf, div) = Medidas.BuscaContraTorres(8);
    Console.WriteLine($"busca contra torres: {conf} celulas conferidas, {div} divergem");
    Console.WriteLine();
    Console.WriteLine("a conta de cada cor, e o produto (n = 8):");
    var (par, impar) = Torres.PorCor(8);
    Console.WriteLine("k    cor par        cor impar");
    for (int k = 0; k < Math.Max(par.Length, impar.Length) && k <= 8; k++)
        Console.WriteLine($"{k,-4} {(k < par.Length ? par[k] : 0),13:n0}  {(k < impar.Length ? impar[k] : 0),13:n0}");
}

void JuizContraBusca()
{
    var (conf, div) = Medidas.JuizContraBusca(5);
    Console.WriteLine($"juiz por subconjunto contra busca: {conf} celulas, {div} divergem");
}

void Maximos()
{
    Console.WriteLine("n   maximo   conjuntos maximos   2^n   bate");
    foreach (var (n, max, q, dois, bate) in Medidas.Maximos(11))
        Console.WriteLine($"{n,-3} {max,6}   {q,17:n0}   {dois,5:n0}   {(bate ? "sim" : "NAO")}");
    Console.WriteLine();
    Console.WriteLine("os mesmos conjuntos LISTADOS um a um pela busca:");
    foreach (var (n, q) in Medidas.MaximosListados(7))
        Console.WriteLine($"n = {n}: {q} conjuntos maximos listados");
}

void Dominacao()
{
    Console.WriteLine("n   minimo para dominar   conjuntos minimos");
    foreach (var (n, m, q) in Medidas.Dominacao(6))
        Console.WriteLine($"{n,-3} {m,19}   {q,17:n0}");
}

void Cobertura()
{
    var (conf, div) = Medidas.CobreContraDomina(4);
    Console.WriteLine($"cobertura por diagonal contra casa a casa: {conf:n0} subconjuntos, {div} divergem");
}

void Preco()
{
    Console.WriteLine("n   nos da busca   subconjuntos do juiz   razao");
    foreach (var (n, nos, sub, razao) in Medidas.Preco(8))
        Console.WriteLine($"{n,-3} {nos,14:n0}   {sub,20:n0}   {razao,8:n1}");
}

void Cores()
{
    Console.WriteLine("n   cores iguais   total cor par   total cor impar");
    foreach (var (n, iguais, a, b) in Medidas.Cores(11))
        Console.WriteLine($"{n,-3} {(iguais ? "sim" : "nao"),12}   {a,13:n0}   {b,15:n0}");
}
