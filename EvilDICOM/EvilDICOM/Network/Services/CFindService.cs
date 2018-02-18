﻿using EvilDICOM.Core;
using EvilDICOM.Core.Helpers;
using EvilDICOM.Network.DIMSE;
using EvilDICOM.Network.Enums;
using EvilDICOM.Network.Extensions;
using EvilDICOM.Network.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilDICOM.Network.Services
{
    public class CFindService
    {
        private DIMSEService dms;

        public CFindService(DIMSEService dIMSEService)
        {
            this.dms = dIMSEService;
        }

        //The only action that can be set outside of the class
        public Func<DICOMObject, Association, bool> CStorePayloadAction { get; set; }

        public void OnRequestRecieved(CFindRequest req, Association asc)
        {
            asc.Logger.Log("<-- DIMSE" + req.GetLogString());
            req.LogData(asc);
            asc.LastActive = DateTime.Now;
            asc.State = NetworkState.TRANSPORT_CONNECTION_OPEN;
            var resp = new CFindResponse(req);
            RetrieveResults(req, resp);
            var syntax = req.Data.FindFirst(TagHelper.SOP​Class​UID);
            dms.RaiseDIMSERequestReceived(req, asc);

            if (syntax != null)
                if (asc.PresentationContexts.Any(p => p.Id == req.DataPresentationContextId))
                {
                    try
                    {
                        var success = CStorePayloadAction != null ? CStorePayloadAction.Invoke(req.Data, asc) : false;
                        resp.Status = success ? resp.Status : (ushort)Status.FAILURE;
                        PDataMessenger.Send(resp, asc,
                            asc.PresentationContexts.First(p => p.Id == req.DataPresentationContextId));
                    }
                    catch (Exception e)
                    {
                        resp.Status = (ushort)Status.FAILURE;
                        PDataMessenger.Send(resp, asc);
                    }
                }
                else
                {
                    //Abstract syntax not supported
                    resp.Status = (ushort)Status.FAILURE;
                    PDataMessenger.Send(resp, asc);
                }
        }

        /// <summary>
        /// Parses the request and modifies the base response with the results
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resp"></param>
        private void RetrieveResults(CFindRequest req, CFindResponse resp)
        {

        }

        public void OnResponseRecieved(CFindResponse resp, Association asc)
        {
            asc.Logger.Log("<-- DIMSE" + resp.GetLogString());
            asc.LastActive = DateTime.Now;
            dms.RaiseDIMSEResponseReceived(resp, asc);
            resp.LogData(asc);
            if (resp.Status != (ushort)Status.PENDING)
                AssociationMessenger.SendReleaseRequest(asc);
        }

        
    }
}
