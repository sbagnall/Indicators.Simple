namespace SteveBagnall.Trading.Indicators
{
    //    //+------------------------------------------------------------------+
    ////|                                             Trending or Ranging? |
    ////|                                                     ToR_1.20.mq4 |
    ////|                                       Copyright © 2008 Tom Balfe |
    ////|                                                                  |
    ////| This indicator shows you whether a pair is trending or ranging.  | 
    ////| For trending markets use moving averages and for ranging         |
    ////| market use oscillators.                                          |
    ////|                                                                  |
    ////| Best of luck in all your trades...                               |
    ////|                                                                  |
    ////| Special thanks to whereswaldo!                                   |
    ////|                                                                  |
    ////| Version: 1.12                                                    |
    ////|                                                                  |
    ////| Changelog:                                                       |
    ////|     1.20 - added CCI                                             |
    ////|     1.12 - made color assignment more efficient, made arrow      |
    ////|            assignment more efficient                             |
    ////|     1.11 - adjusted fonts, spacing, added ranging, text broken   |
    ////|     1.10 - added ADX increasing and decreasing notice            |
    ////|     1.03 - adjusted spacing, fonts, unreleased                   |
    ////|     1.02 - added arrows, ranging icon, no zero space state       |
    ////|            for icons/arrows, spacing got messed up, now          | 
    ////|            fixed                                                 |
    ////|     1.01 - unreleased, Reduced number of colors, functional      |
    ////|     1.0  - unreleased, too many colors for ADX values            |
    ////|                                                                  |
    ////|                   http://www.forex-tsd.com/members/nittany1.html |
    ////+------------------------------------------------------------------+
    //#property copyright "Copyright © 2007-2008 Tom Balfe"
    //#property link      "http://www.forex-tsd.com/members/nittany1.html"
    //#property link      "redcarsarasota@yahoo.com"
    //#property indicator_separate_window

    //int spread;
    ////---- user selectable stuff
    //extern int     SpreadThreshold      = 6;
    //extern string  ADX_Settings         = "=== ADX Settings ===";
    //extern int     ADX_trend_level      = 23;
    //extern int     ADX_trend_strong     = 28;
    //extern string  CCI_Settings         = "=== CCI Settings ===";
    //extern int     M1_EntryCCI_Period   = 14;
    //extern int     M1_TrendCCI_Period   = 50;
    //extern int     M5_EntryCCI_Period   = 14;
    //extern int     M5_TrendCCI_Period   = 34;
    //extern int     M15_EntryCCI_Period  = 6;
    //extern int     M15_TrendCCI_Period  = 14;

    ////+------------------------------------------------------------------+
    ////| Custom indicator initialization function                         |
    ////+------------------------------------------------------------------+
    //int init()
    //  {
    //   //---- indicator short name
    //   IndicatorShortName("ToR 1.20 ");

    //   return(0);
    //  }
    ////+------------------------------------------------------------------+
    ////| Custom indicator deinitialization function                       |
    ////+------------------------------------------------------------------+
    //int deinit()
    //  {
    //   //---- need to delete objects should user remove indicator
    //   ObjectsDeleteAll(0,OBJ_LABEL);
    //     ObjectDelete("ToR120-1");ObjectDelete("ToR120-2");ObjectDelete("ToR120-3");
    //     ObjectDelete("ToR120-3a");ObjectDelete("ToR120-4");ObjectDelete("ToR120-4a");
    //     ObjectDelete("ToR120-4d");ObjectDelete("ToR120-5");ObjectDelete("ToR120-6");
    //     ObjectDelete("ToR120-6a");ObjectDelete("ToR120-6d");ObjectDelete("ToR120-7");
    //     ObjectDelete("ToR120-8");ObjectDelete("ToR120-8a");ObjectDelete("ToR120-8d");
    //     ObjectDelete("ToR120-9");ObjectDelete("ToR120-10");ObjectDelete("ToR120-10a");
    //     ObjectDelete("ToR120-10d");ObjectDelete("ToR120-11");ObjectDelete("ToR120-12");
    //     ObjectDelete("ToR120-12a");ObjectDelete("ToR120-13");ObjectDelete("ToR120-12d");
    //     ObjectDelete("ToR120-14");ObjectDelete("ToR120-15");ObjectDelete("ToR120-18d");
    //     ObjectDelete("ToR120-cci1");ObjectDelete("ToR120-cci2");ObjectDelete("ToR120-cci3");
    //     ObjectDelete("ToR120-cci4");ObjectDelete("ToR120-cci1a");ObjectDelete("ToR120-cci1b");
    //     ObjectDelete("ToR120-cci1c");ObjectDelete("ToR120-cci5");ObjectDelete("ToR120-cci6");
    //     ObjectDelete("ToR120-cci7");ObjectDelete("ToR120-cci8");ObjectDelete("ToR120-cci5b");
    //     ObjectDelete("ToR120-cci5c");ObjectDelete("ToR120-cci9");ObjectDelete("ToR120-cci10");
    //     ObjectDelete("ToR120-cci11");ObjectDelete("ToR120-cci12");ObjectDelete("ToR120-9b");
    //     ObjectDelete("ToR120-cci9c");

    //   return(0);
    //  }

    //int start()
    //  {
    //   //---- let's define some stuff 
    //   // M1 ADX data
    //   double adx_m1 = iADX(NULL,1,14,PRICE_CLOSE,0,0); // ADX 1 min
    //   double adx_1ago_m1 = iADX(NULL,1,14,PRICE_CLOSE,0,1); // ADX 1 min 1 bar ago
    //   double di_p_m1 = iADX(NULL,1,14,PRICE_CLOSE,1,0); // DI+ 1 min
    //   double di_m_m1 = iADX(NULL,1,14,PRICE_CLOSE,2,0); // DI- 1 min
    //   // M5 ADX data
    //   double adx_m5 = iADX(NULL,5,14,PRICE_CLOSE,0,0); // ADX 5 min
    //   double adx_1ago_m5 = iADX(NULL,5,14,PRICE_CLOSE,0,1); // ADX 5 min 1 bar ago
    //   double di_p_m5 = iADX(NULL,5,14,PRICE_CLOSE,1,0); // DI+ 5 min
    //   double di_m_m5 = iADX(NULL,5,14,PRICE_CLOSE,2,0); // DI- 5 min
    //   // M15 ADX data
    //   double adx_m15 = iADX(NULL,15,14,PRICE_CLOSE,0,0); // ADX 15 min
    //   double adx_1ago_m15 = iADX(NULL,15,14,PRICE_CLOSE,0,1); // ADX 15 min 1 bar ago
    //   double di_p_m15 = iADX(NULL,15,14,PRICE_CLOSE,1,0); // DI+ 15 min
    //   double di_m_m15 = iADX(NULL,15,14,PRICE_CLOSE,2,0); // DI- 15 min
    //   // M30 ADX data
    //   double adx_m30 = iADX(NULL,30,14,PRICE_CLOSE,0,0); // ADX 30 min
    //   double adx_1ago_m30 = iADX(NULL,30,14,PRICE_CLOSE,0,1); // ADX 30 min 1 bar ago
    //   double di_p_m30 = iADX(NULL,30,14,PRICE_CLOSE,1,0); // DI+ 30 min
    //   double di_m_m30 = iADX(NULL,30,14,PRICE_CLOSE,2,0); // DI- 30 min
    //   // h1 ADX data
    //   double adx_h1 = iADX(NULL,60,14,PRICE_CLOSE,0,0); // ADX 1 hour
    //   double adx_1ago_h1 = iADX(NULL,60,14,PRICE_CLOSE,0,1); // ADX 1 hr 1 bar ago
    //   double di_p_h1 = iADX(NULL,60,14,PRICE_CLOSE,1,0); // DI+ 1 hour
    //   double di_m_h1 = iADX(NULL,60,14,PRICE_CLOSE,2,0); // DI- 1 hour
    //   // h4 ADX data
    //   double adx_h4 = iADX(NULL,240,14,PRICE_CLOSE,0,0); // ADX 4 hour
    //   double adx_1ago_h4 = iADX(NULL,240,14,PRICE_CLOSE,0,1); // ADX 4 hr 1 bar ago
    //   double di_p_h4 = iADX(NULL,240,14,PRICE_CLOSE,1,0); // DI+ 4 hour
    //   double di_m_h4 = iADX(NULL,240,14,PRICE_CLOSE,2,0); // DI- 4 hour

    //   // M1 CCI data
    //   double cci_entry_m1 = iCCI(NULL,1,M1_EntryCCI_Period,PRICE_TYPICAL,0); 
    //   double cci_trend_m1 = iCCI(NULL,1,M1_TrendCCI_Period,PRICE_TYPICAL,0); 
    //   double cci_entry_1ago_m1 = iCCI(NULL,1,M1_EntryCCI_Period,PRICE_TYPICAL,1); 
    //   double cci_trend_1ago_m1 = iCCI(NULL,1,M1_TrendCCI_Period,PRICE_TYPICAL,1);

    //   // M5 CCI data
    //   double cci_entry_m5 = iCCI(NULL,5,M5_EntryCCI_Period,PRICE_TYPICAL,0);
    //   double cci_trend_m5 = iCCI(NULL,5,M5_TrendCCI_Period,PRICE_TYPICAL,0);
    //   double cci_entry_1ago_m5 = iCCI(NULL,5,M5_EntryCCI_Period,PRICE_TYPICAL,1);
    //   double cci_trend_1ago_m5 = iCCI(NULL,5,M5_TrendCCI_Period,PRICE_TYPICAL,1);

    //   // M15 CCI data
    //   double cci_entry_m15 = iCCI(NULL,15,M15_EntryCCI_Period,PRICE_TYPICAL,0);
    //   double cci_trend_m15 = iCCI(NULL,15,M15_TrendCCI_Period,PRICE_TYPICAL,0);
    //   double cci_entry_1ago_m15 = iCCI(NULL,15,M15_EntryCCI_Period,PRICE_TYPICAL,1);
    //   double cci_trend_1ago_m15 = iCCI(NULL,15,M15_TrendCCI_Period,PRICE_TYPICAL,1);   

    //   //---- define colors and arrows 
    //   color adx_color_m1,adx_color_m5,adx_color_m15,adx_color_m30,adx_color_h1,adx_color_h4,
    //         cci_color_entry_m1,cci_color_entry_m5,cci_color_entry_m15, cci_color_trend_m1,
    //         cci_color_trend_m5,cci_color_trend_m15;

    //   string  adx_arrow_m1,adx_arrow_m5,adx_arrow_m15,adx_arrow_m30,adx_arrow_h1,adx_arrow_h4;

    //   //----- Assign colors

    //   adx_color_m1 = assigncolor(adx_m1,ADX_trend_level,di_p_m1, di_m_m1);
    //   adx_color_m5 = assigncolor(adx_m5,ADX_trend_level,di_p_m5, di_m_m5);
    //   adx_color_m15 = assigncolor(adx_m15,ADX_trend_level,di_p_m15, di_m_m15);
    //   adx_color_m30 = assigncolor(adx_m30,ADX_trend_level,di_p_m30, di_m_m30);
    //   adx_color_h1 = assigncolor(adx_h1,ADX_trend_level,di_p_h1, di_m_h1);
    //   adx_color_h4 = assigncolor(adx_h4,ADX_trend_level,di_p_h4, di_m_h4);

    //   cci_color_entry_m1 = assignCCIcolor (cci_entry_m1);
    //   cci_color_entry_m5 = assignCCIcolor (cci_entry_m5);
    //   cci_color_entry_m15 = assignCCIcolor (cci_entry_m15);
    //   cci_color_trend_m1 = assignCCIcolor (cci_trend_m1);
    //   cci_color_trend_m5 = assignCCIcolor (cci_trend_m5);
    //   cci_color_trend_m15 = assignCCIcolor (cci_trend_m15);   

    //   //---- feed all the ADX values into strings      
    //   string adx_value_m1 = adx_m1;
    //   string adx_value_m5 = adx_m5;
    //   string adx_value_m15 = adx_m15;
    //   string adx_value_m30 = adx_m30;
    //   string adx_value_h1 = adx_h1;
    //   string adx_value_h4 = adx_h4;

    //   //---- feed all the CCI values into strings 
    //   string cci_value_entry_m1 = cci_entry_m1;
    //   string cci_value_trend_m1 = cci_trend_m1;
    //   string cci_value_e1ago_m1 = cci_entry_1ago_m1;
    //   string cci_value_t1ago_m1 = cci_entry_1ago_m1;
    //   string cci_value_entry_m5 = cci_entry_m5;
    //   string cci_value_trend_m5 = cci_trend_m5;
    //   string cci_value_e1ago_m5 = cci_entry_1ago_m5;
    //   string cci_value_t1ago_m5 = cci_entry_1ago_m5;
    //   string cci_value_entry_m15 = cci_entry_m15;
    //   string cci_value_trend_m15 = cci_trend_m15;
    //   string cci_value_e1ago_m15 = cci_entry_1ago_m15;
    //   string cci_value_t1ago_m15 = cci_entry_1ago_m15;

    //   //----- Assign Arrows
    //   adx_arrow_m1 = AssignArrow(adx_m1,di_p_m1,di_m_m1);
    //   adx_arrow_m5 = AssignArrow(adx_m5,di_p_m5,di_m_m5);
    //   adx_arrow_m15= AssignArrow(adx_m15,di_p_m15,di_m_m15);
    //   adx_arrow_m30= AssignArrow(adx_m30,di_p_m30,di_m_m30);
    //   adx_arrow_h1 = AssignArrow(adx_h1,di_p_h1,di_m_h1);
    //   adx_arrow_h4 = AssignArrow(adx_h4,di_p_h4,di_m_h4);

    //   //---- defines what spread is 
    //   spread=MarketInfo(Symbol(),MODE_SPREAD);

    //   //+------------------------------------------------------------------+
    //   //|  Spread                                                          |
    //   //+------------------------------------------------------------------+
    //   ObjectCreate("ToR120-1", OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-1","SPREAD:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-1", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-1", OBJPROP_XDISTANCE, 65);
    //     ObjectSet("ToR120-1", OBJPROP_YDISTANCE, 2);
    //   //---- creates spread number, Lime if less than threshold, Red if above it
    //   ObjectCreate("ToR120-2", OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     if (spread<=SpreadThreshold)
    //     {
    //     ObjectSetText("ToR120-2",DoubleToStr(spread ,0),10, "Arial Bold", Lime);
    //     }
    //     else
    //     ObjectSetText("ToR120-2",DoubleToStr(spread ,0),10, "Arial Bold", Red);
    //     ObjectSet("ToR120-2", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-2", OBJPROP_XDISTANCE, 115);
    //     ObjectSet("ToR120-2", OBJPROP_YDISTANCE, 2);

    //   //+------------------------------------------------------------------+
    //   //|  1 MIN                                                           |
    //   //+------------------------------------------------------------------+ 
    //   ObjectCreate("ToR120-3",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-3","1 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-3", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-3", OBJPROP_XDISTANCE, 150);
    //     ObjectSet("ToR120-3", OBJPROP_YDISTANCE, 2);
    //   //---- create text "Getting: "
    //   ObjectCreate("ToR120-3a",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-3a","CHANGE:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-3a", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-3a", OBJPROP_XDISTANCE, 135);
    //     ObjectSet("ToR120-3a", OBJPROP_YDISTANCE, 17);

    //   //---- create 1 min value
    //   ObjectCreate("ToR120-4",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-4", " ADX "+StringSubstr(adx_value_m1,0,5)+" ",9, "Arial Bold",adx_color_m1);
    //     ObjectSet("ToR120-4", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-4", OBJPROP_XDISTANCE, 185);
    //     ObjectSet("ToR120-4", OBJPROP_YDISTANCE, 2);
    //   //---- create 1 min arrow, squiggle if ranging
    //   ObjectCreate("ToR120-4a",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-4a",adx_arrow_m1,9, "Wingdings",adx_color_m1);
    //     ObjectSet("ToR120-4a", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-4a", OBJPROP_XDISTANCE, 250);
    //     ObjectSet("ToR120-4a", OBJPROP_YDISTANCE, 2); 

    //   //----create 1 min Strong/Weaker/Ranging
    //   ObjectCreate("ToR120-4d",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //   ObjectSet("ToR120-4d", OBJPROP_CORNER, 0);
    //   ObjectSet("ToR120-4d", OBJPROP_XDISTANCE, 190);
    //   ObjectSet("ToR120-4d", OBJPROP_YDISTANCE, 17);
    //   if (adx_m1 < ADX_trend_level)
    //   {
    //     ObjectSetText("ToR120-4d", " RANGING ",8, "Arial Bold",Silver);
    //   } else if (adx_m1 > adx_1ago_m1)
    //            {
    //            ObjectSetText("ToR120-4d", " STRONGER ",8, "Arial Bold",Silver);
    //            } else 
    //                 {
    //                  ObjectSetText("ToR120-4d", " WEAKER ",8, "Arial Bold",Silver);
    //                 }
    //   //---- CCI stuff ---- :)~~~
    //   //---- 1st "1 MIN:"
    //   ObjectCreate("ToR120-cci1",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci1","1 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-cci1", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci1", OBJPROP_XDISTANCE, 125);
    //     ObjectSet("ToR120-cci1", OBJPROP_YDISTANCE, 35);
    //   //---- create 1 min cci entry value
    //   ObjectCreate("ToR120-cci2",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci2", " CCI(" + M1_EntryCCI_Period + ") "+StringSubstr(cci_value_entry_m1,0,6)+" ",9, "Arial Bold",cci_color_entry_m1);
    //     ObjectSet("ToR120-cci2", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci2", OBJPROP_XDISTANCE, 160);
    //     ObjectSet("ToR120-cci2", OBJPROP_YDISTANCE, 35);
    //   //---- 2nd "1 MIN:"
    //   ObjectCreate("ToR120-cci3",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci3","1 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-cci3", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci3", OBJPROP_XDISTANCE, 250);
    //     ObjectSet("ToR120-cci3", OBJPROP_YDISTANCE, 35);
    //   //---- creat 1 min cci trend value
    //   ObjectCreate("ToR120-cci4",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci4", " CCI(" + M1_TrendCCI_Period + ") "+StringSubstr(cci_value_trend_m1,0,6)+" ",9, "Arial Bold",cci_color_trend_m1);
    //     ObjectSet("ToR120-cci4", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci4", OBJPROP_XDISTANCE, 290);
    //     ObjectSet("ToR120-cci4", OBJPROP_YDISTANCE, 35);

    //   //---- create text "Heading: "
    //   ObjectCreate("ToR120-cci1a",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci1a","HEADING:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-cci1a", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci1a", OBJPROP_XDISTANCE, 105);
    //     ObjectSet("ToR120-cci1a", OBJPROP_YDISTANCE, 50);

    //   //---- create 1 min entry CCI direction
    //   ObjectCreate("ToR120-cci1b",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSet("ToR120-cci1b", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci1b", OBJPROP_XDISTANCE, 180);
    //     ObjectSet("ToR120-cci1b", OBJPROP_YDISTANCE, 50);
    //     if (cci_entry_m1 > cci_entry_1ago_m1)
    //     {
    //       ObjectSetText("ToR120-cci1b", " UPHILL ",8, "Arial Bold",Silver);
    //     } else 
    //            {
    //            ObjectSetText("ToR120-cci1b", " DOWNHILL ",8, "Arial Bold",Silver);
    //            }    
    //   //---- create 1 min trend cci direction
    //   ObjectCreate("ToR120-cci1c",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSet("ToR120-cci1c", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci1c", OBJPROP_XDISTANCE, 310);
    //     ObjectSet("ToR120-cci1c", OBJPROP_YDISTANCE, 50);
    //     if (cci_trend_m1 > cci_trend_1ago_m1)
    //     {
    //       ObjectSetText("ToR120-cci1c", " UPHILL ",8, "Arial Bold",Silver);
    //     } else 
    //            {
    //            ObjectSetText("ToR120-cci1c", " DOWNHILL ",8, "Arial Bold",Silver);
    //            }      

    //   //+------------------------------------------------------------------+
    //   //|  5 MIN                                                           |
    //   //+------------------------------------------------------------------+
    //   ObjectCreate("ToR120-5",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-5","5 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-5", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-5", OBJPROP_XDISTANCE, 270);
    //     ObjectSet("ToR120-5", OBJPROP_YDISTANCE, 2);
    //   //---- create 5 min value
    //   ObjectCreate("ToR120-6",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-6", " ADX "+StringSubstr(adx_value_m5,0,5)+" ",9, "Arial Bold",adx_color_m5);
    //     ObjectSet("ToR120-6", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-6", OBJPROP_XDISTANCE, 305);
    //     ObjectSet("ToR120-6", OBJPROP_YDISTANCE, 2);
    //   //---- create 5 min arrow, squiggle if ranging
    //   ObjectCreate("ToR120-6a",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-6a",adx_arrow_m5,9, "Wingdings",adx_color_m5);
    //     ObjectSet("ToR120-6a", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-6a", OBJPROP_XDISTANCE, 370);
    //     ObjectSet("ToR120-6a", OBJPROP_YDISTANCE, 2); 

    //   //----create 5 min Strong/Weaker/Ranging
    //   ObjectCreate("ToR120-6d",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //   ObjectSet("ToR120-6d", OBJPROP_CORNER, 0);
    //   ObjectSet("ToR120-6d", OBJPROP_XDISTANCE, 310);
    //   ObjectSet("ToR120-6d", OBJPROP_YDISTANCE, 17);
    //   if (adx_m5 < ADX_trend_level)
    //   {
    //     ObjectSetText("ToR120-6d", " RANGING ",8, "Arial Bold",Silver);
    //   } else if (adx_m5 > adx_1ago_m5)
    //            {
    //            ObjectSetText("ToR120-6d", " STRONGER ",8, "Arial Bold",Silver);
    //            } else 
    //                 {
    //                  ObjectSetText("ToR120-6d", " WEAKER ",8, "Arial Bold",Silver);
    //                 }
    //   //---- CCI stuff ---- :)~~~
    //   //---- 1st "5 MIN:"
    //   ObjectCreate("ToR120-cci5",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci5","5 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-cci5", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci5", OBJPROP_XDISTANCE, 380);
    //     ObjectSet("ToR120-cci5", OBJPROP_YDISTANCE, 35);
    //   //---- create 5 min cci entry value
    //   ObjectCreate("ToR120-cci6",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci6", " CCI(" + M5_EntryCCI_Period + ") "+StringSubstr(cci_value_entry_m5,0,6)+" ",9, "Arial Bold",cci_color_entry_m5);
    //     ObjectSet("ToR120-cci6", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci6", OBJPROP_XDISTANCE, 415);
    //     ObjectSet("ToR120-cci6", OBJPROP_YDISTANCE, 35);
    //   //---- 2nd "5 MIN:"
    //   ObjectCreate("ToR120-cci7",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci7","5 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-cci7", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci7", OBJPROP_XDISTANCE, 510);
    //     ObjectSet("ToR120-cci7", OBJPROP_YDISTANCE, 35);
    //   //---- creat 5 min cci trend value
    //   ObjectCreate("ToR120-cci8",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci8", " CCI(" + M5_TrendCCI_Period + ") "+StringSubstr(cci_value_trend_m5,0,6)+" ",9, "Arial Bold",cci_color_trend_m5);
    //     ObjectSet("ToR120-cci8", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci8", OBJPROP_XDISTANCE, 545);
    //     ObjectSet("ToR120-cci8", OBJPROP_YDISTANCE, 35);

    //   //---- create 5 min entry CCI direction
    //   ObjectCreate("ToR120-cci5b",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSet("ToR120-cci5b", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci5b", OBJPROP_XDISTANCE, 430);
    //     ObjectSet("ToR120-cci5b", OBJPROP_YDISTANCE, 50);
    //     if (cci_entry_m5 > cci_entry_1ago_m5)
    //     {
    //       ObjectSetText("ToR120-cci5b", " UPHILL ",8, "Arial Bold",Silver);
    //     } else 
    //            {
    //            ObjectSetText("ToR120-cci5b", " DOWNHILL ",8, "Arial Bold",Silver);
    //            }    
    //   //---- create 5 min trend CCI direction
    //   ObjectCreate("ToR120-cci5c",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSet("ToR120-cci5c", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci5c", OBJPROP_XDISTANCE, 560);
    //     ObjectSet("ToR120-cci5c", OBJPROP_YDISTANCE, 50);
    //     if (cci_trend_m5 > cci_trend_1ago_m5)
    //     {
    //       ObjectSetText("ToR120-cci5c", " UPHILL ",8, "Arial Bold",Silver);
    //     } else 
    //            {
    //            ObjectSetText("ToR120-cci5c", " DOWNHILL ",8, "Arial Bold",Silver);
    //            }      

    //   //+------------------------------------------------------------------+
    //   //|  15 MIN                                                          |
    //   //+------------------------------------------------------------------+
    //   ObjectCreate("ToR120-7",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-7","15 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-7", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-7", OBJPROP_XDISTANCE, 390);
    //     ObjectSet("ToR120-7", OBJPROP_YDISTANCE, 2);
    //   //---- create 15 min value
    //   ObjectCreate("ToR120-8",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-8", " ADX "+StringSubstr(adx_value_m15,0,5)+" ",9, "Arial Bold",adx_color_m15);
    //     ObjectSet("ToR120-8", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-8", OBJPROP_XDISTANCE, 430);
    //     ObjectSet("ToR120-8", OBJPROP_YDISTANCE, 2);
    //   //---- create 15 min arrow, squiggle if ranging
    //   ObjectCreate("ToR120-8a",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-8a",adx_arrow_m15,9, "Wingdings",adx_color_m15);
    //     ObjectSet("ToR120-8a", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-8a", OBJPROP_XDISTANCE, 495);
    //     ObjectSet("ToR120-8a", OBJPROP_YDISTANCE, 2); 

    //   //----create 15 min Strong/Weaker/Ranging
    //   ObjectCreate("ToR120-8d",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //   ObjectSet("ToR120-8d", OBJPROP_CORNER, 0);
    //   ObjectSet("ToR120-8d", OBJPROP_XDISTANCE, 435);
    //   ObjectSet("ToR120-8d", OBJPROP_YDISTANCE, 17);
    //   if (adx_m15 < ADX_trend_level)
    //   {
    //     ObjectSetText("ToR120-8d", " RANGING ",8, "Arial Bold",Silver);
    //   } else if (adx_m15 > adx_1ago_m15)
    //            {
    //            ObjectSetText("ToR120-8d", " STRONGER ",8, "Arial Bold",Silver);
    //            } else 
    //                 {
    //                  ObjectSetText("ToR120-8d", " WEAKER ",8, "Arial Bold",Silver);
    //                 }
    //   //---- CCI stuff ---- :)~~~
    //   //---- 1st "15 MIN:"
    //   ObjectCreate("ToR120-cci9",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci9","15 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-cci9", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci9", OBJPROP_XDISTANCE, 635);
    //     ObjectSet("ToR120-cci9", OBJPROP_YDISTANCE, 35);
    //   //---- create 15 min cci entry value
    //   ObjectCreate("ToR120-cci10",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci10", " CCI(" + M15_EntryCCI_Period + ") "+StringSubstr(cci_value_entry_m15,0,6)+" ",9, "Arial Bold",cci_color_entry_m15);
    //     ObjectSet("ToR120-cci10", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci10", OBJPROP_XDISTANCE, 675);
    //     ObjectSet("ToR120-cci10", OBJPROP_YDISTANCE, 35);
    //   //---- 2nd "15 MIN:"
    //   ObjectCreate("ToR120-cci11",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci11","15 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-cci11", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci11", OBJPROP_XDISTANCE, 755);
    //     ObjectSet("ToR120-cci11", OBJPROP_YDISTANCE, 35);
    //   //---- create 15 min cci trend value
    //   ObjectCreate("ToR120-cci12",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-cci12", " CCI(" + M15_TrendCCI_Period + ") "+StringSubstr(cci_value_trend_m15,0,6)+" ",9, "Arial Bold",cci_color_trend_m15);
    //     ObjectSet("ToR120-cci12", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci12", OBJPROP_XDISTANCE, 795);
    //     ObjectSet("ToR120-cci12", OBJPROP_YDISTANCE, 35);

    //   //---- create 15 min entry CCI direction
    //   ObjectCreate("ToR120-cci9b",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSet("ToR120-cci9b", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci9b", OBJPROP_XDISTANCE, 690);
    //     ObjectSet("ToR120-cci9b", OBJPROP_YDISTANCE, 50);
    //     if (cci_entry_m15 > cci_entry_1ago_m15)
    //     {
    //       ObjectSetText("ToR120-cci9b", " UPHILL ",8, "Arial Bold",Silver);
    //     } else 
    //            {
    //            ObjectSetText("ToR120-cci9b", " DOWNHILL ",8, "Arial Bold",Silver);
    //            }    
    //   //---- create 15 min trend CCI direction
    //   ObjectCreate("ToR120-cci9c",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSet("ToR120-cci9c", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-cci9c", OBJPROP_XDISTANCE, 810);
    //     ObjectSet("ToR120-cci9c", OBJPROP_YDISTANCE, 50);
    //     if (cci_trend_m15 > cci_trend_1ago_m15)
    //     {
    //       ObjectSetText("ToR120-cci9c", " UPHILL ",8, "Arial Bold",Silver);
    //     } else 
    //            {
    //            ObjectSetText("ToR120-cci9c", " DOWNHILL ",8, "Arial Bold",Silver);
    //            }      

    //   //+------------------------------------------------------------------+
    //   //|  30 MIN                                                          |
    //   //+------------------------------------------------------------------+
    //   ObjectCreate("ToR120-9",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-9","30 MIN:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-9", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-9", OBJPROP_XDISTANCE, 515);
    //     ObjectSet("ToR120-9", OBJPROP_YDISTANCE, 2);
    //   //---- create 30 min value
    //   ObjectCreate("ToR120-10",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-10", " ADX "+StringSubstr(adx_value_m30,0,5)+" ",9, "Arial Bold",adx_color_m30);
    //     ObjectSet("ToR120-10", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-10", OBJPROP_XDISTANCE, 555);
    //     ObjectSet("ToR120-10", OBJPROP_YDISTANCE, 2);
    //   //---- create 30 min arrow, squiggle if ranging
    //   ObjectCreate("ToR120-10a",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-10a",adx_arrow_m30,9, "Wingdings",adx_color_m30);
    //     ObjectSet("ToR120-10a", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-10a", OBJPROP_XDISTANCE, 620);
    //     ObjectSet("ToR120-10a", OBJPROP_YDISTANCE, 2); 

    //   //----create 30 min Strong/Weaker/Ranging
    //   ObjectCreate("ToR120-10d",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //   ObjectSet("ToR120-10d", OBJPROP_CORNER, 0);
    //   ObjectSet("ToR120-10d", OBJPROP_XDISTANCE, 560);
    //   ObjectSet("ToR120-10d", OBJPROP_YDISTANCE, 17);
    //   if (adx_m30 < ADX_trend_level)
    //   {
    //     ObjectSetText("ToR120-10d", " RANGING ",8, "Arial Bold",Silver);
    //   } else if (adx_m30 > adx_1ago_m30)
    //            {
    //            ObjectSetText("ToR120-10d", " STRONGER ",8, "Arial Bold",Silver);
    //            } else 
    //                 {
    //                  ObjectSetText("ToR120-10d", " WEAKER ",8, "Arial Bold",Silver);
    //                 }

    //   //+------------------------------------------------------------------+
    //   //|  1 HOUR                                                          |
    //   //+------------------------------------------------------------------+
    //   ObjectCreate("ToR120-11",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-11","1 HR:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-11", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-11", OBJPROP_XDISTANCE, 640);
    //     ObjectSet("ToR120-11", OBJPROP_YDISTANCE, 2);
    //   //---- create 15 min value
    //   ObjectCreate("ToR120-12",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-12", " ADX "+StringSubstr(adx_value_h1,0,5)+" ",9, "Arial Bold",adx_color_h1);
    //     ObjectSet("ToR120-12", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-12", OBJPROP_XDISTANCE, 670);
    //     ObjectSet("ToR120-12", OBJPROP_YDISTANCE, 2);
    //   ObjectCreate("ToR120-12a",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-12a",adx_arrow_h1,9, "Wingdings",adx_color_h1);
    //     ObjectSet("ToR120-12a", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-12a", OBJPROP_XDISTANCE, 735);
    //     ObjectSet("ToR120-12a", OBJPROP_YDISTANCE, 2);

    //   //----create hour min Strong/Weaker/Ranging
    //   ObjectCreate("ToR120-12d",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //   ObjectSet("ToR120-12d", OBJPROP_CORNER, 0);
    //   ObjectSet("ToR120-12d", OBJPROP_XDISTANCE, 675);
    //   ObjectSet("ToR120-12d", OBJPROP_YDISTANCE, 17);
    //   if (adx_h1 < ADX_trend_level)
    //   {
    //     ObjectSetText("ToR120-12d", " RANGING ",8, "Arial Bold",Silver);
    //   } else if (adx_h1 > adx_1ago_h1)
    //            {
    //            ObjectSetText("ToR120-12d", " STRONGER ",8, "Arial Bold",Silver);
    //            } else 
    //                 {
    //                  ObjectSetText("ToR120-12d", " WEAKER ",8, "Arial Bold",Silver);
    //                 }

    //   //+------------------------------------------------------------------+
    //   //|  4 HOUR                                                          |
    //   //+------------------------------------------------------------------+
    //   ObjectCreate("ToR120-13",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-13","4 HR:", 8, "Arial Bold", LightSteelBlue);
    //     ObjectSet("ToR120-13", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-13", OBJPROP_XDISTANCE, 760);
    //     ObjectSet("ToR120-13", OBJPROP_YDISTANCE, 2);
    //   //---- create 15 min value
    //   ObjectCreate("ToR120-14",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-14", " ADX "+StringSubstr(adx_value_h4,0,5)+" ",9, "Arial Bold",adx_color_h4);
    //     ObjectSet("ToR120-14", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-14", OBJPROP_XDISTANCE, 790);
    //     ObjectSet("ToR120-14", OBJPROP_YDISTANCE, 2);
    //   ObjectCreate("ToR120-15",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //     ObjectSetText("ToR120-15",adx_arrow_h4,9, "Wingdings",adx_color_h4);
    //     ObjectSet("ToR120-15", OBJPROP_CORNER, 0);
    //     ObjectSet("ToR120-15", OBJPROP_XDISTANCE, 855);
    //     ObjectSet("ToR120-15", OBJPROP_YDISTANCE, 2);

    //   //----create 4hour min Strong/Weaker/Ranging
    //   ObjectCreate("ToR120-18d",OBJ_LABEL, WindowFind("ToR 1.20 "), 0, 0);
    //   ObjectSet("ToR120-18d", OBJPROP_CORNER, 0);
    //   ObjectSet("ToR120-18d", OBJPROP_XDISTANCE, 795);
    //   ObjectSet("ToR120-18d", OBJPROP_YDISTANCE, 17);
    //   if (adx_h4 < ADX_trend_level)
    //   {
    //     ObjectSetText("ToR120-18d", " RANGING ",8, "Arial Bold",Silver);
    //   } else if (adx_h4 > adx_1ago_h4)
    //            {
    //            ObjectSetText("ToR120-18d", " STRONGER ",8, "Arial Bold",Silver);
    //            } else 
    //                 {
    //                  ObjectSetText("ToR120-18d", " WEAKER ",8, "Arial Bold",Silver);
    //                 }

    //   return(0);
    //  } //---- end of main loop

    //  //+------------------------------------------------------------------+
    //  //|  Common Functions                                                |
    //  //+------------------------------------------------------------------+
    //   color assigncolor(double adx_time,double ADX_trend_level,double di_p_time,double di_m_time)
    //   {
    //   if (adx_time < ADX_trend_level) { return (LightSkyBlue); }
    //   else if (di_p_time > di_m_time) { return (Lime); }
    //   else {return (Red); }

    //   }

    //   color assignCCIcolor(double cci_time)
    //   {
    //   if (cci_time > 0) { return (Lime); }
    //   else {return (Red); }                            
    //   }

    //   string AssignArrow (double adx_time, double di_p_time, double di_m_time)
    //    {                                
    //            if ((adx_time < ADX_trend_level) && (adx_time != 0)) 
    //            {
    //                 return ("h");
    //            }
    //            else if (adx_time < ADX_trend_strong )
    //                   {
    //                         if (di_p_time > di_m_time)
    //                         {
    //                          return ("ì");
    //                         }  
    //                         else
    //                          {
    //                            return ("î");
    //                          }

    //                    } else 
    //                         {
    //                             if (di_p_time > di_m_time)
    //                             {
    //                                  return ("é");
    //                             } else
    //                                {
    //                                   return ("ê");
    //                                 }

    //                         }
    //     } //End Function

}
