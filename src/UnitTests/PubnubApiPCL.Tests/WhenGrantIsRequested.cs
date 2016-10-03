﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using System.Threading;
//using PubnubApi;

//namespace PubNubMessaging.Tests
//{
//    [TestFixture]
//    public class WhenGrantIsRequested : TestHarness
//    {
//        ManualResetEvent grantManualEvent = new ManualResetEvent(false);
//        ManualResetEvent revokeManualEvent = new ManualResetEvent(false);
//        bool receivedGrantMessage = false;
//        bool receivedRevokeMessage = false;
//        int multipleChannelGrantCount = 5;
//        int multipleAuthGrantCount = 5;
//        string currentUnitTestCase = "";

//        Pubnub pubnub = null;

//        [Test]
//        public void ThenSubKeyLevelWithReadWriteShouldReturnSuccess()
//        {
//            currentUnitTestCase = "ThenSubKeyLevelWithReadWriteShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Read(true).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToSubKeyLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenSubKeyLevelWithReadWriteShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenSubKeyLevelWithReadWriteShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenSubKeyLevelWithReadShouldReturnSuccess()
//        {
//            currentUnitTestCase = "ThenSubKeyLevelWithReadShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Read(true).Write(false).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToSubKeyLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenSubKeyLevelWithReadShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenSubKeyLevelWithReadShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenSubKeyLevelWithWriteShouldReturnSuccess()
//        {
//            currentUnitTestCase = "ThenSubKeyLevelWithWriteShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Read(false).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToSubKeyLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenSubKeyLevelWithWriteShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenSubKeyLevelWithWriteShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenChannelLevelWithReadWriteShouldReturnSuccess()
//        {
//            string channel = "hello_my_channel";

//            currentUnitTestCase = "ThenChannelLevelWithReadWriteShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            //unitTest.StubRequestResponse(string.Format("http{0}://{1}/v1/auth/grant/sub-key/{2}?signature=d5Pt7JMC5yEcRKTVeVU99JiFaeWTgmH-VLpgw7vv7p4=&channel={3}&pnsdk={4}&r=1&timestamp=1356998400&ttl=5&uuid={5}&w=1", config.Secure ? "s" : "", config.Origin, PubnubCommon.SubscribeKey, channel, config.SdkVersion, config.Uuid),
//            //        "{\"message\":\"Success\",\"payload\":{\"level\":\"channel\",\"subscribe_key\":\"pam\",\"ttl\":5,\"channels\":{\"hello_my_channel\":{\"r\":1,\"w\":1,\"m\":0}}},\"service\":\"Access Manager\",\"status\":200}"
//            //    );

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Channels(new string[] { channel }).Read(true).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToChannelLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenChannelLevelWithReadWriteShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenChannelLevelWithReadWriteShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenChannelLevelWithReadShouldReturnSuccess()
//        {
//            string channel = "hello_my_channel";

//            currentUnitTestCase = "ThenChannelLevelWithReadShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);
//            //unitTest.StubRequestResponse(string.Format("http{0}://{1}/v1/auth/grant/sub-key/{2}?signature=fh2CWqssGhYP0GU4e1-_zKdw9OIZ7cm_-4nQAnefhE4=&channel={3}&pnsdk={4}&r=1&timestamp=1356998400&ttl=5&uuid={5}&w=0", config.Secure ? "s" : "", config.Origin, PubnubCommon.SubscribeKey, channel, config.SdkVersion, config.Uuid),
//            //        "{\"message\":\"Success\",\"payload\":{\"level\":\"channel\",\"subscribe_key\":\"pam\",\"ttl\":5,\"channels\":{\"hello_my_channel\":{\"r\":1,\"w\":0,\"m\":0}}},\"service\":\"Access Manager\",\"status\":200}"
//            //    );

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Channels(new string[] { channel }).Read(true).Write(false).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToChannelLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenChannelLevelWithReadShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenChannelLevelWithReadShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenChannelLevelWithWriteShouldReturnSuccess()
//        {
//            string channel = "hello_my_channel";

