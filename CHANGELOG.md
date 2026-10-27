# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.0.0] - Unreleased

### Changed

- **Breaking:** Targets .NET 10 only. Support for .NET 6 and .NET 8 has been dropped.
- **Breaking:** Types previously in the `Ranger.PasswordHash.Core` namespace, such as
  `PasswordHashConfiguration` and `PasswordHashServiceCollectionExtensions`, now live in
  `Ranger.PasswordHash`. Remove any `using Ranger.PasswordHash.Core;` directives.
- **Breaking:** `Pbkdfv2Base` moved to the `Ranger.PasswordHash.Algorithms` namespace.
- License changed from MIT to MIT No Attribution (MIT-0). Attribution is no longer required.
- Updated `Microsoft.Extensions.DependencyInjection.Abstractions` to 10.0.0.
- PBKDFv2 hashing now uses `Rfc2898DeriveBytes.Pbkdf2`. Output is unchanged, so existing
  stored hashes continue to validate.

## [2.0.0] - 2023-12-09

### Added

- Dependency injection support through `AddDynamicPasswordHashing()` on `IServiceCollection`.
- `IPasswordHashConfiguration` for registering custom algorithms, choosing a preferred
  algorithm, and opting out of the built-in algorithms with `UseOnlyExternalAlgorithms()`.
- `PBKDFv2-610k` algorithm (610,000 iterations).
- `IPasswordSaltGenerator` to generate a salt when a request does not supply one.
- `IsObsolete` on hash results, so callers know when to rehash with the preferred algorithm.

### Changed

- **Breaking:** `AgilePasswordHash` and `PasswordAlgorithmRepo` replaced by `PasswordHashService`
  (`IPasswordHash`).
- **Breaking:** `PasswordHashResponse` replaced by `PasswordHashResult`.
- Targets .NET 6 and .NET 8 instead of .NET Standard 2.0.

## [1.0.0] - 2020-07-19

### Added

- Initial release with the `PBKDFv2-10k` algorithm and swappable algorithm support.
- Targets .NET Standard 2.0.

[3.0.0]: https://github.com/UnholyRanger/Ranger.PasswordHash/compare/3471c8c...HEAD
[2.0.0]: https://github.com/UnholyRanger/Ranger.PasswordHash/compare/b773df2...3471c8c
[1.0.0]: https://github.com/UnholyRanger/Ranger.PasswordHash/commit/b773df2
