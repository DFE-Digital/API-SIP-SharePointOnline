module "azurerm_key_vault" {
  source = "github.com/DFE-Digital/terraform-azurerm-key-vault-tfvars?ref=v0.1.1"

  environment                           = local.environment
  project_name                          = local.project_name
  resource_group_name                   = module.azure_web_app_services_hosting.azurerm_resource_group_default.name
  azure_location                        = local.azure_location
  key_vault_access_users                = local.key_vault_access_users
  tfvars_filename                       = local.tfvars_filename
  diagnostic_log_analytics_workspace_id = module.azure_web_app_services_hosting.azurerm_log_analytics_workspace_web_app_service.id
  diagnostic_storage_account_id         = module.azure_web_app_services_hosting.azurerm_storage_account_logs.id
  tags                                  = local.tags
}
