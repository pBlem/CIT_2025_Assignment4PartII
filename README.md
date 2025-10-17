# Assignment 4 -- Part II

General notes on how stuff was done

## Encoding of files

Once again, there is an issue with the encoding of certain files.

```console
$ file -I Assignment4.Tests/DataServiceTests.cs
Assignment4.Tests/DataServiceTests.cs: text/plain; charset=iso-8859-1
```

Shows that the file is in *latin1*

```console
$ iconv -f ISO-8859-1 -t UTF-8 Assignment4.Tests/DataServiceTests.cs >
Assignment4.Tests/DataServiceTests.cs.utf8

$ mv Assignment4.Tests/DataServiceTests.cs.utf8
Assignment4.Tests/DataServiceTests.cs

$ file -I Assignment4.Tests/DataServiceTests.cs

Assignment4.Tests/DataServiceTests.cs: text/plain; charset=utf-8
```

This might need to be done for the other test file as well

```console
$ file -I Assignment4.Tests/WebServiceTests.cs
Assignment4.Tests/WebServiceTests.cs: text/plain; charset=utf-8
```

Turns out the second test file is fine