//            currentUnitTestCase = "ThenChannelLevelWithWriteShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            //unitTest.StubRequestResponse(string.Format("http{0}://{1}/v1/auth/grant/sub-key/{2}?signature=_51ym4mwmZxp4crPwWjF-MFQwSybNFJFsmswrdahVDs=&channel={3}&pnsdk={4}&r=0&timestamp=1356998400&ttl=5&uuid={5}&w=1", config.Secure ? "s" : "", config.Origin, PubnubCommon.SubscribeKey, channel, config.SdkVersion, config.Uuid),
//            //        "{\"message\":\"Success\",\"payload\":{\"level\":\"channel\",\"subscribe_key\":\"pam\",\"ttl\":5,\"channels\":{\"hello_my_channel\":{\"r\":0,\"w\":1,\"m\":0}}},\"service\":\"Access Manager\",\"status\":200}"
//            //    );

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Channels(new string[] { channel }).Read(false).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToChannelLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenChannelLevelWithWriteShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenChannelLevelWithWriteShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenUserLevelWithReadWriteShouldReturnSuccess()
//        {
//            currentUnitTestCase = "ThenUserLevelWithReadWriteShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            string channel = "hello_my_authchannel";
//            string authKey = "hello_my_authkey";
//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Channels(new string[] { channel }).AuthKeys(new string[] { authKey }).Read(true).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToUserLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenUserLevelWithReadWriteShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenUserLevelWithReadWriteShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenUserLevelWithReadShouldReturnSuccess()
//        {
//            currentUnitTestCase = "ThenUserLevelWithReadShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            string channel = "hello_my_authchannel";
//            string authKey = "hello_my_authkey";
//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Channels(new string[] { channel }).AuthKeys(new string[] { authKey }).Read(true).Write(false).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToUserLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenUserLevelWithReadShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenUserLevelWithReadShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenUserLevelWithWriteShouldReturnSuccess()
//        {
//            currentUnitTestCase = "ThenUserLevelWithWriteShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            string channel = "hello_my_authchannel";
//            string authKey = "hello_my_authkey";
//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Channels(new string[] { channel }).AuthKeys(new string[] { authKey }).Read(false).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToUserLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenUserLevelWithWriteShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenUserLevelWithWriteShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenMultipleChannelGrantShouldReturnSuccess()
//        {
//            currentUnitTestCase = "ThenMultipleChannelGrantShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            StringBuilder channelBuilder = new StringBuilder();

//            for (int index = 0; index < multipleChannelGrantCount; index++)
//            {
//                if (index == multipleChannelGrantCount - 1)
//                {
//                    channelBuilder.AppendFormat("csharp-hello_my_channel-{0}", index);
//                }
//                else
//                {
//                    channelBuilder.AppendFormat("csharp-hello_my_channel-{0},", index);
//                }
//            }

//            string channel = "";

//            if (!PubnubCommon.EnableStubTest)
//            {
//                channel = channelBuilder.ToString();
//            }
//            else
//            {
//                multipleChannelGrantCount = 5;
//                channel = "csharp-hello_my_channel-0,csharp-hello_my_channel-1,csharp-hello_my_channel-2,csharp-hello_my_channel-3,csharp-hello_my_channel-4";
//            }

//            //unitTest.StubRequestResponse(@"http://pubsub.pubnub.com/v1/auth/grant/sub-key/demo-36?signature=tDimpjL8e8-G9WTJj9K4UDzeTAgU3QQHE0mSNWn34Kk=&channel=csharp-hello_my_channel-0%2Ccsharp-hello_my_channel-1%2Ccsharp-hello_my_channel-2%2Ccsharp-hello_my_channel-3%2Ccsharp-hello_my_channel-4&pnsdk=PubNub CSharp 4.0&r=1&timestamp=1356998400&ttl=5&uuid=mytestuuid&w=1",
//            //    "{\"message\":\"Success\",\"payload\":{\"level\":\"channel\",\"subscribe_key\":\"pam\",\"ttl\":5,\"channels\":{\"csharp-hello_my_channel-0\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-hello_my_channel-1\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-hello_my_channel-2\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-hello_my_channel-3\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-hello_my_channel-4\":{\"r\":1,\"w\":1,\"m\":0}}},\"service\":\"Access Manager\",\"status\":200}"
//            //);
            
//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Channels(new string[] { channel }).Read(true).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToMultiChannelGrantCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenMultipleChannelGrantShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenMultipleChannelGrantShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenMultipleAuthGrantShouldReturnSuccess()
//        {
//            currentUnitTestCase = "ThenMultipleAuthGrantShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            StringBuilder authKeyBuilder = new StringBuilder();
//            for (int index = 0; index < multipleAuthGrantCount; index++)
//            {
//                if (index == multipleAuthGrantCount - 1)
//                {
//                    authKeyBuilder.AppendFormat("csharp-auth_key-{0}", index);
//                }
//                else
//                {
//                    authKeyBuilder.AppendFormat("csharp-auth_key-{0},", index);
//                }
//            }
//            string auth = "";

