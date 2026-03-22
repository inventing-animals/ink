from __future__ import annotations

import argparse
import json
import re
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Any

PROJECT_DIR = Path(__file__).resolve().parent
ICON_OUTPUT = PROJECT_DIR / "Icon.cs"
METADATA_OUTPUT = PROJECT_DIR / "Generated" / "FontAwesomeMetadata.g.cs"

FACE_MAP: dict[str, tuple[str, ...]] = {
    "solid": ("ClassicSolid",),
    "regular": ("ClassicRegular",),
    "light": ("ClassicLight",),
    "thin": ("ClassicThin",),
    "duotone": ("DuotoneRegular", "DuotoneSolid", "DuotoneLight", "DuotoneThin"),
    "brands": ("Brands",),
}

FACE_FALLBACK_ORDER = (
    "ClassicSolid",
    "ClassicRegular",
    "ClassicLight",
    "ClassicThin",
    "DuotoneSolid",
    "DuotoneRegular",
    "DuotoneLight",
    "DuotoneThin",
    "SharpSolid",
    "SharpRegular",
    "SharpLight",
    "SharpThin",
    "SharpDuotoneSolid",
    "SharpDuotoneRegular",
    "SharpDuotoneLight",
    "SharpDuotoneThin",
    "Brands",
)

DIGIT_NAMES = {
    "0": "Zero",
    "1": "One",
    "2": "Two",
    "3": "Three",
    "4": "Four",
    "5": "Five",
    "6": "Six",
    "7": "Seven",
    "8": "Eight",
    "9": "Nine",
}


@dataclass(frozen=True)
class IconDefinition:
    source_name: str
    identifier: str
    unicode_hex: str
    faces: tuple[str, ...]


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description=(
            "Load Font Awesome icon metadata from a local icons.json file. "
            "Use the Pro Font Awesome package as input metadata only; this tool is for generating "
            "definitions and does not bundle licensed files."
        ),
    )
    parser.add_argument(
        "icons_json",
        type=Path,
        help="Path to the Font Awesome icons.json file.",
    )
    return parser.parse_args()


def validate_icons_json(icons_json: Path) -> Path:
    if not icons_json.exists():
        raise SystemExit(f"icons.json file does not exist: {icons_json}")

    if not icons_json.is_file():
        raise SystemExit(f"icons.json path is not a file: {icons_json}")

    if icons_json.name.lower() != "icons.json":
        raise SystemExit(
            f"Expected a file named icons.json, got: {icons_json.name}",
        )

    return icons_json


def load_icons(icons_json: Path) -> dict[str, Any]:
    with icons_json.open("r", encoding="utf-8") as handle:
        data = json.load(handle)

    if not isinstance(data, dict):
        raise SystemExit("icons.json root must be a JSON object.")

    return data


def to_identifier(source_name: str) -> str:
    if source_name in DIGIT_NAMES:
        return DIGIT_NAMES[source_name]

    parts = re.split(r"[^0-9A-Za-z]+", source_name)
    parts = [part for part in parts if part]
    if not parts:
        return "Icon"

    identifier = "".join(part[:1].upper() + part[1:] for part in parts)
    if identifier[0].isdigit():
        identifier = f"Icon{identifier}"

    return identifier


def collect_faces(styles: list[Any], icon_name: str) -> tuple[str, ...]:
    faces: list[str] = []
    unknown_styles: list[str] = []

    for style in styles:
        if not isinstance(style, str):
            raise SystemExit(f"Icon '{icon_name}' has a non-string style value: {style!r}")

        mapped_faces = FACE_MAP.get(style)
        if mapped_faces is None:
            unknown_styles.append(style)
            continue

        for face in mapped_faces:
            if face not in faces:
                faces.append(face)

    if unknown_styles:
        styles_csv = ", ".join(sorted(unknown_styles))
        raise SystemExit(f"Icon '{icon_name}' uses unsupported styles: {styles_csv}")

    return tuple(faces)


