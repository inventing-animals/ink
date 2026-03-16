# Contributing

## How versioning works

All packages share a single version. The canonical version is the **git tag** - it is never stored in source files.

`Directory.Build.props` contains `<Version>0.0.0</Version>` permanently. Local builds are always stamped `0.0.0`, which makes it obvious they are not release builds. Do not change this file as part of a release.

The CI `publish` job only runs when a `v*` tag is pushed. It derives the version from the tag name by stripping the leading `v`:

```
tag: v0.5.0  →  NuGet package version: 0.5.0
```

---

## Publishing a new release

### 1. Merge all changes to main

Make sure everything you want in the release is on `main` and the CI `build-and-test` job is green.

### 2. Tag the release commit

```bash
git tag v0.5.0
git push origin v0.5.0
```

That is the entire release process. The tag push triggers the CI `publish` job, which:

- Builds all packages in Release configuration
- Packs each `.csproj` with the version from the tag
- Pushes all `.nupkg` files to NuGet.org

### 3. Verify

Check the Actions tab on GitHub - the `publish` job should appear and go green. NuGet indexing typically takes a few minutes after the push succeeds.

---

## What the CI does

| Trigger | Jobs that run |
|---|---|
| Push to `main` | `build-and-test`, `deploy-pages` (docs + WASM demo) |
| Pull request to `main` | `build-and-test` |
| Push a `v*` tag | `build-and-test`, `publish` (NuGet) |

`deploy-pages` and `publish` are independent - docs deploy on every merge to `main`, NuGet only on tags.

---

## Adding a new package

When a new `.csproj` is added that should be published to NuGet:

1. Add the standard NuGet metadata to the `.csproj` (`PackageId`, `Description`, `Authors`, etc.) - follow the pattern in any existing package.
2. Add restore, build, and pack lines for it in **both** the `build-and-test` and `publish` jobs in `.github/workflows/ci.yml`.
