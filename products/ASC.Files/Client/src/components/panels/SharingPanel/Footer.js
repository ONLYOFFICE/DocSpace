import React from "react";

import Checkbox from "@appserver/components/checkbox";
import Button from "@appserver/components/button";
import Textarea from "@appserver/components/textarea";

import { StyledFooterContent } from "./StyledSharingPanel";

const Footer = ({
  buttonLabel,
  checkboxLabel,
  textareaPlaceholder,
  message,
  isPersonal,
  isNotifyUsers,
  isLoading,
  onNotifyUsersChange,
  onSaveClick,
  onChangeMessage,
}) => {
  return (
    <StyledFooterContent>
      {isNotifyUsers && (
        <Textarea
          className="sharing_panel-notification"
          placeholder={textareaPlaceholder}
          onChange={onChangeMessage}
          value={message}
          isDisabled={isLoading}
        />
      )}

      {!isPersonal && (
        <Checkbox
          isChecked={isNotifyUsers}
          label={checkboxLabel}
          onChange={onNotifyUsersChange}
          className="sharing_panel-checkbox"
          isDisabled={isLoading}
        />
      )}
      <Button
        className="sharing_panel-button"
        label={buttonLabel}
        scale={true}
        size={"medium"}
        primary
        onClick={onSaveClick}
        isDisabled={isLoading}
      />
    </StyledFooterContent>
  );
};

export default React.memo(Footer);
