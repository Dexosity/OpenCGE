#version 330
 
uniform sampler2D s_texture;

in vec2 v_TexCoord;
in vec3 Normal;

in vec3 FragPos;
in vec3[] posLight;
in vec3 posEye;

in int Lights;
out vec4 Color;
 
void main()
{
	vec3 lightColor = vec3(1.0, 1.0, 1.0);
	vec3 norm = normalize(Normal);

	vec3 result;

	for(int i = 0; i < Lights; i++)
	{
		result += CalculateAllLightSources(norm, FragPos, posEye, posLight[i]);
	}
	

	Color = vec4(result, 1) * vec4(vec3(texture2D(s_texture, v_TexCoord)), 1.0);
}
vec3 CalculateAllLightSources(vec3 normal, vec3 frag, vec3 eye, vec3 light)
{
	vec3 LightDirection = normalize(light - frag);
	vec3 EyeDirection = normalize(eye - frag);

	vec3 ambient = 0.5 * lightColor;

	float diff = max(dot(normal, LightDirection), 0.0);
	vec3 diffuse = diff * lightColor;

	vec3 reflectDirection = reflect(-LightDirection, normal);
	float spec = pow(max(dot(EyeDirection, reflectDirection), 0.0), 16);
	vec3 specular = 0.5 * spec * lightColor;

	return (ambient + diffuse + specular);
}