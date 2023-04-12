variable "environment" {
  description = "Environment name. Will be used along with `project_name` as a prefix for all resources."
  type        = string
}

variable "project_name" {
  description = "Project name. Will be used along with `environment` as a prefix for all resources."
  type        = string
}

variable "azure_location" {
  description = "Azure location in which to launch resources."
  type        = string
}

variable "tfvars_filename" {
  description = "tfvars filename. This file is uploaded and stored encrupted within Key Vault, to ensure that the latest tfvars are stored in a shared place."
  type        = string
}

variable "key_vault_access_users" {
  description = "List of users that require access to the Key Vault where tfvars are stored. This should be a list of User Principle Names (Found in Active Directory) that need to run terraform"
  type        = list(string)
}

variable "launch_in_vnet" {
  description = "Conditionally launch into a VNet"
  type        = bool
}

variable "service_plan_sku" {
  description = "Service plan sku"
  type        = string
}

variable "service_plan_os" {
  description = "Service plan operating system. Valid values are `Windows` or `Linux`."
  type        = string
}

variable "service_stack" {
  description = "The application stack for the web app. Valid values are `dotnet`, `dotnetcore`, `node`, `python`, `php`, `java`, `ruby` or `go`."
  type        = string
}

variable "service_stack_version" {
  description = "Service stack version"
  type        = string
}

variable "service_app_settings" {
  description = "Service app settings"
  type        = map(string)
}

variable "service_health_check_path" {
  description = "Service health check path"
  type        = string
}

variable "service_health_check_eviction_time_in_min" {
  description = "The amount of time in minutes that a node can be unhealthy before being removed from the load balancer"
  type        = number
}

variable "service_worker_count" {
  description = "The number of Workers for the App Service"
  type        = number
}

variable "service_log_retention" {
  description = "Service log retention in days"
  type        = number
}

variable "service_log_storage_sas_start" {
  description = "Service log sas token start date/time"
  type        = string
}

variable "service_log_storage_sas_expiry" {
  description = "Service log sas token start date/time"
  type        = string
}

variable "enable_monitoring" {
  description = "Create an App Insights instance and notification group for the Web App Service"
  type        = bool
}

variable "monitor_email_receivers" {
  description = "A list of email addresses that should be notified by monitoring alerts"
  type        = list(string)
}

variable "monitor_enable_slack_webhook" {
  description = "Enable slack webhooks to send monitoring notifications to a channel"
  type        = bool
}

variable "monitor_slack_webhook_receiver" {
  description = "A Slack App webhook URL"
  type        = string
}

variable "monitor_slack_channel" {
  description = "Slack channel name/id to send messages to"
  type        = string
}

variable "monitor_endpoint_healthcheck" {
  description = "Specify a route that should be monitored for a 200 OK status"
  type        = string
}

variable "virtual_network_address_space" {
  description = "Virtual Network address space CIDR"
  type        = string
}

variable "enable_cdn_frontdoor" {
  description = "Enable Azure CDN Front Door. This will use the Web App default hostname as the origin."
  type        = bool
}

variable "cdn_frontdoor_origin_fqdn_override" {
  description = "Manually specify the hostname that the CDN Front Door should target. Defaults to the App Service hostname"
  type        = string
  default     = ""
}

variable "cdn_frontdoor_sku" {
  description = "Azure CDN Front Door SKU"
  type        = string
}

variable "cdn_frontdoor_health_probe_interval" {
  description = "Specifies the number of seconds between health probes."
  type        = number
  default     = 120
}

variable "cdn_frontdoor_health_probe_path" {
  description = "Specifies the path relative to the origin that is used to determine the health of the origin."
  type        = string
  default     = "/"
}

variable "cdn_frontdoor_response_timeout" {
  description = "Azure CDN Front Door response timeout in seconds"
  type        = number
}

variable "cdn_frontdoor_host_add_response_headers" {
  description = "List of response headers to add at the CDN Front Door `[{ \"Name\" = \"Strict-Transport-Security\", \"value\" = \"max-age=31536000\" }]`"
  type        = list(map(string))
  default     = []
}

variable "cdn_frontdoor_remove_response_headers" {
  description = "List of response headers to remove at the CDN Front Door"
  type        = list(string)
  default     = []
}

variable "cdn_frontdoor_host_redirects" {
  description = "CDN FrontDoor host redirects `[{ \"from\" = \"example.com\", \"to\" = \"www.example.com\" }]`"
  type        = list(map(string))
  default     = []
}

variable "cdn_frontdoor_enable_rate_limiting" {
  description = "CDN Front Door enable rate limiting"
  type        = bool
}

variable "cdn_frontdoor_rate_limiting_duration_in_minutes" {
  description = "CDN Front Door rate limiting duration in minutes"
  type        = number
  default     = 1
}

variable "cdn_frontdoor_rate_limiting_threshold" {
  description = "Maximum number of concurrent requests before Rate Limiting policy is applied"
  type        = number
  default     = 300
}

variable "cdn_frontdoor_rate_limiting_bypass_ip_list" {
  description = "List if IP CIDRs to bypass CDN Front Door rate limiting"
  type        = list(string)
  default     = []
}

variable "restrict_web_app_service_to_cdn_inbound_only" {
  description = "Restricts access to the Web App by addin an ip restriction rule which only allows 'AzureFrontDoor.Backend' inbound and matches the cdn fdid header. It also creates a network security group that only allows 'AzureFrontDoor.Backend' inbound, and attaches it to the subnet of the web app."
  type        = bool
}

variable "tags" {
  description = "Tags to be applied to all resources"
  type        = map(string)
}