def build_icon_definitions(icons: dict[str, Any]) -> list[IconDefinition]:
    definitions: list[IconDefinition] = []
    seen_identifiers: dict[str, str] = {}

    for source_name, metadata in icons.items():
        if not isinstance(metadata, dict):
            raise SystemExit(f"Icon '{source_name}' metadata must be an object.")

        unicode_hex = metadata.get("unicode")
        if not isinstance(unicode_hex, str) or not unicode_hex:
            raise SystemExit(f"Icon '{source_name}' is missing a valid unicode value.")

        styles = metadata.get("styles")
        if not isinstance(styles, list):
            raise SystemExit(f"Icon '{source_name}' is missing a valid styles list.")

        identifier = to_identifier(source_name)
        previous_source = seen_identifiers.get(identifier)
        if previous_source is not None and previous_source != source_name:
            raise SystemExit(
                f"Generated identifier collision: '{source_name}' and '{previous_source}' both map to '{identifier}'.",
            )

        seen_identifiers[identifier] = source_name

        definitions.append(
            IconDefinition(
                source_name=source_name,
                identifier=identifier,
                unicode_hex=unicode_hex.upper(),
                faces=collect_faces(styles, source_name),
            ),
        )

    definitions.sort(key=lambda icon: icon.identifier)
    return definitions


def render_icon_enum(definitions: list[IconDefinition]) -> str:
    lines = [
        "namespace Ink.FontAwesome;",
        "",
        "/// <summary>",
        "/// Generated Font Awesome icon identifiers.",
        "/// </summary>",
        "public enum Icon",
        "{",
        "    /// <summary>",
        "    /// No icon.",
        "    /// </summary>",
        "    None = 0,",
        "",
    ]

    for definition in definitions:
        lines.extend(
            [
                "    /// <summary>",
                f"    /// {definition.source_name}.",
                "    /// </summary>",
                f"    {definition.identifier},",
                "",
            ],
        )

    lines[-1] = "}"
    lines.append("")
    return "\n".join(lines)


