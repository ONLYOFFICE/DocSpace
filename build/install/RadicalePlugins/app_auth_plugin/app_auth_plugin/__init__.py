import base64
import hashlib
import hmac
import json
import base64
import sys
import os
import http.client

from radicale.auth import BaseAuth
from radicale.log import logger
import platform
from urllib.parse import urlparse

if platform.system() == "Linux":
    sys.path.insert(0,'/usr/lib/python3/site-packages')
import requests
from urllib import request
from urllib.parse import urlsplit
from datetime import datetime, date, time

PLUGIN_CONFIG_SCHEMA = {
    "auth": {
        "portal_url": {"value": "", "type": str},
        "machine_key": {"value": "", "type": str}
        }
    }


class Auth(BaseAuth):
    def __init__(self, configuration):
        super().__init__(configuration.copy(PLUGIN_CONFIG_SCHEMA))

    def create_auth_token(self, pkey, machine_key):

        machine_key = bytes(machine_key, 'UTF-8')
        now = datetime.strftime(datetime.utcnow(), "%Y%m%d%H%M%S")

        message = bytes('{0}\n{1}'.format(now, pkey), 'UTF-8')

        _hmac = hmac.new(machine_key, message, hashlib.sha1)
        
        signature = str(base64.urlsafe_b64encode(_hmac.digest()), 'UTF-8')
        signature = signature.replace('-', '+')
        signature = signature.replace('_', '/')
        token = 'ASC {0}:{1}:{2}'.format(pkey, now, signature)

        logger.info('Auth token: %r', token)
        return token
    
    def get_external_login(self, environ):
        self._environ = environ
        portal = ""
        if self._environ.get("PATH_INFO"):
            if len(self._environ.get("PATH_INFO").split('/')) >= 2:
                userName = self._environ.get("PATH_INFO").split('/')[1]
                if userName.find('@')!=-1:
                    portal = userName.split('@')[2]
        if self._environ.get("HTTP_X_REWRITER_URL"):
            os.environ[portal + 'HTTP_X_REWRITER_URL'] = self._environ["HTTP_X_REWRITER_URL"] # hack: common value for all modules
        else:
            urlScheme = ""
            try:
                c = http.client.HTTPSConnection(portal)
                c.request("GET", "/")
                response = c.getresponse()
                urlScheme = "https"
                os.environ[portal + 'HTTP_X_REWRITER_URL'] = self._environ["HTTP_X_REWRITER_URL"]
            except:
                urlScheme = "http"
            os.environ[portal + 'HTTP_X_REWRITER_URL'] = urlScheme + "://" + portal
        
        return()
    def login(self, login, password):
        portal_url = ""
        machine_key = self.configuration.get("auth", "machine_key")
        auth_token = self.create_auth_token("radicale", machine_key)

        portal = ""
        if self._environ.get("PATH_INFO"):
            if len(self._environ.get("PATH_INFO").split('/')) >= 2:
                userName = self._environ.get("PATH_INFO").split('/')[1]
                if userName.find('@')!=-1:
                    portal = userName.split('@')[2]

        remote_host = ""
        rewriter_url = ""
        if os.environ[portal + 'HTTP_X_REWRITER_URL']:
            rewriter_url = os.environ[portal + 'HTTP_X_REWRITER_URL']
            parsed_uri = urlparse(rewriter_url)
            if parsed_uri.netloc != '':
                remote_host = parsed_uri.netloc.replace("'", "").split(':')[0]
            elif parsed_uri.path != '':
                remote_host = parsed_uri.path.replace("'", "").split(':')[0]
            else:
                logger.error("Authenticated error. Parse REWRITER_URL")
                return ""
        else:
            logger.error("Authenticated error. not exist HTTP_X_REWRITER_URL")
            return ""

        try:
            logger.info('Remote host: %r', remote_host)
            portal_url = self.configuration.get("auth", "portal_url")
            url = portal_url+"/is_caldav_authenticated"
            payload = {'User': login+"@"+remote_host, 'Password': password}
            headers = {'Content-type': 'application/json', 'Authorization': auth_token, 'HTTP_X_REWRITER_URL': rewriter_url}
            res = requests.post(url, data=json.dumps(payload), headers=headers)

        except:
            logger.error("Authenticated error. API system")
            res = False
        
        try:
            response = res.json()
        except:
            logger.error("Authenticated error.")
            return ""

        if res.status_code != 200:
            logger.error("Error login response: %r", response)
            return ""
        if 'error' in response:
            logger.error("Error login response: %r", response)
            return ""
        else:
            if 'value' in response:
                if response['value'] != "true":
                    logger.error("Error login response: %r", response)
                    return ""
                else:
                    return login+"@"+remote_host