# This file is part of Radicale Server - Calendar Server
# Copyright © 2014 Jean-Marc Martins
# Copyright © 2012-2017 Guillaume Ayoub
# Copyright © 2017-2018 Unrud <unrud@outlook.com>
#
# This library is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This library is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with Radicale.  If not, see <http://www.gnu.org/licenses/>.

import os
import pickle

from radicale import item as radicale_item
from radicale import pathutils
from radicale import logger
from datetime import datetime, date, time
import time
import base64
import hashlib
import hmac
import requests
from urllib import request
from threading import Thread
from configparser import RawConfigParser
class CollectionUploadMixin:
    def upload(self, href, item):
        if not pathutils.is_safe_filesystem_path_component(href):
            raise pathutils.UnsafePathError(href)
        try:
            self._store_item_cache(href, item)
        except Exception as e:
            raise ValueError("Failed to store item %r in collection %r: %s" %
                             (href, self.path, e)) from e
        path = pathutils.path_to_filesystem(self._filesystem_path, href)
        with self._atomic_write(path, newline="") as fd:
            fd.write(item.serialize())
        # Clean the cache after the actual item is stored, or the cache entry
        # will be removed again.
        self._clean_item_cache()
        # Track the change
        user = self.path.split("/")[0]
        domain = user.split("@")[2]
        try:
            if item.serialize().find("PRODID:-//Office//Portal//EN") == -1:
                th = Thread(target=self.set_to_portals, args=(self.path + "/" + href,domain))
                th.start()

        except:
            logger.error("Portal sending error.")

        self._update_history_etag(href, item)
        self._clean_history()
        return self._get(href, verify_href=False)
    def create_auth_token(self, pkey, machine_key):

        machine_key = bytes(machine_key, 'UTF-8')
        now = datetime.strftime(datetime.utcnow(), "%Y%m%d%H%M%S")

        message = bytes('{0}\n{1}'.format(now, pkey), 'UTF-8')

        _hmac = hmac.new(machine_key, message, hashlib.sha1)
        
        signature = str(base64.urlsafe_b64encode(_hmac.digest()), 'UTF-8')
        signature = signature.replace('-', '+')
        signature = signature.replace('_', '/')
        token = 'ASC {0}:{1}:{2}'.format(pkey, now, signature)

        logger.debug('Auth token: %r', token)
        return token

    def set_to_portals(self, path, domain):
        portal = ""
        userName = path.split('/')[0]
        portal = userName.split('@')[2]

        rewriter_url = os.environ.get("localhost" + 'HTTP_X_REWRITER_URL', '')
        portal_url = self._storage.configuration.get("storage", "portal_url")
        machine_key = self._storage.configuration.get("auth", "machine_key")
        auth_token = self.create_auth_token("radicale", machine_key)
        headers = {'Authorization': auth_token, 'HTTP_X_REWRITER_URL': rewriter_url if rewriter_url.find(domain) != -1 else ""  }
        url = portal_url+"/change_to_storage?change={}".format (path)
        resp = requests.get(url, headers=headers)

    def _upload_all_nonatomic(self, items, suffix=""):
        """Upload a new set of items.

        This takes a list of vobject items and
        uploads them nonatomic and without existence checks.

        """
        cache_folder = os.path.join(self._filesystem_path,
                                    ".Radicale.cache", "item")
        self._storage._makedirs_synced(cache_folder)
        hrefs = set()
        for item in items:
            uid = item.uid
            try:
                cache_content = self._item_cache_content(item)
            except Exception as e:
                raise ValueError(
                    "Failed to store item %r in temporary collection %r: %s" %
                    (uid, self.path, e)) from e
            href_candidate_funtions = []
            if os.name in ("nt", "posix"):
                href_candidate_funtions.append(
                    lambda: uid if uid.lower().endswith(suffix.lower())
                    else uid + suffix)
            href_candidate_funtions.extend((
                lambda: radicale_item.get_etag(uid).strip('"') + suffix,
                lambda: radicale_item.find_available_uid(hrefs.__contains__,
                                                         suffix)))
            href = f = None
            while href_candidate_funtions:
                href = href_candidate_funtions.pop(0)()
                if href in hrefs:
                    continue
                if not pathutils.is_safe_filesystem_path_component(href):
                    if not href_candidate_funtions:
                        raise pathutils.UnsafePathError(href)
                    continue
                try:
                    f = open(pathutils.path_to_filesystem(
                        self._filesystem_path, href),
                        "w", newline="", encoding=self._encoding)
                    break
                except OSError as e:
                    if href_candidate_funtions and (
                            os.name == "posix" and e.errno == 22 or
                            os.name == "nt" and e.errno == 123):
                        continue
                    raise
            with f:
                f.write(item.serialize())
                f.flush()
                self._storage._fsync(f)
            hrefs.add(href)
            with open(os.path.join(cache_folder, href), "wb") as f:
                pickle.dump(cache_content, f)
                f.flush()
                self._storage._fsync(f)
        self._storage._sync_directory(cache_folder)
        self._storage._sync_directory(self._filesystem_path)
