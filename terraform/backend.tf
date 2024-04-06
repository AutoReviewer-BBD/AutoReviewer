terraform {
  backend "s3" {
    bucket = "autoreviewerbucket"
    key = "autoreviewer/terraform.tfstate"  # Specify the path/key for your state file
    region = "eu-west-1"
  }
}