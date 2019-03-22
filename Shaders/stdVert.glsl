#version 330

uniform mat4 WorldViewProj;
uniform mat4 Transform;

in vec3 a_Position;
in vec2 a_TexCoord;

out vec2 v_TexCoord;

void main()
{
    gl_Position = WorldViewProj * Transform * vec4(a_Position, 1.0);
    v_TexCoord = a_TexCoord;
}