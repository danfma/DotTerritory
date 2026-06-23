# Reorganização da Estrutura do Projeto DotTerritory

Reorganizar o repositório para a convenção .NET padrão (`.sln` no root; `src/`,
`tests/`, `samples/`, `benchmark/` no root), eliminar pastas órfãs/lixo, e migrar
toda a suíte de testes de **xUnit + Shouldly** para **TUnit** (asserções nativas),
consolidando os 13 testes órfãos do root no projeto de testes único.

## Contexto e Decisões (confirmadas com o usuário)

Estado inicial relevante (capturado em 2026-06-23, branch `main`, working tree limpo):

- `.sln`, `dotnet-releaser.toml` e `CLAUDE.md` estão em `src/` (deveriam estar no root).
- Projeto de testes **real** (no `.sln`): `src/tests/DotTerritory.Tests/` — 21 arquivos, **xUnit `[Fact]/[Theory]` + Shouldly**. `FluentAssertions` está no `.csproj` mas **não é usado** em nenhum código.
- `tests/DotTerritory.Tests/` (root): **13 testes órfãos** SEM `.csproj`, fora do build. Cobrem operações que o projeto real **não** cobre (BooleanClockwise, BooleanParallel, Envelope, Explode, LineIntersect, LineSliceAlong, PointOnFeature, PointToLineDistance, PointToPolygonDistance, RhumbDestination, RhumbDistance, Simplify, Square). Testado: **8 compilam** contra a API atual; **5 estão obsoletos** — `ExplodeTests.cs`, `LineIntersectTests.cs`, `LineSliceAlongTests.cs`, `PointOnFeatureTests.cs`, `SimplifyTests.cs` (assinaturas mudaram: `Simplify` ambíguo, `GetCoordinateN` removido, `Distance` sem overload de 3 args, `PointOnFeature` espera `Point[]` e não `Coordinate[]`).
- `src/DotTerritory.GeoJson/`: 7 arquivos tracked, **sem `.csproj`, fora do `.sln`**.
- `src/src/DotTerritory/`: apenas `bin/obj` locais — **lixo de build, nada tracked**.
- Ruído local (gitignored, não tracked): `src/.idea/`, `.DS_Store` espalhados, `src/DotTerritory.sln.DotSettings.user`.

Decisões do usuário:

1. **Layout**: `src/` só com bibliotecas publicáveis; `samples/`, `benchmark/`, `tests/` em pastas próprias no root; `.sln` no root.
2. **Testes órfãos**: mesclar **todos os 13** (corrigindo os 5 obsoletos contra a API atual).
3. **DotTerritory.GeoJson**: **remover**.
4. **Configs**: mover `dotnet-releaser.toml` **e** `CLAUDE.md` para o root.
5. **Framework de testes**: migrar para **TUnit** (estável atual: **1.56.25**), asserções **nativas async** (`await Assert.That(...)`), **remover Shouldly**.
6. **FluentAssertions**: **remover** do `.csproj` (não usado; v8.x é licença comercial paga).

Estrutura-alvo:

```
/
├── DotTerritory.sln
├── dotnet-releaser.toml
├── CLAUDE.md
├── src/
│   └── DotTerritory/
├── samples/
│   └── DotTerritory.Playground/
├── benchmark/
│   └── TimeAndMemoryUsage/
└── tests/
    └── DotTerritory.Tests/   (21 + 13 testes, todos em TUnit)
```

Fatos técnicos úteis para a execução:

