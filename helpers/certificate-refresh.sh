#!/bin/bash

CERT=/etc/letsencrypt/live/voip.sufficit.com.br/certificate.pfx
DEST=/opt/sufficit-telephony-blazorpanel/certificate.pfx

yes | cp -rf ${CERT} ${DEST} 2>/dev/null
chown dotnetuser ${DEST} 2>/dev/null

