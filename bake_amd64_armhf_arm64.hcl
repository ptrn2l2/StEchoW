# docker context use default
# docker buildx ls
# docker buildx use mybuild
# docker buildx bake -f bake_tagged_amd64_armhf_arm64.hcl --print
variable "TAG" {
  default = "latest"
}

group "default" {
  targets = ["stechow"]
}

target "stechow" {
  context = "."
  dockerfile = "./Dockerfile"
  output = ["type=registry"]
  # -----------------------------------------------------------------------------------------------------------
  # Doesn't work: https://docs.docker.com/engine/reference/commandline/buildx_bake/#hcl-variables-and-functions
  # TAG=latest docker buildx bake -f bake_amd64_armhf_arm64.hcl --print
  tags = ["ptrn2l2/stechow:${TAG}"]
  # -----------------------------------------------------------------------------------------------------------
  platforms = ["linux/amd64", "linux/armhf", "linux/arm64"]
}

