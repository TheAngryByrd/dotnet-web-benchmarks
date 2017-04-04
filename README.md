# Dotnet Web Benchmarks

Benchmarks for various dotnet/mono web frameworks

### Requirements
* Debian 8
  * [wrk](https://github.com/wg/wrk/wiki/Installing-Wrk-on-Linux)
  * [Mono](http://www.mono-project.com/download/)
  * [Dotnet Core](https://www.microsoft.com/net/core#linuxdebian)
* OSX
  * [wrk](https://github.com/wg/wrk/wiki/Installing-wrk-on-OSX)
  * [Mono](http://www.mono-project.com/download/)
  * [Dotnet Core](https://www.microsoft.com/net/core#macos)
  
### Running
#### Locally

```
build.sh
``` 
will compile and run all the projects on port 8083.  It will then run wrk against the web server.  It will then generate a report page in reports and show them.


#### Docker

```
./dockerbuild.sh
or
docker build -t web-benchmarks . && docker run web-benchmarks
```


### Results

Take with much salt. Maybe garlic and hot sauce. These are same machine, simple route, text only responses.  

#### My System Info

Key | Value 
--- | --- 
CPU | Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz
Operating System | Unix 15.6.0.0 (OSX/macOS)
Processor Count |	8
Mono Version	| Mono JIT compiler version 4.6.2 (mono-4.6.0-branch/ac9e222 Wed Dec 14 17:02:09 EST 2016)
Dotnet Version | 1.0.1

#### My Numbers  

     WebServer | WebFramework | IsMono | TargetFramework | TotalRequests | Duration | RequestsPerSecond
     --------- | ------------ | ------ | --------------- | -------------:| --------:| -----------------:
     Kestrel   | MVC          | False  | netcoreapp1.1   |        721996 |       10 |             72199
     Kestrel   | Plain        | False  | netcoreapp1.1   |        718370 |       10 |             71837
     Kestrel   | Giraffe      | False  | netcoreapp1.1   |        691345 |       10 |             69134
     Kestrel   | Suave        | False  | netcoreapp1.1   |        602124 |       10 |             60212
     Kestrel   | Suave        | True   | net462          |        216679 |       10 |             21667
     Kestrel   | Nancy        | False  | netcoreapp1.1   |        200550 |       10 |             20055
     Kestrel   | Plain        | True   | net462          |        165394 |       10 |             16539
     Kestrel   | MVC          | True   | net462          |        144021 |       10 |             14402
     Nowin     | WebApi       | True   | net462          |        127152 |       10 |             12715
     Nowin     | Plain        | True   | net462          |        104240 |       10 |             10424
     Katana    | Plain        | True   | net462          |         71495 |       10 |              7149
     Katana    | WebApi       | True   | net462          |         56375 |       10 |              5637
     Nowin     | Freya        | True   | net462          |         54737 |       10 |              5473
     Katana    | Freya        | True   | net462          |         45958 |       10 |              4595
     Kestrel   | Nancy        | True   | net462          |         43897 |       10 |              4389
     Katana    | Nancy        | True   | net462          |         37571 |       10 |              3757
     Nowin     | Nancy        | True   | net462          |         28585 |       10 |              2858
     Suave     | Plain        | False  | netcoreapp1.1   |         20682 |       10 |              2068
     Suave     | Plain        | True   | net462          |         19897 |       10 |              1989
     Suave     | WebApi       | True   | net462          |          8470 |       10 |               847
     Suave     | Freya        | True   | net462          |          2026 |       10 |               202
     Suave     | Nancy        | True   | net462          |           314 |       10 |                31

