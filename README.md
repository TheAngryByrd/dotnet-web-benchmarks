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
     Kestrel   | Plain        | False  | netcoreapp1.1   |        739374 |       10 |             73937
     Kestrel   | Giraffe      | False  | netcoreapp1.1   |        737062 |       10 |             73706
     Kestrel   | MVC          | False  | netcoreapp1.1   |        732144 |       10 |             73214
     Kestrel   | Suave        | False  | netcoreapp1.1   |        723393 |       10 |             72339
     Kestrel   | Suave        | True   | net462          |        257297 |       10 |             25729
     Kestrel   | Nancy        | False  | netcoreapp1.1   |        217424 |       10 |             21742
     Kestrel   | Plain        | True   | net462          |        163823 |       10 |             16382
     Kestrel   | MVC          | True   | net462          |        160314 |       10 |             16031
     Nowin     | Plain        | True   | net462          |        133952 |       10 |             13395
     Katana    | Plain        | True   | net462          |        117491 |       10 |             11749
     Nowin     | WebApi       | True   | net462          |         76616 |       10 |              7661
     Nowin     | Freya        | True   | net462          |         71763 |       10 |              7176
     Katana    | WebApi       | True   | net462          |         59985 |       10 |              5998
     Katana    | Freya        | True   | net462          |         46485 |       10 |              4648
     Kestrel   | Nancy        | True   | net462          |         45196 |       10 |              4519
     Katana    | Nancy        | True   | net462          |         40739 |       10 |              4073
     Nowin     | Nancy        | True   | net462          |         32692 |       10 |              3269
     Suave     | Plain        | True   | net462          |         23180 |       10 |              2318
     Suave     | Plain        | False  | netcoreapp1.1   |         21702 |       10 |              2170
     Suave     | WebApi       | True   | net462          |          8142 |       10 |               814
     Suave     | Freya        | True   | net462          |          3096 |       10 |               309
     Suave     | Nancy        | True   | net462          |          1579 |       10 |               157
