# Grammophone.Domos.Tests.Music.DataAccess.EntityFrameworkCore

Entity Framework Core data access provider for the Domos music test application.

This project implements the EF Core domain container and adapter for `IMusicDomosDomainContainer`, including provider-specific model configuration for the music security and workflow tests.

## Target Framework

- `net8.0`

## Required Projects

This project expects these sibling projects to be available when building from the solution or from extracted submodules:

- `Grammophone.DataAccess.EntityFrameworkCore`
- `Grammophone.Domos.DataAccess.EntityFrameworkCore`
- `Grammophone.Domos.Tests.Music.DataAccess`
