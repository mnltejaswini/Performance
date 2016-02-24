The pooling levels are controled by the settings in `hosting.json`

If the values are not present they are set to zero which means no pooling; this is the default

`kestrel.maxPooledStreams` - how many Response+Request+Duplex stream objects Kestrel will at max have pooled
`kestrel.maxPooledHeaders` - how many Response+Request header objects Kestrel will at max have pooled

It also has a `PooledHttpContext` and `PooledHttpContextFactory` in the project, which pool the `DefaultHttpRequest` and `DefaultHttpResponse` objects

The pooling of the HttpContext checks the following setting

`hosting.maxPooledContexts` which is the maximum number that will be pooled
