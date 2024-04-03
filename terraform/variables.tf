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

variable "key_vault_access_ipv4" {
  description = "List of IPv4 Addresses that are permitted to access the Key Vault"
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

variable "service_worker_count" {
  description = "The number of Workers for the App Service"
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

variable "existing_logic_app_workflow" {
  description = "Name, and Resource Group of an existing Logic App Workflow. Leave empty to create a new Resource"
  type = object({
    name : string
    resource_group_name : string
  })
  default = {
    name                = ""
    resource_group_name = ""
  }
}

variable "monitor_endpoint_healthcheck" {
  description = "Specify a route that should be monitored for a 200 OK status"
  type        = string
}

variable "virtual_network_address_space" {
  description = "Virtual Network address space CIDR"
  type        = string
}

variable "web_app_service_allow_ips_inbound" {
  description = "Restricts access to the Web App by creating a network security group rule that only allow inbound traffic from the provided list of IPs"
  type        = list(string)
  default     = []
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

variable "cdn_frontdoor_origin_host_header_override" {
  description = "Manually specify the host header that the CDN sends to the target. Defaults to the recieved host header. Set to null to set it to the host_name (`cdn_frontdoor_origin_fqdn_override`)"
  type        = string
  default     = ""
  nullable    = true
}

variable "cdn_frontdoor_forwarding_protocol" {
  description = "Azure CDN Front Door forwarding protocol"
  type        = string
  default     = "HttpsOnly"
}

variable "cdn_frontdoor_sku" {
  description = "Azure CDN Front Door SKU"
  type        = string
}

variable "enable_cdn_frontdoor_health_probe" {
  description = "Enable CDN Front Door health probe"
  type        = bool
  default     = false
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

variable "restrict_web_app_service_to_cdn_inbound_only" {
  description = "Restricts access to the Web App by addin an ip restriction rule which only allows 'AzureFrontDoor.Backend' inbound and matches the cdn fdid header. It also creates a network security group that only allows 'AzureFrontDoor.Backend' inbound, and attaches it to the subnet of the web app."
  type        = bool
}

variable "tags" {
  description = "Tags to be applied to all resources"
  type        = map(string)
}

variable "enable_event_hub" {
  description = "Send App Service logs to an Event Hub sink"
  type        = bool
  default     = false
}

variable "enable_logstash_consumer" {
  description = "Create an Event Hub consumer group for Logstash"
  type        = bool
  default     = false
}

variable "eventhub_export_log_analytics_table_names" {
  description = "List of Log Analytics table names that you want to export to Event Hub. See https://learn.microsoft.com/en-gb/azure/azure-monitor/logs/logs-data-export?tabs=portal#supported-tables for a list of supported tables"
  type        = list(string)
  default     = []
}

variable "service_log_ipv4_allow_list" {
  description = "IPv4 addresses that are authorised to modify the Log storage account"
  type        = list(string)
  default     = []
}

variable "statuscake_api_token" {
  description = "API token for StatusCake"
  type        = string
  sensitive   = true
  default     = "00000000000000000000000000000"
}

variable "statuscake_contact_group_name" {
  description = "Name of the contact group in StatusCake"
  type        = string
  default     = ""
}

variable "statuscake_contact_group_integrations" {
  description = "List of Integration IDs to connect to your Contact Group"
  type        = list(string)
  default     = []
}

variable "statuscake_monitored_resource_addresses" {
  description = "The URLs to perform TLS checks on"
  type        = list(string)
  default     = []
}

variable "statuscake_contact_group_email_addresses" {
  description = "List of email address that should receive notifications from StatusCake"
  type        = list(string)
  default     = []
}
