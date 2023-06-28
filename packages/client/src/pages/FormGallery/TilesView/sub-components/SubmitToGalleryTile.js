import { Button } from "@docspace/components";
import React from "react";
import styled from "styled-components";
import CrossIcon from "PUBLIC_DIR/images/cross.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import { Base } from "@docspace/components/themes";
import { observer, inject } from "mobx-react";
import { withTranslation } from "react-i18next";

export const StyledSubmitToGalleryTile = styled.div`
  position: relative;

  width: 100%;
  height: 220px;

  padding: 16px;
  box-sizing: border-box;

  border: 1px solid #388bde;
  border-radius: 6px;
  background-color: rgba(82, 153, 224, 0.03);

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
      color: #388bde;
      font-weight: 600;
      font-size: 14px;
      line-height: 16px;
    }
    .body {
      font-weight: 400;
      font-size: 12px;
      line-height: 16px;
      color: #555f65;
    }
  }
`;

const StyledCloseIcon = styled(CrossIcon)`
  ${commonIconsStyles}
  position: absolute;
  top: 10px;
  right: 10px;
  cursor: pointer;

  path {
    fill: #657077;
  }
`;

StyledCloseIcon.defaultProps = { theme: Base };

const SubmitToGalleryTile = ({
  t,
  hideSubmitToGalleryTile,
  setSubmitToGalleryDialogVisible,
}) => {
  const onSubmitToGallery = () => setSubmitToGalleryDialogVisible(true);

  return (
    <StyledSubmitToGalleryTile>
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

export default inject(({ oformsStore, dialogsStore }) => ({
  hideSubmitToGalleryTile: oformsStore.hideSubmitToGalleryTile,
  setSubmitToGalleryDialogVisible: dialogsStore.setSubmitToGalleryDialogVisible,
}))(withTranslation("Common", "FormGallery")(observer(SubmitToGalleryTile)));
