import React, { useState, useCallback, useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import Button from "@docspace/components/button";

const StyledComponent = styled(ModalDialog)`
  .modal-dialog-aside-footer {
    width: 100%;
    bottom: 0 !important;
    left: 0;
    padding: 16px;
    box-shadow: 0px 12px 40px rgba(4, 15, 27, 0.12);
  }

  .flex {
    display: flex;
    justify-content: space-between;

    :not(:last-child) {
      padding-bottom: 20px;
    }
  }

  .text {
    font-weight: 700;
    font-size: 18px;
    line-height: 24px;
  }

  .color-button {
    width: 46px;
    height: 46px;
  }

  .relative {
    position: relative;
  }
`;

const ColorSchemeDialog = (props) => {
  const {
    visible,
    onClose,
    header,

    nodeButtonsColor,
    nodeAccentColor,

    nodeHexColorPickerAccent,
    nodeHexColorPickerButtons,

    viewMobile,

    openHexColorPickerButtons,
    openHexColorPickerAccent,

    showRestoreToDefaultButtonDialog,

    showSaveButtonDialog,
    onSaveColorSchemeDialog,
    t,
  } = props;

  return (
    <StyledComponent visible={visible} onClose={onClose} displayType="aside">
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <div>
          <div className="flex relative">
            <div className="text">Accent</div>
            {nodeAccentColor}

            {!viewMobile && nodeHexColorPickerAccent}
          </div>

          <div className="flex relative">
            <div className="text">Buttons</div>
            {nodeButtonsColor}

            {!viewMobile && nodeHexColorPickerButtons}
          </div>
        </div>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        {viewMobile && openHexColorPickerAccent && nodeHexColorPickerAccent}
        {viewMobile && openHexColorPickerButtons && nodeHexColorPickerButtons}

        <>
          {showSaveButtonDialog && (
            <Button
              label="Save"
              size="normal"
              className="save-button"
              primary={true}
              scale={true}
              onClick={onSaveColorSchemeDialog}
            />
          )}
          {showRestoreToDefaultButtonDialog && (
            <Button
              label="Restore to default"
              className="button"
              size="normal"
              scale={true}
            />
          )}
        </>
      </ModalDialog.Footer>
    </StyledComponent>
  );
};

export default ColorSchemeDialog;
