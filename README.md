# GLSL language integration (for VS2017, 2019 and 2022)

[![Build status](https://ci.appveyor.com/api/projects/status/dgkpbnfgna2gakrd?svg=true)](https://ci.appveyor.com/project/danielscherzer/glsl)

Download this extension from
- Visual Studio Marketplace version [VS2017 & 2019](https://marketplace.visualstudio.com/items?itemName=DanielScherzer.GLSL) or [VS 2022](https://marketplace.visualstudio.com/items?itemName=DanielScherzer.GLSL2022)
- Open VSIX Gallery version [VS2017 & 2019](https://www.vsixgallery.com/extension/b62242eb-0ae5-4494-b013-6158ade63816) or
[VS 2022](https://www.vsixgallery.com/extension/999396e8-1400-4b23-ae9e-17e823006a12)
- [Github releases](https://github.com/danielscherzer/GLSL/releases/latest)

---------------------------------------

VSIX Project that provides GLSL language integration.
Includes syntax highlighting, code completion (OpenGL 4.6 + identifiers in shader file), error tagging with squiggles and in error list (error list support is very alpha). Error tagging uses a separate OpenGL thread that compiles the shader on the primary graphics card. You can also select an external compiler executable to use for this purpose in the options menu. Please see section "External comiler" for details.

See the [change log](CHANGELOG.md) for changes and road map.

## Features

- Syntax highlighting (default file extensions: `glsl`, `frag`, `vert`, `geom`, `comp`, `tese`, `tesc`, `mesh`, `task`, `rgen`, `rint`, `rmiss`, `rahit`, `rchit`, `rcall`) Set color under Options (Fonts and Colors)
- Code completion (OpenGL 4.6 keywords + all identifiers in shader file)
- Outlining
- Error tagging with squiggles and in error list (error list support is very alpha)
  - For the file extension `glsl` the extension tries to auto detect the type of shader you use based on reserved words used in the shader code.
  - Note that `GLSL_NV_ray_tracing` shader types support for error tagging is currently only provided via an external compiler (like `glslangValidator`).
  - Note that support for **Vulkan** shader types is currently only provided via an external compiler (like `glslangValidator`).
  - Auto-detection of shader stage: If you use the `glsl` file extension the source code is searched for keywords only used in certain shader stages.
- Configurable (file extensions, code compilation, highlighting style, compiler)
- Controlling the extension with comments (see below)

## Configuration (extension options)
+ Options of the extension can be found via the Visual Studio options dialog (`Tools` -> `Options` -> `glsl language integration`).
+ Configure Fonts and Colors via "Environment" -> "Fonts and Colors"). All "Display Items" of the extension start with GLSL.

## Controlling the extension with comments
The original idea is from [Clocktown](https://github.com/Clocktown). The discussion can be found under [#15](https://github.com/danielscherzer/GLSL/issues/15). 

When developing shaders, it is common to split one shader into multiple files, reuse files, and generally generate some parts of code in the application in order to make things easier.

Unfortunately, this does not go well with extensions like this that compile these files without any knowledge about what happens on the side of the application. One way around this are *special* comments `//!` and `//?` that are transformed into code before compiling. 

Some examples:
```
//! #version 430
//! #define WORK_GROUP_SIZE 32
#include "/utils.glsl" //! #include "../include/utils.glsl"
```
compiles into 
```
#version 430
#define WORK_GROUP_SIZE 32
#include "../include/utils.glsl"
```
Everything in the line in front of the `//!` is removed and replaced by the code in the comment.
`//!` is always expanded to code, while `//?` is only expanded if the file being compiled has not been included, e.g.: 

File A.glsl
```
//? #version 430 // Only expanded if this file is compiled directly, not when included in B.glsl
// Lots of utility functions
```
File B.glsl
```
//! #version 430
#include "/utils.glsl" //! #include "A.glsl"
```

## External compiler
In the option menu you can set a file path and arguments to an external compiler. If an existing path is given compilations will be executed by starting the given executable. You can use environment variables, like `%SystemDrive%` in your arguments and path. The shader code (including include code and all substitutions) will be written into a temporary file named "shader.vert|frag|comp|..." (extensions follow glslangValidator standard) in the current users temp path. This temporary shader file is then used as the first argument to the external compiler executable. 

## Errors and questions
Please us the GitHub [Issue function](https://github.com/danielscherzer/GLSL/issues/new) to report errors or ask questions.

## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.

For cloning and building this project yourself, make sure to install the
[Extensibility Tools 2015](https://visualstudiogallery.msdn.microsoft.com/ab39a092-1343-46e2-b0f1-6a3f91155aa6)
extension for Visual Studio which enables some features used by this project.

## License
[Apache 2.0](/src/Resources/LICENSE.txt)
