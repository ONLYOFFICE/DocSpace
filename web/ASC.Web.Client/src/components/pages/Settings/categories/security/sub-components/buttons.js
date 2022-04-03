import React from "react";
import Button from "@appserver/components/button";
import Text from "@appserver/components/text";
import { ButtonsWrapper } from "../StyledSecurity";

const Buttons = (props) => {
  const { t, showReminder, onSaveClick, onCancelClick } = props;

  return (
    <ButtonsWrapper>
      <Button
        label={t("Common:SaveButton")}
        size="small"
        primary={true}
        className="button"
        onClick={onSaveClick}
        isDisabled={!showReminder}
      />
      <Button
        label={t("Common:CancelButton")}
        size="small"
        className="button"
        onClick={onCancelClick}
        isDisabled={!showReminder}
      />
      {showReminder && (
        <Text
          color="#A3A9AE"
          fontSize="12px"
          fontWeight="600"
          className="reminder"
        >
          {t("YouHaveUnsavedChanges")}
        </Text>
      )}
    </ButtonsWrapper>
  );
};

export default Buttons;