//            if (!PubnubCommon.EnableStubTest)
//            {
//                auth = authKeyBuilder.ToString();
//            }
//            else
//            {
//                multipleAuthGrantCount = 5;
//                auth = "csharp-auth_key-0,csharp-auth_key-1,csharp-auth_key-2,csharp-auth_key-3,csharp-auth_key-4";
//            }

//            //unitTest.StubRequestResponse("http://pubsub.pubnub.com/v1/auth/grant/sub-key/demo-36?signature=RHYOczfs7yJBKoowJLytR6zLGAIU4aAaGYHN-i5mO9E=&auth=csharp-auth_key-0%2Ccsharp-auth_key-1%2Ccsharp-auth_key-2%2Ccsharp-auth_key-3%2Ccsharp-auth_key-4&channel=csharp-auth_key-0%2Ccsharp-auth_key-1%2Ccsharp-auth_key-2%2Ccsharp-auth_key-3%2Ccsharp-auth_key-4&pnsdk=PubNub CSharp 4.0&r=1&timestamp=1356998400&ttl=5&uuid=mytestuuid&w=1",
//            //    "{\"message\":\"Success\",\"payload\":{\"level\":\"user\",\"subscribe_key\":\"pam\",\"ttl\":5,\"channels\":{\"csharp-auth_key-0\":{\"auths\":{\"csharp-auth_key-0\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-1\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-2\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-3\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-4\":{\"r\":1,\"w\":1,\"m\":0}}},\"csharp-auth_key-1\":{\"auths\":{\"csharp-auth_key-0\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-1\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-2\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-3\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-4\":{\"r\":1,\"w\":1,\"m\":0}}},\"csharp-auth_key-2\":{\"auths\":{\"csharp-auth_key-0\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-1\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-2\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-3\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-4\":{\"r\":1,\"w\":1,\"m\":0}}},\"csharp-auth_key-3\":{\"auths\":{\"csharp-auth_key-0\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-1\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-2\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-3\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-4\":{\"r\":1,\"w\":1,\"m\":0}}},\"csharp-auth_key-4\":{\"auths\":{\"csharp-auth_key-0\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-1\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-2\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-3\":{\"r\":1,\"w\":1,\"m\":0},\"csharp-auth_key-4\":{\"r\":1,\"w\":1,\"m\":0}}}}},\"service\":\"Access Manager\",\"status\":200}"
//            //);

//            string[] authArray = auth.Split(',');

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().Channels(authArray).AuthKeys(authArray).Read(true).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToMultiAuthGrantCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests();
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenMultipleAuthGrantShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenMultipleAuthGrantShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenRevokeAtSubKeyLevelReturnSuccess()
//        {
//            currentUnitTestCase = "ThenRevokeAtSubKeyLevelReturnSuccess";

//            receivedGrantMessage = false;
//            receivedRevokeMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            if (PubnubCommon.PAMEnabled)
//            {
//                if (!PubnubCommon.EnableStubTest)
//                {
//                    grantManualEvent = new ManualResetEvent(false);
//                    pubnub.Grant().Read(true).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToSubKeyLevelCallback, Error = DummyErrorCallback });
//                    Thread.Sleep(1000);
//                    grantManualEvent.WaitOne();
//                }
//                else
//                {
//                    receivedGrantMessage = true;
//                }
//                if (receivedGrantMessage)
//                {
//                    revokeManualEvent = new ManualResetEvent(false);
//                    Console.WriteLine("WhenGrantIsRequested -> ThenRevokeAtSubKeyLevelReturnSuccess -> Grant ok..Now trying Revoke");
//                    pubnub.Grant().Read(false).Write(false).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = RevokeToSubKeyLevelCallback, Error = DummyErrorCallback });
//                    Thread.Sleep(1000);
//                    revokeManualEvent.WaitOne();
                    
//                    pubnub.EndPendingRequests(); 
//                    pubnub.PubnubUnitTest = null;
//                    pubnub = null;
//                    Assert.IsTrue(receivedRevokeMessage, "WhenGrantIsRequested -> ThenRevokeAtSubKeyLevelReturnSuccess -> Grant success but revoke failed.");
//                }
//                else
//                {
//                    Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenRevokeAtSubKeyLevelReturnSuccess failed. -> Grant not occured, so is revoke");
//                }
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenRevokeAtSubKeyLevelReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenRevokeAtChannelLevelReturnSuccess()
//        {
//            string channel = "hello_my_channel";

