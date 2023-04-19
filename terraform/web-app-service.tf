module "azure_web_app_services_hosting" {
  source = "github.com/DFE-Digital/terraform-azurerm-web-app-services-hosting?ref=v0.1.2"

  environment    = local.environment
  project_name   = local.project_name
  azure_location = local.azure_location
  tags           = local.tags

  launch_in_vnet                 = local.launch_in_vnet
  virtual_network_address_space  = local.virtual_network_address_space
  service_plan_sku               = local.service_plan_sku
  service_plan_os                = local.service_plan_os
  service_stack                  = local.service_stack
  service_stack_version          = local.service_stack_version
  service_worker_count           = local.service_worker_count
  service_app_settings           = local.service_app_settings
  service_health_check_path      = local.service_health_check_path
  service_log_storage_sas_start  = local.service_log_storage_sas_start
  service_log_storage_sas_expiry = local.service_log_storage_sas_expiry

  enable_monitoring              = local.enable_monitoring
  monitor_endpoint_healthcheck   = local.monitor_endpoint_healthcheck
  monitor_email_receivers        = local.monitor_email_receivers
  monitor_enable_slack_webhook   = local.monitor_enable_slack_webhook
  monitor_slack_webhook_receiver = local.monitor_slack_webhook_receiver
  monitor_slack_channel          = local.monitor_slack_channel

  enable_cdn_frontdoor                            = local.enable_cdn_frontdoor
  restrict_web_app_service_to_cdn_inbound_only    = local.restrict_web_app_service_to_cdn_inbound_only
  cdn_frontdoor_sku                               = local.cdn_frontdoor_sku
  cdn_frontdoor_health_probe_path                 = local.cdn_frontdoor_health_probe_path
  cdn_frontdoor_health_probe_interval             = local.cdn_frontdoor_health_probe_interval
  cdn_frontdoor_response_timeout                  = local.cdn_frontdoor_response_timeout
  cdn_frontdoor_host_redirects                    = local.cdn_frontdoor_host_redirects
  cdn_frontdoor_host_add_response_headers         = local.cdn_frontdoor_host_add_response_headers
  cdn_frontdoor_remove_response_headers           = local.cdn_frontdoor_remove_response_headers
  cdn_frontdoor_enable_rate_limiting              = local.cdn_frontdoor_enable_rate_limiting
  cdn_frontdoor_rate_limiting_duration_in_minutes = local.cdn_frontdoor_rate_limiting_duration_in_minutes
  cdn_frontdoor_rate_limiting_threshold           = local.cdn_frontdoor_rate_limiting_threshold
  cdn_frontdoor_origin_fqdn_override              = local.cdn_frontdoor_origin_fqdn_override
}