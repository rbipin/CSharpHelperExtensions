# Upgrade to .NET 10 + Migrate to .slnx Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Upgrade both projects from `net6.0`/`netstandard2.1` to `net10.0`, update all NuGet packages, migrate the solution file from `.sln` to `.slnx`, and update CI to match.

**Architecture:** All changes are configuration-only (no source code logic changes). The library project moves from `netstandard2.1` to `net10.0`, the test project from `net6.0` to `net10.0`, NuGet packages are updated to compatible versions, and `dotnet sln migrate` converts the solution format. CI is updated last.

**Tech Stack:** .NET 10 SDK (10.0.201), xUnit 2.x, FluentAssertions 7.x, GitHub Actions

---

## Files Modified

| File | Change |
|------|--------|
| `CSharpHelperExtensions/CSharpHelperExtensions.csproj` | `netstandard2.1` → `net10.0`, `LangVersion` → `latest`, Newtonsoft.Json update |
| `CSharpHelperExtensions.Test/CSharpHelperExtensions.Test.csproj` | `net6.0` → `net10.0`, all package updates |
| `CSharpHelperExtensions.sln` | Deleted after migration |
| `CSharpHelperExtensions.slnx` | Created by `dotnet sln migrate` |
| `.github/workflows/dotnet.yml` | `dotnet-version: 6.0.x` → `10.0.x`, action versions bumped |
| `global.json` | New file — pins SDK to 10.0.x |
| `CLAUDE.md` | Remove the runtime mismatch warning |

---

## Task 1: Upgrade the library project to .NET 10

**Files:**
- Modify: `CSharpHelperExtensions/CSharpHelperExtensions.csproj`

- [ ] **Step 1: Replace the csproj content**

Open `CSharpHelperExtensions/CSharpHelperExtensions.csproj` and replace it with:

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <PackageId>CSharpHelperExtensions</PackageId>
        <Version>1.0.0</Version>
        <Authors>Bipin Radhakrishnan</Authors>
        <TargetFramework>net10.0</TargetFramework>
        <PackageVersion>2.0.0</PackageVersion>
        <Description>A set of helper extension methods that are used very often when coding</Description>
        <RepositoryUrl>https://github.com/rbipin/dry-extensions-csharp</RepositoryUrl>
        <RepositoryType>git</RepositoryType>
        <PackageReleaseNotes></PackageReleaseNotes>
        <AssemblyName>CSharpHelperExtensions</AssemblyName>
        <RootNamespace>CSharpHelperExtensions</RootNamespace>
        <LangVersion>latest</LangVersion>
    </PropertyGroup>

    <ItemGroup>
      <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    </ItemGroup>

</Project>
```

- [ ] **Step 2: Build the library to confirm it compiles**

```bash
dotnet build CSharpHelperExtensions/CSharpHelperExtensions.csproj
```

Expected: `Build succeeded.`

- [ ] **Step 3: Commit**

```bash
git add CSharpHelperExtensions/CSharpHelperExtensions.csproj
git commit -m "chore: upgrade library to net10.0, LangVersion latest, Newtonsoft.Json 13.0.3"
```

---

## Task 2: Upgrade the test project to .NET 10

**Files:**
- Modify: `CSharpHelperExtensions.Test/CSharpHelperExtensions.Test.csproj`

- [ ] **Step 1: Replace the test csproj content**

Open `CSharpHelperExtensions.Test/CSharpHelperExtensions.Test.csproj` and replace it with:

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>net10.0</TargetFramework>
        <IsPackable>false</IsPackable>
        <RootNamespace>ReusableExtensions.Unittest</RootNamespace>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="FluentAssertions" Version="7.2.0" />
        <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
        <PackageReference Include="xunit" Version="2.9.3" />
        <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
            <PrivateAssets>all</PrivateAssets>
        </PackageReference>
        <PackageReference Include="coverlet.collector" Version="6.0.4">
            <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
            <PrivateAssets>all</PrivateAssets>
        </PackageReference>
    </ItemGroup>

    <ItemGroup>
      <ProjectReference Include="..\CSharpHelperExtensions\CSharpHelperExtensions.csproj" />
    </ItemGroup>

    <ItemGroup>
      <None Remove="EnumerableExtension.Test.cs" />
    </ItemGroup>

</Project>
```

> **Note on FluentAssertions 7.x:** FA 7 introduces `using FluentAssertions;` namespace change in some edge cases, but `Should().BeTrue()`, `Should().BeFalse()`, `Should().Be()`, `Should().Equal()`, `Should().BeNull()`, `Should().BeOfType()` — all used in this project — are unchanged.

- [ ] **Step 2: Restore packages**

```bash
dotnet restore CSharpHelperExtensions.Test/CSharpHelperExtensions.Test.csproj
```

Expected: `Restore succeeded.`

- [ ] **Step 3: Run all tests to verify everything passes**

```bash
dotnet test --verbosity normal
```

