module "azurerm_key_vault" {
  source = "github.com/DFE-Digital/terraform-azurerm-key-vault-tfvars?ref=v0.1.1"

  environment                           = local.environment
  project_name                          = local.project_name
  resource_group_name                   = azurerm_resource_group.default.name
  azure_location                        = local.azure_location
  key_vault_access_users                = local.key_vault_access_users
  tfvars_filename                       = local.tfvars_filename
  diagnostic_log_analytics_workspace_id = null
  diagnostic_eventhub_name              = null
  tags                                  = local.tags
}
