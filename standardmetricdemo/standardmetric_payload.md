## Sample json payload sent for Standard metric

```json
{"name":"AppMetrics","time":"2021-02-17T02:29:48.0000000Z","iKey":"457e0aaf-fd2a-4e37-8df1-18c95bb66ca3","tags":{"ai.application.ver":"1.0.0.0","ai.cloud.roleInstance":"cithomasmain","ai.internal.sdkVersion":"m-agg2c:2.16.0-18277","ai.internal.nodeName":"cithomasmain.redmond.corp.microsoft.com"},"data":{"baseType":"MetricData","baseData":{"ver":2,"metrics":[{"name":"Server response time","kind":"Aggregation","value":623.7227999999994,"count":451,"min":0.1278,"max":177.9727,"stdDev":13.206579048253152}],"properties":{"Request.Success":"True","cloud/roleInstance":"cithomasmain","_MS.AggregationIntervalMs":"73000","request/performanceBucket":"<250ms","request/resultCode":"200","operation/synthetic":"False","DeveloperMode":"true","cloud/roleName":"Unknown","AspNetCoreEnvironment":"Development","_MS.MetricId":"requests/duration","_MS.IsAutocollected":"True"}}}}
```