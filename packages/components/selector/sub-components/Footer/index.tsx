import React from "react";

import Button from "../../../button";

import { StyledFooter, StyledComboBox } from "./StyledFooter";
import { FooterProps } from "./Footer.types";

const Footer = React.memo(
  ({
    isMultiSelect,
    acceptButtonLabel,
    selectedItemsCount,
    withCancelButton,
    cancelButtonLabel,
    withAccessRights,
    accessRights,
    selectedAccessRight,
    onAccept,
    onCancel,
    onChangeAccessRights,
  }: FooterProps) => {
    const label =
      selectedItemsCount && isMultiSelect
        ? `${acceptButtonLabel} (${selectedItemsCount})`
        : acceptButtonLabel;

    return (
      <StyledFooter>
        <Button
          className={"button accept-button"}
          label={label}
          primary
          scale
          size={"normal"}
          onClick={onAccept}
        />

        {withAccessRights && (
          <StyledComboBox
            onSelect={onChangeAccessRights}
            options={accessRights}
            size="content"
            scaled={false}
            manualWidth="fit-content"
            selectedOption={selectedAccessRight}
            showDisabledItems
            directionX={"right"}
            directionY={"top"}
          />
        )}

        {withCancelButton && (
          <Button
            className={"button cancel-button"}
            label={cancelButtonLabel}
            scale
            size={"normal"}
            onClick={onCancel}
          />
        )}
      </StyledFooter>
    );
  }
);

export default Footer;
