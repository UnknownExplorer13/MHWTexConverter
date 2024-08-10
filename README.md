# MHWTexConverter
### A two-way converter for .tex and .dds files for Monster Hunter World resources.

## Usage
- This tool is developed based on .NET Framework 4.8.
- Drag and drop the .tex or .dds file onto the .exe file. The program will auto generate the output file in the same directory as the input file.
- If the file type is .tex, the output .dds file will add a prefix of its compression type.

You will need to install a Photoshop plugin like Intel Texture Works or Nvidia Texture Tools to open and save the edited .dds file.

Links to the plugins:
* [Intel](http://gametechdev.github.io/Intel-Texture-Works-Plugin/)
* [Nvidia](https://developer.nvidia.com/texture-tools-exporter)
* [Nvidia (Photoshop 5.0-CS6)](https://developer.nvidia.com/gameworksdownload#?dn=texture-tools-for-adobe-photoshop-8-55)

## To save .dds file after editing
If the prefix of the output `.dds` file is:
* DXT1: Better to use Nvidia tool with compression type: `DXT1 ARGB 4bpp | 1bit alpha`, other configs stay default
* DTX3: Better to use Nvidia tool with compression type: `DXT3 ARGB 8bpp | explicit alpha`, other configs stay default
* BC5: Use the Intel tool with save configuration: `Normal Map`,  `BC5 8bpp (Linear, 2 Channel tangent map)`, other configs stay default
* BC6H: You may **never** edit this type of texture, ignore it.
* BC7: Use the Intel tool with save configuration: `Color + Alpha`, `BC7 8bpp Fine (sRGB, DX11+)`, other configs stay default
* Notice: If the texture you are editing does not contain mipmaps (such as the monster icons), you should not generate mipmaps. Instead, select `None` but not `Auto Generate as Mip Maps` generation option in the Intel tool.
