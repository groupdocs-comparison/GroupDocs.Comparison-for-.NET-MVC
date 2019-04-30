# GroupDocs.Comparison for .NET MVC Example
New GroupDocs.Comparison for .NET MVC UI Example
###### version 1.8.0

[![Build status](https://ci.appveyor.com/api/projects/status/a0u7vnsndwl64krd/branch/master?svg=true)](https://ci.appveyor.com/project/egorovpavel/groupdocs-comparison-for-net-mvc/branch/master)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/b697a8607aac43d5a049697cb380b7b5)](https://www.codacy.com/app/GroupDocs/GroupDocs.Comparison-for-.NET-MVC?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=groupdocs-comparison/GroupDocs.Comparison-for-.NET-MVC&amp;utm_campaign=Badge_Grade)
[![GitHub license](https://img.shields.io/github/license/groupdocs-comparison/GroupDocs.Comparison-for-.NET-MVC.svg)](https://github.com/groupdocs-comparison/GroupDocs.Comparison-for-.NET-MVC/blob/master/LICENSE)

## System Requirements
- .NET Framework 4.5
- Visual Studio 2015


## Compare documents with .NET API

**GroupDocs.Comparison for .NET** is a library that allows you to **compare PDF, DOCX, PPT, XLS,** and over 90 other document formats. With GroupDocs.Comparison for Java you will be able to compare two or more files, perform style and text comparison and generate a detailed report with changes.

This application allows you to compare multiple documents and can be used as a standalone application or integrated as part of your project.

**Note:** without a license application will run in trial mode, purchase [GroupDocs.Comparison for .NET license](https://purchase.groupdocs.com/order-online-step-1-of-8.aspx) or request [GroupDocs.Comparison for .NET temporary license](https://purchase.groupdocs.com/temporary-license).


## Demo Video
Coming soon

## Features
#### GroupDocs.Comparison
- Clean, modern and intuitive design
- Easily switchable colour theme (create your own colour theme in 5 minutes)
- Responsive design
- Mobile support (open application on any mobile device)
- HTML and image modes
- Fully customizable navigation panel
- Compare documents
- Multi-compare several documents
- Compare password protected documents
- Upload documents
- Display clearly visible differences
- Download comparison results
- Print comparison results
- Smooth document scrolling
- Preload pages for faster document rendering
- Multi-language support for displaying errors
- Cross-browser support (Safari, Chrome, Opera, Firefox)
- Cross-platform support (Windows, Linux, MacOS)

## How to run

You can run this sample by one of following methods

#### Build from source

Download [source code](https://github.com/groupdocs-comparison/GroupDocs.Comparison-for-.NET-MVC/archive/master.zip) from github or clone this repository.

```bash
git clone https://github.com/groupdocs-comparison/GroupDocs.Comparison-for-.NET-MVC
```

Open solution in the VisualStudio.
Update common parameters in `web.config` and example related properties in the `configuration.yml` to meet your requirements.

Open http://localhost:8080/comparison in your favorite browser

#### Docker image
Use [docker](https://hub.docker.com/u/groupdocs) image.

```bash
mkdir DocumentSamples
mkdir Licenses
docker run -p 8080:8080 --env application.hostAddress=localhost -v `pwd`/DocumentSamples:/home/groupdocs/app/DocumentSamples -v `pwd`/Licenses:/home/groupdocs/app/Licenses groupdocs/comparison
## Open http://localhost:8080/comparison in your favorite browser.
```

## Configuration
For all methods above you can adjust settings in `configuration.yml`. By default in this sample will lookup for license file in `./Licenses` folder, so you can simply put your license file in that folder or specify relative/absolute path by setting `licensePath` value in `configuration.yml`. 

### Comparison configuration options

| Option                             | Type    |   Default value   | Description                                                                                                                                  |
| ---------------------------------- | ------- |:-----------------:|:-------------------------------------------------------------------------------------------------------------------------------------------- |
| **`filesDirectory`**               | String  | `DocumentSamples` | Files directory path. Indicates where uploaded and predefined files are stored. It can be absolute or relative path                          |
| **`fontsDirectory`**               | String  |                   | Path to custom fonts directory.                                                                                                              |
| **`defaultDocument`**              | String  |                   | Absolute path to default document that will be loaded automaticaly.                                                                          |
| **`preloadPageCount`**             | Integer |        `0`        | Indicate how many pages from a document should be loaded, remaining pages will be loaded on page scrolling.Set `0` to load all pages at once |
| **`multiComparing`**               | String  |      `true`       | Enable/disable multi comparing feature                                                                                                       |

## License
The MIT License (MIT). 

Please have a look at the LICENSE.md for more details

## GroupDocs Comparison on other platforms & frameworks

- [Comapre documents](https://github.com/groupdocs-comparison/GroupDocs.Comparison-for-Java-Spring) with JAVA Spring 
- [Comapre documents](https://github.com/groupdocs-comparison/GroupDocs.Comparison-for-Java-Dropwizard) with JAVA Dropwizard 
- [Comapre documents](https://github.com/groupdocs-comparison/GroupDocs.Comparison-for-.NET-MVC) with .NET MVC 
- [Comapre documents](https://github.com/groupdocs-comparison/GroupDocs.Comparison-for-.NET-WebForms) with .NET WebForms 

## Resources
- **Website:** [www.groupdocs.com](http://www.groupdocs.com)
- **Product Home:** [GroupDocs.Comparison for .NET](https://products.groupdocs.com/Comparison/net)
- **Product API References:** [GroupDocs.Comparison for .NET API](https://apireference.groupdocs.com)
- **Download:** [Download GroupDocs.Comparison for .NET](https://downloads.groupdocs.com/Comparison/net)
- **Documentation:** [GroupDocs.Comparison for .NET Documentation](https://docs.groupdocs.com/dashboard.action)
- **Free Support Forum:** [GroupDocs.Comparison for .NET Free Support Forum](https://forum.groupdocs.com/c/Comparison)
- **Paid Support Helpdesk:** [GroupDocs.Comparison for .NET Paid Support Helpdesk](https://helpdesk.groupdocs.com)
- **Blog:** [GroupDocs.Comparison for .NET Blog](https://blog.groupdocs.com/category/groupdocs-Comparison-product-family)