- Todos os `ProjectReference` hoje apontam para `..\..\DotTerritory\DotTerritory.csproj`. Como samples/benchmark/tests sobem para o root e `DotTerritory` permanece em `src/`, o novo caminho relativo (mesma profundidade) passa a ser `..\..\src\DotTerritory\DotTerritory.csproj` para os três.
- No `.sln` (movido para o root): apenas a linha do projeto `DotTerritory` muda para `src\DotTerritory\DotTerritory.csproj`. As entradas de `samples`, `tests` e `benchmark` são *solution folders* virtuais + projetos cujos caminhos relativos ao root permanecem iguais.
- `dotnet-releaser.toml` usa `project = "DotTerritory.sln"` (relativo ao próprio `.toml`) — continua válido com ambos no root.
- CI (`.github/workflows/publish.yml`) referencia `src/DotTerritory.sln` e `src/dotnet-releaser.toml` — precisa atualizar para os caminhos no root.
- Formatação: `csharpier` 0.30.6 é tool local — comando `dotnet csharpier .` (ou `dotnet dotnet-csharpier .`). Rodar ao final para manter consistência.
- Sempre usar `git mv` / `git rm` para itens tracked (preserva histórico). Lixo não tracked usa `rm -rf` direto.

## For Future Agents
As work proceeds: mark checkboxes `- [x]` as items complete; when a phase is done,
set its status to `Complete` and write its **Phase Summary** (what was done, key
decisions, anything needed to continue with zero context); run the phase's
**Verification Plan** and record the result before moving on. When all phases are
done, fill in **Final Recap** and **Deployment Plan**.

---

## Phase 1: Reestruturação de pastas e arquivos de build (testes ainda em xUnit)
Status: Complete

Objetivo: deixar o repositório na estrutura-alvo, com a solução compilando, **sem
ainda migrar o framework de testes**. Os 13 testes órfãos ficam fisicamente em
`tests/DotTerritory.Tests/` mas são **temporariamente excluídos da compilação**
(via `<Compile Remove>` no `.csproj`) para isolar esta fase.

