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

+---------------------------+---------------+--------------+-------+
| ProjectName               | TotalRequests | Duration (s) | Req/s |
+---------------------------+---------------+--------------+-------+
| KestrelPlain              | 793996        | 10           | 79399 |
+---------------------------+---------------+--------------+-------+
| AspNetCoreLambdaOnKestrel | 790936        | 10           | 79093 |
+---------------------------+---------------+--------------+-------+
| SuaveOnKestrel            | 738309        | 10           | 73830 |
+---------------------------+---------------+--------------+-------+
| MvcOnKestrel              | 716942        | 10           | 71694 |
+---------------------------+---------------+--------------+-------+
| NancyOnKestrel            | 215632        | 10           | 21563 |
+---------------------------+---------------+--------------+-------+
| NowinOnMono               | 179826        | 10           | 17982 |
+---------------------------+---------------+--------------+-------+
| KatanaPlain               | 118931        | 10           | 11893 |
+---------------------------+---------------+--------------+-------+
| FreyaOnNowin              | 68510         | 10           | 6851  |
+---------------------------+---------------+--------------+-------+
| WebApiOnNowin             | 66755         | 10           | 6675  |
+---------------------------+---------------+--------------+-------+
| FreyaOnKatana             | 55295         | 10           | 5529  |
+---------------------------+---------------+--------------+-------+
| WebApiOnKatana            | 54866         | 10           | 5486  |
+---------------------------+---------------+--------------+-------+
| NancyOnKatana             | 53558         | 10           | 5355  |
+---------------------------+---------------+--------------+-------+
| SuaveOnMono               | 24082         | 10           | 2408  |
+---------------------------+---------------+--------------+-------+
| SuaveOnCoreCLR            | 20153         | 10           | 2015  |
+---------------------------+---------------+--------------+-------+
| NancyOnSuave              | 6703          | 10           | 670   |
+---------------------------+---------------+--------------+-------+
| NancyOnNowin              | 0             | 10           | 0     |
+---------------------------+---------------+--------------+-------+