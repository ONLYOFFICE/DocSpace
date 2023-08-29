/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


"use strict";

const config = require("../../config").get(),
    logger = require("../log.js");
    URL = require("url");

// ReSharper disable once InconsistentNaming
module.exports = function () {

    function getBaseUrl(req) {
        const proto = req.headers['x-forwarded-proto']?.split(',').shift();
        const host = req.headers['x-forwarded-host']?.split(',').shift();

        return `${proto}://${host}`;
    }

    function getPortalSsoHandlerUrl(req) {
        const url = getBaseUrl(req) + config.app.portal.ssoUrl;
        return url;
    }

    function getPortalSsoConfigUrl(req) {
        const url = getPortalSsoHandlerUrl(req) +
            "?config=saml";
        logger.debug("getPortalSsoConfigUrl: " + url);
        return url;
    }

    function getPortalSsoLoginUrl(req, data) {
        const url = getPortalSsoHandlerUrl(req) + "?auth=true&data=" + data;
        logger.debug("getPortalSsoLoginUrl: " + url);
        return url;
    }

    function getPortalSsoLogoutUrl(req, data) {
        const url = getPortalSsoHandlerUrl(req) + "?logout=true&data=" + data;
        logger.debug("getPortalSsoLogoutUrl: " + url);
        return url;
    }

    function getPortalAuthUrl(req) {
        const url = getBaseUrl(req) + config.app.portal.authUrl;
        logger.debug("getPortalAuthUrl: " + url);
        return url;
    }

    const ErrorMessageKey = {
        Error: 1,
        SsoError: 17,
        SsoAuthFailed: 18,
        SsoAttributesNotFound: 19,
    };

    function getPortalAuthErrorUrl(req, errorKey) {
        const url = getBaseUrl(req) + "/login/error?messageKey=" + errorKey;
        logger.debug("getPortalAuthErrorUrl: " + url);
        return url;
    }

    function getPortalErrorUrl(req) {
        const url = getBaseUrl(req) + "/login/error?messageKey=" + ErrorMessageKey.Error;
        logger.debug("getPortal500Url: " + url);
        return url;
    }

    function getPortal404Url(req) {
        const url = getBaseUrl(req) + "/login/error?messageKey=" + ErrorMessageKey.SsoError;
        logger.debug("getPortal404Url: " + url);
        return url;
    }

    return {
        getBaseUrl: getBaseUrl,

        getPortalSsoHandlerUrl: getPortalSsoHandlerUrl,

        getPortalSsoConfigUrl: getPortalSsoConfigUrl,

        getPortalSsoLoginUrl: getPortalSsoLoginUrl,

        getPortalSsoLogoutUrl: getPortalSsoLogoutUrl,

        getPortalAuthUrl: getPortalAuthUrl,

        ErrorMessageKey: ErrorMessageKey,

        getPortalAuthErrorUrl: getPortalAuthErrorUrl,

        getPortal500Url: getPortalErrorUrl,

        getPortal404Url: getPortal404Url
    };
};