- [x] Mover, via `git mv`, o conteúdo de `src/tests/DotTerritory.Tests/` (todos os `.cs` + `DotTerritory.Tests.csproj`, exceto `bin/`/`obj/`) para `tests/DotTerritory.Tests/`, coabitando com os 13 órfãos já presentes (sem colisão de nomes: sufixos `Test` vs `Tests`).
- [x] `git mv src/samples` → `samples/` (raiz).
- [x] `git mv src/benchmark` → `benchmark/` (raiz).
- [x] `git mv src/DotTerritory.sln` → `DotTerritory.sln` (raiz).
- [x] `git mv src/dotnet-releaser.toml` → `dotnet-releaser.toml` (raiz).
- [x] `git mv src/CLAUDE.md` → `CLAUDE.md` (raiz).
- [x] `git rm -r src/DotTerritory.GeoJson` (remover projeto órfão).
- [x] Remover lixo não tracked: `rm -rf src/src src/.idea`; remover `src/DotTerritory.sln.DotSettings.user` e os `.DS_Store` (`find . -name .DS_Store -delete`). Também removida `src/tests/` (só restava `bin/obj`).
- [x] Atualizar caminho do projeto `DotTerritory` no `DotTerritory.sln` para `src\DotTerritory\DotTerritory.csproj` (confirmar que samples/tests/benchmark permanecem corretos).
- [x] Atualizar os 3 `ProjectReference` (`samples/DotTerritory.Playground`, `benchmark/TimeAndMemoryUsage`, `tests/DotTerritory.Tests`) de `..\..\DotTerritory\` para `..\..\src\DotTerritory\`.
- [x] No `tests/DotTerritory.Tests/DotTerritory.Tests.csproj`, adicionar bloco temporário excluindo os 13 órfãos da compilação: `<ItemGroup><Compile Remove="BooleanClockwiseTests.cs" />…</ItemGroup>` (os 13 nomes). Marcar com comentário `<!-- TEMP: reativar na Fase 3 -->`.
- [x] Atualizar `.github/workflows/publish.yml`: `src/DotTerritory.sln` → `DotTerritory.sln` e `src/dotnet-releaser.toml` → `dotnet-releaser.toml`.
- [x] Confirmar `dotnet-releaser.toml` (`project = "DotTerritory.sln"`) válido no root.

### Verification Plan
- `dotnet build DotTerritory.sln -c Debug --nologo -v q` → **Build succeeded, 0 Error(s)** (os 21 testes em xUnit + lib + samples + benchmark compilam; órfãos excluídos).
- `test ! -d src/DotTerritory.GeoJson && test ! -d src/src && test ! -d src/tests && test ! -d src/samples && test ! -d src/benchmark && echo OK` → imprime `OK`.
- `test -f DotTerritory.sln && test -f dotnet-releaser.toml && test -f CLAUDE.md && echo OK` → imprime `OK`.
- `ls src/` → contém apenas `DotTerritory/`.
- `grep -c "src/DotTerritory.sln\|src/dotnet-releaser.toml" .github/workflows/publish.yml` → imprime `0`.

### Phase Summary
Concluída em 2026-06-23. Toda a movimentação feita com `git mv`/`git rm` (histórico
preservado). Resultado da verificação: **Build succeeded, 0 Warning(s), 0 Error(s)**;
todas as checagens estruturais imprimiram OK; `src/` contém apenas `DotTerritory/`;
0 referências antigas no CI.

Decisões/observações para continuar:
- A pasta `tests/DotTerritory.Tests/` agora tem 34 `.cs` = 20 testes (ex-`src/tests`)
  + 13 órfãos + `GlobalUsings.cs`, mais o `.csproj`. Não houve colisão de nomes
  (sufixos `Test` vs `Tests`).
- Os 13 órfãos estão **excluídos via `<Compile Remove>`** no `.csproj` (bloco marcado
  `<!-- TEMP: reativar na Fase 3 -->`). O build atual valida apenas os 20 testes
  herdados, ainda em **xUnit + Shouldly**.
- `.sln`: apenas a linha do projeto `DotTerritory` mudou (`src\DotTerritory\...`);
  solution folders `samples`/`tests`/`benchmark` e seus projetos mantêm caminho
  relativo ao root.
- `dotnet-releaser.toml` (`project = "DotTerritory.sln"`) funciona no root sem
  alteração de conteúdo.
- Nenhum commit feito ainda (aguardando conclusão das fases ou instrução do usuário).

---

## Phase 2: Migração do projeto de testes para TUnit (20 testes existentes)
Status: Complete

Objetivo: converter os 20 arquivos de teste vindos de `src/tests` de xUnit+Shouldly
para TUnit nativo, deixando os 13 órfãos ainda excluídos (Fase 3).

- [x] Reescrever `tests/DotTerritory.Tests/DotTerritory.Tests.csproj`: remover `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `Shouldly`, `FluentAssertions`, `FluentAssertions.Analyzers`, `coverlet.collector`; adicionar `<PackageReference Include="TUnit" Version="1.56.25" />`; definir `<OutputType>Exe</OutputType>`; manter `NetTopologySuite.Features` e `NetTopologySuite.IO.GeoJSON4STJ`. Removido `IsTestProject` (segue template oficial TUnit / Microsoft.Testing.Platform). Mantido o `<Compile Remove>` temporário dos 13 órfãos.
- [x] Atualizar `tests/DotTerritory.Tests/GlobalUsings.cs`: remover `Shouldly`/`Xunit`. TUnit fornece os usings automaticamente (TUnit.Core/Assertions/Extensions) com `ImplicitUsings`. Adicionados globals de domínio (`NetTopologySuite.Features/Geometries`, `UnitsNet`, `UnitsNet.Units`) para que os testes não dependam de usings locais.
- [x] Converter os 20 arquivos de teste: `[Fact]`→`[Test]`; assinaturas `public void`→`public async Task`; asserções Shouldly → `await Assert.That(...)` nativo do TUnit. (Não havia `[Theory]/[InlineData]` em nenhum arquivo.)
- [x] Garantir que nenhum resíduo de `Shouldly`/`Xunit`/`FluentAssertions` permaneça nos 20 arquivos.

