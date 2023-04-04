import React from "react";
import Zendesk, { ZendeskAPI } from "@docspace/common/components/Zendesk";
import { LIVE_CHAT_LOCAL_STORAGE_KEY } from "../../../constants";
import { inject, observer } from "mobx-react";

const ArticleLiveChat = ({ language, email }) => {
  // const setting = {
  //   webWidget: {
  //     //offset: { horizontal: "100px", vertical: "150px" },
  //     // mobile: {
  //     //   horizontal: "230px",
  //     //   vertical: "100px",
  //     // },
  //     chat: {
  //       connectOnPageLoad: false,
  //     },
  //   },
  //   // color: {
  //   //   theme: "#000",
  //   // },
  //   // launcher: {
  //   //   chatLabel: {
  //   //     "en-US": "Need Help",
  //   //   },
  //   // },
  //   contactForm: {
  //     fields: [{ id: "email", prefill: { "*": email } }],
  //   },
  // };

  const onZendeskLoaded = React.useCallback(() => {
    console.log("Zendesk is loaded", { email, language });

    const isShowLiveChat =
      localStorage.getItem(LIVE_CHAT_LOCAL_STORAGE_KEY) === "true" || false;

    ZendeskAPI("webWidget", isShowLiveChat ? "show" : "hide");
    ZendeskAPI("webWidget", "setLocale", language);
    ZendeskAPI("webWidget", "prefill", {
      email: {
        value: email,
        //creadOnly: true, // optional
      },
    });
  }, [language, email]);

  return (
    <Zendesk
      defer
      zendeskKey="d9e277ac-20cf-47a7-8010-57cd31a8d619" //"wb3DOpEFN82ZF0iJ15QwMiA2wZSJzpTB"
      onLoaded={onZendeskLoaded}
      //{...setting}
    />
  );
};

ArticleLiveChat.displayName = "LiveChat";

export default inject(({ auth }) => {
  const { settingsStore, languageBaseName, userStore } = auth;
  const { theme } = settingsStore;

  const { user } = userStore;
  const { email } = user;

  return {
    email,
    language: languageBaseName,
    theme,
  };
})(observer(ArticleLiveChat));
