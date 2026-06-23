# Migração para .slnx e Suporte ao .NET 10

Finalizar a substituição do `DotTerritory.sln` pelo `DotTerritory.slnx`, adicionar
`net10.0` aos projetos e modernizar o CI (actions, SDKs, tools) para suportar a nova
estrutura e o runner de testes TUnit (Microsoft.Testing.Platform).

## Contexto e Decisões (confirmadas com o usuário)

Estado inicial (capturado em 2026-06-23, branch `main`, **com o trabalho anterior de
reorganização ainda não commitado** — 59 itens no working tree):

- O `DotTerritory.slnx` **já existe** no root (untracked) e está correto (referencia os 4
  projetos com os caminhos da nova estrutura). O `DotTerritory.sln` foi removido do working
  tree (ainda aparece staged como rename `src/DotTerritory.sln -> DotTerritory.sln`).
  `dotnet restore DotTerritory.slnx` funciona.
- TFMs atuais: lib `net8.0;net9.0`; `tests` `net9.0`; `benchmark` `net9.0`; `samples` `net8.0`.
- SDKs locais: **10.0.301** e **11.0.100-preview** (sem `global.json`). Runtime 9.0 ausente
  localmente (rodar net9 exige `DOTNET_ROLL_FORWARD=Major`); runtime 10 presente.
- `.config/dotnet-tools.json`: `dotnet-releaser` **0.11.0**, `csharpier` **0.30.6**.
- `dotnet-releaser`: **0.15.1** adicionou detecção de `.slnx`; **0.16.0** adicionou suporte a
  **Microsoft.Testing.Platform (MTP)** — necessário para rodar os testes TUnit. Última: **0.21.0**.
  Portanto a 0.11.0 do projeto NÃO serve (nem slnx, nem MTP).
- CI (`.github/workflows/publish.yml`): `actions/checkout@v4`, `actions/setup-dotnet@v4`
  (`dotnet-version: 9.x`), `dotnet restore DotTerritory.sln`, `dotnet tool install --global
  dotnet-releaser` (latest), e `dotnet-releaser run … dotnet-releaser.toml`.
- `dotnet-releaser.toml`: `[msbuild] project = "DotTerritory.sln"`.
- Validado em probe isolado: **build multi-target `net8.0;net9.0;net10.0` com SDK 10 funciona**
  (baixa os reference packs via NuGet); os pacotes (NetTopologySuite 2.5, UnitsNet 6.0-pre013)
  são compatíveis. `actions/setup-dotnet` está em **v5** (v5.2.0); `actions/checkout` em **v5**.
- Nenhuma referência a `csharpier` em CI/scripts (uso apenas local).

Decisões do usuário:

1. **TFMs da lib `DotTerritory`**: `net8.0;net9.0;net10.0` (adiciona 10, mantém 8 e 9).
2. **TFMs de `tests`, `samples`, `benchmark`**: `net10.0` apenas.
3. **Tools**: fixar no manifest (`dotnet-releaser` 0.21.0, `csharpier` 1.3.0) e o **CI usa
   `dotnet tool restore`** (versões reproduzíveis). Atualizar comandos do csharpier 1.x.
4. **Atualizar as actions do CI** (`checkout@v5`, `setup-dotnet@v5`).
5. **Migrar `.sln` → `.slnx`** (já iniciado; finalizar).

Notas técnicas:
- csharpier **1.x** mudou a CLI: formatar = `dotnet csharpier format <dir>`; checar =
  `dotnet csharpier check <dir>` (a 0.30.6 usava `dotnet csharpier <dir>`).
- CI precisa dos SDKs **8.0.x, 9.0.x, 10.0.x** (a lib multi-targeta; o build de net8/net9 usa os
  targeting packs — instalar os 3 SDKs é o mais robusto e garante runtimes para eventuais testes).
- Testes são `net10.0` apenas → rodam no runtime 10 (sem roll-forward).
- `dotnet-releaser` 0.16+ detecta MTP automaticamente; se necessário, ajustar `[test]` no `.toml`
  (validar ao rodar o release localmente).

## For Future Agents
As work proceeds: mark checkboxes `- [x]` as items complete; when a phase is done,
set its status to `Complete` and write its **Phase Summary**; run the phase's
**Verification Plan** and record the result before moving on. When all phases are
done, fill in **Final Recap** and **Deployment Plan**.

---

## Phase 1: Finalizar migração .sln → .slnx
Status: Complete

- [x] Regularizar o estado git: `DotTerritory.slnx` rastreado (`A`); `DotTerritory.sln` removido do índice (`D src/DotTerritory.sln`). Não há mais `.sln` no repo.
- [x] Atualizar `dotnet-releaser.toml`: `[msbuild] project = "DotTerritory.sln"` → `"DotTerritory.slnx"`.
- [x] (O `dotnet restore … .slnx` no `publish.yml` é tratado na Fase 3, junto com o resto do CI.)