### Verification Plan
- `dotnet build DotTerritory.sln -c Debug --nologo -v q` → **Build succeeded, 0 Error(s)**.
- `DOTNET_ROLL_FORWARD=Major dotnet run --project tests/DotTerritory.Tests/DotTerritory.Tests.csproj -c Debug --framework net9.0` → todos **Passed**, 0 Failed.
- `grep -rl "Shouldly\|using Xunit\|FluentAssertions" tests/DotTerritory.Tests/*.cs | grep -v -E "<os 13 órfãos>"` → vazio.

### Phase Summary
Concluída em 2026-06-23. Sintaxe TUnit validada empiricamente em probe isolado
(`scratchpad/tunitprobe`) antes de propagar: `.IsEqualTo(x)`, `.IsEqualTo(x).Within(t)`,
`.IsNotEqualTo`, `.IsTrue/.IsFalse`, `.IsNull/.IsNotNull`, `.IsTypeOf<T>`,
`.IsLessThan(OrEqualTo)`, `.IsGreaterThan(OrEqualTo)`, `.IsBetween(lo,hi)`, `.Contains`.
Conversão dos 18 arquivos restantes feita em paralelo por 5 subagents com guia validado;
os 2 pilotos (`AreaTest`, `BBoxTest`) convertidos manualmente.

**Resultado da verificação: Build succeeded; `Passed! total: 89, failed: 0`.**

Decisões/descobertas para continuar:
- Mapa de conversão Shouldly→TUnit (validado): vide acima. `ShouldSatisfyAllConditions`
  (1 uso em `WalkAlongTest`) → asserções sequenciais. **Caso novo descoberto**:
  `Should.Throw<T>(() => ...)` (Shouldly, 2 usos em `BooleanPointInPolygonTest`) →
  `await Assert.That(() => ...).Throws<T>()` — confirmado pelos testes passando.
- **Execução de testes**: o runtime .NET 9.0 NÃO está instalado nesta máquina (só 10.0 e
  11.0-preview). `dotnet test` via VSTest também não é suportado no SDK 11. Por isso a
  verificação usa `DOTNET_ROLL_FORWARD=Major dotnet run --project … --framework net9.0`.
  No CI isso não é problema (`setup-dotnet` com `9.x` instala o runtime 9.0).
- TUnit roda em **Engine Mode: SourceGenerated**; saída traz "Test run summary: Passed!".
- Os 89 testes correspondem aos 20 arquivos não-órfãos. Os ~83 testes restantes virão
  dos 13 órfãos na Fase 3 (total esperado ao final ≈ 172).

---

## Phase 3: Integração e migração dos 13 testes órfãos para TUnit
Status: Complete

Objetivo: reativar os 13 órfãos no build, corrigir os 5 obsoletos contra a API
atual e converter todos os 13 para TUnit nativo.

- [x] Remover o bloco `<Compile Remove>` temporário do `.csproj` (os órfãos voltam à compilação).
- [x] Corrigir os 5 testes obsoletos contra a API atual do `Territory`/NTS:
  - `SimplifyTests`: `Simplify(x, tol)` ambíguo (2 overloads de 3 params com default) → desambiguado com `highQuality: false`; `Geometry.GetCoordinateN` (inexistente) → `.Coordinates[n]`; `Coordinates.ShouldBe(arr)` → `IsEquivalentTo`.
  - `ExplodeTests`: `Polygon.GetCoordinateN` (inexistente) → `polygon.Coordinates[i]`.
  - `LineSliceAlongTests`: `Distance(c, c, LengthUnit)` → `Distance(c, c)` (API atual tem 2 params).
  - `PointOnFeatureTests`: `CreateMultiPoint(Coordinate[])` → `CreateMultiPointFromCoords` (NTS 2.x).
  - `LineIntersectTests`: `Assert.*` do xUnit convertido; corrigido bug do próprio teste (verificava (2,2) duas vezes).
