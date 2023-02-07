resource "azurerm_service_plan" "default" {
  name                = "${local.resource_prefix}default"
  resource_group_name = azurerm_resource_group.default.name
  location            = azurerm_resource_group.default.location
  os_type             = "Windows"
  sku_name            = local.service_plan_sku

  tags = local.tags
}

resource "azurerm_windows_web_app" "default" {
  name                = "${local.resource_prefix}default"
  resource_group_name = azurerm_resource_group.default.name
  location            = azurerm_service_plan.default.location
  service_plan_id     = azurerm_service_plan.default.id

  virtual_network_subnet_id = azurerm_subnet.app_service_subnet.id
  https_only                = true
  app_settings              = local.service_app_settings

  site_config {
    always_on                         = true
    health_check_path                 = local.service_health_check_path
    health_check_eviction_time_in_min = local.service_health_check_eviction_time_in_min
    vnet_route_all_enabled            = true
    http2_enabled                     = true
    minimum_tls_version               = "1.2"
    worker_count                      = local.service_worker_count
    application_stack {
      current_stack  = "dotnet"
      dotnet_version = local.service_dotnet_version
    }
    ip_restriction {
      name   = "Allow CDN Traffic"
      action = "Allow"
      headers {
        x_azure_fdid = [
          azurerm_cdn_frontdoor_profile.cdn.resource_guid
        ]
        x_fd_health_probe = ["1"]
      }
      service_tag = "AzureFrontDoor.Backend"
    }
  }

  logs {
    application_logs {
      azure_blob_storage {
        level             = local.service_log_level
        retention_in_days = local.service_log_retention
        sas_url           = local.service_log_app_sas_url
      }
      file_system_level = local.service_log_level
    }
    http_logs {
      azure_blob_storage {
        retention_in_days = local.service_log_retention
        sas_url           = local.service_log_http_sas_url
      }
    }
    detailed_error_messages = true
    failed_request_tracing  = true
  }

  tags = local.tags
}