Expected output (all 16 tests pass):
```
Passed!  - Failed: 0, Passed: 16, Skipped: 0, Total: 16
```

If any test fails due to a FluentAssertions API change, check the FA 7.x migration guide at https://fluentassertions.com/upgradingtov7

- [ ] **Step 4: Commit**

```bash
git add CSharpHelperExtensions.Test/CSharpHelperExtensions.Test.csproj
git commit -m "chore: upgrade test project to net10.0, update all test packages"
```

---

## Task 3: Migrate solution file from .sln to .slnx

**Files:**
- Delete: `CSharpHelperExtensions.sln`
- Create: `CSharpHelperExtensions.slnx` (auto-generated)

- [ ] **Step 1: Run the migration command**

```bash
cd /Users/bipin/repo/CSharpHelperExtensions
dotnet sln migrate CSharpHelperExtensions.sln
```

Expected: A new file `CSharpHelperExtensions.slnx` is created in the same directory.

> The `.slnx` format is XML-based and human-readable. It was introduced in .NET 9 SDK and is supported by Visual Studio 2022 17.x+ and VS Code with the C# Dev Kit extension.

- [ ] **Step 2: Verify the .slnx is valid by building through it**

```bash
dotnet build CSharpHelperExtensions.slnx
```

Expected: `Build succeeded.`

- [ ] **Step 3: Run tests through the new solution file**

```bash
dotnet test CSharpHelperExtensions.slnx --verbosity normal
```

Expected: `Passed! - Failed: 0, Passed: 16, Skipped: 0, Total: 16`

- [ ] **Step 4: Delete the old .sln file**

```bash
rm CSharpHelperExtensions.sln
```

- [ ] **Step 5: Commit**

```bash
git add CSharpHelperExtensions.slnx
git rm CSharpHelperExtensions.sln
git commit -m "chore: migrate solution from .sln to .slnx format"
```

---

## Task 4: Add global.json to pin the SDK version

**Files:**
- Create: `global.json`

- [ ] **Step 1: Create global.json**

Create `/Users/bipin/repo/CSharpHelperExtensions/global.json`:

```json
{
  "sdk": {
    "version": "10.0.201",
    "rollForward": "latestMinor"
  }
}
```

> `rollForward: latestMinor` means it will use any newer patch/minor of the 10.x SDK if available, but won't silently jump to .NET 11.

- [ ] **Step 2: Verify build still works**

```bash
dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 3: Commit**

```bash
git add global.json
git commit -m "chore: add global.json to pin SDK to 10.0.x"
```

---

## Task 5: Update CI/CD pipeline

**Files:**
- Modify: `.github/workflows/dotnet.yml`

- [ ] **Step 1: Replace the workflow content**

Open `.github/workflows/dotnet.yml` and replace it with:

```yaml
name: .NET

on:
  push:
    branches: [ "main" ]
  pull_request:
    branches: [ "main" ]

jobs:
  build:

    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v4
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: 10.0.x
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
```

Changes from original:
- `actions/checkout@v3` → `@v4`
- `actions/setup-dotnet@v2` → `@v4`
- `dotnet-version: 6.0.x` → `10.0.x`

- [ ] **Step 2: Commit**

```bash
git add .github/workflows/dotnet.yml
git commit -m "ci: upgrade GitHub Actions to .NET 10 and bump action versions"
```

---

## Task 6: Update CLAUDE.md

**Files:**
- Modify: `CLAUDE.md`

- [ ] **Step 1: Remove the runtime mismatch warning**

Find and remove this block from `CLAUDE.md`:

```
> ⚠️ **Runtime mismatch:** The test project targets `net6.0`, but the machine has .NET 8 and .NET 10 installed. Tests will fail to run until the test project's `TargetFramework` in `CSharpHelperExtensions.Test/CSharpHelperExtensions.Test.csproj` is updated to `net8.0` or `net10.0`.
```

- [ ] **Step 2: Update solution file reference**

In the Commands section of `CLAUDE.md`, add a note that the solution file is now `.slnx`:

```bash
# Build using the solution file explicitly
dotnet build CSharpHelperExtensions.slnx

# Test using the solution file explicitly
dotnet test CSharpHelperExtensions.slnx
```

- [ ] **Step 3: Commit**

```bash
git add CLAUDE.md
git commit -m "docs: update CLAUDE.md for .NET 10 and .slnx solution"
```

---

## Verification

Full end-to-end check after all tasks:

```bash
# Full clean build
dotnet build

# All tests pass
dotnet test --verbosity normal

# Confirm .sln is gone and .slnx is present
ls *.sln* *.slnx

# Confirm SDK version in use
dotnet --version
```

Expected final state:
- `dotnet --version` → `10.0.201` (or later 10.x)
- `dotnet test` → `Passed! - Failed: 0, Passed: 16, Skipped: 0, Total: 16`
- Only `CSharpHelperExtensions.slnx` exists (no `.sln`)
