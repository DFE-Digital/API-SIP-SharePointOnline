module "azurerm_key_vault" {
  source = "github.com/DFE-Digital/terraform-azurerm-key-vault-tfvars?ref=v0.5.1"

  environment                             = local.environment
  project_name                            = local.project_name
  existing_resource_group                 = module.azure_web_app_services_hosting.azurerm_resource_group_default.name
  azure_location                          = local.azure_location
  key_vault_access_use_rbac_authorization = true
  key_vault_access_users                  = []
  key_vault_access_ipv4                   = local.key_vault_access_ipv4
  tfvars_filename                         = local.tfvars_filename
  diagnostic_log_analytics_workspace_id   = module.azure_web_app_services_hosting.azurerm_log_analytics_workspace_web_app_service.id
  diagnostic_storage_account_id           = module.azure_web_app_services_hosting.azurerm_storage_account_logs.id
  tags                                    = local.tags
}