### Verification Plan
- `test ! -e DotTerritory.sln && test -f DotTerritory.slnx && echo OK` → `OK`.
- `find . -name '*.sln' -not -path '*/bin/*' -not -path '*/obj/*'` → vazio.
- `dotnet restore DotTerritory.slnx` → restaura sem erro.
- `grep -c 'DotTerritory.slnx' dotnet-releaser.toml` → `1`; `grep -c 'DotTerritory.sln\"' dotnet-releaser.toml` → `0`.

### Phase Summary
Concluída em 2026-06-23. O `.slnx` (criado fora desta sessão) foi mantido e rastreado; o `.sln`
saiu do índice (resultado: `A DotTerritory.slnx`, `D src/DotTerritory.sln`). `dotnet-releaser.toml`
agora aponta para `DotTerritory.slnx`. Verificação: 0 arquivos `.sln` no repo; `dotnet restore
DotTerritory.slnx` OK. O `restore` no `publish.yml` é atualizado na Fase 3.

---

## Phase 2: Suporte ao .NET 10 nos projetos
Status: Complete

- [x] `src/DotTerritory/DotTerritory.csproj`: `net8.0;net9.0` → `net8.0;net9.0;net10.0`.
- [x] `tests/DotTerritory.Tests/DotTerritory.Tests.csproj`: `net9.0` → `net10.0`.
- [x] `samples/DotTerritory.Playground/DotTerritory.Playground.csproj`: `net8.0` → `net10.0`.
- [x] `benchmark/TimeAndMemoryUsage/TimeAndMemoryUsage.csproj`: `net9.0` → `net10.0`.

### Verification Plan
- `dotnet build DotTerritory.slnx -c Release --nologo -v q` → **Build succeeded, 0 Error(s), 0 Warning(s)** (lib compila em net8/net9/net10).
- `dotnet run --project tests/DotTerritory.Tests/DotTerritory.Tests.csproj -c Release --framework net10.0` → `Passed! total: 146, failed: 0` (sem `DOTNET_ROLL_FORWARD`, pois o runtime 10 está instalado).
- `grep TargetFramework src/DotTerritory/DotTerritory.csproj` → contém `net8.0;net9.0;net10.0`.

### Phase Summary
Concluída em 2026-06-23. Lib agora multi-targeta `net8.0;net9.0;net10.0`; tests/samples/benchmark
em `net10.0`. **Verificação: Build Release succeeded (0 warnings); `Passed! total: 146, failed: 0`
em net10.0 sem roll-forward** (runtime 10 presente). SDK 10 baixou os ref packs de net8/net9.

---

## Phase 3: Atualizar tools (manifest) e CI (actions, SDKs, restore, .slnx)
Status: Complete

- [x] `.config/dotnet-tools.json`: `dotnet-releaser` `0.11.0` → `0.21.0`; `csharpier` `0.30.6` → `1.3.0` (via `dotnet tool update`; o csharpier 1.x renomeou o comando de `dotnet-csharpier` para `csharpier`).
- [x] `publish.yml`: `actions/checkout@v4` → `@v5`; `actions/setup-dotnet@v4` → `@v5`.
- [x] `publish.yml`: `dotnet-version: 9.x` → multilinha `8.0.x` / `9.0.x` / `10.0.x`.
- [x] `publish.yml`: `dotnet restore DotTerritory.sln` → `dotnet restore DotTerritory.slnx`.
- [x] `publish.yml`: passo `dotnet tool install --global dotnet-releaser` → `dotnet tool restore`; release via `dotnet dotnet-releaser run … dotnet-releaser.toml` (tool local).
- [x] Sem comandos `csharpier` com sintaxe antiga em CI/scripts.

### Verification Plan
- `dotnet tool restore` → instala `dotnet-releaser 0.21.0` e `csharpier 1.3.0` sem erro.
- `dotnet csharpier check .` → executa (sintaxe 1.x válida) e reporta o estado de formatação.
- `python3 -c "import yaml,sys; yaml.safe_load(open('.github/workflows/publish.yml'))" ` (ou similar) → YAML válido.
- `grep -E 'checkout@v5|setup-dotnet@v5|DotTerritory.slnx|tool restore|10.0.x' .github/workflows/publish.yml` → todas as linhas presentes; `grep -c 'DotTerritory.sln ' publish.yml` → 0.

### Phase Summary
Concluída em 2026-06-23. Manifest atualizado via `dotnet tool update` (releaser 0.21.0, csharpier
1.3.0 — comando renomeado para `csharpier`). `publish.yml` reescrito: `checkout@v5`, `setup-dotnet@v5`,
SDKs `8.0.x/9.0.x/10.0.x`, `restore DotTerritory.slnx`, `dotnet tool restore`, release via
`dotnet dotnet-releaser run`. Verificações: `dotnet tool restore` OK; `dotnet csharpier check` executa
(sintaxe 1.x válida; exit 1 apenas porque há arquivos a reformatar no estilo 1.3.0 — aplicado na Fase 4,
inclui `.csproj`/XML, que a 1.x passa a formatar); YAML válido; 0 referências a `.sln` no CI.

