# Contributing

Thanks for your interest in contributing to Ink.

## What we welcome

Bug fixes, new components, accessibility improvements, and general enhancements are all welcome. Open an issue or a pull request and we will take a look.

## What to know before contributing

Ink is the foundation of [Octopus](https://inventing-animals.com/octopus), our commercial product. This means the library has real production constraints that shape what we can accept.

**Regressions are a blocker.** If a change breaks existing behavior, it will not be merged regardless of how well-intentioned it is.

**Substantial rewrites or architectural changes need prior discussion.** Open an issue first and describe what you want to change and why. We are happy to talk it through, but we cannot accept large structural changes that were not agreed on upfront.

**Security is non-negotiable.** Any change that introduces a security concern will be rejected outright. If you find a security issue, please disclose it responsibly by emailing us directly rather than opening a public issue.

Everything else is fair game. If you are unsure whether something fits, just ask.

## Publishing a new release

1. Increment the version in `Directory.Build.props` at the repo root:
   ```xml
   <Version>x.y.z</Version>
   ```
   This applies to all published packages (`InventingAnimals.Ink`, `InventingAnimals.Ink.Platform`, `InventingAnimals.Ink.Platform.Browser`).

2. Commit the version bump, then tag and push:
   ```bash
   git tag vx.y.z
   git push origin vx.y.z
   ```

The CI pipeline picks up the tag and publishes the NuGet package automatically.
