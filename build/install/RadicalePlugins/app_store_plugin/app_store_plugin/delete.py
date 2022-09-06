import os
from tempfile import TemporaryDirectory

from radicale import pathutils, storage
import requests
from urllib import request

class CollectionDeleteMixin:
    def delete(self, href=None):
        if href != None:
            user = self.path.split("/")[0]
            domain = user.split("@")[2]
            self.delete_event_portals(self.path+"/"+href, domain)
        if href is None:
            # Delete the collection
            parent_dir = os.path.dirname(self._filesystem_path)
            try:
                os.rmdir(self._filesystem_path)
            except OSError:
                with TemporaryDirectory(
                        prefix=".Radicale.tmp-", dir=parent_dir) as tmp:
                    os.rename(self._filesystem_path, os.path.join(
                        tmp, os.path.basename(self._filesystem_path)))
                    self._storage._sync_directory(parent_dir)
            else:
                self._storage._sync_directory(parent_dir)
        else:
            # Delete an item
            if not pathutils.is_safe_filesystem_path_component(href):
                raise pathutils.UnsafePathError(href)
            path = pathutils.path_to_filesystem(self._filesystem_path, href)
            if not os.path.isfile(path):
                raise storage.ComponentNotFoundError(href)
            os.remove(path)
            self._storage._sync_directory(os.path.dirname(path))
            # Track the change
            self._update_history_etag(href, None)
            self._clean_history()
    
    def delete_event_portals(self, path, domain):
        portal = ""
        userName = path.split('/')[0]
        portal = userName.split('@')[2]

        rewriter_url = os.environ.get(portal + 'HTTP_X_REWRITER_URL', '')
        portal_url = self._storage.configuration.get("storage", "portal_url")
        machine_key = self._storage.configuration.get("auth", "machine_key")
        auth_token = self.create_auth_token("radicale", machine_key)

        headers = {'Authorization': auth_token, 'HTTP_X_REWRITER_URL': rewriter_url if rewriter_url.find(domain) != -1 else ""}
        url = portal_url+"/caldav_delete_event?eventInfo={}".format (path)
        resp = requests.get(url, headers=headers)