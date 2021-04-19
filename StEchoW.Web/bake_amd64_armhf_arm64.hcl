# docker context use default
# docker buildx ls
# docker buildx use mybuild
# docker buildx bake -f bake_amd64_armhf_arm64.hcl --print
variable "TAG" {
  default = "latest"
}
variable DNSDK_BASE_IMAGE {
  default = "mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64"
}

group "default" {
  targets = ["stechow"]
}

target "stechow" {
  args = {
    DNSDK_BASE_IMAGE="${DNSDK_BASE_IMAGE}"
  }
  context = "."
  dockerfile = "./Dockerfile"
  output = ["type=registry"]
  # -----------------------------------------------------------------------------------------------------------
  tags = ["ptrn2l2/stechow:${TAG}"]
  # -----------------------------------------------------------------------------------------------------------
  platforms = ["linux/amd64", "linux/armhf", "linux/arm64"]
}