---

## Phase 4: Validação holística (simular o release localmente)
Status: Complete

- [x] `dotnet csharpier format .` (119 arquivos; a 1.3.0 também formata `.csproj`/XML).
- [x] Build **Release** da solução (`.slnx`) em todos os TFMs.
- [x] Suíte de testes (net10.0) em verde.
- [x] Executar o pipeline do release **sem publicar** (build multi-TFM + empacotamento) com o `dotnet-releaser` 0.21.0 e o `.slnx`.
- [x] **Re-plan**: 2 obstáculos resolvidos (ver Phase Summary): `global.json` (SDK 10) e testes desacoplados do releaser.
- [x] Revisar `git status` — sem artefatos no índice.

### Verification Plan
- `dotnet build DotTerritory.slnx -c Release --nologo -v q` → **Build succeeded, 0 Error(s), 0 Warning(s)**.
- `dotnet run --project tests/DotTerritory.Tests/DotTerritory.Tests.csproj -c Release` → `Passed! total: 146, failed: 0`.
- `dotnet dotnet-releaser build --force dotnet-releaser.toml` → exit 0; gera `artifacts-dotnet-releaser/DotTerritory.0.8.0.nupkg`.
- `dotnet csharpier check .` → exit 0.
- `git status --porcelain | grep -E "bin/|obj/|artifacts|\.idea|\.DS_Store"` → vazio.

### Phase Summary
Concluída em 2026-06-23. **Resultado: build Release OK; 146 testes verdes; `dotnet-releaser build`
exit 0 gerando `DotTerritory.0.8.0.nupkg`.**

Dois obstáculos surgiram ao validar o release e foram resolvidos (re-plan):
1. **SDK instável**: o `dotnet` default era o **11.0.100-preview**, sob o qual o `dotnet-releaser`
   falhava no MSBuild. **Solução**: `global.json` fixando o SDK em `10.0.301` (`rollForward: latestMinor`).
   Boa prática para um projeto que mira net10; o CI instala 8/9/10 e o global.json seleciona o 10.
2. **TUnit/MTP × dotnet-releaser**: o releaser 0.21.0 reconhece o projeto como Testing Platform
   (pula cobertura), mas ainda tenta executar via **VSTest** (`--target:VSTest … --project`), falhando com
   `MSB1001: Unknown switch`. **Solução**: `[test] enable = false` no `dotnet-releaser.toml` + passo
   dedicado no CI `dotnet run --project tests/… -c Release` (executa o TUnit via MTP nativamente).

## Final Recap

Migração para `.slnx` + suporte ao .NET 10 + modernização de tooling/CI, em 4 fases verificadas:

1. **`.slnx`**: finalizada a substituição do `.sln` — `DotTerritory.slnx` rastreado, `.sln` removido,
   `dotnet-releaser.toml` apontando para `.slnx`.
2. **.NET 10**: lib `net8.0;net9.0;net10.0`; `tests`/`samples`/`benchmark` em `net10.0`.
3. **Tools + CI**: `dotnet-releaser 0.21.0` (slnx+MTP) e `csharpier 1.3.0` no manifest; `publish.yml`
   com `checkout@v5`, `setup-dotnet@v5`, SDKs `8/9/10.x`, `restore .slnx`, `dotnet tool restore`.
4. **Validação**: `global.json` (SDK 10); testes desacoplados do releaser (passo `dotnet run` + `[test]
   enable=false`); release validado localmente (gera `.nupkg`).

**Estado final: build Release multi-TFM sem warnings; 146 testes verdes; release empacota com sucesso.**

Adições além do plano original, necessárias e documentadas: `global.json` e o passo de testes no CI.
A versão do pacote permanece `0.8.0`.

## Deployment Plan

Entrega: branch `chore/restructure-slnx-net10` + PR. Após merge:

1. **CI em PR/push**: checkout@v5 → setup-dotnet (8/9/10) → `restore .slnx` → `tool restore` →
   **testes** (`dotnet run`, TUnit/MTP) → `dotnet-releaser run` (build+pack; publica só em tag).
2. **Publicação (NuGet/GitHub)**: em push de **tag** (fluxo `dotnet-releaser run`, agora via tool local +
   `.slnx`). Usa os secrets `NUGET_TOKEN` e `GITHUB_TOKEN` já presentes.
3. **Validação pós-merge**: no 1º run do CI, confirmar instalação dos SDKs 8/9/10, testes verdes e
   (em tag) publicação do `DotTerritory.<versão>.nupkg`. O `global.json` garante o SDK 10.
4. **Observação**: o branch parte do trabalho de reorganização anterior (não commitado em main) e contém
   ambos. Separar o histórico exigiria rebase/cherry-pick manual.
