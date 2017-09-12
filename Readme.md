# OICNet [![Build status](https://ci.appveyor.com/api/projects/status/ss24gekefu1tbis5?svg=true)](https://ci.appveyor.com/project/NZSmartie/oicnet) [![NuGet](https://img.shields.io/nuget/v/NZSmartie.OICNet.svg)](https://www.nuget.org/packages/NZSmartie.OICNet/) [![MyGet](https://img.shields.io/myget/oicnet/v/NZSmartie.OICNet.svg?label=myget)](https://www.myget.org/feed/Packages/oicnet) [![license](https://img.shields.io/github/license/NZSmartie/OICNet.svg)](https://github.com/NZSmartie/OICNet/blob/master/LICENSE)

> **Note**: Nuget will be used for stable releases and release candidates. MyGet will be used for pre-release builds straight from the build server. Sorry for the confusion with the above `-alpha` and `-pre` versions

A clean and portable library (written for .Net Standard) to discover and interact with recourses on IoT Devices supporting OpenConenctivity's (OCF) OpenInterConnect (OIC) Specification.

OIC is used to standardaise (and defragment) commmunication and access across the Internet of Things. 

This library is transport agnostic; Examples will be provided on how to implement CoAP, MQTT, HTTP in the near future

## Motive

This lirbary is built to help me learn the OIC specification and create an Xamarin.Forms App (for Windows, Android, iOS) that will communicate with my [IoTNode Project](https://github.com/NZSmartie/IotNode)

## Features
  - Encodes/Decodes 
    - [X] `appliction/cbor`
    - [X] `appliction/json`
    - [ ] `appliction/xml` (See [Support for (De)Serialisation of application/xml #2](https://github.com/NZSmartie/OICNet/issues/2))
  - [X] Resource Discovery
  - [ ] Securtiy (See [Support for OIC Security #3](https://github.com/NZSmartie/OICNet/issues/3))
  - CRUDN Operations
    - [ ] Create (POST/PUT)
    - [ ] Retreive (GET)
    - [ ] Update (POST)
    - [ ] Delete (DELETE)
    - [ ] Notify (OBSERVE)
  - IoT Data Models 
    - [ ] Enforce data validation
    - [ ] Generate objects directly from [Openinterconnect supplied JSON-Schemas](https://github.com/openconnectivityfoundation/core) (See [JSON Schemas not used as .Net Standard doesn't support Embedded Resources #1](https://github.com/NZSmartie/OICNet/issues/1))
