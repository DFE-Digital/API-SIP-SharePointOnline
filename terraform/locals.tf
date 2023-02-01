locals {
  environment            = var.environment
  project_name           = var.project_name
  resource_prefix        = "${local.environment}${local.project_name}"
  azure_location         = var.azure_location
  tfvars_filename        = var.tfvars_filename
  key_vault_access_users = toset(var.key_vault_access_users)
  tags                   = var.tags
}
