# Contributing

## Publishing a new release

1. Increment the version in `src/Ink.UI/Ink.UI.csproj`:
   ```xml
   <Version>x.y.z</Version>
   ```

2. Commit the version bump, then tag and push:
   ```bash
   git tag vx.y.z
   git push origin vx.y.z
   ```

The CI pipeline picks up the tag and publishes the NuGet package automatically.
