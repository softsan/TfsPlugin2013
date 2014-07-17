TfsPlugin2013
=============

Tfs plugin (server side event handler) in 2013

This sample illustrate how to use ISubscriber -?processEvent of TFS 2013. 

We can control TFS related action such as work item changed, build success/ failed, check-in, checkout activity using this plugin.

TFS plugin should be deployed on TFS server under following location:
ex.
c:\program files\Microsoft Team Foundation Server\Application Tier\Web services\bin\Plugins

TFS will automatically grab the plugins/component placed inside above folder and execute by itself.

One note: In my experience if you add ANYTHING to the constructor of the plugin it will cause TFS to hang.
