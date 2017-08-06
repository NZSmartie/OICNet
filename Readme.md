# OICNet [![Build Status](https://travis-ci.org/NZSmartie/OICNet.svg?branch=master)](https://travis-ci.org/NZSmartie/OICNet) [![NuGet version](https://badge.fury.io/nu/NZSmartie.OICNet.svg)](https://www.nuget.org/packages/NZSmartie.OICNet/)

A clean and portable library to allow C# developers to discover and access recourses on devices supporting OpenConenctivity's (OCF) OpenInterConnect (OIC) Specification.

OIC is used to standardaise the commmunication and access to resources across the Internet of Things. 

## Motive

This lirbary is built to help me learn the OIC spec and create an Android client that will communicate with my [IoTNode Project](https://github.com/NZSmartie/IotNode)

## Issues

  - [JSON Schemas not used as .Net Standard doesn't support Embedded Resources](https://github.com/NZSmartie/OICNet/issues/1)

Serialisation will follow hardcoded classes instead being based on JSON schemas since it's not possible to compile the JSON schemas into the lirbary (Waiting on .NEt Standard 2.0 to be released)