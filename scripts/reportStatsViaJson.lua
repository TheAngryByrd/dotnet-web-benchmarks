JSON = require("scripts/JSON")
done = function(summary, latency, requests)
   io.write("------------------------------\n")
   simpleLatency = {
    min = latency.min,
    max = latency.max,           
    mean = latency.mean,            
    stdev =latency.stdev            
   }
   io.write(string.format("%s\n", JSON:encode(simpleLatency)))
   io.write(string.format("%s\n", JSON:encode(summary)))

end