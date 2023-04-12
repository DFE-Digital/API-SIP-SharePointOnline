locals {
  environment            = var.environment
  project_name           = var.project_name
  resource_prefix        = "${local.environment}${local.project_name}"
  azure_location         = var.azure_location
  tfvars_filename        = var.tfvars_filename
  key_vault_access_users = toset(var.key_vault_access_users)
  launch_in_vnet         = var.launch_in_vnet
  service_plan_sku       = var.service_plan_sku
  service_plan_os        = var.service_plan_os
  service_stack          = var.service_stack
  service_stack_version  = var.service_stack_version
  service_app_settings   = var.service_app_settings
  service_app_insights_settings = {
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
  service_log_retention                     = var.service_log_retention
  service_log_storage_sas_start             = var.service_log_storage_sas_start
  service_log_storage_sas_expiry            = var.service_log_storage_sas_expiry
  service_log_types                         = toset(["app", "http"])
  service_diagnostic_setting_types = toset([
    "AppServiceHTTPLogs",
    "AppServiceConsoleLogs",
    "AppServiceAppLogs",
    "AppServiceAuditLogs",
    "AppServiceIPSecAuditLogs",
    "AppServicePlatformLogs"
  ])
  enable_monitoring                               = var.enable_monitoring
  monitor_endpoint_healthcheck                    = var.monitor_endpoint_healthcheck
  virtual_network_address_space                   = var.virtual_network_address_space
  virtual_network_address_space_mask              = element(split("/", local.virtual_network_address_space), 1)
  app_service_subnet_cidr                         = cidrsubnet(local.virtual_network_address_space, 23 - local.virtual_network_address_space_mask, 0)
  enable_cdn_frontdoor                            = var.enable_cdn_frontdoor
  cdn_frontdoor_sku                               = var.cdn_frontdoor_sku
  cdn_frontdoor_health_probe_interval             = var.cdn_frontdoor_health_probe_interval
  cdn_frontdoor_health_probe_path                 = var.cdn_frontdoor_health_probe_path
  cdn_frontdoor_response_timeout                  = var.cdn_frontdoor_response_timeout
  cdn_frontdoor_host_redirects                    = var.cdn_frontdoor_host_redirects
  cdn_frontdoor_host_add_response_headers         = var.cdn_frontdoor_host_add_response_headers
  cdn_frontdoor_remove_response_headers           = var.cdn_frontdoor_remove_response_headers
  cdn_frontdoor_enable_rate_limiting              = var.cdn_frontdoor_enable_rate_limiting
  cdn_frontdoor_rate_limiting_duration_in_minutes = var.cdn_frontdoor_rate_limiting_duration_in_minutes
  cdn_frontdoor_rate_limiting_threshold           = var.cdn_frontdoor_rate_limiting_threshold
  cdn_frontdoor_rate_limiting_bypass_ip_list      = var.cdn_frontdoor_rate_limiting_bypass_ip_list
  cdn_frontdoor_origin_fqdn_override              = var.cdn_frontdoor_origin_fqdn_override
  restrict_web_app_service_to_cdn_inbound_only    = var.restrict_web_app_service_to_cdn_inbound_only
  tags                                            = var.tags
}
