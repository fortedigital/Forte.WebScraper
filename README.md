#WebScraper

##Config file specification

Main body consists of one json object with list of pages as properties:

- page_name - name of your choice for this type of page

```json
{
  %page_name%: {
    ...
  },
  ...
}
```

Each page property can have 5 items:

- test - string of conditions which identify this page type; make sure that these conditions satisfy only one page type

- pageLinks - list of children pages with a selector to element from which url can be extracted

- properties - list of properties to be extracted from this page type with a selector to wanted element

- languages - list of links to this page in other languages

- pagination - selector to element containing next page link if page has pagination

```json
"test": [%condition%,...],
"pageLinks":{
  %name%: %selector%,
  ...
},
"properties":{
  %name%: %extractor%:%selector%
},
"languages":{
  %lang_identifier%: %selector%
},
"pagination":%selector%
```

Conditions:

Access page object:

```doc```

Get element using Css selector or XPath:

```text
.Css(...)
.XPath(...)
```

Access element inner text and work with it:

```.InnerText = "..."```

```InnerText``` is of type string, so you can access/call string properties/methods:

```.InnerText.StartsWith(...)```

Check url if it contains value:

```doc.UrlContains(...)```

Check page language:

```doc.Language = ...```
 
<br/>
 Extractors:
 
 - innertext extractor - extracts inner text only
 
 - innerhtml extractor - extracts inner html of an element; all content of and element (e.g. images) 
 is downloaded and path to local temporary folder is put in href place
 
 - outerhtml extractor - extracts outer html of an element; all content of and element (e.g. images) 
 is downloaded and path to local temporary folder is put in href place
 
 - download extractor - downloads item to temporary folder and prints path to output file
 
 Selectors:
 
 Both CSS and XPath are valid selectors. However XPath has to be written in special format 
 (except when using .XPath(...) condition):
 
 \*[xpath>'%path%']
 
 where %path% is valid XPath. If XPath contains quotes write them in json as escaped double quotes (\\").