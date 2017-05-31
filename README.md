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
CPU | Intel Core i7-4870HQ CPU 2.50GHz (Haswell)
Operating System | debian 8
Processor Count |	8
Mono Version	| Mono 4.6.2 (Stable 4.6.2.16/ac9e222)
Dotnet Version | 1.0.1

#### My Numbers  

         WebServer | WebFramework | TargetFramework | IsMono | Route | Iteration | TotalRequests | Duration | RequestsPerSecond |     Bytes | LatencyMax | LatencyMin | LatencyMean | LatencyStdDev | ErrorConnect | ErrorRead | ErrorStatus | ErrorTimeout | ErrorWrite
     --------- | ------------ | --------------- | ------ | ----- | ---------:| -------------:| --------:| -----------------:| ---------:| ----------:| ----------:| -----------:| -------------:| ------------:| ---------:| -----------:| ------------:| ----------:
     Kestrel   | Plain        | netcoreapp1.1   | False  | /     |         1 |       1060546 |       10 |            106054 | 130447158 |  259561.00 |      54.00 |     5277.47 |      14165.44 |            0 |         0 |           0 |            0 |          0
     Kestrel   | MVC          | netcoreapp1.1   | False  | /     |         1 |       1034063 |       10 |            103406 | 102372237 |  290796.00 |      52.00 |     5391.72 |      16162.06 |            0 |         0 |     1034063 |            0 |          0
     Kestrel   | Uhura        | netcoreapp1.1   | False  | /     |         1 |       1015145 |       10 |            101514 | 148211170 |  236932.00 |      61.00 |     5677.47 |      13048.55 |            0 |         0 |           0 |            0 |          0
     Kestrel   | GiraffeTask  | netcoreapp1.1   | False  | /     |         1 |       1000345 |       10 |            100034 | 131045195 |  237455.00 |      54.00 |     5700.70 |      13522.63 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Giraffe      | netcoreapp1.1   | False  | /     |         1 |        870300 |       10 |             87030 | 114009300 |  276175.00 |      96.00 |     7305.18 |      18247.46 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Suave        | netcoreapp1.1   | False  | /     |         1 |        660156 |       10 |             66015 |  69316380 |  335601.00 |     103.00 |     8879.25 |      22668.52 |            0 |         0 |           0 |            0 |          0
     Nowin     | Plain        | net462          | True   | /     |         1 |        538460 |       10 |             53846 |  67845960 |  348920.00 |      89.00 |     8135.10 |      13278.98 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Freya.Hopac  | netcoreapp1.1   | False  | /     |         1 |        296085 |       10 |             29608 |  42932325 |  593006.00 |     112.00 |    22828.04 |      54159.21 |            0 |         0 |           0 |            0 |          0
     Kestrel   | GiraffeTask  | net462          | True   | /     |         1 |        247164 |       10 |             24716 |  32378484 |  205664.00 |      49.00 |    17034.42 |      15520.47 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Uhura        | net462          | True   | /     |         1 |        245500 |       10 |             24550 |  35843000 |  210986.00 |      97.00 |    18693.84 |      17903.50 |            0 |         0 |           0 |            0 |          0
     Kestrel   | MVC          | net462          | True   | /     |         1 |        240795 |       10 |             24079 |  23838705 |  229201.00 |      84.00 |    17373.01 |      15601.54 |            0 |         0 |      240795 |            0 |          0
     Kestrel   | Plain        | net462          | True   | /     |         1 |        237962 |       10 |             23796 |  29269326 |  269492.00 |      41.00 |    17735.88 |      15899.80 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Nancy        | netcoreapp1.1   | False  | /     |         1 |        153905 |       10 |             15390 |  25240897 |  876062.00 |     135.00 |    32310.61 |      50723.56 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Giraffe      | net462          | True   | /     |         1 |        153801 |       10 |             15380 |  20147931 |  248542.00 |     490.00 |    28603.97 |      25844.67 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Suave        | net462          | True   | /     |         1 |        123342 |       10 |             12334 |  12950910 |  279379.00 |     496.00 |    33100.21 |      26666.56 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Freya        | netcoreapp1.1   | False  | /     |         1 |        106716 |       10 |             10671 |  15473820 |  496459.00 |     863.00 |    39773.58 |      39703.33 |            0 |         0 |           0 |            0 |          0
     Katana    | Plain        | net462          | True   | /     |         1 |         70586 |       10 |              7058 |  13389674 |  343293.00 |    1040.00 |    53897.57 |      17161.31 |            0 |         0 |           0 |            0 |          0
     Nowin     | WebApi       | net462          | True   | /     |         1 |         67742 |       10 |              6774 |  10296784 | 1978811.00 |     946.00 |    74797.98 |     146274.23 |            0 |         0 |           0 |           77 |          0
     Nowin     | Freya        | net462          | True   | /     |         1 |         62773 |       10 |              6277 |   8976539 | 1857936.00 |     587.00 |    92571.19 |     141911.91 |            0 |         0 |           0 |           17 |          0
     Katana    | Nancy        | net462          | True   | /     |         1 |         55927 |       10 |              5592 |  11425148 | 1385824.00 |    2168.00 |    81020.23 |      92869.33 |            0 |         0 |           0 |            0 |          0
     Nowin     | Nancy        | net462          | True   | /     |         1 |         52773 |       10 |              5277 |   7440993 | 1980466.00 |     908.00 |    85518.30 |     149136.82 |            0 |         0 |           0 |           44 |          0
     Katana    | WebApi       | net462          | True   | /     |         1 |         48523 |       10 |              4852 |  10315714 | 1161407.00 |    7885.00 |    89805.55 |      90929.35 |            0 |         0 |           0 |            0 |          0
     Nowin     | Freya.Hopac  | net462          | True   | /     |         1 |         45229 |       10 |              4522 |   6467747 | 1988240.00 |     443.00 |   105973.37 |     161455.26 |            0 |         0 |           0 |           51 |          0
     Kestrel   | Nancy        | net462          | True   | /     |         1 |         33313 |       10 |              3331 |   5463491 |  677332.00 |     248.00 |   115505.34 |      65668.00 |            0 |         0 |           0 |            0 |          0
     Kestrel   | Freya.Hopac  | net462          | True   | /     |         1 |         22410 |       10 |              2241 |   3249450 | 1995994.00 |     153.00 |   174835.15 |     259997.06 |            0 |         0 |           0 |          191 |          0
     Katana    | Freya.Hopac  | net462          | True   | /     |         1 |         22044 |       10 |              2204 |   4077371 | 1931531.00 |     610.00 |   140487.63 |     193104.16 |            0 |         0 |           0 |          234 |          0
     Kestrel   | Freya        | net462          | True   | /     |         1 |         18523 |       10 |              1852 |   2685835 |  788537.00 |    1629.00 |   204499.24 |     128339.65 |            0 |         0 |           0 |            0 |          0
     Katana    | Freya        | net462          | True   | /     |         1 |         16778 |       10 |              1677 |   3104040 | 1983555.00 |    2308.00 |   139718.56 |     129948.02 |            0 |         0 |           0 |          154 |          0
     Suave     | Plain        | net462          | True   | /     |         1 |         11429 |       10 |              1142 |   1680063 | 1999706.00 |    1729.00 |   162512.60 |     141976.08 |            0 |         0 |           0 |          101 |          0
     Suave     | Freya.Hopac  | net462          | True   | /     |         1 |          4896 |       10 |               489 |    975052 | 1979089.00 |    4356.00 |   438394.75 |     320684.19 |            0 |         0 |           0 |           34 |          0
     Suave     | WebApi       | net462          | True   | /     |         1 |          3376 |       10 |               337 |    432128 | 1884758.00 |   50812.00 |   274847.33 |     222169.69 |            0 |      3376 |           0 |           28 |          0
     Suave     | Nancy        | net462          | True   | /     |         1 |          2242 |       10 |               224 |    320606 | 1925343.00 |   68051.00 |   420300.97 |     352848.92 |            0 |      2242 |           0 |           26 |          0
     Suave     | Freya        | net462          | True   | /     |         1 |          1290 |       10 |               129 |    262320 | 1973407.00 |   47273.00 |   591340.62 |     489579.40 |            0 |         0 |           0 |           74 |          0

