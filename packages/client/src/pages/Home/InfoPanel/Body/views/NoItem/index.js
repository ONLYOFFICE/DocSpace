import React from "react";
import styled from "styled-components";
import { withTranslation } from "react-i18next";

const EmptyScreenContainer = styled.div`
  margin: 80px auto;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
  gap: 32px;

  .empty-screen-text {
    font-family: "Open Sans";
    font-style: normal;
    font-weight: 600;
    font-size: 14px;
    line-height: 16px;
    text-align: center;
  }
`;

const NoItem = ({ t }) => {
  return (
    <EmptyScreenContainer>
      <div className="no-thumbnail-img-wrapper">
        <img
          size="96px"
          className="no-thumbnail-img"
          src="images/empty_screen_info_panel.png"
        />
      </div>

      <div className="empty-screen-text">{t("EmptyScreenText")}</div>
    </EmptyScreenContainer>
  );
};

export default withTranslation(["InfoPanel"])(NoItem);
