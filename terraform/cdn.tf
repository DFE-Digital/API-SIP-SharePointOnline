resource "azurerm_cdn_frontdoor_profile" "cdn" {
  name                     = "${local.resource_prefix}cdn"
  resource_group_name      = azurerm_resource_group.default.name
  sku_name                 = local.cdn_frontdoor_sku
  response_timeout_seconds = local.cdn_frontdoor_response_timeout
  tags                     = local.tags
}

resource "azurerm_cdn_frontdoor_origin_group" "group" {
  name                     = "${local.resource_prefix}origingroup"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.cdn.id

  load_balancing {}

  health_probe {
    protocol            = "Https"
    interval_in_seconds = local.cdn_frontdoor_health_probe_interval
    request_type        = "GET"
    path                = local.cdn_frontdoor_health_probe_path
  }
}

resource "azurerm_cdn_frontdoor_origin" "origin" {
  name                           = "${local.resource_prefix}origin"
  cdn_frontdoor_origin_group_id  = azurerm_cdn_frontdoor_origin_group.group.id
  enabled                        = true
  certificate_name_check_enabled = true
  host_name                      = azurerm_windows_web_app.default.default_hostname
  origin_host_header             = azurerm_windows_web_app.default.default_hostname
  http_port                      = 80
  https_port                     = 443
}

resource "azurerm_cdn_frontdoor_endpoint" "endpoint" {
  name                     = "${local.resource_prefix}cdnendpoint"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.cdn.id
  tags                     = local.tags
}

resource "azurerm_cdn_frontdoor_route" "route" {
  name                          = "${local.resource_prefix}route"
  cdn_frontdoor_endpoint_id     = azurerm_cdn_frontdoor_endpoint.endpoint.id
  cdn_frontdoor_origin_group_id = azurerm_cdn_frontdoor_origin_group.group.id
  cdn_frontdoor_origin_ids      = [azurerm_cdn_frontdoor_origin.origin.id]
  cdn_frontdoor_rule_set_ids    = local.ruleset_ids
  enabled                       = true

  forwarding_protocol    = "HttpsOnly"
  https_redirect_enabled = true
  patterns_to_match      = ["/*"]
  supported_protocols    = ["Http", "Https"]

  link_to_default_domain = true
}

resource "azurerm_cdn_frontdoor_rule_set" "redirects" {
  count = length(local.cdn_frontdoor_host_redirects) > 0 ? 1 : 0

  name                     = "${replace(local.resource_prefix, "-", "")}redirects"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.cdn.id
}

resource "azurerm_cdn_frontdoor_rule" "redirect" {
  for_each = { for index, host_redirect in local.cdn_frontdoor_host_redirects : index => { "from" : host_redirect.from, "to" : host_redirect.to } }

  depends_on = [azurerm_cdn_frontdoor_origin_group.group, azurerm_cdn_frontdoor_origin.origin]

  name                      = "redirect${each.key}"
  cdn_frontdoor_rule_set_id = azurerm_cdn_frontdoor_rule_set.redirects[0].id
  order                     = each.key
  behavior_on_match         = "Continue"

  actions {
    url_redirect_action {
      redirect_type        = "Moved"
      redirect_protocol    = "Https"
      destination_hostname = each.value.to
    }
  }

  conditions {
    host_name_condition {
      operator         = "Equal"
      negate_condition = false
      match_values     = [each.value.from]
      transforms       = ["Lowercase", "Trim"]
    }
  }
}

resource "azurerm_cdn_frontdoor_rule_set" "add_response_headers" {
  count = length(local.cdn_frontdoor_host_add_response_headers) > 0 ? 1 : 0

  name                     = "${replace(local.resource_prefix, "-", "")}addresponseheaders"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.cdn.id
}

resource "azurerm_cdn_frontdoor_rule" "add_response_headers" {
  for_each = { for index, response_header in local.cdn_frontdoor_host_add_response_headers : index => { "name" : response_header.name, "value" : response_header.value } }

  depends_on = [azurerm_cdn_frontdoor_origin_group.group, azurerm_cdn_frontdoor_origin.origin]

  name                      = replace("addresponseheaders${each.key}", "-", "")
  cdn_frontdoor_rule_set_id = azurerm_cdn_frontdoor_rule_set.add_response_headers[0].id
  order                     = 0
  behavior_on_match         = "Continue"

  actions {
    response_header_action {
      header_action = "Overwrite"
      header_name   = each.value.name
      value         = each.value.value
    }
  }
}

resource "azurerm_cdn_frontdoor_rule_set" "remove_response_headers" {
  count = length(local.cdn_frontdoor_remove_response_headers) > 0 ? 1 : 0

  name                     = "${replace(local.resource_prefix, "-", "")}removeresponseheaders"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.cdn.id
}

resource "azurerm_cdn_frontdoor_rule" "remove_response_header" {
  for_each = toset(local.cdn_frontdoor_remove_response_headers)

  depends_on = [azurerm_cdn_frontdoor_origin_group.group, azurerm_cdn_frontdoor_origin.origin]

  name                      = replace("removeresponseheader${each.value}", "-", "")
  cdn_frontdoor_rule_set_id = azurerm_cdn_frontdoor_rule_set.remove_response_headers[0].id
  order                     = 0
  behavior_on_match         = "Continue"

  actions {
    response_header_action {
      header_action = "Delete"
      header_name   = each.value
    }
  }
}

resource "azurerm_cdn_frontdoor_firewall_policy" "waf" {
  name                              = "${replace(local.resource_prefix, "-", "")}waf"
  resource_group_name               = azurerm_resource_group.default.name
  sku_name                          = azurerm_cdn_frontdoor_profile.cdn.sku_name
  enabled                           = true
  mode                              = "Prevention"
  custom_block_response_status_code = 403
  custom_block_response_body        = filebase64("${path.module}/html/waf-response.html")

  custom_rule {
    name                           = "RateLimiting"
    enabled                        = true
    priority                       = 1
    rate_limit_duration_in_minutes = local.cdn_frontdoor_rate_limiting_duration_in_minutes
    rate_limit_threshold           = local.cdn_frontdoor_rate_limiting_threshold
    type                           = "RateLimitRule"
    action                         = "Block"

    dynamic "match_condition" {
      for_each = length(local.cdn_frontdoor_rate_limiting_bypass_ip_list) > 0 ? [0] : []

      content {
        match_variable     = "RemoteAddr"
        operator           = "IPMatch"
        negation_condition = true
        match_values       = local.cdn_frontdoor_rate_limiting_bypass_ip_list
      }
    }

    match_condition {
      match_variable     = "RequestUri"
      operator           = "RegEx"
      negation_condition = false
      match_values       = ["/.*"]
    }
  }

  tags = local.tags
}

resource "azurerm_cdn_frontdoor_security_policy" "waf" {
  name                     = "${replace(local.resource_prefix, "-", "")}wafsecuritypolicy"
  cdn_frontdoor_profile_id = azurerm_cdn_frontdoor_profile.cdn.id

  security_policies {
    firewall {
      cdn_frontdoor_firewall_policy_id = azurerm_cdn_frontdoor_firewall_policy.waf.id

      association {
        patterns_to_match = ["/*"]

        domain {
          cdn_frontdoor_domain_id = azurerm_cdn_frontdoor_endpoint.endpoint.id
        }
      }
    }
  }
}