//            currentUnitTestCase = "ThenRevokeAtChannelLevelReturnSuccess";

//            receivedGrantMessage = false;
//            receivedRevokeMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            if (PubnubCommon.PAMEnabled)
//            {
//                if (!PubnubCommon.EnableStubTest)
//                {
//                    grantManualEvent = new ManualResetEvent(false);
//                    pubnub.Grant().Channels(new string[] { channel }).Read(true).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToChannelLevelCallback, Error = DummyErrorCallback });
//                    Thread.Sleep(1000);
//                    grantManualEvent.WaitOne(310*1000);
//                }
//                else
//                {
//                    receivedGrantMessage = true;
//                }

//                if (receivedGrantMessage)
//                {
//                    //unitTest.StubRequestResponse(string.Format("http{0}://{1}/v1/auth/grant/sub-key/{2}?signature=7kycJQxN4mABxs9wR-XhRPkyVJ2lNsRXIqHgIt2vugA=&channel={3}&pnsdk={4}&r=0&timestamp=1356998400&uuid={5}&w=0", config.Secure ? "s" : "", config.Origin, PubnubCommon.SubscribeKey, channel, config.SdkVersion, config.Uuid),
//                    //    "{\"message\":\"Success\",\"payload\":{\"level\":\"channel\",\"subscribe_key\":\"pam\",\"ttl\":1,\"channels\":{\"hello_my_channel\":{\"r\":0,\"w\":0,\"m\":0}}},\"service\":\"Access Manager\",\"status\":200}"
//                    //);

//                    revokeManualEvent = new ManualResetEvent(false);
//                    Console.WriteLine("WhenGrantIsRequested -> ThenRevokeAtChannelLevelReturnSuccess -> Grant ok..Now trying Revoke");
//                    pubnub.Grant().Channels(new string[] { channel }).Read(false).Write(false).Manage(false).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = RevokeToChannelLevelCallback, Error = DummyErrorCallback });
//                    Thread.Sleep(1000);
//                    revokeManualEvent.WaitOne();

//                    pubnub.EndPendingRequests(); 
//                    pubnub.PubnubUnitTest = null;
//                    pubnub = null;
//                    Assert.IsTrue(receivedRevokeMessage, "WhenGrantIsRequested -> ThenRevokeAtChannelLevelReturnSuccess -> Grant success but revoke failed.");
//                }
//                else
//                {
//                    Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenRevokeAtChannelLevelReturnSuccess failed. -> Grant not occured, so is revoke");
//                }
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenRevokeAtChannelLevelReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenRevokeAtUserLevelReturnSuccess()
//        {
//            currentUnitTestCase = "ThenRevokeAtUserLevelReturnSuccess";

//            receivedGrantMessage = false;
//            receivedRevokeMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            string channel = "hello_my_authchannel";
//            string authKey = "hello_my_authkey";
//            if (PubnubCommon.PAMEnabled)
//            {
//                if (!PubnubCommon.EnableStubTest)
//                {
//                    grantManualEvent = new ManualResetEvent(false);
//                    pubnub.Grant().Channels(new string[] { channel }).AuthKeys(new string[] { authKey }).Read(true).Write(true).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToUserLevelCallback, Error = DummyErrorCallback });
//                    Thread.Sleep(1000);
//                    grantManualEvent.WaitOne();
//                }
//                else
//                {
//                    receivedGrantMessage = true;
//                }
//                if (receivedGrantMessage)
//                {
//                    revokeManualEvent = new ManualResetEvent(false);
//                    Console.WriteLine("WhenGrantIsRequested -> ThenRevokeAtUserLevelReturnSuccess -> Grant ok..Now trying Revoke");
//                    pubnub.Grant().Channels(new string[] { channel }).AuthKeys(new string[] { authKey }).Read(false).Write(false).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = RevokeToUserLevelCallback, Error = DummyErrorCallback });
//                    Thread.Sleep(1000);
//                    revokeManualEvent.WaitOne();

//                    pubnub.EndPendingRequests(); 
//                    pubnub.PubnubUnitTest = null;
//                    pubnub = null;
//                    Assert.IsTrue(receivedRevokeMessage, "WhenGrantIsRequested -> ThenRevokeAtUserLevelReturnSuccess -> Grant success but revoke failed.");
//                }
//                else
//                {
//                    Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenRevokeAtUserLevelReturnSuccess failed. -> Grant not occured, so is revoke");
//                }
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenRevokeAtUserLevelReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenChannelGroupLevelWithReadManageShouldReturnSuccess()
//        {
//            string channelgroup = "hello_my_group";

