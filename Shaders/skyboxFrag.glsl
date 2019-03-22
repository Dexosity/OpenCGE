#version 330
 
in vec3 v_TexCoord;

uniform samplerCube cubeMap;

out vec4 Color;
 
void main()
{
    Color = texture(cubeMap, v_TexCoord);
}