This documentation covers the deployment of the infrastructure to host the app.

## Azure infrastructure

The infrastructure is managed using [Terraform](https://www.terraform.io/).<br>
The state is stored remotely in encrypted Azure storage.<br>
[Terraform workspaces](https://www.terraform.io/docs/state/workspaces.html) are used to separate environments.

#### Configuring the storage backend

The Terraform state is stored remotely in Azure, this allows multiple team members to
make changes and means the state file is backed up. The state file contains
sensitive information so access to it should be restricted, and it should be stored
encrypted at rest.

##### Create a new storage backend

This step only needs to be done once per project (eg. not per environment).
If it has already been created, obtain the storage backend attributes and skip to the next step.

The [Azure tutorial](https://docs.microsoft.com/en-us/azure/developer/terraform/store-state-in-azure-storage) outlines the steps to create a storage account and container for the state file. You will need:

- resource_group_name: The name of the resource group used for the Azure Storage account.
- storage_account_name: The name of the Azure Storage account.
- container_name: The name of the blob container.
- key: The name of the state store file to be created.

##### Create a backend configuration file

Create a new file named `backend.vars` with the following content:

```
subscription_id      = [the id of the Azure subscription]
resource_group_name  = [the name of the Azure resource group]
storage_account_name = [the name of the Azure Storage account]
container_name       = [the name of the blob container]
key                  = "terraform.tstate"
```

##### Install dependencies

We can use [Homebrew](https://brew.sh) to install the dependecies we need to deploy the infrastructure (eg. tfenv, Azure cli).
These are listed in the `Brewfile`

to install, run:

```
$ brew bundle
```

##### Log into azure with the Azure CLI

Log in to your account:

```
$ az login
```

Confirm which account you are currently using:

```
$ az account show
```

To list the available subscriptions, run:

```
$ az account list
```

Then if needed, switch to it using the 'id':

```
$ az account set --subscription <id>
```

##### Initialise Terraform

Install the required terraform version with the Terraform version manager `tfenv`:

```
$ tfenv install
```

Initialize Terraform to download the required Terraform modules and configure the remote state backend
to use the settings you specified in the previous step.

`$ terraform init -backend-config=backend.vars`

##### Create a Terraform variables file

Each environment will need it's own `tfvars` file.

Copy the `terraform.tfvars.example` to `environment-name.tfvars` and modify the contents as required

##### Create the infrastructure

Now Terraform has been initialised you can create a workspace if needed:

`$ terraform workspace new staging`

Or to check what workspaces already exist:

`$ terraform workspace list`

Switch to the new or existing workspace:

`$ terraform workspace select staging`

Plan the changes:

`$ terraform plan -var-file=staging.tfvars`

Terraform will ask you to provide any variables not specified in an `*.auto.tfvars` file.
Now you can run:

`$ terraform apply -var-file=staging.tfvars`

If everything looks good, answer `yes` and wait for the new infrastructure to be created.

##### Azure resources

<!-- BEGIN_TF_DOCS -->
## Requirements

| Name | Version |
|------|---------|
| <a name="requirement_terraform"></a> [terraform](#requirement\_terraform) | ~> 1.9 |
| <a name="requirement_azapi"></a> [azapi](#requirement\_azapi) | ~> 1.13 |
| <a name="requirement_azurerm"></a> [azurerm](#requirement\_azurerm) | ~> 4.0 |
| <a name="requirement_statuscake"></a> [statuscake](#requirement\_statuscake) | ~> 2.1 |

## Providers

No providers.

## Modules

| Name | Source | Version |
|------|--------|---------|
| <a name="module_azure_web_app_services_hosting"></a> [azure\_web\_app\_services\_hosting](#module\_azure\_web\_app\_services\_hosting) | github.com/DFE-Digital/terraform-azurerm-web-app-services-hosting | v0.8.0 |
| <a name="module_azurerm_key_vault"></a> [azurerm\_key\_vault](#module\_azurerm\_key\_vault) | github.com/DFE-Digital/terraform-azurerm-key-vault-tfvars | v0.5.1 |
| <a name="module_statuscake-tls-monitor"></a> [statuscake-tls-monitor](#module\_statuscake-tls-monitor) | github.com/dfe-digital/terraform-statuscake-tls-monitor | v0.1.5 |

## Resources

No resources.

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| <a name="input_azure_client_id"></a> [azure\_client\_id](#input\_azure\_client\_id) | Service Principal Client ID | `string` | n/a | yes |
| <a name="input_azure_client_secret"></a> [azure\_client\_secret](#input\_azure\_client\_secret) | Service Principal Client Secret | `string` | n/a | yes |
| <a name="input_azure_location"></a> [azure\_location](#input\_azure\_location) | Azure location in which to launch resources. | `string` | n/a | yes |
| <a name="input_azure_subscription_id"></a> [azure\_subscription\_id](#input\_azure\_subscription\_id) | Service Principal Subscription ID | `string` | n/a | yes |
| <a name="input_azure_tenant_id"></a> [azure\_tenant\_id](#input\_azure\_tenant\_id) | Service Principal Tenant ID | `string` | n/a | yes |
| <a name="input_cdn_frontdoor_enable_rate_limiting"></a> [cdn\_frontdoor\_enable\_rate\_limiting](#input\_cdn\_frontdoor\_enable\_rate\_limiting) | CDN Front Door enable rate limiting | `bool` | n/a | yes |
| <a name="input_cdn_frontdoor_forwarding_protocol"></a> [cdn\_frontdoor\_forwarding\_protocol](#input\_cdn\_frontdoor\_forwarding\_protocol) | Azure CDN Front Door forwarding protocol | `string` | `"HttpsOnly"` | no |
| <a name="input_cdn_frontdoor_health_probe_interval"></a> [cdn\_frontdoor\_health\_probe\_interval](#input\_cdn\_frontdoor\_health\_probe\_interval) | Specifies the number of seconds between health probes. | `number` | `120` | no |
| <a name="input_cdn_frontdoor_health_probe_path"></a> [cdn\_frontdoor\_health\_probe\_path](#input\_cdn\_frontdoor\_health\_probe\_path) | Specifies the path relative to the origin that is used to determine the health of the origin. | `string` | `"/"` | no |
| <a name="input_cdn_frontdoor_host_add_response_headers"></a> [cdn\_frontdoor\_host\_add\_response\_headers](#input\_cdn\_frontdoor\_host\_add\_response\_headers) | List of response headers to add at the CDN Front Door `[{ "Name" = "Strict-Transport-Security", "value" = "max-age=31536000" }]` | `list(map(string))` | `[]` | no |
| <a name="input_cdn_frontdoor_host_redirects"></a> [cdn\_frontdoor\_host\_redirects](#input\_cdn\_frontdoor\_host\_redirects) | CDN FrontDoor host redirects `[{ "from" = "example.com", "to" = "www.example.com" }]` | `list(map(string))` | `[]` | no |
| <a name="input_cdn_frontdoor_origin_fqdn_override"></a> [cdn\_frontdoor\_origin\_fqdn\_override](#input\_cdn\_frontdoor\_origin\_fqdn\_override) | Manually specify the hostname that the CDN Front Door should target. Defaults to the App Service hostname | `string` | `""` | no |
| <a name="input_cdn_frontdoor_origin_host_header_override"></a> [cdn\_frontdoor\_origin\_host\_header\_override](#input\_cdn\_frontdoor\_origin\_host\_header\_override) | Manually specify the host header that the CDN sends to the target. Defaults to the recieved host header. Set to null to set it to the host\_name (`cdn_frontdoor_origin_fqdn_override`) | `string` | `""` | no |
| <a name="input_cdn_frontdoor_rate_limiting_duration_in_minutes"></a> [cdn\_frontdoor\_rate\_limiting\_duration\_in\_minutes](#input\_cdn\_frontdoor\_rate\_limiting\_duration\_in\_minutes) | CDN Front Door rate limiting duration in minutes | `number` | `1` | no |
| <a name="input_cdn_frontdoor_rate_limiting_threshold"></a> [cdn\_frontdoor\_rate\_limiting\_threshold](#input\_cdn\_frontdoor\_rate\_limiting\_threshold) | Maximum number of concurrent requests before Rate Limiting policy is applied | `number` | `300` | no |
| <a name="input_cdn_frontdoor_remove_response_headers"></a> [cdn\_frontdoor\_remove\_response\_headers](#input\_cdn\_frontdoor\_remove\_response\_headers) | List of response headers to remove at the CDN Front Door | `list(string)` | `[]` | no |
| <a name="input_cdn_frontdoor_response_timeout"></a> [cdn\_frontdoor\_response\_timeout](#input\_cdn\_frontdoor\_response\_timeout) | Azure CDN Front Door response timeout in seconds | `number` | n/a | yes |
| <a name="input_cdn_frontdoor_sku"></a> [cdn\_frontdoor\_sku](#input\_cdn\_frontdoor\_sku) | Azure CDN Front Door SKU | `string` | n/a | yes |
| <a name="input_enable_cdn_frontdoor"></a> [enable\_cdn\_frontdoor](#input\_enable\_cdn\_frontdoor) | Enable Azure CDN Front Door. This will use the Web App default hostname as the origin. | `bool` | n/a | yes |
| <a name="input_enable_cdn_frontdoor_health_probe"></a> [enable\_cdn\_frontdoor\_health\_probe](#input\_enable\_cdn\_frontdoor\_health\_probe) | Enable CDN Front Door health probe | `bool` | `false` | no |
| <a name="input_enable_event_hub"></a> [enable\_event\_hub](#input\_enable\_event\_hub) | Send App Service logs to an Event Hub sink | `bool` | `false` | no |
| <a name="input_enable_logstash_consumer"></a> [enable\_logstash\_consumer](#input\_enable\_logstash\_consumer) | Create an Event Hub consumer group for Logstash | `bool` | `false` | no |
| <a name="input_enable_monitoring"></a> [enable\_monitoring](#input\_enable\_monitoring) | Create an App Insights instance and notification group for the Web App Service | `bool` | n/a | yes |
| <a name="input_environment"></a> [environment](#input\_environment) | Environment name. Will be used along with `project_name` as a prefix for all resources. | `string` | n/a | yes |
| <a name="input_eventhub_export_log_analytics_table_names"></a> [eventhub\_export\_log\_analytics\_table\_names](#input\_eventhub\_export\_log\_analytics\_table\_names) | List of Log Analytics table names that you want to export to Event Hub. See https://learn.microsoft.com/en-gb/azure/azure-monitor/logs/logs-data-export?tabs=portal#supported-tables for a list of supported tables | `list(string)` | `[]` | no |
| <a name="input_existing_logic_app_workflow"></a> [existing\_logic\_app\_workflow](#input\_existing\_logic\_app\_workflow) | Name, and Resource Group of an existing Logic App Workflow. Leave empty to create a new Resource | <pre>object({<br/>    name : string<br/>    resource_group_name : string<br/>  })</pre> | <pre>{<br/>  "name": "",<br/>  "resource_group_name": ""<br/>}</pre> | no |
| <a name="input_key_vault_access_ipv4"></a> [key\_vault\_access\_ipv4](#input\_key\_vault\_access\_ipv4) | List of IPv4 Addresses that are permitted to access the Key Vault | `list(string)` | n/a | yes |
| <a name="input_launch_in_vnet"></a> [launch\_in\_vnet](#input\_launch\_in\_vnet) | Conditionally launch into a VNet | `bool` | n/a | yes |
| <a name="input_monitor_email_receivers"></a> [monitor\_email\_receivers](#input\_monitor\_email\_receivers) | A list of email addresses that should be notified by monitoring alerts | `list(string)` | n/a | yes |
| <a name="input_monitor_endpoint_healthcheck"></a> [monitor\_endpoint\_healthcheck](#input\_monitor\_endpoint\_healthcheck) | Specify a route that should be monitored for a 200 OK status | `string` | n/a | yes |
| <a name="input_project_name"></a> [project\_name](#input\_project\_name) | Project name. Will be used along with `environment` as a prefix for all resources. | `string` | n/a | yes |
| <a name="input_restrict_web_app_service_to_cdn_inbound_only"></a> [restrict\_web\_app\_service\_to\_cdn\_inbound\_only](#input\_restrict\_web\_app\_service\_to\_cdn\_inbound\_only) | Restricts access to the Web App by addin an ip restriction rule which only allows 'AzureFrontDoor.Backend' inbound and matches the cdn fdid header. It also creates a network security group that only allows 'AzureFrontDoor.Backend' inbound, and attaches it to the subnet of the web app. | `bool` | n/a | yes |
| <a name="input_service_app_settings"></a> [service\_app\_settings](#input\_service\_app\_settings) | Service app settings | `map(string)` | n/a | yes |
| <a name="input_service_health_check_path"></a> [service\_health\_check\_path](#input\_service\_health\_check\_path) | Service health check path | `string` | n/a | yes |
| <a name="input_service_log_ipv4_allow_list"></a> [service\_log\_ipv4\_allow\_list](#input\_service\_log\_ipv4\_allow\_list) | IPv4 addresses that are authorised to modify the Log storage account | `list(string)` | `[]` | no |
| <a name="input_service_log_storage_sas_expiry"></a> [service\_log\_storage\_sas\_expiry](#input\_service\_log\_storage\_sas\_expiry) | Service log sas token start date/time | `string` | n/a | yes |
| <a name="input_service_log_storage_sas_start"></a> [service\_log\_storage\_sas\_start](#input\_service\_log\_storage\_sas\_start) | Service log sas token start date/time | `string` | n/a | yes |
| <a name="input_service_plan_os"></a> [service\_plan\_os](#input\_service\_plan\_os) | Service plan operating system. Valid values are `Windows` or `Linux`. | `string` | n/a | yes |
| <a name="input_service_plan_sku"></a> [service\_plan\_sku](#input\_service\_plan\_sku) | Service plan sku | `string` | n/a | yes |
| <a name="input_service_stack"></a> [service\_stack](#input\_service\_stack) | The application stack for the web app. Valid values are `dotnet`, `dotnetcore`, `node`, `python`, `php`, `java`, `ruby` or `go`. | `string` | n/a | yes |
| <a name="input_service_stack_version"></a> [service\_stack\_version](#input\_service\_stack\_version) | Service stack version | `string` | n/a | yes |
| <a name="input_service_worker_count"></a> [service\_worker\_count](#input\_service\_worker\_count) | The number of Workers for the App Service | `number` | n/a | yes |
| <a name="input_statuscake_api_token"></a> [statuscake\_api\_token](#input\_statuscake\_api\_token) | API token for StatusCake | `string` | `"00000000000000000000000000000"` | no |
| <a name="input_statuscake_contact_group_email_addresses"></a> [statuscake\_contact\_group\_email\_addresses](#input\_statuscake\_contact\_group\_email\_addresses) | List of email address that should receive notifications from StatusCake | `list(string)` | `[]` | no |
| <a name="input_statuscake_contact_group_integrations"></a> [statuscake\_contact\_group\_integrations](#input\_statuscake\_contact\_group\_integrations) | List of Integration IDs to connect to your Contact Group | `list(string)` | `[]` | no |
| <a name="input_statuscake_contact_group_name"></a> [statuscake\_contact\_group\_name](#input\_statuscake\_contact\_group\_name) | Name of the contact group in StatusCake | `string` | `""` | no |
| <a name="input_statuscake_monitored_resource_addresses"></a> [statuscake\_monitored\_resource\_addresses](#input\_statuscake\_monitored\_resource\_addresses) | The URLs to perform TLS checks on | `list(string)` | `[]` | no |
| <a name="input_tags"></a> [tags](#input\_tags) | Tags to be applied to all resources | `map(string)` | n/a | yes |
| <a name="input_tfvars_filename"></a> [tfvars\_filename](#input\_tfvars\_filename) | tfvars filename. This file is uploaded and stored encrupted within Key Vault, to ensure that the latest tfvars are stored in a shared place. | `string` | n/a | yes |
| <a name="input_virtual_network_address_space"></a> [virtual\_network\_address\_space](#input\_virtual\_network\_address\_space) | Virtual Network address space CIDR | `string` | n/a | yes |
| <a name="input_web_app_service_allow_ips_inbound"></a> [web\_app\_service\_allow\_ips\_inbound](#input\_web\_app\_service\_allow\_ips\_inbound) | Restricts access to the Web App by creating a network security group rule that only allow inbound traffic from the provided list of IPs | `list(string)` | `[]` | no |

## Outputs

No outputs.
<!-- END_TF_DOCS -->
