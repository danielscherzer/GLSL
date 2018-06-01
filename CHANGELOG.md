# Road map
- [ ] Error list jump to line
- [ ] Show `glsl' shader in ToolBoxWindow
- [ ] Better parsing of glsl compiler errors for shorter squiggle spans
- [ ] Allow switching between native compile on separate thread and glslang reference compiler

Features that have a check mark are complete and available for download in the
[CI build](http://vsixgallery.com/extension/b62242eb-0ae5-4494-b013-6158ade63816/).

# Change log
These are the changes to each version that has been released on the official Visual Studio extension gallery.

## 0.1
- [x] Initial release
- [x] Syntax highlighting (file extensions: `glsl`, `frag`, `vert`, `geom`, `comp`, `tesse`, `tessc`)
- [x] Code completion (OpenGL 4.5 keywords + all identifiers in shader file)
- [x] Error tagging with squiggles and in error list (error list support is very alpha)

## 0.2
- [x] Options page for custom file extensions and disabling of live compilation