locals {
  environment            = var.environment
  project_name           = var.project_name
  resource_prefix        = "${local.environment}${local.project_name}"
  azure_location         = var.azure_location
  tfvars_filename        = var.tfvars_filename
  key_vault_access_users = toset(var.key_vault_access_users)
  service_plan_sku       = var.service_plan_sku
  service_dotnet_version = var.service_dotnet_version
  service_app_settings   = var.service_app_settings
  service_app_insights_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY"                  = azurerm_application_insights.service_monitoring.instrumentation_key,
    "APPLICATIONINSIGHTS_CONNECTION_STRING"           = azurerm_application_insights.service_monitoring.connection_string,
    "APPINSIGHTS_PROFILERFEATURE_VERSION"             = "1.0.0",
    "APPINSIGHTS_SNAPSHOTFEATURE_VERSION"             = "1.0.0",
    "ApplicationInsightsAgent_EXTENSION_VERSION"      = "~2",
    "DiagnosticServices_EXTENSION_VERSION"            = "~3",
    "InstrumentationEngine_EXTENSION_VERSION"         = "disabled",
    "SnapshotDebugger_EXTENSION_VERSION"              = "disabled",
    "XDT_MicrosoftApplicationInsights_BaseExtensions" = "disabled",
    "XDT_MicrosoftApplicationInsights_Mode"           = "recommended",
    "XDT_MicrosoftApplicationInsights_PreemptSdk"     = "disabled",
    "XDT_MicrosoftApplicationInsights_Java"           = "1",
    "XDT_MicrosoftApplicationInsights_NodeJS"         = "1",
  }
  service_health_check_path                 = var.service_health_check_path
  service_health_check_eviction_time_in_min = var.service_health_check_eviction_time_in_min
  service_worker_count                      = var.service_worker_count
  service_log_level                         = var.service_log_level
  service_log_retention                     = var.service_log_retention
  service_log_storage_sas_start             = var.service_log_storage_sas_start
  service_log_storage_sas_expiry            = var.service_log_storage_sas_expiry
  service_log_types                         = toset(["app", "http"])
  service_log_app_sas_url                   = "${azurerm_storage_account.logs.primary_blob_endpoint}${azurerm_storage_container.logs["app"].name}${data.azurerm_storage_account_blob_container_sas.logs["app"].sas}"
  service_log_http_sas_url                  = "${azurerm_storage_account.logs.primary_blob_endpoint}${azurerm_storage_container.logs["http"].name}${data.azurerm_storage_account_blob_container_sas.logs["http"].sas}"
  service_diagnostic_setting_types = toset([
    "AppServiceHTTPLogs",
    "AppServiceConsoleLogs",
    "AppServiceAppLogs",
    "AppServiceAuditLogs",
    "AppServiceIPSecAuditLogs",
    "AppServicePlatformLogs"
  ])
  virtual_network_address_space                   = var.virtual_network_address_space
  virtual_network_address_space_mask              = element(split("/", local.virtual_network_address_space), 1)
  app_service_subnet_cidr                         = cidrsubnet(local.virtual_network_address_space, 23 - local.virtual_network_address_space_mask, 0)
  cdn_frontdoor_sku                               = var.cdn_frontdoor_sku
  cdn_frontdoor_health_probe_interval             = var.cdn_frontdoor_health_probe_interval
  cdn_frontdoor_health_probe_path                 = var.cdn_frontdoor_health_probe_path
  cdn_frontdoor_response_timeout                  = var.cdn_frontdoor_response_timeout
  cdn_frontdoor_host_redirects                    = var.cdn_frontdoor_host_redirects
  cdn_frontdoor_host_add_response_headers         = var.cdn_frontdoor_host_add_response_headers
  cdn_frontdoor_remove_response_headers           = var.cdn_frontdoor_remove_response_headers
  cdn_frontdoor_rate_limiting_duration_in_minutes = var.cdn_frontdoor_rate_limiting_duration_in_minutes
  cdn_frontdoor_rate_limiting_threshold           = var.cdn_frontdoor_rate_limiting_threshold
  cdn_frontdoor_rate_limiting_bypass_ip_list      = var.cdn_frontdoor_rate_limiting_bypass_ip_list
  ruleset_redirects_id                            = length(local.cdn_frontdoor_host_redirects) > 0 ? [azurerm_cdn_frontdoor_rule_set.redirects[0].id] : []
  ruleset_add_response_headers_id                 = length(local.cdn_frontdoor_host_add_response_headers) > 0 ? [azurerm_cdn_frontdoor_rule_set.add_response_headers[0].id] : []
  ruleset_remove_response_headers_id              = length(local.cdn_frontdoor_remove_response_headers) > 0 ? [azurerm_cdn_frontdoor_rule_set.remove_response_headers[0].id] : []
  ruleset_ids = concat(
    local.ruleset_redirects_id,
    local.ruleset_add_response_headers_id,
    local.ruleset_remove_response_headers_id,
  )
  tags = var.tags
}
