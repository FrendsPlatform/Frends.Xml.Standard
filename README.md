- [Frends.Xml.Standard](#frendsxmlstandard)
  - [Installing](#installing)
  - [Building](#building)
  - [Contributing](#contributing)
  - [Documentation](#documentation)
    - [Xml.Standard.XpathQuery](#xmlstandardxpathquery)
      - [Input](#input)
      - [Options](#options)
      - [Result](#result)
    - [Xml.Standard.XpathQuerySingle](#xmlstandardxpathquerysingle)
      - [Input](#input-1)
      - [Options](#options-1)
      - [Result](#result-1)
    - [Xml.Standard.Transform](#xmlstandardtransform)
      - [Input](#input-2)
      - [Result](#result-2)
    - [Xml.Standard.ConvertJsonToXml](#xmlstandardconvertjsontoxml)
      - [Input](#input-3)
      - [Result](#result-3)
  - [License](#license)

# Frends.Xml.Standard

FRENDS XML processing Tasks compatible with .NET Standard.
This version supports .NET Standard which has only XPath and XSLT version 1 support for now because no there is no library or built-in support for v2 and v3.

## Installing

You can install the task via FRENDS UI Task view, by searching for packages. You can also download the latest NuGet package from https://www.myget.org/feed/frends/package/nuget/Frends.Xml.Standard and import it manually via the Task view.

## Building

Clone a copy of the repo

`git clone https://github.com/FrendsPlatform/Frends.Xml.Standard.git`

Restore dependencies

`nuget restore frends.xml.standard`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Xml.Standard\Frends.Xml.Standard.Tests\bin\Release\netcoreapp3.0\Frends.Xml.Standard.Tests.dll`

## Contributing

When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

## Documentation

### Xml.Standard.XpathQuery

Query XML with XPath (version 1) and return a list of results.

#### Input

| Property        | Type     | Description       |
|-----------------|----------|-------------------|
| Xml             | string   | XML to be queried | 
| XpathQuery      | string   | XPath query       |

#### Options

| Property                 | Type             | Description                                    |
|--------------------------|------------------|------------------------------------------------|
| ThrowErrorOnEmptyResults | bool             | Task will throw an exception if no results found |


#### Result

| Property/Method   | Type           | Description                 |
|-------------------|----------------|-----------------------------|
| Data              | List<object>   | List of query results. Object type depends on the query. If selecting a node the type will be string and contain the xml node.  |
| ToJson()          | List<JToken>   | Returns a JSON representation of the xml data. It is possible to access this date by dot notation. `#result.ToJson()[0].Foo.Bar` |
| ToJson(int index) | JToken         | Returns a single result as JSON  |

### Xml.Standard.XpathQuerySingle

Query XML with XPath (version 1) and return a single result.

#### Input

| Property        | Type     | Description       |
|-----------------|----------|-------------------|
| Xml             | string   | XML to be queried | 
| XpathQuery      | string   | XPath query       |

#### Options

| Property                 | Type             | Description                                    |
|--------------------------|------------------|------------------------------------------------|
| ThrowErrorOnEmptyResults | bool             | Task will throw an exception if no results found |


#### Result

| Property/Method   | Type           | Description                 |
|-------------------|----------------|-----------------------------|
| Data              | object         | Object type depends on the query. If selecting a node the type will be string and contain the xml node.  |
| ToJson()          | JToken         | Returns a JSON representation of the xml data. It is possible to access this date by dot notation. `#result.ToJson().Foo.Bar` |

### Xml.Standard.Transform 

Create a XSLT transformation.

#### Input

| Property        | Type                             | Description                  |
|-----------------|----------------------------------|------------------------------|
| Xml             | string                           |                              | 
| Xslt            | string                           |                              | 
| XsltParameters  | List {string Name, string Value} |                              |

#### Result
string

### Xml.Standard.ConvertJsonToXml

This task takes JSON text and deserializes it into an xml text.
Because valid XML must have one root element, the JSON passed to the task should have one property in the root JSON object. If the root JSON object has multiple properties, then the XmlRootElementName should be used. A root element with that name will be inserted into the XML text.

Example input json: 
```json
{
  "?xml": {
    "@version": "1.0",
    "@standalone": "no"
  },
  "root": {
    "person": [
      {
        "@id": "1",
        "name": "Alan"
      },
      {
        "@id": "2",
        "name": "Louis"
      }
    ]
  }
}
````

Example result:
```xml
<?xml version="1.0" standalone="no"?>
 <root>
   <person id="1">
     <name>Alan</name>
   </person>
   <person id="2">
     <name>Louis</name>
   </person>
</root>
```
#### Input

| Property        | Type      | Description                                  |
|-----------------|-----------|----------------------------------------------|
| Json             | string   | Json string to be converted to XML  |
| XmlRootElementName      | string  | The name for the root XML element |


#### Result

string

## License

This project is licensed under the MIT License - see the LICENSE file for details
