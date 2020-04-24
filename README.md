# Ropu.AesGcm
Ropu.AesGcm is a dotnet wrapper for AesGcm from OpenSSL, does not include OpenSSL lib you will need to include it yourself.

## nuget
Ropu.AesGcm is published as a [nuget](https://www.nuget.org/packages/Ropu.AesGcm) package

```shell
dotnet add package Ropu.AesGcm
```
## Licence
Ropu.AesGcm is published under the MIT licence, contributions are welcome.

## I thought AesGcm was supported in .net standard 2.1?
Although it is in .net standard 2.1 mono have chosen not to implement it, or rather have implemented it to always PlatformNotSupportedException. This means the it doesn't work on Xamarin.Android. I would prefer to have it implemented in Mono, if you find yourself needing this wrapper please add your support to the mono issue https://github.com/mono/mono/issues/19285 so we can get it implemented where it should be.