import { Button } from "@docspace/components";
import styled, { css } from "styled-components";
import CrossIcon from "PUBLIC_DIR/images/cross.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { Base } from "@docspace/components/themes";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";
import hexToRgba from "hex-to-rgba";

export const StyledSubmitToGalleryTile = styled.div`
  position: relative;

  width: 100%;
  height: 220px;

  padding: 16px;
  box-sizing: border-box;

  border: 1px solid
    ${({ currentColorScheme }) => currentColorScheme.main.accent};
  border-radius: 6px;
  background-color: ${({ currentColorScheme }) =>
    hexToRgba(currentColorScheme.main.accent, 0.03)};

  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 16px;

  .info {
    display: flex;
    flex-direction: column;
    align-items: start;
    gap: 8px;

    .title {
      color: ${({ currentColorScheme }) => currentColorScheme.main.accent};
      font-weight: 600;
      font-size: 14px;
      line-height: 16px;
    }
    .body {
      font-weight: 400;
      font-size: 12px;
      line-height: 16px;
      color: ${({ theme }) => theme.submitToGalleryTile.bodyText};
    }
  }
`;

StyledSubmitToGalleryTile.defaultProps = { theme: Base };

const StyledCloseIcon = styled(CrossIcon)`
  ${commonIconsStyles}
  position: absolute;
  top: 10px;
  cursor: pointer;

  ${(props) =>
    props.theme.interfaceDirection === "ltr"
      ? css`
          right: 10px;
        `
      : css`
          left: 10px;
        `}

  path {
    fill: ${({ theme }) => theme.submitToGalleryTile.closeIconFill};
  }
`;

StyledCloseIcon.defaultProps = { theme: Base };

const SubmitToGalleryTile = ({
  t,
  submitToGalleryTileIsVisible,
  hideSubmitToGalleryTile,
  setSubmitToGalleryDialogVisible,
  currentColorScheme,
}) => {
  if (!submitToGalleryTileIsVisible) return null;

  const onSubmitToGallery = () => setSubmitToGalleryDialogVisible(true);

  return (
    <StyledSubmitToGalleryTile currentColorScheme={currentColorScheme}>
      <StyledCloseIcon
        onClick={hideSubmitToGalleryTile}
        className="close-icon"
        size="medium"
      />

      <div className="info">
        <div className="title">
          {t("FormGallery:SubmitToGalleryBlockHeader")}
        </div>
        <div className="body">{t("FormGallery:SubmitToGalleryBlockBody")}</div>
      </div>

      <Button
        onClick={onSubmitToGallery}
        size="small"
        label={t("Common:SubmitToFormGallery")}
        scale
      />
    </StyledSubmitToGalleryTile>
  );
};

export default inject(({ auth, oformsStore, dialogsStore }) => ({
  submitToGalleryTileIsVisible: oformsStore.submitToGalleryTileIsVisible,
  hideSubmitToGalleryTile: oformsStore.hideSubmitToGalleryTile,
  setSubmitToGalleryDialogVisible: dialogsStore.setSubmitToGalleryDialogVisible,
  currentColorScheme: auth.settingsStore.currentColorScheme,
}))(withTranslation("Common", "FormGallery")(observer(SubmitToGalleryTile)));
