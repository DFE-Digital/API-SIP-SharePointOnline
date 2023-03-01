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

variable "service_plan_sku" {
  description = "Service plan sku"
  type        = string
}

variable "service_dotnet_version" {
  description = "Service dotnet version"
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

variable "service_log_level" {
  description = "Service log level"
  type        = string
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

variable "virtual_network_address_space" {
  description = "Virtual Network address space CIDR"
  type        = string
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

variable "tags" {
  description = "Tags to be applied to all resources"
  type        = map(string)
}
