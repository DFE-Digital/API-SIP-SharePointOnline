locals {
  environment                               = var.environment
  project_name                              = var.project_name
  resource_prefix                           = "${local.environment}${local.project_name}"
  azure_location                            = var.azure_location
  tfvars_filename                           = var.tfvars_filename
  key_vault_access_users                    = toset(var.key_vault_access_users)
  service_plan_sku                          = var.service_plan_sku
  service_dotnet_version                    = var.service_dotnet_version
  service_health_check_path                 = var.service_health_check_path
  service_health_check_eviction_time_in_min = var.service_health_check_eviction_time_in_min
  service_worker_count                      = var.service_worker_count
  virtual_network_address_space             = var.virtual_network_address_space
  virtual_network_address_space_mask        = element(split("/", local.virtual_network_address_space), 1)
  app_service_subnet_cidr                   = cidrsubnet(local.virtual_network_address_space, 23 - local.virtual_network_address_space_mask, 0)
  cdn_frontdoor_sku                         = var.cdn_frontdoor_sku
  cdn_frontdoor_health_probe_interval       = var.cdn_frontdoor_health_probe_interval
  cdn_frontdoor_health_probe_path           = var.cdn_frontdoor_health_probe_path
  cdn_frontdoor_response_timeout            = var.cdn_frontdoor_response_timeout
  cdn_frontdoor_host_redirects              = var.cdn_frontdoor_host_redirects
  cdn_frontdoor_host_add_response_headers   = var.cdn_frontdoor_host_add_response_headers
  cdn_frontdoor_remove_response_headers     = var.cdn_frontdoor_remove_response_headers
  ruleset_redirects_id                      = length(local.cdn_frontdoor_host_redirects) > 0 ? [azurerm_cdn_frontdoor_rule_set.redirects[0].id] : []
  ruleset_add_response_headers_id           = length(local.cdn_frontdoor_host_add_response_headers) > 0 ? [azurerm_cdn_frontdoor_rule_set.add_response_headers[0].id] : []
  ruleset_remove_response_headers_id        = length(local.cdn_frontdoor_remove_response_headers) > 0 ? [azurerm_cdn_frontdoor_rule_set.remove_response_headers[0].id] : []
  ruleset_ids = concat(
    local.ruleset_redirects_id,
    local.ruleset_add_response_headers_id,
    local.ruleset_remove_response_headers_id,
  )
  tags = var.tags
}
