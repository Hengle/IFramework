﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.4.17f1
 *Date:           2020-02-28
 *Description:    Description
 *History:        2020-02-28--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using IFramework;

namespace IFramework_Demo
{
	public class Panel01ViewModel : TUIViewModel_MVVM<Panel01Model>
	{
 		private Int32 _count;
		public Int32 count
		{
			get { return GetProperty(ref _count, this.GetPropertyName(() => _count)); }
			private set			{
				Tmodel.count = value;
				SetProperty(ref _count, value, this.GetPropertyName(() => _count));
			}
		}
        protected override void SubscribeMessage()
        {
            base.SubscribeMessage();
            this.message.Subscribe<Panel01View>(Listen);
        }
        protected override void UnSubscribeMessage()
        {
            base.UnSubscribeMessage();
            this.message.UnSubscribe<Panel01View>(Listen);
        }
        private void Listen(Type publishType, int code, IEventArgs args, object[] param)
        {
            if ((args as SubArg)!=null)
            {
                count--;
            }
            if ((args as AddArg) != null)
            {
                count++;
            }
        }

        protected override void SyncModelValue()
		{
 			this.count = Tmodel.count;

		}

	}
}
