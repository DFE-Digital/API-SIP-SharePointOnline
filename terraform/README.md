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
| <a name="requirement_terraform"></a> [terraform](#requirement\_terraform) | >= 1.3.7 |
| <a name="requirement_azurerm"></a> [azurerm](#requirement\_azurerm) | >= 3.41.0 |

## Providers

No providers.

## Modules

No modules.

## Resources

No resources.

## Inputs

| Name | Description | Type | Default | Required |
|------|-------------|------|---------|:--------:|
| <a name="input_azure_location"></a> [azure\_location](#input\_azure\_location) | Azure location in which to launch resources. | `string` | n/a | yes |
| <a name="input_environment"></a> [environment](#input\_environment) | Environment name. Will be used along with `project_name` as a prefix for all resources. | `string` | n/a | yes |
| <a name="input_project_name"></a> [project\_name](#input\_project\_name) | Project name. Will be used along with `environment` as a prefix for all resources. | `string` | n/a | yes |
| <a name="input_tags"></a> [tags](#input\_tags) | Tags to be applied to all resources | `map(string)` | n/a | yes |

## Outputs

No outputs.
<!-- END_TF_DOCS -->