//            currentUnitTestCase = "ThenChannelGroupLevelWithReadManageShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            //unitTest.StubRequestResponse(string.Format("http{0}://{1}/v1/auth/grant/sub-key/{2}?signature=rf_RVAZE2vf4kHhmRFuZw0Di1Z0bEXNwjjXg8AoFXqo=&channel-group={3}&m=1&pnsdk={4}&r=1&timestamp=1356998400&ttl=5&uuid={5}", config.Secure ? "s" : "", config.Origin, PubnubCommon.SubscribeKey, channelgroup, config.SdkVersion, config.Uuid),
//            //        "{\"message\":\"Success\",\"payload\":{\"level\":\"channel-group\",\"subscribe_key\":\"pam\",\"ttl\":5,\"channel-groups\":{\"hello_my_group\":{\"r\":1,\"w\":0,\"m\":1}}},\"service\":\"Access Manager\",\"status\":200}"
//            //    );

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().ChannelGroups(new string[] { channelgroup }).Read(true).Write(true).Manage(true).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToChannelLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenChannelGroupLevelWithReadManageShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenChannelGroupLevelWithReadManageShouldReturnSuccess.");
//            }
//        }

//        [Test]
//        public void ThenChannelGroupLevelWithReadShouldReturnSuccess()
//        {
//            string channelgroup = "hello_my_group";

//            currentUnitTestCase = "ThenChannelGroupLevelWithReadShouldReturnSuccess";

//            receivedGrantMessage = false;

//            PNConfiguration config = new PNConfiguration()
//            {
//                PublishKey = PubnubCommon.PublishKey,
//                SubscribeKey = PubnubCommon.SubscribeKey,
//                SecretKey = PubnubCommon.SecretKey,
//                Uuid = "mytestuuid",
//            };

//            pubnub = this.createPubNubInstance(config);

//            //unitTest.StubRequestResponse(string.Format("http{0}://{1}/v1/auth/grant/sub-key/{2}?signature=lXzuy743zRu4LESs5xt1jZ-myQtp1JriUecgP2n_49g=&channel-group={3}&m=0&pnsdk={4}&r=1&timestamp=1356998400&ttl=5&uuid={5}", config.Secure ? "s" : "", config.Origin, PubnubCommon.SubscribeKey, channelgroup, config.SdkVersion, config.Uuid),
//            //        "{\"message\":\"Success\",\"payload\":{\"level\":\"channel-group\",\"subscribe_key\":\"pam\",\"ttl\":5,\"channel-groups\":{\"hello_my_group\":{\"r\":1,\"w\":0,\"m\":0}}},\"service\":\"Access Manager\",\"status\":200}"
//            //    );

//            if (PubnubCommon.PAMEnabled)
//            {
//                grantManualEvent = new ManualResetEvent(false);
//                pubnub.Grant().ChannelGroups(new string[] { channelgroup }).Read(true).Write(false).Manage(false).TTL(5).Async(new PNCallback<PNAccessManagerGrantResult>() { Result = AccessToChannelLevelCallback, Error = DummyErrorCallback });
//                Thread.Sleep(1000);

//                grantManualEvent.WaitOne();

//                pubnub.EndPendingRequests(); 
//                pubnub.PubnubUnitTest = null;
//                pubnub = null;
//                Assert.IsTrue(receivedGrantMessage, "WhenGrantIsRequested -> ThenChannelGroupLevelWithReadShouldReturnSuccess failed.");
//            }
//            else
//            {
//                Assert.Ignore("PAM Not Enabled for WhenGrantIsRequested -> ThenChannelGroupLevelWithReadShouldReturnSuccess.");
//            }
//        }


//        void AccessToSubKeyLevelCallback(PNAccessManagerGrantResult receivedMessage)
//        {
//            try
//            {
//                if (receivedMessage != null)
//                {
//                    int statusCode = receivedMessage.StatusCode;
//                    string statusMessage = receivedMessage.StatusMessage;
//                    if (statusCode == 200 && statusMessage.ToLower() == "success")
//                    {
//                        if (receivedMessage.Payload != null && receivedMessage.Payload.Access != null)
//                        {
//                            bool read = receivedMessage.Payload.Access.Read;
//                            bool write = receivedMessage.Payload.Access.Write;
//                            string level = receivedMessage.Payload.Level;
//                            if (level == "subkey")
//                            {
//                                switch (currentUnitTestCase)
//                                {
//                                    case "ThenSubKeyLevelWithReadWriteShouldReturnSuccess":
//                                    case "ThenRevokeAtSubKeyLevelReturnSuccess":
//                                        if (read && write) receivedGrantMessage = true;
//                                        break;
//                                    case "ThenSubKeyLevelWithReadShouldReturnSuccess":
//                                        if (read && !write) receivedGrantMessage = true;
//                                        break;
//                                    case "ThenSubKeyLevelWithWriteShouldReturnSuccess":
//                                        if (!read && write) receivedGrantMessage = true;
//                                        break;
//                                    default:
//                                        break;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            catch { }
//            finally
//            {
//                grantManualEvent.Set();
//            }
//        }

