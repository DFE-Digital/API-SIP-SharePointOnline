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
