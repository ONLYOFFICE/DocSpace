import PlusThemeSvgUrl from "PUBLIC_DIR/images/plus.theme.svg?url";
import React, { useEffect } from "react";
import ModalDialog from "@docspace/components/modal-dialog";
import styled from "styled-components";
import Button from "@docspace/components/button";
import { withTranslation } from "react-i18next";

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

  .name-color {
    font-weight: 700;
    font-size: 18px;
    line-height: 24px;
  }

  .relative {
    position: relative;
  }

  .accent-box {
    background: ${(props) =>
      props.currentColorAccent
        ? props.currentColorAccent
        : props.theme.isBase
        ? `#eceef1 url(${PlusThemeSvgUrl}) no-repeat center`
        : `#474747 url(${PlusThemeSvgUrl}) no-repeat center`};
  }

  .buttons-box {
    background: ${(props) =>
      props.currentColorButtons
        ? props.currentColorButtons
        : props.theme.isBase
        ? `#eceef1 url(${PlusThemeSvgUrl}) no-repeat center`
        : `#474747 url(${PlusThemeSvgUrl}) no-repeat center`};
  }

  .modal-add-theme {
    width: 46px;
    height: 46px;
    border-radius: 8px;
    cursor: pointer;
  }
`;

const ColorSchemeDialog = (props) => {
  const {
    visible,
    onClose,
    header,
    nodeHexColorPickerAccent,
    nodeHexColorPickerButtons,
    viewMobile,
    showSaveButtonDialog,
    onSaveColorSchemeDialog,
    t,
    onClickColor,
    currentColorAccent,
    currentColorButtons,
  } = props;

  const onKeyPress = (e) =>
    (e.key === "Esc" || e.key === "Escape") && onClose();

  useEffect(() => {
    window.addEventListener("keyup", onKeyPress);
    return () => window.removeEventListener("keyup", onKeyPress);
  });

  return (
    <StyledComponent
      visible={visible}
      onClose={onClose}
      displayType="aside"
      currentColorAccent={currentColorAccent}
      currentColorButtons={currentColorButtons}
      withFooterBorder={showSaveButtonDialog}
    >
      <ModalDialog.Header>{header}</ModalDialog.Header>
      <ModalDialog.Body>
        <div>
          <div className="flex relative">
            <div className="name-color">{t("Settings:AccentColor")}</div>
            <div
              id="accent"
              className="modal-add-theme accent-box"
              onClick={onClickColor}
            />

            {!viewMobile && nodeHexColorPickerAccent}
          </div>

          <div className="flex relative">
            <div className="name-color">{t("Settings:ButtonsColor")}</div>
            <div
              id="buttons"
              className="modal-add-theme buttons-box"
              onClick={onClickColor}
            />

            {!viewMobile && nodeHexColorPickerButtons}
          </div>
        </div>
      </ModalDialog.Body>

      <ModalDialog.Footer>
        {showSaveButtonDialog && (
          <>
            <Button
              className="save"
              label={t("Common:SaveButton")}
              size="normal"
              primary
              scale
              onClick={onSaveColorSchemeDialog}
            />
            <Button
              className="cancel-button"
              label={t("Common:CancelButton")}
              size="normal"
              scale
              onClick={onClose}
            />
          </>
        )}
      </ModalDialog.Footer>
    </StyledComponent>
  );
};

export default withTranslation(["Common", "Settings"])(ColorSchemeDialog);