def render_metadata(definitions: list[IconDefinition]) -> str:
    lines = [
        "using System;",
        "",
        "namespace Ink.FontAwesome;",
        "",
        "[Flags]",
        "internal enum FaceSet",
        "{",
        "    None = 0,",
        "    ClassicRegular = 1 << 0,",
        "    ClassicSolid = 1 << 1,",
        "    ClassicLight = 1 << 2,",
        "    ClassicThin = 1 << 3,",
        "    DuotoneRegular = 1 << 4,",
        "    DuotoneSolid = 1 << 5,",
        "    DuotoneLight = 1 << 6,",
        "    DuotoneThin = 1 << 7,",
        "    SharpRegular = 1 << 8,",
        "    SharpSolid = 1 << 9,",
        "    SharpLight = 1 << 10,",
        "    SharpThin = 1 << 11,",
        "    SharpDuotoneRegular = 1 << 12,",
        "    SharpDuotoneSolid = 1 << 13,",
        "    SharpDuotoneLight = 1 << 14,",
        "    SharpDuotoneThin = 1 << 15,",
        "    Brands = 1 << 16,",
        "}",
        "",
        "/// <summary>",
        "/// Generated metadata lookup for Font Awesome icons.",
        "/// </summary>",
        "internal static class FontAwesomeMetadata",
        "{",
        "    public static string GetGlyph(Icon icon)",
        "        => char.ConvertFromUtf32(GetCodePoint(icon));",
        "",
        "    public static Face ResolveFace(Icon icon, Face requestedFace)",
        "    {",
        "        var supportedFaces = GetSupportedFaces(icon);",
        "        var requestedFlag = ToFaceSet(requestedFace);",
        "",
        "        if ((supportedFaces & requestedFlag) != 0)",
        "            return requestedFace;",
        "",
    ]

    for face in FACE_FALLBACK_ORDER:
        lines.extend(
            [
                f"        if ((supportedFaces & FaceSet.{face}) != 0)",
                f"            return Face.{face};",
                "",
            ],
        )

    lines.extend(
        [
            '        throw new ArgumentOutOfRangeException(nameof(icon), icon, "The icon has no supported faces.");',
            "    }",
            "",
            "    private static int GetCodePoint(Icon icon)",
            "        => icon switch",
            "        {",
        ],
    )

    for definition in definitions:
        lines.append(f"            Icon.{definition.identifier} => 0x{definition.unicode_hex},")

    lines.extend(
        [
            "            Icon.None => 0x0000,",
            "            _ => throw new ArgumentOutOfRangeException(nameof(icon), icon, null),",
            "        };",
            "",
            "    private static FaceSet GetSupportedFaces(Icon icon)",
            "        => icon switch",
            "        {",
        ],
    )

    for definition in definitions:
        face_flags = " | ".join(f"FaceSet.{face}" for face in definition.faces)
        lines.append(f"            Icon.{definition.identifier} => {face_flags},")

    lines.extend(
        [
            "            Icon.None => FaceSet.None,",
            "            _ => throw new ArgumentOutOfRangeException(nameof(icon), icon, null),",
            "        };",
            "",
            "    private static FaceSet ToFaceSet(Face face)",
            "        => face switch",
            "        {",
            "            Face.ClassicRegular => FaceSet.ClassicRegular,",
            "            Face.ClassicSolid => FaceSet.ClassicSolid,",
            "            Face.ClassicLight => FaceSet.ClassicLight,",
            "            Face.ClassicThin => FaceSet.ClassicThin,",
            "            Face.DuotoneRegular => FaceSet.DuotoneRegular,",
            "            Face.DuotoneSolid => FaceSet.DuotoneSolid,",
            "            Face.DuotoneLight => FaceSet.DuotoneLight,",
            "            Face.DuotoneThin => FaceSet.DuotoneThin,",
            "            Face.SharpRegular => FaceSet.SharpRegular,",
            "            Face.SharpSolid => FaceSet.SharpSolid,",
            "            Face.SharpLight => FaceSet.SharpLight,",
            "            Face.SharpThin => FaceSet.SharpThin,",
            "            Face.SharpDuotoneRegular => FaceSet.SharpDuotoneRegular,",
            "            Face.SharpDuotoneSolid => FaceSet.SharpDuotoneSolid,",
            "            Face.SharpDuotoneLight => FaceSet.SharpDuotoneLight,",
            "            Face.SharpDuotoneThin => FaceSet.SharpDuotoneThin,",
            "            Face.Brands => FaceSet.Brands,",
            "            _ => FaceSet.None,",
            "        };",
            "}",
            "",
        ],
    )

    return "\n".join(lines)


def write_if_changed(path: Path, content: str) -> bool:
    path.parent.mkdir(parents=True, exist_ok=True)

    if path.exists():
        existing = path.read_text(encoding="utf-8")
        if existing == content:
            return False

    path.write_text(content, encoding="utf-8", newline="\n")
    return True


def generate_files(icons: dict[str, Any]) -> tuple[int, bool, bool]:
    definitions = build_icon_definitions(icons)
    icon_changed = write_if_changed(ICON_OUTPUT, render_icon_enum(definitions))
    metadata_changed = write_if_changed(METADATA_OUTPUT, render_metadata(definitions))
    return len(definitions), icon_changed, metadata_changed


def print_summary(
    icons_json: Path,
    icon_count: int,
    icon_changed: bool,
    metadata_changed: bool,
) -> None:
    print(f"Loaded Font Awesome metadata from: {icons_json.resolve()}")
    print(f"Generated icons: {icon_count}")
    print(f"{ICON_OUTPUT.name}: {'updated' if icon_changed else 'unchanged'}")
    print(f"{METADATA_OUTPUT.name}: {'updated' if metadata_changed else 'unchanged'}")


def main() -> int:
    args = parse_args()
    icons_json = validate_icons_json(args.icons_json.expanduser().resolve())
    icons = load_icons(icons_json)
    icon_count, icon_changed, metadata_changed = generate_files(icons)
    print_summary(icons_json, icon_count, icon_changed, metadata_changed)
    return 0


if __name__ == "__main__":
    sys.exit(main())
