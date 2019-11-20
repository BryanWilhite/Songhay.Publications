# Songhay.Publications

## core reusable definitions for Songhay Studio Publications

Selected Songhay Studio Publications are based on automation ‘pipelines’ for static HTML and EPUB. The fundamental document format of these pipelines is markdown. It follows that the `MarkdownEntry` [class](./Songhay.Publications/Models/MarkdownEntry.cs) is a core definition of this studio.

Architecturally, [MarkdownEntryExtensions](./Songhay.Publications/Extensions/MarkdownEntryExtensions.cs) augment the `MarkdownEntry` to define [eleventy](https://www.11ty.io/)-flavored methods for generating a draft and publishing.

@[BryanWilhite](https://twitter.com/BryanWilhite)
