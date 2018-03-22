# GLSL language integration

[![Build status](https://ci.appveyor.com/api/projects/status/dgkpbnfgna2gakrd?svg=true)](https://ci.appveyor.com/project/danielscherzer/glsl)

Download this extension from the [VS Gallery](https://marketplace.visualstudio.com/items?itemName=DanielScherzer.ZenselessVsix)
or get the [CI build](http://vsixgallery.com/extension/b62242eb-0ae5-4494-b013-6158ade63816/).

---------------------------------------

VSIX Project that provides GLSL language integration.
Includes syntax highlighting (file extensions: glsl, frag, vert, geom, comp, tesse, tessc), code completion (OpenGL 4.5 + identifiers in shader file), error tagging with squiggles and in error list (error list support is very alpha). For error tagging a separate OpenGL thread is used for shader compiling on the current hardware.

See the [change log](https://github.com/danielscherzer/GLSL/blob/master/CHANGELOG.md) for changes and road map.

## Features

- Syntax highlighting (file extensions: `glsl`, `frag`, `vert`, `geom`, `comp`, `tesse`, `tessc`)
- Code completion (OpenGL 4.5 keywords + all identifiers in shader file)
- Error tagging with squiggles and in error list (error list support is very alpha)

## Contribute
Check out the [contribution guidelines](https://github.com/danielscherzer/GLSL/blob/master/CONTRIBUTING.md)
if you want to contribute to this project.

For cloning and building this project yourself, make sure
to install the
[Extensibility Tools 2015](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
extension for Visual Studio which enables some features
used by this project.

## License
[Apache 2.0](/src/Resources/LICENSE.txt)