//        void AccessToChannelLevelCallback(PNAccessManagerGrantResult receivedMessage)
//        {
//            try
//            {
//                if (receivedMessage != null)
//                {
//                    int statusCode = receivedMessage.StatusCode;
//                    string statusMessage = receivedMessage.StatusMessage;
//                    if (statusCode == 200 && statusMessage.ToLower() == "success")
//                    {
//                        if (receivedMessage.Payload != null)
//                        {
//                            string level = receivedMessage.Payload.Level;
//                            if (level == "channel")
//                            {
//                                Dictionary<string, PNAccessManagerGrantResult.Data.ChannelData> channels = receivedMessage.Payload.Channels;
//                                if (channels != null && channels.Count > 0)
//                                {
//                                    string currentChannel = channels.Keys.ToList()[0];
//                                    if (channels[currentChannel].Access != null)
//                                    {
//                                        bool read = channels[currentChannel].Access.Read;
//                                        bool write = channels[currentChannel].Access.Write;
//                                        switch (currentUnitTestCase)
//                                        {
//                                            case "ThenChannelLevelWithReadWriteShouldReturnSuccess":
//                                            case "ThenRevokeAtChannelLevelReturnSuccess":
//                                                if (read && write) receivedGrantMessage = true;
//                                                break;
//                                            case "ThenChannelLevelWithReadShouldReturnSuccess":
//                                                if (read && !write) receivedGrantMessage = true;
//                                                break;
//                                            case "ThenChannelLevelWithWriteShouldReturnSuccess":
//                                                if (!read && write) receivedGrantMessage = true;
//                                                break;
//                                            default:
//                                                break;
//                                        }
//                                    }
//                                }
//                            }
//                            else if (level == "channel-group")
//                            {
//                                Dictionary<string, PNAccessManagerGrantResult.Data.ChannelGroupData> channelgroups = receivedMessage.Payload.Channelgroups;
//                                if (channelgroups != null && channelgroups.Count > 0)
//                                {
//                                    string currentChannelGroup = channelgroups.Keys.ToList()[0];
//                                    if (channelgroups[currentChannelGroup].Access != null)
//                                    {
//                                        bool read = channelgroups[currentChannelGroup].Access.Read;
//                                        bool manage = channelgroups[currentChannelGroup].Access.Manage;
//                                        switch (currentUnitTestCase)
//                                        {
//                                            case "ThenChannelGroupLevelWithReadManageShouldReturnSuccess":
//                                                if (read && manage) receivedGrantMessage = true;
//                                                break;
//                                            case "ThenChannelGroupLevelWithReadShouldReturnSuccess":
//                                                if (read && !manage) receivedGrantMessage = true;
//                                                break;
//                                            default:
//                                                break;
//                                        }
//                                    }
//                                }
//                            } //end of if
//                        }
//                    }
//                }
//            }
//            catch { }
//            finally
//            {
//                grantManualEvent.Set();
//            }
//        }

