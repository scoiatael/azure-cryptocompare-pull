#r "Newtonsoft.Json"
open FSharp.Data
open FSharp.Interop.Dynamic
open Newtonsoft.Json
open System
open Microsoft.FSharp.Collections

let source = "min-api.cryptocompare"
let uri = "https://min-api.cryptocompare.com/data/pricemulti?fsyms=ETH,DASH,BTC&tsyms=BTC,USD,EUR"
type Response = JsonProvider<""" {"ETH":{"BTC":0.07656,"USD":320.82,"EUR":273.43},"DASH":{"BTC":0.0704,"USD":294.74,"EUR":253.17},"BTC":{"BTC":1,"USD":4186.4,"EUR":3573.83}} """>
type CryptoCompareValue = { from_currency: String; to_currency: String; value: Decimal }
type CryptoCompareDocument =
    { id : String
      values : seq<CryptoCompareValue>
      timestamp: DateTime
      uri : String }

let ResponseToValues(response: Response.Root) = 
    [|  { from_currency = "ETH"; to_currency = "BTC"; value = response.Eth.Btc}
    ;   { from_currency = "ETH"; to_currency = "USD"; value = response.Eth.Usd}
    ;   { from_currency = "ETH"; to_currency = "EUR"; value = response.Eth.Eur}
    ;   { from_currency = "DASH"; to_currency = "BTC"; value = response.Dash.Btc}
    ;   { from_currency = "DASH"; to_currency = "USD"; value = response.Dash.Usd}
    ;   { from_currency = "DASH"; to_currency = "EUR"; value = response.Dash.Eur}
    ;   { from_currency = "BTC"; to_currency = "USD"; value = response.Btc.Usd}
    ;   { from_currency = "BTC"; to_currency = "EUR"; value = response.Btc.Eur}
    |]

let Run(myTimer: TimerInfo, cryptoCompareDocument: byref<obj>, log: TraceWriter) =
    let now = DateTime.Now
    let unix =  DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()
    let id = sprintf "%s=%d" source unix
    let response = Response.Load(uri)
    log.Info(
            (sprintf "Received ETH->BTC %f: saving under %s" response.Eth.Btc id),
            (now.ToString()))
    cryptoCompareDocument <- {
            id = id
            values = ResponseToValues(response)
            timestamp = now
            uri = uri
    }