- [x] Converter os 13 órfãos de xUnit+Shouldly (e `Assert.*` do xUnit) para TUnit nativo. Os 8 que já compilavam via 2 subagents; os 5 obsoletos manualmente.
- [x] Avaliar falhas e aplicar a decisão do usuário (descartar inválidos).

### Verification Plan
- `dotnet build DotTerritory.sln -c Debug --nologo -v q` → **Build succeeded, 0 Error(s)**.
- `DOTNET_ROLL_FORWARD=Major dotnet run --project tests/DotTerritory.Tests/DotTerritory.Tests.csproj -c Debug --framework net9.0` → todos **Passed**, 0 Failed.
- `grep -rln "Shouldly\|using Xunit\|FluentAssertions\|\[Fact\]\|\.ShouldBe\|GetCoordinateN" tests/DotTerritory.Tests/*.cs` → **vazio**.

### Phase Summary
Concluída em 2026-06-23. **Resultado: Build succeeded; `Passed! total: 146, failed: 0, skipped: 0`.**

Descoberta crítica e decisão: ao integrar os 13 órfãos (85 testes), **28 falharam** —
todos eram testes que **nunca rodaram** (estavam sem `.csproj`). Classificação:
- **Cat. A (23) — premissa planar vs cálculo geográfico**: testes assumiam 1° = "1 unidade",
  mas a lib calcula distância geográfica real (1° ≈ 111.195 m). Arquivos: PointToLineDistance,
  LineSliceAlong, PointToPolygonDistance, RhumbDistance, RhumbDestination.
- **Cat. B (2) — teste logicamente incorreto**: `LineIntersect_MultipleLines` (as 3 linhas
  coincidem em (2,2) → 1 interseção); `BooleanParallel_SinglePointLines` (cria `LineString`
  de 1 ponto, rejeitado pelo NTS).
- **Cat. C (3) — PointOnFeature**: investigado empiricamente (probe). Não é bug da lib —
  ponto fica sobre a trajetória **geográfica** (LineString/MultiLineString, ~0.04°/0.12° da
  corda planar) ou na **borda** do polígono (PolygonWithHole: centroid cai no buraco → borda;
  teste usava `Contains`, que exclui borda, em vez de `Covers`).

**Decisão do usuário: descartar todos os 28 inválidos.** Removidos: `LineSliceAlongTests.cs`
inteiro (6), + 19 métodos de 6 arquivos (via subagent), + 3 métodos de `PointOnFeatureTests`.
Os 57 órfãos com expectativa válida foram mantidos e migrados.

**Estado final dos testes: 146 testes, todos verdes** (89 da Fase 2 + 57 órfãos válidos).

Observação de qualidade registrada (NÃO corrigida — fora do escopo): `Territory.PointOnFeature`
usa cálculo geográfico (`WalkAlong`) que desvia do modelo planar do NTS, e pode retornar ponto
na borda (não interior) quando o centroid cai num buraco. Melhoria futura opcional: alinhar ao
TurfJS (garantir ponto sobre a feature no modelo planar / interior de polígonos).

---

## Phase 4: Verificação holística e formatação
Status: Complete

- [x] `dotnet csharpier .` para formatar todo o código alterado (114 arquivos processados; csharpier 0.30.6 usa `dotnet csharpier <dir>`, sem subcomando `format`/`check`).
- [x] Build **Release** completo da solução (0 errors, 0 warnings após corrigir CS8625).
- [x] Suíte de testes completa em verde.
- [x] Revisar `git status` — só arquivos pretendidos; `git mv` preservou histórico (R/RM); nenhum artefato no índice.
- [x] `README.md` e demais arquivos rastreados sem caminhos antigos.
- [x] Corrigir 4 warnings CS8625 em `LineIntersectTests` (`null` → `null!` no teste de entradas null).

