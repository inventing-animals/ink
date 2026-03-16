# Design Philosophy

## Two-layer component model

Ink takes a deliberate two-layer approach to UI components.

**Layer 1 - Avalonia theming.** Including `InkTheme` in your application automatically styles all standard Avalonia controls (Button, TextBox, CheckBox, etc.) to match the Ink design system. You get consistent typography, spacing, colors, and interaction states with no extra work. This makes Ink a safe drop-in even for existing applications, and ensures third-party controls that render standard Avalonia primitives internally look coherent.

**Layer 2 - Ink components.** For everything beyond base theming - variants, sizes, additional behaviors, slots - Ink provides its own set of components. Each Ink component is a subclass of its Avalonia counterpart wherever possible, so nothing is duplicated. You get the full Avalonia control infrastructure (commands, accessibility, keyboard handling) plus Ink-specific capabilities on top.

```xml
<!-- Plain Avalonia Button - gets Ink theming, secondary appearance by default -->
<Button Command="{Binding Save}">Save</Button>

<!-- Ink Button - full variant support and future features -->
<ink:Button Variant="Primary" Command="{Binding Save}">Save</ink:Button>
```

The rule is simple: helpers and attached properties are not part of the Ink API. If a capability requires configuring a control, it belongs on an Ink component as a first-class property.

## Why subclass instead of attach?

The attached property pattern (e.g. `ButtonAssist.Variant`) is appropriate when styling controls you do not own. Since Ink owns its own components, subclassing is the right tool:

- Properties are first-class (`Variant="Primary"` rather than `ink:ButtonAssist.Variant="Primary"`)
- Style selectors are scoped precisely (`ink|Button.ink-primary` only matches Ink Buttons)
- Future properties (size, icon, loading state) extend naturally without accumulating more helpers
- No static class registration or constructor side effects required

## Avalonia control coverage

Every Avalonia control that Ink has an opinion on will have a corresponding `ink:` component. The base Avalonia controls remain styled and functional on their own - the Ink counterpart simply adds the richer API.

| Avalonia | Ink | Notes |
|---|---|---|
| `Button` | `ink:Button` | Adds `Variant` (Primary, Secondary, Ghost, Danger) |

This table grows as the library matures.
