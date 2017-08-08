# OICNet [![Travis CI Build Status](https://travis-ci.org/NZSmartie/OICNet.svg?branch=master)](https://travis-ci.org/NZSmartie/OICNet) [![Visual Studio Online Build Status](https://nzsmartie.visualstudio.com/_apis/public/build/definitions/47411984-aff3-4bd4-b530-cd3d59876ef2/1/badge)]() [![NuGet version](https://badge.fury.io/nu/NZSmartie.OICNet.svg)](https://www.nuget.org/packages/NZSmartie.OICNet/)

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