//        void AccessToUserLevelCallback(PNAccessManagerGrantResult receivedMessage)
//        {
//            try
//            {
//                if (receivedMessage != null)
//                {
//                    int statusCode = receivedMessage.StatusCode;
//                    string statusMessage = receivedMessage.StatusMessage;
//                    if (statusCode == 200 && statusMessage.ToLower() == "success")
//                    {
//                        if (receivedMessage.Payload != null)
//                        {
//                            string level = receivedMessage.Payload.Level;
//                            Dictionary<string, PNAccessManagerGrantResult.Data.ChannelData> channels = receivedMessage.Payload.Channels;
//                            foreach (string channel in channels.Keys)
//                            {
//                                PNAccessManagerGrantResult.Data.ChannelData channelData = channels[channel];
//                                if (channelData.Auths != null)
//                                {
//                                    Dictionary<string, PNAccessManagerGrantResult.Data.ChannelData.AuthData> authDataDic = channelData.Auths;
//                                    if (authDataDic != null)
//                                    {
//                                        foreach (string key in authDataDic.Keys)
//                                        {
//                                            PNAccessManagerGrantResult.Data.ChannelData.AuthData authData = authDataDic[key];
//                                            if (authData != null && authData.Access != null)
//                                            {
//                                                bool read = authData.Access.Read;
//                                                bool write = authData.Access.Write;
//                                                if (level == "user")
//                                                {
//                                                    switch (currentUnitTestCase)
//                                                    {
//                                                        case "ThenUserLevelWithReadWriteShouldReturnSuccess":
//                                                        case "ThenRevokeAtUserLevelReturnSuccess":
//                                                            if (read && write) receivedGrantMessage = true;
//                                                            break;
//                                                        case "ThenUserLevelWithReadShouldReturnSuccess":
//                                                            if (read && !write) receivedGrantMessage = true;
//                                                            break;
//                                                        case "ThenUserLevelWithWriteShouldReturnSuccess":
//                                                            if (!read && write) receivedGrantMessage = true;
//                                                            break;
//                                                        default:
//                                                            break;
//                                                    }
//                                                }
//                                            }
//                                        }
//                                    }
//                                }
//                            }

//                        }
//                    }
//                }
//            }
//            catch { }
//            finally
//            {
//                grantManualEvent.Set();
//            }
//        }

//        void AccessToMultiChannelGrantCallback(PNAccessManagerGrantResult receivedMessage)
//        {
//            try
//            {
//                if (receivedMessage != null)
//                {
//                    int statusCode = receivedMessage.StatusCode;
//                    string statusMessage = receivedMessage.StatusMessage;
//                    if (statusCode == 200 && statusMessage.ToLower() == "success")
//                    {
//                        if (receivedMessage.Payload != null)
//                        {
//                            string level = receivedMessage.Payload.Level;
//                            Dictionary<string, PNAccessManagerGrantResult.Data.ChannelData> channels = receivedMessage.Payload.Channels;
//                            if (channels != null && channels.Count >= 0)
//                            {
//                                Console.WriteLine("{0} - AccessToMultiChannelGrantCallback - Grant MultiChannel Count (Received/Sent) = {1}/{2}", currentUnitTestCase, channels.Count, multipleChannelGrantCount);
//                                if (channels.Count == multipleChannelGrantCount)
//                                {
//                                    receivedGrantMessage = true;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            catch { }
//            finally
//            {
//                grantManualEvent.Set();
//            }
//        }

//        void AccessToMultiAuthGrantCallback(PNAccessManagerGrantResult receivedMessage)
//        {
//            try
//            {
//                if (receivedMessage != null)
//                {
//                    int statusCode = receivedMessage.StatusCode;
//                    string statusMessage = receivedMessage.StatusMessage;
//                    if (statusCode == 200 && statusMessage.ToLower() == "success")
//                    {
//                        if (receivedMessage.Payload != null)
//                        {
//                            string level = receivedMessage.Payload.Level;
//                            Dictionary<string, PNAccessManagerGrantResult.Data.ChannelData> channelsData = receivedMessage.Payload.Channels;
//                            if (channelsData != null && channelsData.Count > 0)
//                            {
//                                List<string> channels = channelsData.Keys.ToList();
//                                string channel = channels[0];
//                                //string channel = 
//                                PNAccessManagerGrantResult.Data.ChannelData channelData = channelsData[channel];
//                                if (channelData != null && channelData.Auths != null)
//                                {
//                                    Console.WriteLine("{0} - AccessToMultiAuthGrantCallback - Grant Auth Count (Received/Sent) = {1}/{2}", currentUnitTestCase, channelData.Auths.Count, multipleAuthGrantCount);
//                                    if (channelData.Auths.Count == multipleAuthGrantCount)
//                                    {
//                                        receivedGrantMessage = true;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            catch { }
//            finally
//            {
//                grantManualEvent.Set();
//            }
//        }

