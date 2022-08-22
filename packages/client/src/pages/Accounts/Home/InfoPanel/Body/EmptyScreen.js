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

  .no-thumbnail-img-wrapper {
    height: auto;
    width: 100%;
    display: flex;
    justify-content: center;
    .no-thumbnail-img {
      height: 96px;
      width: 96px;
    }
  }
`;

const EmptyScreen = ({ t }) => {
  return (
    <EmptyScreenContainer>
      <div className="no-thumbnail-img-wrapper">
        <img
          size="96px"
          className="no-thumbnail-img"
          src="/static/images/empty_screen-accounts-info-panel.png"
        />
      </div>

      <div className="empty-screen-text">{t("AccountsEmptyScreenText")}</div>
    </EmptyScreenContainer>
  );
};

export default withTranslation(["InfoPanel"])(EmptyScreen);
