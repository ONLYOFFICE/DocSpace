import { inject, observer } from "mobx-react";
import React from "react";
import Text from "@docspace/components/text";
import HelpButton from "@docspace/components/help-button";
import copy from "copy-to-clipboard";
import toastr from "@docspace/components/toast/toastr";
const TelegramConnectionContainer = ({
  t,
  isTelegramConnected,
  secretLink,
}) => {
  const onCopyLink = () => {
    toastr.success(t("Translations:LinkCopySuccess"));
    copy(secretLink);
  };

  const onDisconnect = () => {
    console.log("disconnect!");
  };

  const tooltipContent = (
    <>
      <Text noSelect style={{ marginBottom: "10px" }}>
        {t("BotConnection")}
      </Text>
      <Text
        style={{ textDecoration: "underline dashed", cursor: "pointer" }}
        onClick={onCopyLink}
      >
        {"Click"}
      </Text>
    </>
  );

  return (
    <div className="subscription-container">
      <Text
        fontSize="15px"
        fontWeight="600"
        className="subscription-title"
        noSelect
      >
        {t("TelegramConnection")}
      </Text>

      {isTelegramConnected ? (
        <Text
          className="subscription-title subscription_click-text"
          noSelect
          onClick={onDisconnect}
        >
          {t("Common:Disconnect")}
        </Text>
      ) : (
        <HelpButton
          label={t("Common:Connect")}
          size={12}
          offsetRight={0}
          place="right"
          tooltipContent={tooltipContent}
        />
      )}
    </div>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;
  const { isTelegramConnected } = targetUserStore;

  const secretLink = "/";

  return { isTelegramConnected, secretLink };
})(observer(TelegramConnectionContainer));
