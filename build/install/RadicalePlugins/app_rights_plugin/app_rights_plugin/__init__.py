import configparser
import re

from radicale import pathutils, rights
from radicale.log import logger
from radicale import pathutils


class Rights(rights.BaseRights):
    def __init__(self, configuration):
        super().__init__(configuration)
        self._filename = configuration.get("rights", "file")
        self.full_write_file_path = ""

    def authorization(self, user, path):
        user = user or ""
        sane_path = pathutils.strip_path(path)
        # Prevent "regex injection"
        escaped_user = re.escape(user)
        rights_config = configparser.ConfigParser()
        try:
            if not rights_config.read(self._filename):
                raise RuntimeError("No such file: %r" %
                                   self._filename)
        except Exception as e:
            raise RuntimeError("Failed to load rights file %r: %s" %
                               (self._filename, e)) from e
        if  path=="/" or path=='':
            return rights_config.get("owner-write", "permissions")

        if path.find("_write.ics") != -1:
            self.full_write_file_path = path
        elif path.find(".ics") != -1 and path.find("_write.ics") == -1:
            self.full_write_file_path = ""

        for section in rights_config.sections():
            try:
                user_pattern = rights_config.get(section, "user")
                collection_pattern = rights_config.get(section, "collection")
                # Use empty format() for harmonized handling of curly braces
                user_match = re.fullmatch(user_pattern.format(), user)
                collection_match = user_match and re.fullmatch(
                    collection_pattern.format(
                        *map(re.escape, user_match.groups()),
                        user=escaped_user), sane_path)
                file_match = True if self.full_write_file_path.find(path) != -1 and self.full_write_file_path != "" else False

            except Exception as e:
                raise RuntimeError("Error in section %r of rights file %r: "
                                   "%s" % (section, self._filename, e)) from e
            if user_match and collection_match:
                if file_match and section == "allow-readonly":
                    logger.debug("Rule %r:%r matches %r:%r from section %r Full Access",
                                user, sane_path, user_pattern,
                                collection_pattern, section)
                    self.full_write_file_path = ""
                    return rights_config.get("admin", "permissions")
                else:
                    logger.debug("Rule %r:%r matches %r:%r from section %r",
                                user, sane_path, user_pattern,
                                collection_pattern, section)
                    return rights_config.get(section, "permissions")
            logger.debug("Rule %r:%r doesn't match %r:%r from section %r",
                         user, sane_path, user_pattern, collection_pattern,
                         section)
        logger.info("Rights: %r:%r doesn't match any section", user, sane_path)
        return ""