### Verification Plan
- `dotnet build DotTerritory.sln -c Release --nologo -v q` → **Build succeeded, 0 Error(s), 0 Warning(s)**.
- `DOTNET_ROLL_FORWARD=Major dotnet run --project tests/DotTerritory.Tests/DotTerritory.Tests.csproj -c Release --framework net9.0` → `Passed! total: 146, failed: 0`.
- `git status --porcelain | grep -E "bin/|obj/|\.idea|\.DS_Store"` → **vazio**.
- `git ls-files | xargs grep -l "src/DotTerritory.sln\|src/dotnet-releaser"` → **vazio**.

### Phase Summary
Concluída em 2026-06-23. Formatação via csharpier; build Release limpo (0 warnings após `null!`);
**146 testes verdes em Release**. `git status`: 6 renames puros (R), 25 renamed+modified (RM),
8 deletes (7 do GeoJson + LineSliceAlong), 18 modified, 1 untracked (`plans/`). Nenhum artefato
de build no índice. README sem caminhos antigos.

---

## Final Recap

Reorganização e modernização do repositório DotTerritory concluída em 4 fases, todas verificadas:

1. **Estrutura** → convenção .NET padrão: `DotTerritory.sln`, `dotnet-releaser.toml` e `CLAUDE.md`
   no root; `src/` contém apenas a biblioteca `DotTerritory`; `tests/`, `samples/` e `benchmark/`
   em pastas próprias no root. CI (`publish.yml`) e todos os `ProjectReference` atualizados.
2. **Limpeza** → removidos o projeto órfão `DotTerritory.GeoJson` (fora do build), o lixo
   `src/src/` (bin/obj), `.idea` duplicado e `.DS_Store`.
3. **Framework de testes** → migração completa de **xUnit + Shouldly** para **TUnit 1.56.25**
   (asserções nativas async `await Assert.That(...)`); `FluentAssertions` (não usado),
   `Microsoft.NET.Test.Sdk` e `coverlet` removidos. Os dois conjuntos de testes (o real em
   `src/tests` e os 13 órfãos do root) consolidados num único projeto em `tests/`.
4. **Qualidade dos testes órfãos** → dos 85 testes órfãos (que nunca haviam rodado), 57 válidos
   mantidos; 28 com expectativas comprovadamente inválidas (premissa planar vs. cálculo geográfico,
   geometria incorreta, `Contains` vs `Covers`) descartados por decisão do usuário, após diagnóstico
   e investigação (incl. confirmação de que os 3 PointOnFeature não eram bug da biblioteca).

**Estado final: 146 testes, 100% verdes (Debug e Release), build sem warnings.**

Pendência opcional registrada (fora do escopo): melhorar `Territory.PointOnFeature` para aderir ao
modelo planar do NTS (ponto sobre a feature / interior de polígonos com buraco), alinhando ao TurfJS.

## Deployment Plan

Não há "deploy" tradicional — a entrega é o estado do repositório. Passos para integrar/publicar:

1. **Revisar e commitar** (nenhum commit foi feito durante o trabalho):
   - `git add -A` (inclui renames, deletes e a pasta `plans/`).
   - Sugestão de mensagem: `refactor: reorganiza estrutura para convenção .NET e migra testes para TUnit`.
   - Os `git mv` preservam histórico (renames no diff).
2. **CI**: `publish.yml` já aponta para `DotTerritory.sln` e `dotnet-releaser.toml` no root. Como o
   `setup-dotnet` usa `9.x`, o runtime 9.0 estará presente no CI (localmente foi necessário
   `DOTNET_ROLL_FORWARD=Major` por ausência do runtime 9.0).
3. **Validação pós-merge**: confirmar `dotnet restore DotTerritory.sln` e o empacotamento via
   `dotnet-releaser run … dotnet-releaser.toml`. Atenção: TUnit usa Microsoft.Testing.Platform —
   se o `dotnet-releaser`/CI executar testes via VSTest, pode ser necessário ajustar o passo de
   testes para `dotnet run`/MTP (validar no primeiro run do CI).
4. **`plans/`**: pode ser commitado (documentação durável) ou mantido apenas local.
