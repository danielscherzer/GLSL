# Road map
- [ ] Error list jump to line
- [ ] Show `glsl' shader in ToolBoxWindow
- [ ] Better parsing of glsl compiler errors for shorter squiggle spans

Features that have a check mark are complete and available for download in the
[CI build](http://vsixgallery.com/extension/b62242eb-0ae5-4494-b013-6158ade63816/).

# Change log
These are the changes to each version that has been released on the official Visual Studio extension gallery.

## 0.5
- [x] Allow switching between graphics card shader compiler and external compiler.

## 0.4
- [x] Auto detect shader type for the file extension `glsl` based on reserved words used in the shader code.

## 0.3
- [x] Controlling the extension with comments

## 0.2
- [x] Options page for custom file extensions and disabling of live compilation (disables error squiggles)
- [x] Include support

## 0.1
- [x] Initial release
- [x] Syntax highlighting (default file extensions: `glsl`, `frag`, `vert`, `geom`, `comp`, `tesse`, `tessc`)
- [x] Code completion (OpenGL 4.6 keywords + all identifiers in shader file)
- [x] Error tagging with squiggles and in error list (error list support is very alpha)
