import React from "react";

import styled from "styled-components";

import Button from "../../button";
import Combobox from "../../combobox";

import { StyledSelectorFooter } from "../StyledSelector";

const StyledComboBox = styled(Combobox)`
  margin-bottom: 2px;
  max-height: 50px;

  .combo-button {
    min-height: 40px;
  }

  .combo-buttons_arrow-icon {
    margin-top: 16px;
  }

  .combo-button-label,
  .combo-button-label:hover {
    font-size: 14px;
    text-decoration: none;
  }
`;

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
  }) => {
    const label =
      selectedItemsCount && isMultiSelect
        ? `${acceptButtonLabel} (${selectedItemsCount})`
        : acceptButtonLabel;

    return (
      <StyledSelectorFooter>
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
      </StyledSelectorFooter>
    );
  }
);

export default Footer;
