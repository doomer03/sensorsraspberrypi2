﻿using System;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

namespace RPI2.Sensors.ADConverter
{
    public class SPIADConverterBase : IADConverter, IDisposable
    {
        #region properties
        protected SpiDevice SpiDisplay;
        protected List<int> adcChannel = new List<int>();
        protected byte[] readBuffer = new byte[3];
        protected byte[] writeBuffer = new byte[3];
        protected int maxADCChannel = 0;
        #endregion

        public void Dispose()
        {
            if (SpiDisplay != null)
            {
                SpiDisplay.Dispose();
            }
        }
        private void InitChannel(List<int> adc_channel)
        {
            adcChannel = adc_channel;
            //
            if (adcChannel.Count <= maxADCChannel && !adcChannel.Exists(x => x > maxADCChannel || x < 0))
            {
                //This is the range we are looking for, from CH0 to CHx. 
            }
            else
            {
                throw new IndexOutOfRangeException("Channel Input is out of range, Channel input should be from 0-" + maxADCChannel.ToString() + " (" + (maxADCChannel+1).ToString()  + "-Channel).");
            }
        }

        public async void Init(List<int> adc_channel, SPIPort spi,int clockfrequency, SpiMode mode)
        {
            try
            {
                InitChannel(adc_channel);
                var settings = new SpiConnectionSettings((int)spi);
                settings.ClockFrequency = clockfrequency;
                settings.Mode = mode;

                string spiAqs = SpiDevice.GetDeviceSelector(Enum.GetName(typeof(SPIPort), spi));
                var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
                SpiDisplay = await SpiDevice.FromIdAsync(deviceInfo[0].Id, settings);
            }

            /* If initialization fails, display the exception and stop running */
            catch (Exception ex)
            {
                throw new Exception("SPI Initialization Failed", ex);
            }
        }
        /// <summary>
        /// Analog to digital conversion
        /// </summary>
        public int AnalogToDigital(int adc_channel)
        {
            return readadc(adc_channel);
        }

        virtual protected int readadc(int adc_channel)
        { return 0; }
        
    }
}
