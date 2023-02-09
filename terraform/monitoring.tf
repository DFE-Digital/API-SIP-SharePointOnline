resource "azurerm_log_analytics_workspace" "service_monitoring" {
  name                = "${local.resource_prefix}servicemonitoring"
  resource_group_name = azurerm_resource_group.default.name
  location            = azurerm_resource_group.default.location
  sku                 = "PerGB2018"
  retention_in_days   = local.service_log_retention

  tags = local.tags
}

resource "azurerm_monitor_diagnostic_setting" "service_monitoring" {
  name                       = "${local.resource_prefix}servicemonitoring"
  target_resource_id         = azurerm_windows_web_app.default.id
  log_analytics_workspace_id = azurerm_log_analytics_workspace.service_monitoring.id

  dynamic "enabled_log" {
    for_each = local.service_diagnostic_setting_types
    content {
      category = enabled_log.value

      retention_policy {
        enabled = true
        days    = local.service_log_retention
      }
    }
  }

  metric {
    category = "AllMetrics"

    retention_policy {
      enabled = true
      days    = local.service_log_retention
    }
  }
}

resource "azurerm_application_insights" "service_monitoring" {
  name                = "${local.resource_prefix}-insights"
  location            = azurerm_resource_group.default.location
  resource_group_name = azurerm_resource_group.default.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.service_monitoring.id
  retention_in_days   = 30
  tags                = local.tags
}

resource "azurerm_application_insights_standard_web_test" "service_monitoring" {
  name                    = "${local.resource_prefix}-http"
  resource_group_name     = azurerm_resource_group.default.name
  location                = azurerm_resource_group.default.location
  application_insights_id = azurerm_application_insights.service_monitoring.id
  timeout                 = 10
  enabled                 = true

  geo_locations = [
    "emea-se-sto-edge", # UK West
    "emea-nl-ams-azr",  # West Europe
    "emea-ru-msa-edge"  # UK South
  ]

  request {
    url = "https://${azurerm_cdn_frontdoor_endpoint.endpoint.host_name}${local.cdn_frontdoor_health_probe_path}"
  }

  tags = merge(
    local.tags,
    { "hidden-link:${azurerm_application_insights.service_monitoring.id}" = "Resource" },
  )
}