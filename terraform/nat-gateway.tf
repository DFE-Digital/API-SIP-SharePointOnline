resource "azurerm_public_ip" "nat_gateway" {
  name                = "${local.resource_prefix}natgateway"
  location            = azurerm_resource_group.default.location
  resource_group_name = azurerm_resource_group.default.name
  allocation_method   = "Static"
  sku                 = "Standard"
  ip_version          = "IPv4"

  tags = local.tags
}

resource "azurerm_nat_gateway" "default" {
  name                = "${local.resource_prefix}default"
  location            = azurerm_resource_group.default.location
  resource_group_name = azurerm_resource_group.default.name
  sku_name            = "Standard"

  tags = local.tags
}

resource "azurerm_nat_gateway_public_ip_association" "default" {
  nat_gateway_id       = azurerm_nat_gateway.default.id
  public_ip_address_id = azurerm_public_ip.nat_gateway.id
}

resource "azurerm_subnet_nat_gateway_association" "app_service" {
  nat_gateway_id = azurerm_nat_gateway.default.id
  subnet_id      = azurerm_subnet.app_service_subnet.id
}
