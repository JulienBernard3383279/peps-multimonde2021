using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Avapi.AvapiTIME_SERIES_DAILY_ADJUSTED;
using Avapi.AvapiDIGITAL_CURRENCY_DAILY;
using Avapi;

namespace PEPS_Beta.Models
{
    // https://www.alpha-vantage.community/post/world-indexes-9627480
    // ALPHA VANTAGE API KEY : CJ7QO45PVDJ8371Q
    // WRAPPER C# : AVAPI
    // AVAPI Doc : https://github.com/AvapiDotNet/Avapi/wiki
    
#pragma warning disable CS0436 // Le type est en conflit avec le type importé
    public class HtmlDataRetriever
    {
        /* ASX ESTOX FTSE SP500 N225 Hang */
        #region properties
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content ASX { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content ESTOX { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content FTSE { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content SP500 { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content N225 { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content HANG { get; private set; }

        /* EURUSD EURAUD EURGBP EURJPY EURHKD
         * 
         * Some data need conversion 
         */


        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content EURUSD { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content AUDUSD { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content EURGBP { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content EURJPY { get; private set; }
        public IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED_Content USDHKD { get; private set; }
        #endregion

        public HtmlDataRetriever()
        {

        }

        // dataSize must be "full" or "compact"
        public void RetrieveData(String dataSize)
        { 
            /* 
             *   *******Init Objects for StockExchange ********
             *  
             */


            IAvapiConnection connection = AvapiConnection.Instance;
            connection.Connect("CJ7QO45PVDJ8371Q");
            Int_TIME_SERIES_DAILY_ADJUSTED TSD = connection.GetQueryObject_TIME_SERIES_DAILY_ADJUSTED();

            //ASX
            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED ASXResponse =
                TSD.QueryPrimitive(
                    "^AXJO",
                    dataSize
                    );
            this.ASX = ASXResponse.Data;
            //ESTOX
            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED ESTOXResponse =
                TSD.QueryPrimitive(
                    "^STOXX50E",
                    dataSize
                    );
            this.ESTOX = ESTOXResponse.Data;

            //FTSE
            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED FTSEResponse =
                TSD.QueryPrimitive(
                    "^FTSE",
                    dataSize
                    );
            this.FTSE = FTSEResponse.Data;

            //SP500
            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED SP500Response =
                TSD.QueryPrimitive(
                    "^GSPC",
                    dataSize
                    );
            this.SP500 = SP500Response.Data;

            //N225
            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED N225Response =
                TSD.QueryPrimitive(
                    "^N225",
                    dataSize
                    );
            this.N225 = N225Response.Data;

            //HANG
            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED HangResponse =
                TSD.QueryPrimitive(
                    "^HSI",
                    dataSize
                    );
            this.HANG = HangResponse.Data;



            /* 
        *   ******* CurrencyExchange ********
        *   
        *   Possible to request only 1 value, see currency_exchange_rate
        */
            
            //EURUSD
            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED EURUSDResponse =
                TSD.QueryPrimitive(
                    "EURUSD=X",
                    dataSize
                );

            this.EURUSD = EURUSDResponse.Data;

            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED EURGBPResponse =
                TSD.QueryPrimitive(
                    "EURGBP=X",
                    dataSize
                );

            this.EURGBP = EURGBPResponse.Data;

            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED EURJPYResponse =
                TSD.QueryPrimitive(
                "EURUSD=X",
                dataSize
                );

            this.EURJPY = EURJPYResponse.Data;

            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED AUDUSDResponse =
                TSD.QueryPrimitive(
                    "AUDUSD=X",
                    dataSize
                );

            this.AUDUSD = AUDUSDResponse.Data;

            IAvapiResponse_TIME_SERIES_DAILY_ADJUSTED USDHKDResponse =
                TSD.QueryPrimitive(
                "USDHKD=X",
                dataSize
                );

            this.USDHKD = USDHKDResponse.Data;
#pragma warning restore CS0436 // Le type est en conflit avec le type importé
        }
    }
}