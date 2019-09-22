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

Each page property can have 3 items:

- test - list of conditions which identify this page type; make sure that these conditions satisfy only one page type

- pageLinks - list of children pages with a selector to element from which url can be extracted

- properties - list of properties to be extracted from this page type with a selector to wanted element

```json
"test": [%condition%,...],
"pageLinks":{
  %name%: %selector%,
  ...
},
"properties":{
  %name%: %extractor%:%selector%
}
```

Conditions:

Here are all valid conditions:

- value equal/not equal condition - checks if inner html of this element is equal to value

    ```text
       value:%selector% == %value%
       value:%selector% != %value%
    ```
    
- element exists/not exists condition - checks if element  exists for given selector

    ```text
       exists:%selector%
       notexists:%selector%
    ```
    
- url contains/not contains string - checks if url contains given substring

    ```text
       urlcontains:%value%
       urlnotcontains:%value%
    ```
 
 Extractors:
 
 - innerhtml extractor - extracts inner html of an element
 
 Selectors:
 
 Both CSS and XPath are valid selectors. However XPath has to be written in special format:
 
 \*[xpath>'%path%']
 
 where %path% is valid XPath. If XPath contains quotes write them in json as escaped double quotes (\\") 