JSON = require("scripts/JSON")
done = function(summary, latency, requests)
   io.write("------------------------------\n")
   summaryjson = JSON:encode(summary)  
   io.write(string.format("%s\n", summaryjson))

end