//        void RevokeToSubKeyLevelCallback(PNAccessManagerGrantResult receivedMessage)
//        {
//            try
//            {
//                int statusCode = receivedMessage.StatusCode;
//                string statusMessage = receivedMessage.StatusMessage;
//                if (statusCode == 200 && statusMessage.ToLower() == "success")
//                {
//                    if (receivedMessage.Payload != null && receivedMessage.Payload.Access != null)
//                    {
//                        bool read = receivedMessage.Payload.Access.Read;
//                        bool write = receivedMessage.Payload.Access.Write;
//                        string level = receivedMessage.Payload.Level;
//                        if (level == "subkey")
//                        {
//                            switch (currentUnitTestCase)
//                            {
//                                case "ThenRevokeAtSubKeyLevelReturnSuccess":
//                                    if (!read && !write) receivedRevokeMessage = true;
//                                    break;
//                                case "ThenSubKeyLevelWithReadShouldReturnSuccess":
//                                    //if (read && !write) receivedGrantMessage = true;
//                                    break;
//                                case "ThenSubKeyLevelWithWriteShouldReturnSuccess":
//                                    //if (!read && write) receivedGrantMessage = true;
//                                    break;
//                                default:
//                                    break;
//                            }
//                        }
//                    }
//                }
//            }
//            catch { }
//            finally
//            {
//                revokeManualEvent.Set();
//            }
//        }

//        void RevokeToChannelLevelCallback(PNAccessManagerGrantResult receivedMessage)
//        {
//            try
//            {
//                if (receivedMessage != null)
//                {
//                    int statusCode = receivedMessage.StatusCode;
//                    string statusMessage = receivedMessage.StatusMessage;
//                    if (statusCode == 200 && statusMessage.ToLower() == "success")
//                    {
//                        if (receivedMessage.Payload != null)
//                        {
//                            string level = receivedMessage.Payload.Level;
//                            Dictionary<string, PNAccessManagerGrantResult.Data.ChannelData> channels = receivedMessage.Payload.Channels;
//                            if (channels != null && channels.Count > 0)
//                            {
//                                receivedRevokeMessage = true;
//                                foreach (string ch in channels.Keys)
//                                {
//                                    if (channels.ContainsKey(ch))
//                                    {
//                                        Dictionary<string, object> channelContainer = pubnub.JsonPluggableLibrary.ConvertToDictionaryObject(channels[ch]);
//                                        if (channels[ch].Access != null)
//                                        {
//                                            bool read = channels[ch].Access.Read;
//                                            bool write = channels[ch].Access.Write;
//                                            if (!read && !write)
//                                            {
//                                                receivedRevokeMessage = true;
//                                            }
//                                            break;
//                                        }
//                                    }
//                                }
//                            }
//                            else
//                            {
//                                receivedRevokeMessage = true;
//                            }
//                        }
//                    }
//                }
//            }
//            catch { }
//            finally
//            {
//                revokeManualEvent.Set();
//            }
//        }

//        void RevokeToUserLevelCallback(PNAccessManagerGrantResult receivedMessage)
//        {
//            try
//            {
//                if (receivedMessage != null)
//                {
//                    int statusCode = receivedMessage.StatusCode;
//                    string statusMessage = receivedMessage.StatusMessage;
//                    if (statusCode == 200 && statusMessage.ToLower() == "success")
//                    {
//                        if (receivedMessage.Payload != null)
//                        {
//                            string level = receivedMessage.Payload.Level;
//                            Dictionary<string, PNAccessManagerGrantResult.Data.ChannelData> channelsDataDic = receivedMessage.Payload.Channels;
//                            if (channelsDataDic != null && channelsDataDic.Count > 0)
//                            {
//                                List<string> channelKeyList = channelsDataDic.Keys.ToList();
//                                string channel = channelKeyList[0];

//                                PNAccessManagerGrantResult.Data.ChannelData channelData = channelsDataDic[channel];
//                                if (channelData != null)
//                                {
//                                    Dictionary<string, PNAccessManagerGrantResult.Data.ChannelData.AuthData> authDataDic = channelData.Auths;
//                                    if (authDataDic != null && authDataDic.Count > 0)
//                                    {
//                                        receivedRevokeMessage = true;
//                                        foreach (string key in authDataDic.Keys)
//                                        {
//                                            PNAccessManagerGrantResult.Data.ChannelData.AuthData authData = authDataDic[key];
//                                            if (authData != null && authData.Access != null)
//                                            {
//                                                receivedRevokeMessage = true;
//                                                break;
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        receivedRevokeMessage = false;
//                                    }
//                                }
//                            }
//                        } //end of if payload
//                    }
//                }
//            }
//            catch { }
//            finally
//            {
//                revokeManualEvent.Set();
//            }
//        }

//        private void DummyErrorCallback(PubnubClientError result)
//        {
//            if (currentUnitTestCase == "ThenRevokeAtChannelLevelReturnSuccess")
//            {
//                grantManualEvent.Set();
//            }
//        }

//    }
//}
