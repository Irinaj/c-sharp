﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PubnubApi.Interface;
using System.Net;

namespace PubnubApi.EndPoint
{
    internal class SubscribeManager : PubnubCoreBase
    {
        private static PNConfiguration config = null;
        private static IJsonPluggableLibrary jsonLibrary = null;

        public SubscribeManager(PNConfiguration pubnubConfig) :base(pubnubConfig)
        {
            config = pubnubConfig;
        }

        public SubscribeManager(PNConfiguration pubnubConfig, IJsonPluggableLibrary jsonPluggableLibrary):base(pubnubConfig, jsonPluggableLibrary)
        {
            config = pubnubConfig;
            jsonLibrary = jsonPluggableLibrary;
        }

        internal void MultiChannelUnSubscribeAll<T>(PNOperationType type)
        {
            //Retrieve the current channels already subscribed previously and terminate them
            string[] currentChannels = MultiChannelSubscribe.Keys.ToArray<string>();
            string[] currentChannelGroups = MultiChannelGroupSubscribe.Keys.ToArray<string>();

            if (currentChannels != null && currentChannels.Length >= 0)
            {
                string multiChannelName = (currentChannels.Length > 0) ? string.Join(",", currentChannels) : ",";
                string multiChannelGroupName = (currentChannelGroups.Length > 0) ? string.Join(",", currentChannelGroups) : "";

                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    if (ChannelRequest.ContainsKey(multiChannelName))
                    {
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, Aborting previous subscribe/presence requests having channel(s)={1}; channelgroup(s)={2}", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);

                        HttpWebRequest webRequest = ChannelRequest[multiChannelName];
                        ChannelRequest[multiChannelName] = null;

                        if (webRequest != null)
                        {
                            TerminateLocalClientHeartbeatTimer(webRequest.RequestUri);
                        }

                        HttpWebRequest removedRequest;
                        bool removedChannel = ChannelRequest.TryRemove(multiChannelName, out removedRequest);
                        if (removedChannel)
                        {
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, Success to remove channel(s)={1}; channelgroup(s)={2} from _channelRequest (MultiChannelUnSubscribeInit).", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                        }
                        else
                        {
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unable to remove channel(s)={1}; channelgroup(s)={2} from _channelRequest (MultiChannelUnSubscribeInit).", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                        }
                        if (webRequest != null)
                            TerminatePendingWebRequest(webRequest);
                    }
                    else
                    {
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unable to capture channel(s)={1}; channelgroup(s)={2} from _channelRequest to abort request.", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                    }
                });

                if (type == PNOperationType.PNUnsubscribeOperation)
                {
                    //just fire leave() event to REST API for safeguard
                    string channelsJsonState = BuildJsonUserState(currentChannels, currentChannelGroups, false);
                    IUrlRequestBuilder urlBuilder = new UrlRequestBuilder(config, jsonLibrary);
                    Uri request = urlBuilder.BuildMultiChannelLeaveRequest(currentChannels, currentChannelGroups, config.Uuid, channelsJsonState);

                    RequestState<T> requestState = new RequestState<T>();
                    requestState.Channels = currentChannels;
                    requestState.ChannelGroups = currentChannelGroups;
                    requestState.ResponseType = PNOperationType.Leave;
                    requestState.SubscribeRegularCallback = null;
                    requestState.PresenceRegularCallback = null;
                    requestState.WildcardPresenceCallback = null;
                    requestState.ErrorCallback = null;
                    requestState.ConnectCallback = null;
                    requestState.Reconnect = false;

                    string json = UrlProcessRequest<T>(request, requestState, false);
                }
            }

        }

        internal void MultiChannelUnSubscribeInit<T>(PNOperationType type, string channel, string channelGroup)
        {
            bool channelGroupUnsubscribeOnly = false;
            bool channelUnsubscribeOnly = false;

            string[] rawChannels = (channel != null && channel.Trim().Length > 0) ? channel.Split(',') : new string[] { };
            string[] rawChannelGroups = (channelGroup != null && channelGroup.Trim().Length > 0) ? channelGroup.Split(',') : new string[] { };

            if (rawChannels.Length > 0 && rawChannelGroups.Length <= 0)
            {
                channelUnsubscribeOnly = true;
            }
            if (rawChannels.Length <= 0 && rawChannelGroups.Length > 0)
            {
                channelGroupUnsubscribeOnly = true;
            }

            List<string> validChannels = new List<string>();
            List<string> validChannelGroups = new List<string>();

            if (rawChannels.Length > 0)
            {
                for (int index = 0; index < rawChannels.Length; index++)
                {
                    if (rawChannels[index].Trim().Length > 0)
                    {
                        string channelName = rawChannels[index].Trim();
                        if (type == PNOperationType.PresenceUnsubscribe)
                        {
                            channelName = string.Format("{0}-pnpres", channelName);
                        }
                        if (!MultiChannelSubscribe.ContainsKey(channelName))
                        {
                            StatusBuilder statusBuilder = new StatusBuilder(config, jsonLibrary);
                            PNStatus status = statusBuilder.CreateStatusResponse<T>(PNOperationType.PNUnsubscribeOperation, PNStatusCategory.PNUnexpectedDisconnectCategory, null, HttpStatusCode.Forbidden, null);

                            Announce(status);

                        }
                        else
                        {
                            validChannels.Add(channelName);
                        }
                    }
                    else
                    {
                        StatusBuilder statusBuilder = new StatusBuilder(config, jsonLibrary);
                        PNStatus status = statusBuilder.CreateStatusResponse<T>(PNOperationType.PNUnsubscribeOperation, PNStatusCategory.PNBadRequestCategory, null, HttpStatusCode.Forbidden, null);

                        Announce(status);
                    }
                }
            }

            if (rawChannelGroups.Length > 0)
            {
                for (int index = 0; index < rawChannelGroups.Length; index++)
                {
                    if (rawChannelGroups[index].Trim().Length > 0)
                    {
                        string channelGroupName = rawChannelGroups[index].Trim();
                        if (type == PNOperationType.PresenceUnsubscribe)
                        {
                            channelGroupName = string.Format("{0}-pnpres", channelGroupName);
                        }
                        if (!MultiChannelGroupSubscribe.ContainsKey(channelGroupName))
                        {
                            StatusBuilder statusBuilder = new StatusBuilder(config, jsonLibrary);
                            PNStatus status = statusBuilder.CreateStatusResponse<T>(PNOperationType.PNUnsubscribeOperation, PNStatusCategory.PNUnexpectedDisconnectCategory, null, HttpStatusCode.Forbidden, null);

                            Announce(status);
                        }
                        else
                        {
                            validChannelGroups.Add(channelGroupName);
                        }
                    }
                    else
                    {
                        StatusBuilder statusBuilder = new StatusBuilder(config, jsonLibrary);
                        PNStatus status = statusBuilder.CreateStatusResponse<T>(PNOperationType.PNUnsubscribeOperation, PNStatusCategory.PNBadRequestCategory, null, HttpStatusCode.Forbidden, null);

                        Announce(status);
                    }
                }
            }

            if (validChannels.Count > 0 || validChannelGroups.Count > 0)
            {
                //Retrieve the current channels already subscribed previously and terminate them
                string[] currentChannels = MultiChannelSubscribe.Keys.ToArray<string>();
                string[] currentChannelGroups = MultiChannelGroupSubscribe.Keys.ToArray<string>();

                if (currentChannels != null && currentChannels.Length >= 0)
                {
                    string multiChannelName = (currentChannels.Length > 0) ? string.Join(",", currentChannels) : ",";
                    string multiChannelGroupName = (currentChannelGroups.Length > 0) ? string.Join(",", currentChannelGroups) : "";

                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        if (ChannelRequest.ContainsKey(multiChannelName))
                        {
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, Aborting previous subscribe/presence requests having channel(s)={1}; channelgroup(s)={2}", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);

                            HttpWebRequest webRequest = ChannelRequest[multiChannelName];
                            ChannelRequest[multiChannelName] = null;

                            if (webRequest != null)
                            {
                                TerminateLocalClientHeartbeatTimer(webRequest.RequestUri);
                            }

                            HttpWebRequest removedRequest;
                            bool removedChannel = ChannelRequest.TryRemove(multiChannelName, out removedRequest);
                            if (removedChannel)
                            {
                                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Success to remove channel(s)={1}; channelgroup(s)={2} from _channelRequest (MultiChannelUnSubscribeInit).", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                            }
                            else
                            {
                                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unable to remove channel(s)={1}; channelgroup(s)={2} from _channelRequest (MultiChannelUnSubscribeInit).", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                            }
                            if (webRequest != null)
                                TerminatePendingWebRequest(webRequest);
                        }
                        else
                        {
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unable to capture channel(s)={1}; channelgroup(s)={2} from _channelRequest to abort request.", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                        }
                    });

                    if (type == PNOperationType.PNUnsubscribeOperation)
                    {
                        //just fire leave() event to REST API for safeguard
                        string channelsJsonState = BuildJsonUserState(validChannels.ToArray(), validChannelGroups.ToArray(), false);
                        IUrlRequestBuilder urlBuilder = new UrlRequestBuilder(config, jsonLibrary);
                        Uri request = urlBuilder.BuildMultiChannelLeaveRequest(validChannels.ToArray(), validChannelGroups.ToArray(), config.Uuid, channelsJsonState);

                        RequestState<T> requestState = new RequestState<T>();
                        requestState.Channels = new string[] { channel };
                        requestState.ChannelGroups = new string[] { channelGroup };
                        requestState.ResponseType = PNOperationType.Leave;
                        requestState.SubscribeRegularCallback = null;
                        requestState.PresenceRegularCallback = null;
                        requestState.WildcardPresenceCallback = null;
                        requestState.ErrorCallback = null;
                        requestState.ConnectCallback = null;
                        requestState.Reconnect = false;

                        string json = UrlProcessRequest<T>(request, requestState, false);
                    }
                }

                Dictionary<string, long> originalMultiChannelSubscribe = MultiChannelSubscribe.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                Dictionary<string, long> originalMultiChannelGroupSubscribe = MultiChannelGroupSubscribe.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                //Remove the valid channels from subscribe list for unsubscribe 
                for (int index = 0; index < validChannels.Count; index++)
                {
                    long timetokenValue;
                    string channelToBeRemoved = validChannels[index].ToString();
                    bool unsubscribeStatus = MultiChannelSubscribe.TryRemove(channelToBeRemoved, out timetokenValue);
                    if (unsubscribeStatus)
                    {
                        StatusBuilder statusBuilder = new StatusBuilder(config, jsonLibrary);
                        PNStatus status = statusBuilder.CreateStatusResponse<T>(PNOperationType.PNUnsubscribeOperation, PNStatusCategory.PNDisconnectedCategory, null, HttpStatusCode.OK, null);

                        Announce(status);

                        base.DeleteLocalChannelUserState(channelToBeRemoved);
                    }
                    else
                    {
                        StatusBuilder statusBuilder = new StatusBuilder(config, jsonLibrary);
                        PNStatus status = statusBuilder.CreateStatusResponse<T>(PNOperationType.PNUnsubscribeOperation, PNStatusCategory.PNDisconnectedCategory, null, HttpStatusCode.NotFound, null);

                        Announce(status);
                    }
                }
                for (int index = 0; index < validChannelGroups.Count; index++)
                {
                    long timetokenValue;
                    string channelGroupToBeRemoved = validChannelGroups[index].ToString();
                    bool unsubscribeStatus = MultiChannelGroupSubscribe.TryRemove(channelGroupToBeRemoved, out timetokenValue);
                    if (unsubscribeStatus)
                    {
                        List<object> result = new List<object>();
                        string jsonString = string.Format("[1, \"ChannelGroup {0}Unsubscribed from {1}\"]", IsPresenceChannel(channelGroupToBeRemoved) ? "Presence " : "", channelGroupToBeRemoved.Replace("-pnpres", ""));
                        result = jsonLibrary.DeserializeToListOfObject(jsonString);
                        result.Add(channelGroupToBeRemoved.Replace("-pnpres", ""));
                        result.Add("");
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, JSON response={1}", DateTime.Now.ToString(), jsonString), LoggingMethod.LevelInfo);

                        PubnubChannelGroupCallbackKey callbackKey = new PubnubChannelGroupCallbackKey();
                        callbackKey.ChannelGroup = channelGroupToBeRemoved;//.Replace("-pnpres", "");

                        if (type == PNOperationType.PresenceUnsubscribe || (type == PNOperationType.PNUnsubscribeOperation && IsPresenceChannel(channelGroupToBeRemoved)))
                        {
                            callbackKey.ResponseType = PNOperationType.Presence;
                            //if (ChannelGroupCallbacks.Count > 0 && ChannelGroupCallbacks.ContainsKey(callbackKey))
                            //{
                            //    PubnubPresenceChannelGroupCallback currentPubnubCallback = ChannelGroupCallbacks[callbackKey] as PubnubPresenceChannelGroupCallback;
                            //    if (currentPubnubCallback != null && currentPubnubCallback.DisconnectCallback != null)
                            //    {
                            //        Action<ConnectOrDisconnectAck> targetCallback = currentPubnubCallback.DisconnectCallback;
                            //        currentPubnubCallback.ConnectCallback = null;
                            //        new PNCallbackService(config, jsonLibrary).GoToCallback<ConnectOrDisconnectAck>(result, targetCallback, true, type);
                            //    }
                            //}
                        }
                        else if (type == PNOperationType.PNUnsubscribeOperation)
                        {
                            callbackKey.ResponseType = PNOperationType.PNSubscribeOperation;
                            //if (ChannelGroupCallbacks.Count > 0 && ChannelGroupCallbacks.ContainsKey(callbackKey))
                            //{
                            //    PubnubSubscribeChannelGroupCallback<T> currentPubnubCallback = ChannelGroupCallbacks[callbackKey] as PubnubSubscribeChannelGroupCallback<T>;
                            //    if (currentPubnubCallback != null && currentPubnubCallback.DisconnectCallback != null)
                            //    {
                            //        Action<ConnectOrDisconnectAck> targetCallback = currentPubnubCallback.DisconnectCallback;
                            //        currentPubnubCallback.ConnectCallback = null;
                            //        new PNCallbackService(config, jsonLibrary).GoToCallback<ConnectOrDisconnectAck>(result, targetCallback, true, type);
                            //    }
                            //}
                        }

                        base.DeleteLocalChannelGroupUserState(channelGroupToBeRemoved);
                    }
                    else
                    {
                        string message = "Unsubscribe Error. Please retry the channelgroup unsubscribe operation.";

                        PubnubErrorCode errorType = IsPresenceChannel(channelGroupToBeRemoved) ? PubnubErrorCode.PresenceUnsubscribeFailed : PubnubErrorCode.UnsubscribeFailed;

                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, channelgroup={1} unsubscribe error", DateTime.Now.ToString(), channelGroupToBeRemoved), LoggingMethod.LevelInfo);

                        //new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                        //    "", channelGroupToBeRemoved, errorCallback, message, errorType, null, null);

                    }
                }

                //Get all the channels
                string[] channels = MultiChannelSubscribe.Keys.ToArray<string>();
                string[] channelGroups = MultiChannelGroupSubscribe.Keys.ToArray<string>();

                //Check any chained subscribes while unsubscribe 
                foreach (string key in MultiChannelSubscribe.Keys)
                {
                    if (!originalMultiChannelSubscribe.ContainsKey(key))
                    {
                        return;
                    }
                }
                foreach (string key in MultiChannelGroupSubscribe.Keys)
                {
                    if (!originalMultiChannelGroupSubscribe.ContainsKey(key))
                    {
                        return;
                    }
                }

                channels = (channels != null) ? channels : new string[] { };
                channelGroups = (channelGroups != null) ? channelGroups : new string[] { };

                if (channels.Length > 0 || channelGroups.Length > 0)
                {
                    string multiChannel = (channels.Length > 0) ? string.Join(",", channels) : ",";

                    RequestState<T> state = new RequestState<T>();
                    ChannelRequest.AddOrUpdate(multiChannel, state.Request, (key, oldValue) => state.Request);

                    ResetInternetCheckSettings(channels, channelGroups);

                    //Modify the value for type ResponseType. Presence or Subscrie is ok, but sending the close value would make sense
                    if (string.Join(",", channels).IndexOf("-pnpres") > 0 || string.Join(",", channelGroups).IndexOf("-pnpres") > 0)
                    {
                        type = PNOperationType.Presence;
                        //channelCallbacks.TryGetValue(
                    }
                    else
                    {
                        type = PNOperationType.PNSubscribeOperation;
                    }

                    //Continue with any remaining channels for subscribe/presence
                    MultiChannelSubscribeRequest<T>(type, channels, channelGroups, 0, false);
                }
                else
                {
                    if (presenceHeartbeatTimer != null)
                    {
                        // Stop the presence heartbeat timer if there are no channels subscribed
                        presenceHeartbeatTimer.Dispose();
                        presenceHeartbeatTimer = null;
                    }
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, All channels are Unsubscribed. Further subscription was stopped", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                }
            }

        }

        internal void MultiChannelSubscribeInit<T>(PNOperationType responseType, string[] rawChannels, string[] rawChannelGroups)
        {
            bool channelGroupSubscribeOnly = false;
            bool channelSubscribeOnly = false;

            string channel = (rawChannels != null) ? string.Join(",", rawChannels) : "";
            string channelGroup = (rawChannelGroups != null) ? string.Join(",", rawChannelGroups) : "";

            List<string> validChannels = new List<string>();
            List<string> validChannelGroups = new List<string>();

            bool networkConnection = InternetConnectionStatusWithUnitTestCheck(channel, channelGroup, null, rawChannels, rawChannelGroups);

            if (rawChannels.Length > 0 && networkConnection)
            {
                if (rawChannels.Length != rawChannels.Distinct().Count())
                {
                    rawChannels = rawChannels.Distinct().ToArray();
                    string message = "Detected and removed duplicate channels";

                    //new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                    //    channel, channelGroup, errorCallback, message, PubnubErrorCode.DuplicateChannel, null, null);
                }

                for (int index = 0; index < rawChannels.Length; index++)
                {
                    if (rawChannels[index].Trim().Length > 0)
                    {
                        string channelName = rawChannels[index].Trim();

                        if (responseType == PNOperationType.Presence)
                        {
                            channelName = string.Format("{0}-pnpres", channelName);
                        }
                        if (MultiChannelSubscribe.ContainsKey(channelName))
                        {
                            string message = string.Format("{0}Already subscribed", IsPresenceChannel(channelName) ? "Presence " : "");

                            PubnubErrorCode errorType = IsPresenceChannel(channelName) ? PubnubErrorCode.AlreadyPresenceSubscribed : PubnubErrorCode.AlreadySubscribed;

                            //new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                            //    channelName.Replace("-pnpres", ""), "", errorCallback, message, errorType, null, null);
                        }
                        else
                        {
                            validChannels.Add(channelName);
                        }
                    }
                }
            }

            if (rawChannelGroups != null && rawChannelGroups.Length > 0 && networkConnection)
            {
                if (rawChannelGroups.Length != rawChannelGroups.Distinct().Count())
                {
                    rawChannelGroups = rawChannelGroups.Distinct().ToArray();
                    string message = "Detected and removed duplicate channel groups";

                    //new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                    //    channel, channelGroup, errorCallback, message, PubnubErrorCode.DuplicateChannel, null, null);
                }

                for (int index = 0; index < rawChannelGroups.Length; index++)
                {
                    if (rawChannelGroups[index].Trim().Length > 0)
                    {
                        string channelGroupName = rawChannelGroups[index].Trim();

                        if (responseType == PNOperationType.Presence)
                        {
                            channelGroupName = string.Format("{0}-pnpres", channelGroupName);
                        }
                        if (MultiChannelGroupSubscribe.ContainsKey(channelGroupName))
                        {
                            string message = string.Format("{0}Already subscribed", IsPresenceChannel(channelGroupName) ? "Presence " : "");

                            PubnubErrorCode errorType = IsPresenceChannel(channelGroupName) ? PubnubErrorCode.AlreadyPresenceSubscribed : PubnubErrorCode.AlreadySubscribed;

                            //new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Info, PubnubMessageSource.Client,
                            //    "", channelGroupName.Replace("-pnpres", ""), errorCallback, message, errorType, null, null);
                        }
                        else
                        {
                            validChannelGroups.Add(channelGroupName);
                        }
                    }
                }
            }

            if (validChannels.Count > 0 || validChannelGroups.Count > 0)
            {
                //Retrieve the current channels already subscribed previously and terminate them
                string[] currentChannels = MultiChannelSubscribe.Keys.ToArray<string>();
                string[] currentChannelGroups = MultiChannelGroupSubscribe.Keys.ToArray<string>();

                if (currentChannels != null && currentChannels.Length >= 0)
                {
                    string multiChannelName = (currentChannels.Length > 0) ? string.Join(",", currentChannels) : ",";
                    string multiChannelGroupName = (currentChannelGroups.Length > 0) ? string.Join(",", currentChannelGroups) : "";

                    if (ChannelRequest.ContainsKey(multiChannelName))
                    {
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, Aborting previous subscribe/presence requests having channel(s)={1}; channelgroup(s)={2}", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                        HttpWebRequest webRequest = ChannelRequest[multiChannelName];
                        ChannelRequest[multiChannelName] = null;

                        if (webRequest != null)
                            TerminateLocalClientHeartbeatTimer(webRequest.RequestUri);

                        HttpWebRequest removedRequest;
                        ChannelRequest.TryRemove(multiChannelName, out removedRequest);
                        bool removedChannel = ChannelRequest.TryRemove(multiChannelName, out removedRequest);
                        if (removedChannel)
                        {
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, Success to remove channel(s)={1}; channelgroup(s)={2} from _channelRequest (MultiChannelSubscribeInit).", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                        }
                        else
                        {
                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unable to remove channel(s)={1}; channelgroup(s)={2} from _channelRequest (MultiChannelSubscribeInit).", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                        }
                        if (webRequest != null)
                            TerminatePendingWebRequest(webRequest);
                    }
                    else
                    {
                        LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unable to capture channel(s)={1}; channelgroup(s)={2} from _channelRequest to abort request.", DateTime.Now.ToString(), multiChannelName, multiChannelGroupName), LoggingMethod.LevelInfo);
                    }
                }

                //Add the valid channels to the channels subscribe list for tracking
                for (int index = 0; index < validChannels.Count; index++)
                {
                    string currentLoopChannel = validChannels[index].ToString();
                    MultiChannelSubscribe.GetOrAdd(currentLoopChannel, 0);


                    //if (responseType == PNOperationType.Presence || (responseType == PNOperationType.PNSubscribeOperation && IsPresenceChannel(currentLoopChannel)))
                    //{
                    //    PubnubChannelCallbackKey callbackPresenceKey = new PubnubChannelCallbackKey();
                    //    callbackPresenceKey.Channel = currentLoopChannel;
                    //    callbackPresenceKey.ResponseType = PNOperationType.Presence;

                    //    PubnubPresenceChannelCallback pubnubChannelCallbacks = new PubnubPresenceChannelCallback();
                    //    pubnubChannelCallbacks.PresenceRegularCallback = presenceRegularCallback;
                    //    pubnubChannelCallbacks.ConnectCallback = connectCallback;
                    //    pubnubChannelCallbacks.DisconnectCallback = disconnectCallback;
                    //    pubnubChannelCallbacks.ErrorCallback = errorCallback;

                    //    ChannelCallbacks.AddOrUpdate(callbackPresenceKey, pubnubChannelCallbacks, (key, oldValue) => pubnubChannelCallbacks);
                    //}
                    //else
                    //{
                    //    PubnubChannelCallbackKey callbackSubscribeKey = new PubnubChannelCallbackKey();
                    //    callbackSubscribeKey.Channel = currentLoopChannel;
                    //    callbackSubscribeKey.ResponseType = responseType;

                    //    PubnubSubscribeChannelCallback<T> pubnubChannelCallbacks = new PubnubSubscribeChannelCallback<T>();
                    //    pubnubChannelCallbacks.SubscribeRegularCallback = subscribeRegularCallback;
                    //    pubnubChannelCallbacks.ConnectCallback = connectCallback;
                    //    pubnubChannelCallbacks.DisconnectCallback = disconnectCallback;
                    //    pubnubChannelCallbacks.WildcardPresenceCallback = wildcardPresenceCallback;
                    //    pubnubChannelCallbacks.ErrorCallback = errorCallback;

                    //    ChannelCallbacks.AddOrUpdate(callbackSubscribeKey, pubnubChannelCallbacks, (key, oldValue) => pubnubChannelCallbacks);

                    //    ChannelSubscribeObjectType.AddOrUpdate(currentLoopChannel, typeof(T), (key, oldValue) => typeof(T));
                    //}
                }


                for (int index = 0; index < validChannelGroups.Count; index++)
                {
                    string currentLoopChannelGroup = validChannelGroups[index].ToString();
                    MultiChannelGroupSubscribe.GetOrAdd(currentLoopChannelGroup, 0);

                    PubnubChannelGroupCallbackKey callbackKey = new PubnubChannelGroupCallbackKey();
                    callbackKey.ChannelGroup = currentLoopChannelGroup;

                    //if (responseType == PNOperationType.Presence || (responseType == PNOperationType.PNSubscribeOperation && IsPresenceChannel(currentLoopChannelGroup)))
                    //{
                    //    callbackKey.ResponseType = PNOperationType.Presence;

                    //    PubnubPresenceChannelGroupCallback pubnubChannelGroupCallbacks = new PubnubPresenceChannelGroupCallback();
                    //    pubnubChannelGroupCallbacks.PresenceRegularCallback = presenceRegularCallback;
                    //    pubnubChannelGroupCallbacks.ConnectCallback = connectCallback;
                    //    pubnubChannelGroupCallbacks.DisconnectCallback = disconnectCallback;
                    //    pubnubChannelGroupCallbacks.ErrorCallback = errorCallback;

                    //    ChannelGroupCallbacks.AddOrUpdate(callbackKey, pubnubChannelGroupCallbacks, (key, oldValue) => pubnubChannelGroupCallbacks);
                    //}
                    //else
                    //{
                    //    callbackKey.ResponseType = PNOperationType.PNSubscribeOperation;

                    //    PubnubSubscribeChannelGroupCallback<T> pubnubChannelGroupCallbacks = new PubnubSubscribeChannelGroupCallback<T>();
                    //    pubnubChannelGroupCallbacks.SubscribeRegularCallback = subscribeRegularCallback;
                    //    pubnubChannelGroupCallbacks.WildcardPresenceCallback = wildcardPresenceCallback;
                    //    pubnubChannelGroupCallbacks.ConnectCallback = connectCallback;
                    //    pubnubChannelGroupCallbacks.DisconnectCallback = disconnectCallback;
                    //    pubnubChannelGroupCallbacks.ErrorCallback = errorCallback;

                    //    ChannelGroupCallbacks.AddOrUpdate(callbackKey, pubnubChannelGroupCallbacks, (key, oldValue) => pubnubChannelGroupCallbacks);

                    //    ChannelGroupSubscribeObjectType.AddOrUpdate(currentLoopChannelGroup, typeof(T), (key, oldValue) => typeof(T));
                    //}
                }

                //Get all the channels
                string[] channels = MultiChannelSubscribe.Keys.ToArray<string>();
                string[] channelGroups = MultiChannelGroupSubscribe.Keys.ToArray<string>();

                if (channels != null && channels.Length > 0 && (channelGroups == null || channelGroups.Length == 0))
                {
                    channelSubscribeOnly = true;
                }
                if (channelGroups != null && channelGroups.Length > 0 && (channels == null || channels.Length == 0))
                {
                    channelGroupSubscribeOnly = true;
                }

                RequestState<T> state = new RequestState<T>();
                if (channelGroupSubscribeOnly)
                {
                    ChannelRequest.AddOrUpdate(",", state.Request, (key, oldValue) => state.Request);
                }
                else
                {
                    ChannelRequest.AddOrUpdate(string.Join(",", channels), state.Request, (key, oldValue) => state.Request);
                }

                ResetInternetCheckSettings(channels, channelGroups);
                MultiChannelSubscribeRequest<T>(responseType, channels, channelGroups, 0, false);
            }
        }

        private static void MultiChannelSubscribeRequest<T>(PNOperationType type, string[] channels, string[] channelGroups, object timetoken, bool reconnect)
        {
            //Exit if the channel is unsubscribed
            if (MultiChannelSubscribe != null && MultiChannelSubscribe.Count <= 0 && MultiChannelGroupSubscribe != null && MultiChannelGroupSubscribe.Count <= 0)
            {
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Zero channels/channelGroups. Further subscription was stopped", DateTime.Now.ToString()), LoggingMethod.LevelInfo);
                return;
            }

            string multiChannel = (channels != null && channels.Length > 0) ? string.Join(",", channels) : ",";
            string multiChannelGroup = (channelGroups != null && channelGroups.Length > 0) ? string.Join(",", channelGroups) : "";
            if (!ChannelRequest.ContainsKey(multiChannel))
            {
                return;
            }

            bool networkConnection = CheckInternetConnectionStatus(pubnetSystemActive, null, channels, channelGroups);

            if (!networkConnection)
            {
                ChannelInternetStatus.AddOrUpdate(multiChannel, networkConnection, (key, oldValue) => networkConnection);
                ChannelGroupInternetStatus.AddOrUpdate(multiChannelGroup, networkConnection, (key, oldValue) => networkConnection);
            }

            if (((ChannelInternetStatus.ContainsKey(multiChannel) && !ChannelInternetStatus[multiChannel])
                || (multiChannelGroup != "" && ChannelGroupInternetStatus.ContainsKey(multiChannelGroup) && !ChannelGroupInternetStatus[multiChannelGroup]))
                && pubnetSystemActive)
            {
                if (ReconnectNetworkIfOverrideTcpKeepAlive<T>(type, channels, channelGroups, timetoken))
                {
                    return;
                }

            }

            // Begin recursive subscribe
            try
            {
                RegisterPresenceHeartbeatTimer<T>(channels, channelGroups);

                long lastTimetoken = 0;
                long minimumTimetoken1 = (MultiChannelSubscribe.Count > 0) ? MultiChannelSubscribe.Min(token => token.Value) : 0;
                long minimumTimetoken2 = (MultiChannelGroupSubscribe.Count > 0) ? MultiChannelGroupSubscribe.Min(token => token.Value) : 0;
                long minimumTimetoken = Math.Max(minimumTimetoken1, minimumTimetoken2);

                long maximumTimetoken1 = (MultiChannelSubscribe.Count > 0) ? MultiChannelSubscribe.Max(token => token.Value) : 0;
                long maximumTimetoken2 = (MultiChannelGroupSubscribe.Count > 0) ? MultiChannelGroupSubscribe.Max(token => token.Value) : 0;
                long maximumTimetoken = Math.Max(maximumTimetoken1, maximumTimetoken2);


                if (minimumTimetoken == 0 || reconnect || UuidChanged)
                {
                    lastTimetoken = 0;
                    UuidChanged = false;
                }
                else
                {
                    if (LastSubscribeTimetoken == maximumTimetoken)
                    {
                        lastTimetoken = maximumTimetoken;
                    }
                    else
                    {
                        lastTimetoken = LastSubscribeTimetoken;
                    }
                }
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Building request for channel(s)={1}, channelgroup(s)={2} with timetoken={3}", DateTime.Now.ToString(), multiChannel, multiChannelGroup, lastTimetoken), LoggingMethod.LevelInfo);
                // Build URL
                string channelsJsonState = BuildJsonUserState(channels, channelGroups, false);
                config.Uuid = Uuid; // to make sure we capture if UUID is changed
                IUrlRequestBuilder urlBuilder = new UrlRequestBuilder(config, jsonLibrary);
                Uri request = urlBuilder.BuildMultiChannelSubscribeRequest(channels, channelGroups, (Convert.ToInt64(timetoken.ToString()) == 0) ? Convert.ToInt64(timetoken.ToString()) : lastTimetoken, channelsJsonState);

                RequestState<T> pubnubRequestState = new RequestState<T>();
                pubnubRequestState.Channels = channels;
                pubnubRequestState.ChannelGroups = channelGroups;
                pubnubRequestState.ResponseType = type;
                //pubnubRequestState.ConnectCallback = connectCallback;
                //pubnubRequestState.SubscribeRegularCallback = subscribeRegularCallback;
                //pubnubRequestState.PresenceRegularCallback = presenceRegularCallback;
                //pubnubRequestState.WildcardPresenceCallback = wildcardPresenceCallback;
                //pubnubRequestState.ErrorCallback = errorCallback;
                pubnubRequestState.Reconnect = reconnect;
                pubnubRequestState.Timetoken = Convert.ToInt64(timetoken.ToString());

                // Wait for message
                string json = UrlProcessRequest<T>(request, pubnubRequestState, false);
                if (!string.IsNullOrEmpty(json))
                {
                    List<object> result = ProcessJsonResponse<T>(pubnubRequestState, json);
                    ProcessResponseCallbacks(result, pubnubRequestState);

                    if ((pubnubRequestState.ResponseType == PNOperationType.PNSubscribeOperation || pubnubRequestState.ResponseType == PNOperationType.Presence) && (result != null) && (result.Count > 0))
                    {
                        if (pubnubRequestState.Channels != null)
                        {
                            foreach (string currentChannel in pubnubRequestState.Channels)
                            {
                                MultiChannelSubscribe.AddOrUpdate(currentChannel, Convert.ToInt64(result[1].ToString()), (key, oldValue) => Convert.ToInt64(result[1].ToString()));
                            }
                        }
                        if (pubnubRequestState.ChannelGroups != null && pubnubRequestState.ChannelGroups.Length > 0)
                        {
                            foreach (string currentChannelGroup in pubnubRequestState.ChannelGroups)
                            {
                                MultiChannelGroupSubscribe.AddOrUpdate(currentChannelGroup, Convert.ToInt64(result[1].ToString()), (key, oldValue) => Convert.ToInt64(result[1].ToString()));
                            }
                        }
                    }

                    switch (pubnubRequestState.ResponseType)
                    {
                        case PNOperationType.PNSubscribeOperation:
                        case PNOperationType.Presence:
                            MultiplexInternalCallback<T>(pubnubRequestState.ResponseType, result);
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    MultiplexExceptionHandler<T>(type, channels, channelGroups, false, false);
                }
            }
            catch (Exception ex)
            {
                LoggingMethod.WriteToLog(string.Format("DateTime {0} method:_subscribe \n channel={1} \n timetoken={2} \n Exception Details={3}", DateTime.Now.ToString(), string.Join(",", channels), timetoken.ToString(), ex.ToString()), LoggingMethod.LevelError);

                //new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                //    string.Join(",", channels), string.Join(",", channelGroups), errorCallback, ex, null, null);

                MultiChannelSubscribeRequest<T>(type, channels, channelGroups, timetoken, false);
            }
        }

        private bool InternetConnectionStatusWithUnitTestCheck(string channel, string channelGroup, Action<PubnubClientError> errorCallback, string[] rawChannels, string[] rawChannelGroups)
        {
            bool networkConnection = InternetConnectionStatus(channel, channelGroup, errorCallback, rawChannels, rawChannelGroups);
            if (!networkConnection)
            {
                string message = "Network connnect error - Internet connection is not available.";
                new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                    channel, channelGroup, errorCallback, message,
                    PubnubErrorCode.NoInternet, null, null);
            }

            return networkConnection;
        }

        private static void MultiplexExceptionHandler<T>(PNOperationType type, string[] channels, string[] channelGroups, bool reconnectMaxTried, bool resumeOnReconnect)
        {
            string channel = "";
            string channelGroup = "";
            if (channels != null)
            {
                channel = string.Join(",", channels);
            }
            if (channelGroups != null)
            {
                channelGroup = string.Join(",", channelGroups);
            }

            List<object> result = new List<object>();
            result.Add("0");
            if (resumeOnReconnect)
            {
                result.Add(0); //send 0 time token to enable presence event
            }
            else
            {
                result.Add(LastSubscribeTimetoken); //get last timetoken
            }
            if (channelGroups != null && channelGroups.Length > 0)
            {
                result.Add(channelGroups);
            }
            result.Add(channels); //send channel name

            MultiplexInternalCallback<T>(type, result);
        }

        private static void MultiplexInternalCallback<T>(PNOperationType type, object multiplexResult)
        {
            List<object> message = multiplexResult as List<object>;
            string[] channels = null;
            string[] channelGroups = null;
            if (message != null && message.Count >= 3)
            {
                if (message[message.Count - 1] is string[])
                {
                    channels = message[message.Count - 1] as string[];
                }
                else
                {
                    channels = message[message.Count - 1].ToString().Split(',') as string[];
                }

                if (channels.Length == 1 && channels[0] == "")
                {
                    channels = new string[] { };
                }
                if (message.Count >= 4)
                {
                    if (message[message.Count - 2] is string[])
                    {
                        channelGroups = message[message.Count - 2] as string[];
                    }
                    else if (message[message.Count - 2].ToString() != "")
                    {
                        channelGroups = message[message.Count - 2].ToString().Split(',') as string[];
                    }
                }
            }
            else
            {
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Lost Channel Name for resubscribe", DateTime.Now.ToString()), LoggingMethod.LevelError);
                return;
            }

            if (message != null && message.Count >= 3)
            {
                MultiChannelSubscribeRequest<T>(type, channels, channelGroups, (object)message[1], false);
            }
        }

        private static bool ReconnectNetworkIfOverrideTcpKeepAlive<T>(PNOperationType type, string[] channels, string[] channelGroups, object timetoken)
        {
            if (overrideTcpKeepAlive)
            {
                LoggingMethod.WriteToLog(string.Format("DateTime {0}, Subscribe - No internet connection for channel={1} and channelgroup={2}", DateTime.Now.ToString(), string.Join(",", channels), channelGroups != null ? string.Join(",", channelGroups) : ""), LoggingMethod.LevelInfo);
                ReconnectState<T> netState = new ReconnectState<T>();
                netState.Channels = channels;
                netState.ChannelGroups = channelGroups;
                netState.ResponseType = type;
                //netState.SubscribeRegularCallback = subscribeCallback;
                //netState.PresenceRegularCallback = presenceCallback;
                //netState.WildcardPresenceCallback = presenceWildcardCallback;
                //netState.ErrorCallback = errorCallback;
                //netState.ConnectCallback = connectCallback;
                netState.Timetoken = timetoken;
                ReconnectNetwork<T>(netState);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void ReconnectNetwork<T>(ReconnectState<T> netState)
        {
            if (netState != null && ((netState.Channels != null && netState.Channels.Length > 0) || (netState.ChannelGroups != null && netState.ChannelGroups.Length > 0)))
            {
                System.Threading.Timer timer = new Timer(new TimerCallback(ReconnectNetworkCallback<T>), netState, 0,
                                                      (-1 == PubnubNetworkTcpCheckIntervalInSeconds) ? Timeout.Infinite : PubnubNetworkTcpCheckIntervalInSeconds * 1000);

                if (netState.Channels != null && netState.Channels.Length > 0)
                {
                    ChannelReconnectTimer.AddOrUpdate(string.Join(",", netState.Channels), timer, (key, oldState) => timer);
                }
                if (netState.ChannelGroups != null && netState.ChannelGroups.Length > 0)
                {
                    ChannelGroupReconnectTimer.AddOrUpdate(string.Join(",", netState.ChannelGroups), timer, (key, oldState) => timer);
                }
            }
        }

        protected static void ReconnectNetworkCallback<T>(System.Object reconnectState)
        {
            string channel = "";
            string channelGroup = "";

            ReconnectState<T> netState = reconnectState as ReconnectState<T>;
            try
            {
                if (netState != null && ((netState.Channels != null && netState.Channels.Length > 0) || (netState.ChannelGroups != null && netState.ChannelGroups.Length > 0)))
                {
                    if (netState.Channels != null && netState.Channels.Length > 0)
                    {
                        channel = (netState.Channels.Length > 0) ? string.Join(",", netState.Channels) : ",";

                        if (ChannelInternetStatus.ContainsKey(channel)
                            && (netState.ResponseType == PNOperationType.PNSubscribeOperation || netState.ResponseType == PNOperationType.Presence))
                        {
                            bool networkConnection = CheckInternetConnectionStatus(pubnetSystemActive, netState.ErrorCallback, netState.Channels, netState.ChannelGroups);

                            if (ChannelInternetStatus[channel])
                            {
                                //Reset Retry if previous state is true
                                //ChannelInternetRetry.AddOrUpdate(channel, 0, (key, oldValue) => 0);
                            }
                            else
                            {
                                ChannelInternetStatus.AddOrUpdate(channel, networkConnection, (key, oldValue) => networkConnection);

                                LoggingMethod.WriteToLog(string.Format("DateTime {0}, channel={1} {2} reconnectNetworkCallback. Retry", DateTime.Now.ToString(), channel, netState.ResponseType), LoggingMethod.LevelInfo);

                                if (netState.Channels != null && netState.Channels.Length > 0)
                                {
                                    for (int index = 0; index < netState.Channels.Length; index++)
                                    {
                                        string activeChannel = (netState.Channels != null && netState.Channels.Length > 0) ? netState.Channels[index].ToString() : "";
                                        string activeChannelGroup = (netState.ChannelGroups != null && netState.ChannelGroups.Length > 0) ? netState.ChannelGroups[index].ToString() : "";

                                        string message = "Detected internet connection problem.Retrying connection";

                                        PubnubChannelCallbackKey callbackKey = new PubnubChannelCallbackKey();
                                        callbackKey.Channel = activeChannel;
                                        callbackKey.ResponseType = netState.ResponseType;

                                        //if (ChannelCallbacks.Count > 0 && ChannelCallbacks.ContainsKey(callbackKey))
                                        //{
                                        //    if (netState.ResponseType == PNOperationType.Presence)
                                        //    {
                                        //        PubnubPresenceChannelCallback currentPubnubCallback = ChannelCallbacks[callbackKey] as PubnubPresenceChannelCallback;
                                        //        if (currentPubnubCallback != null && currentPubnubCallback.ErrorCallback != null)
                                        //        {
                                        //            new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Warn, PubnubMessageSource.Client,
                                        //                activeChannel, activeChannelGroup, currentPubnubCallback.ErrorCallback, message, PubnubErrorCode.NoInternet,
                                        //                null, null);
                                        //        }
                                        //    }
                                        //    else
                                        //    {
                                        //        PubnubSubscribeChannelCallback<T> currentPubnubCallback = ChannelCallbacks[callbackKey] as PubnubSubscribeChannelCallback<T>;
                                        //        if (currentPubnubCallback != null && currentPubnubCallback.ErrorCallback != null)
                                        //        {
                                        //            new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Warn, PubnubMessageSource.Client,
                                        //                activeChannel, activeChannelGroup, currentPubnubCallback.ErrorCallback, message, PubnubErrorCode.NoInternet,
                                        //                null, null);
                                        //        }
                                        //    }
                                        //}
                                    }
                                }

                            }
                        }

                        if (ChannelInternetStatus.ContainsKey(channel) && ChannelInternetStatus[channel])
                        {
                            if (ChannelReconnectTimer.ContainsKey(channel))
                            {
                                try
                                {
                                    ChannelReconnectTimer[channel].Change(Timeout.Infinite, Timeout.Infinite);
                                    ChannelReconnectTimer[channel].Dispose();
                                }
                                catch { }
                            }
                            string multiChannel = (netState.Channels != null) ? string.Join(",", netState.Channels) : "";
                            string multiChannelGroup = (netState.ChannelGroups != null) ? string.Join(",", netState.ChannelGroups) : "";
                            string message = "Internet connection available";

                            new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Warn, PubnubMessageSource.Client,
                                multiChannel, multiChannelGroup, netState.ErrorCallback, message, PubnubErrorCode.YesInternet, null, null);

                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, {1} {2} reconnectNetworkCallback. Internet Available : {3}", DateTime.Now.ToString(), channel, netState.ResponseType, ChannelInternetStatus[channel]), LoggingMethod.LevelInfo);
                            switch (netState.ResponseType)
                            {
                                case PNOperationType.PNSubscribeOperation:
                                case PNOperationType.Presence:
                                    MultiChannelSubscribeRequest<T>(netState.ResponseType, netState.Channels, netState.ChannelGroups, netState.Timetoken, true);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    else if (netState.ChannelGroups != null && netState.ChannelGroups.Length > 0)
                    {
                        channelGroup = string.Join(",", netState.ChannelGroups);

                        if (channelGroup != "" && ChannelGroupInternetStatus.ContainsKey(channelGroup)
                            && (netState.ResponseType == PNOperationType.PNSubscribeOperation || netState.ResponseType == PNOperationType.Presence))
                        {
                            bool networkConnection = CheckInternetConnectionStatus(pubnetSystemActive, netState.ErrorCallback, netState.Channels, netState.ChannelGroups);

                            if (ChannelGroupInternetStatus[channelGroup])
                            {
                                //Reset Retry if previous state is true
                                //ChannelGroupInternetRetry.AddOrUpdate(channelGroup, 0, (key, oldValue) => 0);
                            }
                            else
                            {
                                ChannelGroupInternetStatus.AddOrUpdate(channelGroup, networkConnection, (key, oldValue) => networkConnection);

                                //ChannelGroupInternetRetry.AddOrUpdate(channelGroup, 1, (key, oldValue) => oldValue + 1);
                                LoggingMethod.WriteToLog(string.Format("DateTime {0}, channelgroup={1} {2} reconnectNetworkCallback. Retrying", DateTime.Now.ToString(), channelGroup, netState.ResponseType), LoggingMethod.LevelInfo);

                                if (netState.ChannelGroups != null && netState.ChannelGroups.Length > 0)
                                {
                                    for (int index = 0; index < netState.ChannelGroups.Length; index++)
                                    {
                                        string activeChannel = (netState.Channels != null && netState.Channels.Length > 0) ? netState.Channels[index].ToString() : "";
                                        string activeChannelGroup = (netState.ChannelGroups != null && netState.ChannelGroups.Length > 0) ? netState.ChannelGroups[index].ToString() : "";

                                        string message = "Detected internet connection problem. Retrying connection";

                                        PubnubChannelGroupCallbackKey callbackKey = new PubnubChannelGroupCallbackKey();
                                        callbackKey.ChannelGroup = activeChannelGroup;
                                        callbackKey.ResponseType = netState.ResponseType;

                                        //if (ChannelGroupCallbacks.Count > 0 && ChannelGroupCallbacks.ContainsKey(callbackKey))
                                        //{
                                        //    if (netState.ResponseType == PNOperationType.Presence)
                                        //    {
                                        //        PubnubPresenceChannelGroupCallback currentPubnubCallback = ChannelGroupCallbacks[callbackKey] as PubnubPresenceChannelGroupCallback;
                                        //        if (currentPubnubCallback != null && currentPubnubCallback.ErrorCallback != null)
                                        //        {
                                        //            new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Warn, PubnubMessageSource.Client,
                                        //                activeChannel, activeChannelGroup, currentPubnubCallback.ErrorCallback, message, PubnubErrorCode.NoInternet,
                                        //                null, null);
                                        //        }
                                        //    }
                                        //    else
                                        //    {
                                        //        PubnubSubscribeChannelGroupCallback<T> currentPubnubCallback = ChannelGroupCallbacks[callbackKey] as PubnubSubscribeChannelGroupCallback<T>;
                                        //        if (currentPubnubCallback != null && currentPubnubCallback.ErrorCallback != null)
                                        //        {
                                        //            new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Warn, PubnubMessageSource.Client,
                                        //                activeChannel, activeChannelGroup, currentPubnubCallback.ErrorCallback, message, PubnubErrorCode.NoInternet,
                                        //                null, null);
                                        //        }
                                        //    }
                                        //}
                                    }
                                }
                            }
                        }

                        if (ChannelGroupInternetStatus[channelGroup])
                        {
                            if (ChannelGroupReconnectTimer.ContainsKey(channelGroup))
                            {
                                try
                                {
                                    ChannelGroupReconnectTimer[channelGroup].Change(Timeout.Infinite, Timeout.Infinite);
                                    ChannelGroupReconnectTimer[channelGroup].Dispose();
                                }
                                catch { }
                            }
                            string multiChannel = (netState.Channels != null) ? string.Join(",", netState.Channels) : "";
                            string multiChannelGroup = (netState.ChannelGroups != null) ? string.Join(",", netState.ChannelGroups) : "";
                            string message = "Internet connection available";

                            new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Warn, PubnubMessageSource.Client,
                                multiChannel, multiChannelGroup, netState.ErrorCallback, message, PubnubErrorCode.YesInternet, null, null);

                            LoggingMethod.WriteToLog(string.Format("DateTime {0}, channelgroup={1} {2} reconnectNetworkCallback. Internet Available", DateTime.Now.ToString(), channelGroup, netState.ResponseType), LoggingMethod.LevelInfo);
                            switch (netState.ResponseType)
                            {
                                case PNOperationType.PNSubscribeOperation:
                                case PNOperationType.Presence:
                                    MultiChannelSubscribeRequest<T>(netState.ResponseType, netState.Channels, netState.ChannelGroups, netState.Timetoken, true);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    LoggingMethod.WriteToLog(string.Format("DateTime {0}, Unknown request state in reconnectNetworkCallback", DateTime.Now.ToString()), LoggingMethod.LevelError);
                }
            }
            catch (Exception ex)
            {
                if (netState != null)
                {
                    string multiChannel = (netState.Channels != null) ? string.Join(",", netState.Channels) : "";
                    string multiChannelGroup = (netState.ChannelGroups != null) ? string.Join(",", netState.ChannelGroups) : "";

                    new PNCallbackService(config, jsonLibrary).CallErrorCallback(PubnubErrorSeverity.Critical, PubnubMessageSource.Client,
                        multiChannel, multiChannelGroup, netState.ErrorCallback, ex, null, null);
                }

                LoggingMethod.WriteToLog(string.Format("DateTime {0} method:reconnectNetworkCallback \n Exception Details={1}", DateTime.Now.ToString(), ex.ToString()), LoggingMethod.LevelError);
            }
        }

        private static void RegisterPresenceHeartbeatTimer<T>(string[] channels, string[] channelGroups)
        {
            if (presenceHeartbeatTimer != null)
            {
                presenceHeartbeatTimer.Dispose();
                presenceHeartbeatTimer = null;
            }
            if ((channels != null && channels.Length > 0 && channels.Where(s => s.Contains("-pnpres") == false).ToArray().Length > 0)
                || (channelGroups != null && channelGroups.Length > 0 && channelGroups.Where(s => s.Contains("-pnpres") == false).ToArray().Length > 0))
            {
                RequestState<T> presenceHeartbeatState = new RequestState<T>();
                presenceHeartbeatState.Channels = channels;
                presenceHeartbeatState.ChannelGroups = channelGroups;
                presenceHeartbeatState.ResponseType = PNOperationType.PNHeartbeatOperation;
                //presenceHeartbeatState.ErrorCallback = errorCallback;
                presenceHeartbeatState.Request = null;
                presenceHeartbeatState.Response = null;

                if (config.PresenceHeartbeatInterval > 0)
                {
                    presenceHeartbeatTimer = new Timer(OnPresenceHeartbeatIntervalTimeout<T>, presenceHeartbeatState, config.PresenceHeartbeatInterval * 1000, config.PresenceHeartbeatInterval * 1000);
                }
            }
        }

        private static void OnPresenceHeartbeatIntervalTimeout<T>(System.Object presenceHeartbeatState)
        {
            //Make presence heartbeat call
            RequestState<T> currentState = presenceHeartbeatState as RequestState<T>;
            if (currentState != null)
            {
                bool networkConnection = CheckInternetConnectionStatus(pubnetSystemActive, currentState.ErrorCallback, currentState.Channels, currentState.ChannelGroups);
                if (networkConnection)
                {
                    string[] subscriberChannels = (currentState.Channels != null) ? currentState.Channels.Where(s => s.Contains("-pnpres") == false).ToArray() : null;
                    string[] subscriberChannelGroups = (currentState.ChannelGroups != null) ? currentState.ChannelGroups.Where(s => s.Contains("-pnpres") == false).ToArray() : null;

                    if ((subscriberChannels != null && subscriberChannels.Length > 0) || (subscriberChannelGroups != null && subscriberChannelGroups.Length > 0))
                    {
                        string channelsJsonState = BuildJsonUserState(subscriberChannels, subscriberChannelGroups, false);
                        IUrlRequestBuilder urlBuilder = new UrlRequestBuilder(config, jsonLibrary);
                        Uri request = urlBuilder.BuildPresenceHeartbeatRequest(subscriberChannels, subscriberChannelGroups, channelsJsonState);

                        RequestState<T> requestState = new RequestState<T>();
                        requestState.Channels = currentState.Channels;
                        requestState.ChannelGroups = currentState.ChannelGroups;
                        requestState.ResponseType = PNOperationType.PNHeartbeatOperation;
                        requestState.SubscribeRegularCallback = null;
                        requestState.ErrorCallback = currentState.ErrorCallback;
                        requestState.Reconnect = false;
                        requestState.Response = null;

                        string json = UrlProcessRequest<T>(request, requestState, false);
                    }
                }

            }

        }

        

    }
}
