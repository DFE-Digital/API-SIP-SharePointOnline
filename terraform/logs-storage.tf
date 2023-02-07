resource "azurerm_storage_account" "logs" {
  name                      = "${replace(local.resource_prefix, "-", "")}logs"
  resource_group_name       = azurerm_resource_group.default.name
  location                  = azurerm_resource_group.default.location
  account_tier              = "Standard"
  account_kind              = "StorageV2"
  account_replication_type  = "LRS"
  min_tls_version           = "TLS1_2"
  enable_https_traffic_only = true

  tags = local.tags
}

resource "azurerm_storage_container" "logs" {
  for_each = local.service_log_types

  name                  = "${local.resource_prefix}${each.value}logs"
  storage_account_name  = azurerm_storage_account.logs.name
  container_access_type = "private"
}

data "azurerm_storage_account_blob_container_sas" "logs" {
  for_each = local.service_log_types

  connection_string = azurerm_storage_account.logs.primary_connection_string
  container_name    = azurerm_storage_container.logs[each.value].name
  https_only        = true

  start  = local.service_log_storage_sas_start
  expiry = local.service_log_storage_sas_expiry

  permissions {
    read   = true
    add    = false
    create = false
    write  = true
    delete = true
    list   = true
  }
}
