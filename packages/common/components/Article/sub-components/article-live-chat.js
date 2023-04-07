import React, { useEffect } from "react";
import Zendesk, { ZendeskAPI } from "@docspace/common/components/Zendesk";
import { LIVE_CHAT_LOCAL_STORAGE_KEY } from "../../../constants";
import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

const ArticleLiveChat = ({
  languageBaseName,
  email,
  displayName,
  currentColorScheme,
  withMainButton,
  isMobileArticle,
  zendeskKey,
}) => {
  const { t, ready } = useTranslation("Common");
  useEffect(() => {
    //console.log("Zendesk useEffect", { withMainButton, isMobileArticle });
    ZendeskAPI("webWidget", "updateSettings", {
      offset:
        withMainButton && isMobileArticle
          ? { horizontal: "68px", vertical: "11px" }
          : { horizontal: "4px", vertical: "11px" },
    });
  }, [withMainButton, isMobileArticle]);

  useEffect(() => {
    //console.log("Zendesk useEffect", { languageBaseName });
    ZendeskAPI("webWidget", "setLocale", languageBaseName);

    ready &&
      ZendeskAPI("webWidget", "updateSettings", {
        launcher: {
          label: {
            "*": t("Common:Support"),
          },
          chatLabel: {
            "*": t("Common:Support"),
          },
        },
      });
  }, [languageBaseName, ready]);

  useEffect(() => {
    //console.log("Zendesk useEffect", { currentColorScheme });
    ZendeskAPI("webWidget", "updateSettings", {
      color: {
        theme: currentColorScheme?.main?.accent,
      },
    });
  }, [currentColorScheme?.main?.accent]);

  useEffect(() => {
    //console.log("Zendesk useEffect", { email, displayName });
    ZendeskAPI("webWidget", "prefill", {
      email: {
        value: email,
        //readOnly: true, // optional
      },
      name: {
        value: displayName ? displayName.trim() : "",
        //readOnly: true, // optional
      },
    });
  }, [email, displayName]);

  const onZendeskLoaded = () => {
    const isShowLiveChat =
      localStorage.getItem(LIVE_CHAT_LOCAL_STORAGE_KEY) === "true" || false;

    ZendeskAPI("webWidget", isShowLiveChat ? "show" : "hide");
  };

  return zendeskKey ? (
    <Zendesk
      defer
      zendeskKey={zendeskKey}
      onLoaded={onZendeskLoaded}
      zIndex={201}
    />
  ) : (
    <></>
  );
};

ArticleLiveChat.displayName = "LiveChat";

export default inject(({ auth }) => {
  const { settingsStore, languageBaseName, userStore } = auth;
  const { theme, zendeskKey, isMobileArticle } = settingsStore;

  const { user } = userStore;
  const { email, displayName } = user;

  return {
    email,
    displayName,
    languageBaseName,
    theme,
    zendeskKey,
    isMobileArticle,
  };
})(observer(ArticleLiveChat));
