import React from "react";

import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import Textarea from "@docspace/components/textarea";

import { StyledFooterContent } from "./StyledSharingPanel";

const Footer = ({
  t,
  isPersonal,
  message,
  onChangeMessage,
  isNotifyUsers,
  onNotifyUsersChange,
  onSaveClick,
}) => {
  return (
    <StyledFooterContent>
      {isNotifyUsers && (
        <Textarea
          className="sharing_panel-notification"
          placeholder={t("AddShareMessage")}
          onChange={onChangeMessage}
          value={message}
        />
      )}

      {!isPersonal && (
        <Checkbox
          isChecked={isNotifyUsers}
          label={t("Notify users")}
          onChange={onNotifyUsersChange}
          className="sharing_panel-checkbox"
        />
      )}
      <Button
        className="sharing_panel-button"
        label={t("Common:SaveButton")}
        scale={true}
        size={"normal"}
        primary
        onClick={onSaveClick}
      />
    </StyledFooterContent>
  );
};

export default React.memo(Footer);
