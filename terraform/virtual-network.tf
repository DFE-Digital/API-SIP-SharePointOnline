resource "azurerm_virtual_network" "default" {
  name                = "${local.resource_prefix}default"
  address_space       = [local.virtual_network_address_space]
  location            = azurerm_resource_group.default.location
  resource_group_name = azurerm_resource_group.default.name
  tags                = local.tags
}

resource "azurerm_route_table" "default" {
  name                          = "${local.resource_prefix}default"
  location                      = azurerm_resource_group.default.location
  resource_group_name           = azurerm_resource_group.default.name
  disable_bgp_route_propagation = false
  tags                          = local.tags
}

resource "azurerm_subnet" "app_service_subnet" {
  name                 = "${local.resource_prefix}appservice"
  virtual_network_name = azurerm_virtual_network.default.name
  resource_group_name  = azurerm_resource_group.default.name
  address_prefixes     = [local.app_service_subnet_cidr]

  delegation {
    name = "delegation"

    service_delegation {
      name = "Microsoft.Web/serverFarms"
      actions = [
        "Microsoft.Network/virtualNetworks/subnets/action",
      ]
    }
  }
}

resource "azurerm_subnet_route_table_association" "app_service_subnet" {
  subnet_id      = azurerm_subnet.app_service_subnet.id
  route_table_id = azurerm_route_table.default.id
}
