#version 330 core
in vec3 fNormal;
in vec3 fPos;
in vec2 fUv;

struct Material {
      vec3 ambient;
      vec3 diffuse;
      vec3 specular;
      float shininess;
};

struct Light {
      vec3 position;
      vec3 ambient;
      vec3 diffuse;
      vec3 specular;
};

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_diffuse2;
uniform sampler2D texture_diffuse3;
uniform sampler2D texture_specular1;
uniform sampler2D texture_specular2;

uniform Material material;
uniform Light light;
uniform vec3 viewPos;

out vec4 FragColor;

void main()
{
      float ambientStrength = 0.1;
      vec3 ambient = ambientStrength * lightColor;

      vec3 norm = normalize(fNormal);
      vec3 lightDirection = normalize(lightPos - fPos);
      float diff = max(dot(norm, lightDirection), 0.0);
      vec3 diffuse = diff * lightColor;

      float specularStrength = 0.2;
      vec3 viewDirection = normalize(viewPos - fPos);
      vec3 reflectDirection = reflect(-lightDirection, norm);
      float spec = pow(max(dot(viewDirection, reflectDirection), 0.0), 32);
      vec3 specular = specularStrength * spec * lightColor;

      //The resulting colour should be the amount of ambient colour + the amount of additional colour provided by the diffuse of the lamp + the specular amount
      vec3 result = ambient + diffuse + specular;

      FragColor = vec4(result, 1.0) + texture(uTexture0, fUv);
}